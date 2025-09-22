using ScannerAPI;
using ScannerAPI.Wia;

internal class ScannerHost
{
    private static void Main(string[] args)
    {
        IniFile INI = new IniFile("config.ini");
        string port = "8080";
        if (INI.KeyExists("Port", "Server"))
        {
            port = INI.ReadINI("Server", "Port");
        }
        else
        {
            INI.Write("Server", "Port", port);
        }


        var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://*:" + port);
        builder.Services.AddCors();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.WithOrigins("*")
                                                          //.AllowAnyOrigin()
                                                          .AllowAnyHeader()
                                                          .AllowAnyMethod();
                                  });
        });

        var app = builder.Build();
        app.UseStaticFiles();
        //app.UseDefaultFiles();
        app.UseCors(MyAllowSpecificOrigins);

        //app.MapGet("/", () => "WIA Scanner работает");

        //app.MapPost("/api/GetScannerParameters", async (ScannerAPI.Request request) =>
        //{
        //    WiaSource wia= new WiaSource();
        //    return Results.Json(wia.GetSource());
        //});
        //app.MapPost("/api/Scan", async (ScannerAPI.ScanRequest request) =>
        //{
        //    WiaSource wia = new WiaSource();
        //    string path = wia.Scan(request);
        //    string contentType = "image/"+request.FFormat.ToLower();       // установка mime-типа
        //    string downloadName = "file." + request.FFormat.ToLower();
        //    return Results.File(path, contentType, downloadName);
        //});

        //app.Map("/file", async () =>
        //{
        //    string path = "file";
        //    //byte[] fileContent = await File.ReadAllBytesAsync(path);  // считываем файл в массив байтов
        //    string contentType = "image/png";       // установка mime-типа
        //    string downloadName = "file.png";  // установка загружаемого имени
        //    return Results.File(path, contentType, downloadName);
        //});
        //app.MapGet("/routes", (IEnumerable<EndpointDataSource> endpointSources) =>
        //        string.Join("\n", endpointSources.SelectMany(source => source.Endpoints)));

        //app.Run(async (context) =>
        //{
        //    context.Response.StatusCode = 404;
        //    await context.Response.WriteAsync("Page Not Found");
        //});

        app.Run(async (context) =>
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var request = context.Request;

            //var now = DateTime.Now;
            var response = context.Response;

            if (path == "/")
            {
                //context.Response.ContentType = "text/html; charset=utf-8";
                //await response.WriteAsync("WIA Scanner работает");
                context.Response.Redirect("/index.html");
            }
            else if (path == "/api/GetScannerParameters" && method == "POST")
            {
                WiaSource wia = new WiaSource();
                await response.WriteAsJsonAsync(wia.GetSource());
            }
            else if (path == "/api/Scan" && method == "POST")
            {
                WiaSource wia = new WiaSource();
                var json = await request.ReadFromJsonAsync<ScanRequest>();


                string filePath = INI.ReadINI("IMG", "ImgFolder");

#pragma warning disable CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.
                string fileName = wia.Scan(json);
#pragma warning restore CS8604 // Возможно, аргумент-ссылка, допускающий значение NULL.


                string filepath = filePath + fileName; //"wwwroot/" + wia.Scan(json);

                string contentType = "image/" + json.FFormat.ToLower();        // установка mime-типа

                              //string downloadName = fileName;

                //context.Response.ContentType = contentType;

                context.Response.Headers.ContentDisposition = "attachment; filename=" + fileName;
                context.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");
                await response.SendFileAsync(filepath);
                //    return Results.File(path, contentType, downloadName);
            }
            else if (path == "/api/DoPDF" && method == "POST")
            {

                var json = await request.ReadFromJsonAsync<PdfRequest>();
                CreatePDF crtpdf = new CreatePDF();

                string filePath = INI.ReadINI("PDF", "LastPDFFolder");
                //#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                string fileName = crtpdf.createPDF(json.filelist);
                //#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
                string filepath = filePath + fileName;

                //string contentType = "application/pdf";        // установка mime-типа
                //string downloadName = fileName;

                //context.Response.ContentType = contentType;

                context.Response.Headers.ContentDisposition = "attachment; filename=" + fileName;
                context.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");
                await response.SendFileAsync(filepath);
                //await response.WriteAsJsonAsync(json);
            }
            else if (path == "/api/getImage" && method == "POST") { 
            var json = await request.ReadFromJsonAsync<FileRequest>();
                string filePath = INI.ReadINI("IMG", "ImgFolder");
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
                string fileName = json.filename;
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

                string filepath = filePath + fileName; //"wwwroot/" + wia.Scan(json);
                string contentType = "image/" + json.filename.Substring(json.filename.Length-3,3);        // установка mime-типа
                                                                               //string downloadName = fileName;
                //context.Response.ContentType = contentType;
                context.Response.Headers.ContentDisposition = "attachment; filename=" + fileName;
                context.Response.Headers.Append("Access-Control-Expose-Headers", "Content-Disposition");
                await response.SendFileAsync(filepath);
            }
            else
            {
                //context.Response.StatusCode = 404;
                //await context.Response.WriteAsync("Page Not Found");
                context.Response.Redirect("/");
            }
                        
        });


        app.Run();
    }
}