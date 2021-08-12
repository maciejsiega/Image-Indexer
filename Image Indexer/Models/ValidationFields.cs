namespace Image_indexer.Models
{
    public class ValidationFields
    {
        public bool Enabled { get; set; }
        public string FieldName { get; set; }
        public string IndexingData { get; set; }
        public bool Sticky { get; set; }
        public bool Required { get; set; }
        public bool Filename { get; set; }
    }
}
