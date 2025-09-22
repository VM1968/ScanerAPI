using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Text;
using System.Text.Json;
using WIA;

namespace ScannerAPI.Wia
{
    public class WiaSource
    {
        private IDevice? _device;
        public JsonDocument GetSource(string item = "0")
        {
            
            List<ResultsParam> sourcelist= new List<ResultsParam>();

            using var ms = new MemoryStream();
            using var writer = new Utf8JsonWriter(ms);


            DeviceManager DeviceManager1 = new DeviceManager();
            
            foreach (DeviceInfo di in DeviceManager1.DeviceInfos) 
            {
                _device = di.Connect();
                
                ResultsParam source = new ResultsParam
                {
                    key = di.DeviceID,
                    value = di.Properties["Name"].get_Value().ToString()
                };

                sourcelist.Add(source);
            }
            
            //создать JSON
            writer.WriteStartObject();
            
            //"sources"
            writer.WriteStartObject("sources");
            writer.WriteString("selectedSource", "0");
            writer.WritePropertyName("sourcesList");
            writer.WriteRawValue(JsonSerializer.Serialize(sourcelist));
            writer.WriteEndObject();

            if (_device != null)
            {
                //"flatbedResolutions"
                List<float> values = GetAllowableResolutions(_device.Items[1]);
                writer.WritePropertyName("flatbedResolutions");
                writer.WriteRawValue(JsonSerializer.Serialize(values));

                //"pixelTypes"
                writer.WritePropertyName("pixelTypes");
                writer.WriteRawValue(JsonSerializer.Serialize(GlobalLists.PixelTypes));

                //"fileFormats"
                writer.WritePropertyName("fileFormats");
                writer.WriteRawValue(JsonSerializer.Serialize(GlobalLists.FeleTypes));

                //allowedFormats
                writer.WritePropertyName("allowedFormats");
                writer.WriteRawValue(JsonSerializer.Serialize(GetAllowedPageFormats(_device)));
            }

            writer.WriteEndObject();
            writer.Flush();

            string json = Encoding.UTF8.GetString(ms.ToArray());

            return JsonDocument.Parse(json);
            //return json;
        }

        public string Scan(ScannerAPI.ScanRequest request) {
            DeviceManager DeviceManager1 = new DeviceManager();
            IniFile INI = new IniFile("config.ini");

            foreach (DeviceInfo di in DeviceManager1.DeviceInfos)
            {
                _device = di.Connect();
            }
            if (_device != null)
            {
                Item scannerItem = _device.Items[1];

                AdjustScannerSettings(scannerItem, request);

                string WIA_FormatID = WIA.FormatID.wiaFormatPNG;
                string FileExt = ".png";

                switch (request.FFormat)
                {
                    case "JPG":
                        WIA_FormatID = WIA.FormatID.wiaFormatJPEG;
                        FileExt = ".jpg";
                        break;
                    case "PNG":
                        WIA_FormatID = WIA.FormatID.wiaFormatPNG;
                        FileExt = ".png";
                        break;
                    case "TIF":
                        WIA_FormatID = WIA.FormatID.wiaFormatTIFF;
                        FileExt = ".tif";
                        break;
                    case "BMP":
                        WIA_FormatID = WIA.FormatID.wiaFormatBMP;
                        FileExt = ".bmp";
                        break;
                    case "GIF":
                        WIA_FormatID = WIA.FormatID.wiaFormatGIF;
                        FileExt = ".gif";
                        break;
                }


                object scanResult = scannerItem.Transfer(WIA_FormatID); //commonDialogClass.ShowTransfer(scannerItem, WIA_FormatID, false);
                if (scanResult != null)
                {
                    ImageFile image = (ImageFile)scanResult;
                    
                    string filePath = @"IMGtemp\";
                    if (INI.KeyExists("ImgFolder", "IMG"))
                    {
                        filePath = INI.ReadINI("IMG", "ImgFolder");
                    }
                    else
                    {
                        INI.Write("IMG", "ImgFolder", filePath);
                    }
                    if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);


                    string fileName = DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss-fffffff") + FileExt;

                    var quality = 100;

                    //image.SaveFile(fileName);
                    SaveImageToFile(image, filePath+fileName, WIA_FormatID, request.PageFormat,quality);
                    return fileName;
                }
                else
                {
                    return "";
                }
            }
            else {
                return "";
            }
        }

