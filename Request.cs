namespace ScannerAPI
{
    
    public class ScanRequest
    {
        public int source { get; set; }
        public int dpi { get; set; }
        public int ColorMode { get; set; }
        public required string FFormat { get; set; }
        public required PageFormat PageFormat { get; set; }
        public int Brightness { get; set; }
        public int Contrast { get; set; } = 0;
    }
    public class PdfRequest
    {
      public List<FileRequest> filelist { get; set; }
    }
    
    public class FileRequest
    {
        public string filename { get; set; }
        public bool ls { get; set; }
    }
}
