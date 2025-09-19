using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace ScannerAPI
{
    public class CreatePDF
    {
        
        private IniFile INI = new IniFile("config.ini");
        public string createPDF(string[] filelist) {
            
            //string saveFileName = String.Format("{0:D2}{1:D2}{2:D2} NewFileName", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year - 2000)+".pdf";
            string saveFileName = "file.pdf";
            string pdfPath = @"PDFtemp\";
            if (INI.KeyExists("LastPDFFolder", "PDF"))
            {
                pdfPath = INI.ReadINI("PDF", "LastPDFFolder");
            }
            else
            {
                INI.Write("PDF", "LastPDFFolder", pdfPath);
            }
            if (!Directory.Exists(pdfPath)) Directory.CreateDirectory(pdfPath);
            string PDFFileName = pdfPath + saveFileName;
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(PDFFileName));
            string filePath = INI.ReadINI("IMG", "ImgFolder");
            bool fpage = true;
            Document? document = null;

            foreach (string filename in filelist)
            {
                    System.Drawing.Image imageo = System.Drawing.Image.FromFile(filePath + filename);
                    
                    
                    ImageData imageData = ImageDataFactory.Create(filePath + filename);

                    iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                    int dpi =    (int)imageo.HorizontalResolution;
                    double kfw = imageo.Width / (210 * dpi / 25.4);
                    double kfh = imageo.Height / (297 * dpi / 25.4);

                    if (fpage)
                    {
                        document = new Document(pdfDocument, PageSize.A4);
                        document.SetMargins(0, 0, 0, 0);
                    } else {
                        document?.Add(new AreaBreak(PageSize.A4));
                    }
                    if (kfw > 1)
                    {
                        image.SetWidth((int)(pdfDocument.GetDefaultPageSize().GetWidth()));
                        image.SetAutoScaleHeight(true);
                    }
                    else
                    {
                        image.ScaleToFit((int)(pdfDocument.GetDefaultPageSize().GetWidth() * kfw), (int)(pdfDocument.GetDefaultPageSize().GetHeight() * kfh));
                    }
                    document!.Add(image);
                    fpage = false;

            }
            pdfDocument.Close();
            //показать результат
            return saveFileName;
        }
    }
}
