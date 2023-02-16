namespace ScannerAPI
{
    public class Request
    {
        public string method { get; set; }
        public int sourceIndex { get; set; }
    }
    public class ScanRequest
    {
        public string method { get; set; }
        public int source { get; set; }
        public int dpi { get; set; }
        public int ColorMode { get; set; }
        public string FFormat  { get; set; }
        public PageFormat PageFormat { get; set; }
        public int Brightness { get; set; }
        public int Contrast { get; set; } = 0;
    }
    
}
