using System.Diagnostics;

namespace autocomplete_test_task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.Write("Введите номер колонки для поиска (нумерация начинается с 1): ");
            string? inputColumn = Console.ReadLine();

            if (!int.TryParse(inputColumn, out int columnNumber) || columnNumber < 1)
            {
                Console.WriteLine("Некорректный ввод. Номер колонки должен быть целым числом больше 0.");
                return;
            }

            string filePath = "airports.dat";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл airports.dat не найден.");
                return;
            }

            var searchEngine = new SearchEngine(filePath, columnNumber - 1);

            while (true)
            {
                Console.Write("Введите текст для поиска (или !quit для выхода): ");
                string? prefix = Console.ReadLine();

                if (string.Equals(prefix, "!quit", StringComparison.OrdinalIgnoreCase))
                    break;

                var stopwatch = Stopwatch.StartNew();

                var results = searchEngine.Search(prefix ?? string.Empty);

                stopwatch.Stop();

                if (!results.Any())
                {
                    Console.WriteLine("Ничего не найдено.");
                }
                else
                {
                    foreach (var result in results)
                    {
                        Console.WriteLine(result);
                    }

                    Console.WriteLine($"\nНайдено строк: {results.Count()}");
                    Console.WriteLine($"Время поиска: {stopwatch.ElapsedMilliseconds} мс");
                }

                Console.WriteLine();
            }

            Console.WriteLine("Завершение работы.");
        }
    }
}