        private float GetMaxHeight(IDevice device)
        {
            //Debug(string.Format("Get max Height"));
            const int WIA_SCAN_VerticalBedSize = 3075;
            try
            {
                var verticalBedSize = FindProperty(device.Properties, WIA_SCAN_VerticalBedSize);// WiaProperty.VerticalBedSize);
                var vertical = verticalBedSize.get_Value();
                var maxHeight = (float)(int)vertical / 1000;
                //Debug(string.Format("Get max Height success, value: " + maxHeight));
                return maxHeight;
            }
            catch (Exception)
            {
                //Debug(string.Format("Cant't obtain max height, error: " + e));
                throw;
            }
        }

        private float GetMaxWidth(IDevice device)
        {
            //Debug(string.Format("Get max Width"));
            const int WIA_SCAN_HorizontalBedSize = 3074;
            try
            {
                var horizontalBedSize = FindProperty(device.Properties, WIA_SCAN_HorizontalBedSize);// WiaProperty.HorizontalBedSize);
                var horizontal = horizontalBedSize.get_Value();
                var maxWidth = (float)(int)horizontal / 1000;

                //Debug(string.Format("Get max Width success, value: " + maxWidth));
                return maxWidth;
            }
            catch (Exception)
            {
                //Debug(string.Format("Cant't obtain max width, error: " + e));
                throw;
            }
        }

