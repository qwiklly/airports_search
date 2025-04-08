namespace autocomplete_test_task
{
    public class IndexEntry
    {
        public long Offset { get; set; }
        public string? ColumnValue { get; set; }

        public IndexEntry(long offset, string columnValue)
        {
            Offset = offset;
            ColumnValue = columnValue;
        }
    }
}
