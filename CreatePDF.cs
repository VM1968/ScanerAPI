using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;

namespace ScannerAPI
{
    public class CreatePDF
    {

        private IniFile INI = new IniFile("config.ini");
        public string createPDF(List<FileRequest> filelist)
        {

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

            foreach (FileRequest filename in filelist)
            {
                System.Drawing.Image imageo = System.Drawing.Image.FromFile(filePath + filename.filename);


                ImageData imageData = ImageDataFactory.Create(filePath + filename.filename);

                iText.Layout.Element.Image image = new iText.Layout.Element.Image(imageData);
                int dpi = (int)imageo.HorizontalResolution;
                double kfw = imageo.Width / (210 * dpi / 25.4);
                double kfh = imageo.Height / (297 * dpi / 25.4);

                if (fpage)
                {
                    if (filename.ls)
                    {
                        document = new Document(pdfDocument, PageSize.A4.Rotate());
                        image.SetRotationAngle(-1 * Math.PI / 2);
                    }
                    else
                    {
                        document = new Document(pdfDocument, PageSize.A4);
                    }
                    document.SetMargins(0, 0, 0, 0);
                }
                else
                {
                    if (filename.ls)
                    {
                        //document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                        document?.Add(new AreaBreak(PageSize.A4.Rotate()));
                        image.SetRotationAngle(-1 * Math.PI / 2);
                    }
                    else
                    {
                        document?.Add(new AreaBreak(PageSize.A4));
                    }

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
