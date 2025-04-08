using System.Text;

namespace autocomplete_test_task
{
    public class SearchEngine
    {
        private readonly string _filePath;
        private readonly int _columnIndex;
        private readonly Dictionary<string, List<IndexEntry>> _index = new();

        public SearchEngine(string filePath, int columnIndex)
        {
            _filePath = filePath;
            _columnIndex = columnIndex;
            BuildIndex();
        }

        private void BuildIndex()
        {
            using var reader = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(reader, Encoding.UTF8);

            long position = 0;

            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line == null) continue;

                var values = CsvParser.ParseLine(line);
                if (_columnIndex >= values.Count) continue;

                string key = values[_columnIndex].Trim('\"').ToLowerInvariant();
                string prefixKey = key.Length >= 2 ? key.Substring(0, 2) : key;

                if (!_index.ContainsKey(prefixKey))
                    _index[prefixKey] = new List<IndexEntry>();

                _index[prefixKey].Add(new IndexEntry(position, key));
                position = reader.Position;
            }
        }

        public IEnumerable<string> Search(string input)
        {
            string prefix = input.Trim().ToLowerInvariant();
            string prefixKey = prefix.Length >= 2 ? prefix.Substring(0, 2) : prefix;

            if (!_index.TryGetValue(prefixKey, out var entries))
                return Enumerable.Empty<string>();

            var results = new List<(string Display, string FullLine)>();

            using var reader = new FileStream(_filePath, FileMode.Open, FileAccess.Read);
            using var sr = new StreamReader(reader, Encoding.UTF8);

            foreach (var entry in entries)
            {
                if (!entry.ColumnValue!.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                reader.Seek(entry.Offset, SeekOrigin.Begin);
                string? line = sr.ReadLine();
                if (line == null) continue;

                var fields = CsvParser.ParseLine(line);
                string value = fields.Count > _columnIndex ? fields[_columnIndex].Trim('\"') : "";
                results.Add((value, line));
            }

            bool isNumeric = results.All(r => double.TryParse(r.Display, out _));

            if (isNumeric)
            {
                return results
                    .OrderBy(r => double.Parse(r.Display)) 
                    .Select(r => $"{r.Display}[{r.FullLine}]");
            }
            else
            {
                return results
                    .OrderBy(r => r.Display, StringComparer.OrdinalIgnoreCase) 
                    .Select(r => $"{r.Display}[{r.FullLine}]");
            }
        }
    }
}
