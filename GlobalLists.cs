namespace ScannerAPI
{
    internal class GlobalLists
    {
        public static List<PageFormat> PageFormats
        {
            get
            {
                return new List<PageFormat>
                {
                    //new PageFormat{Width=33.11f,Height= 46.81f,Name="A0"},
                    //new PageFormat{Width=23.39f,Height= 33.11f,Name="A1"},
                    //new PageFormat{Width=16.54f,Height= 23.39f,Name="A2"},                    
                    new PageFormat{Width=11.69f,Height= 16.54f,Name="A3"},
                    new PageFormat{Width=8.27f, Height= 11.69f,Name="A4"},
                    new PageFormat{Width=11.69f, Height= 8.27f,Name="A4 LS"},
                    new PageFormat{Width=5.83f, Height= 8.27f,Name="A5"},
                    new PageFormat{Width=8.27f, Height= 5.83f,Name="A5 LS"},
                    new PageFormat{Width=4.13f, Height= 5.84f,Name="A6"},
                    new PageFormat{Width=5.84f, Height= 4.13f,Name="A6 LS"}
                    
                    //new PageFormat{Width=2.91f, Height= 4.13f,Name="A7"}
                };
            }
        }

        public static List<PixelType> PixelTypes
        {
            get
            {
                return new List<PixelType>
                {
                    new PixelType { Id = 1,Color="Color",Description="Цветное"},
                    new PixelType { Id = 2,Color="Greyscale",Description="Оттенки серого"},
                    new PixelType { Id = 4,Color="BlackWhite",Description="Черно-белое"}
                };
            }
        }
        public static List<FileType> FeleTypes
        {
            get
            {
                return new List<FileType> {
                new FileType {Name= "JPG" },
                new FileType {Name = "PNG"},
                new FileType {Name= "TIF" },
                new FileType {Name= "BMP" },
                new FileType {Name= "GIF" }
                };
            }
        }

    };

    class PixelType
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
    }
    public class PageFormat
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get; set; }
    }

    class FileType
    {
        public string Name { get; set; }
    }

    public class ResultsParam
    {
        public string key { get; set; }
        public string value { get; set; }
    }

}