        private List<float> GetAllowableResolutions(IItem source)
        {
            //Debug("Getting resolutions");
            const int WIA_SCAN_VerticalResolution = 6148;
            const int WIA_SCAN_HorizontalResolution = 6147;

            var verticalResolution = FindProperty(source.Properties, WIA_SCAN_VerticalResolution);//WiaProperty.VerticalResolution);
            var horizontalResolution = FindProperty(source.Properties, WIA_SCAN_HorizontalResolution); //WiaProperty.HorizontalResolution);

            if (verticalResolution == null || horizontalResolution == null)
                throw new Exception("Не удалось получить допустимые разрешения сканера");

            var verticalResolutions = new List<float>();
            var horizontalResolutions = new List<float>();

            Vector? verticalResolutionsVector = null;
            Vector? horizontalResolutionsVector = null;

            //Разрешения могут быть представлены либо списком в SubTypeValues, либо минимальным, максимальным значаниями и шагом (SubTypeMin, SubTypeMax, SubTypeStep)
            bool isVector;

            try
            {
                verticalResolutionsVector = verticalResolution.SubTypeValues;
                horizontalResolutionsVector = horizontalResolution.SubTypeValues;

                isVector = true;
            }
            catch (Exception)
            {
                isVector = false;
            }

            if (isVector)
            {
                foreach (var hResolution in horizontalResolutionsVector)
                {
                    horizontalResolutions.Add((int)hResolution);
                }

                foreach (var vResolution in verticalResolutionsVector)
                {
                    verticalResolutions.Add((int)vResolution);
                }
            }
            else
                try
                {
                    for (var i = verticalResolution.SubTypeMin;
                        i <= verticalResolution.SubTypeMax;
                        i += verticalResolution.SubTypeStep)
                    {
                        verticalResolutions.Add(i);
                    }

                    for (var i = horizontalResolution.SubTypeMin;
                        i <= horizontalResolution.SubTypeMax;
                        i += horizontalResolution.SubTypeStep)
                    {
                        horizontalResolutions.Add(i);
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Не удалось получить допустимые разрешения сканера");
                }

            var resolutions = horizontalResolutions.Count < verticalResolutions.Count
                ? horizontalResolutions
                : verticalResolutions;

            //Debug(string.Format("Resolutions was obtained, min: {0}, max: {1}",
            //    resolutions[0],
            //    resolutions[resolutions.Count - 1]));

            return resolutions;
        }

        //тестирование возможностей сканера
        private List<PageFormat> GetAllowedPageFormats(IDevice device)
        {
            var maxHeight = GetMaxHeight(device);
            var maxWidth = GetMaxWidth(device);

            List<PageFormat> _allowedFormats;
            _allowedFormats = new List<PageFormat>();
            foreach (var format in GlobalLists.PageFormats)
            {
                if (format.Height < maxHeight && format.Width < maxWidth)
                {
                    _allowedFormats.Add(format);
                }
            }
            return _allowedFormats;
        }
        private Property FindProperty(WIA.Properties properties, int property) //WiaProperty
        {
            //Log("Try to find property: " + property);
            foreach (Property prop in properties)
            {
                if (prop.PropertyID == (int)property)
                {
                    //Log(string.Format("Property '{0}' was found", property));
                    return prop;
                }
            }
            //Log(string.Format("Property '{0}' was not found", property));
            return null;
        }

        private void AdjustScannerSettings(Item scannerItem, ScannerAPI.ScanRequest request)
        {
            //размер страниц
            int DPI = request.dpi;
            SetPage(scannerItem.Properties, DPI, (PageFormat)request.PageFormat);

            // Цвет
            const string WIA_SCAN_COLORING = "6146"; //4 is Black-white, 2 is gray, 1 is color
            int COLOR = request.ColorMode; //color
            SetWIAProperty(scannerItem.Properties, WIA_SCAN_COLORING, COLOR);

            // Яркость
            const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
            int brightnessPercents = 0;
            try
            {
                brightnessPercents = request.Brightness; //-1000 до 1000
            }
            catch
            {
            }
            SetWIAProperty(scannerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);

            // Контрастность
            const string WIA_SCAN_CONTRAST_PERCENTS = "6155";
            int contrastPercents = 0;
            try
            {
                contrastPercents = request.Contrast;   //-1000 до 1000
            }
            catch
            {
            }
            SetWIAProperty(scannerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
        }

        private static void SetWIAProperty(IProperties properties, object propName, object propValue)

        {
            Property prop = properties.get_Item(ref propName);
            prop.set_Value(ref propValue);
        }

        private static void SetPage(WIA.IProperties properties, int dpi, PageFormat pf)
        {
            // Разрешение
            const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
            const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
            // Размеры сканирования
            const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
            const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
            const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
            const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";


            //пересчет размера в новом разрешении
            int width = (int)(pf.Width * dpi);
            int height = (int)(pf.Height * dpi);

            SetWIAProperty(properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, dpi);
            SetWIAProperty(properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, dpi);
            SetWIAProperty(properties, WIA_HORIZONTAL_SCAN_START_PIXEL, 0);
            SetWIAProperty(properties, WIA_VERTICAL_SCAN_START_PIXEL, 0);
            SetWIAProperty(properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, width);
            SetWIAProperty(properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, height);

            //ориентация
            //const string WIA_SCAN_Orientation = "6156"; //0 1
            //SetWIAProperty(properties, WIA_SCAN_Orientation, 0);
        }
        private static void SaveImageToFile(ImageFile image, string fileName, string WIA_FormatID, PageFormat pf = null, int quality = 100)

        {
            ImageProcess imgProcess = new ImageProcess();
            object convertFilter = "Convert";
            string convertFilterID = imgProcess.FilterInfos.get_Item(ref convertFilter).FilterID;

            //rotate
            if (pf.Name.Contains("LS"))
            {
                var angle = 90;

                Object ix1 = (Object)"RotateFlip";
                WIA.FilterInfo fi1 = imgProcess.FilterInfos.get_Item(ref ix1);
                imgProcess.Filters.Add(fi1.FilterID, 0);

                Object p1 = (Object)"RotationAngle";
                Object pv1 = (Object)angle;
                imgProcess.Filters[1].Properties.get_Item(ref p1).set_Value(ref pv1);
                //image = imgProcess.Apply(image);
            }
            //Quality
            //if (WIA_FormatID == WIA.FormatID.wiaFormatJPEG) {
            //imgProcess.Filters.Add(imgProcess.FilterInfos["Convert"].FilterID);
            //imgProcess.Filters[1].Properties["FormatID"].set_Value("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}");
            //imgProcess.Filters[1].Properties["Quality"].set_Value(quality);
            //    Object p1 = (Object)"Quality";
            //    Object pv1 = (Object)quality;
            //    imgProcess.Filters[1].Properties.get_Item(ref p1).set_Value(ref pv1);
            //ImageFile image = myip.Apply(imageFile);
            //}

            imgProcess.Filters.Add(convertFilterID, 0);
            SetWIAProperty(imgProcess.Filters[imgProcess.Filters.Count].Properties, "FormatID", WIA_FormatID);

            image = imgProcess.Apply(image);
            //image = image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            image.SaveFile(fileName);
        }



    }
}
