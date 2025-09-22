# ScanerAPI
WIA Scan in the browser

URL: "http://localhost:8080"

<strong>Query scanner settings</strong><br>
Resource URI: [URL]/api/GetScannerParameters<br>
Metod: POST<br>
Description: Returns some scanner settings.<br>
<br>
<strong>Headers Information</strong><br>
Content-Type: application/json;<br>
<br>
<strong>Request Information</strong><br> 
not necessary<br>
<strong>Response Formats</strong>
Content-Type: application/json;<br>
<pre>
{
    "sources": {
        "selectedSource": "0",
        "sourcesList": [
            {
                "key": "{6BDD1FC6-810F-11D0-BEC7-08002BE2092F}\\0000",
                "value": "HP LaserJet MFP M28-M31 (USB)"
            }
        ]
    },
    "flatbedResolutions": [
        200,
        300,
        600
    ],
    "pixelTypes": [
        {
            "Id": 1,
            "Color": "Color",
            "Description": "Цветное"
        },
        {
            "Id": 2,
            "Color": "Greyscale",
            "Description": "Оттенки серого"
        },
        {
            "Id": 4,
            "Color": "BlackWhite",
            "Description": "Черно-белое"
        }
    ],
    "fileFormats": [
        {
            "Name": "JPG"
        },
        {
            "Name": "PNG"
        },
        {
            "Name": "TIF"
        },
        {
            "Name": "BMP"
        },
        {
            "Name": "GIF"
        }
    ],
    "allowedFormats": [
        {
            "Width": 8.27,
            "Height": 11.69,
            "Name": "A4"
        },
        {
            "Width": 5.83,
            "Height": 8.27,
            "Name": "A5"
        },
        {
            "Width": 8.27,
            "Height": 5.83,
            "Name": "A5 LS"
        },
        {
            "Width": 4.13,
            "Height": 5.84,
            "Name": "A6"
        },
        {
            "Width": 5.84,
            "Height": 4.13,
            "Name": "A6 LS"
        }
    ]
}
</pre>

<br><br>
<strong>Scanning</strong><br>
Resource URI: [URL]/api/Scan<br>
Metod: POST<br>
Description: Scanning and return image.<br>
<br>
<strong>Headers Information</strong><br>
Content-Type: application/json;<br>
<br>
<strong>Request Information</strong><br> 
(Body - raw - JSON)<br>
<pre>
{
    "source": "0",
    "dpi": 200,
    "ColorMode": 4,
    "FFormat": "PNG",
    "PageFormat": 
            {
                "Width": 8.27, 
                "Height": 11.69, 
                "Name": "A4"
                },
    "Brightness": 0,
    "Contrast": 0
}
</pre>
<br>
<strong>Response Formats</strong><br>
Content-Disposition: attachment; filename=19-09-2025-10-33-05-0587203.png<br>
<pre>blob:http:// ....</pre>

<br><br>
<strong>Query scanned image</strong><br>
Resource URI: [URL]/api/getImage<br>
Metod: POST<br>
Description: Returns scanned image.<br>
<br>
<strong>Headers Information</strong><br>
Content-Type: application/json;<br>
<br>
<strong>Request Information</strong><br> 
<br>
(Body - raw - JSON)<br>
<pre>
{ "filename": "19-09-2025-09-17-16-2160046.png" }
</pre>
<strong>Response Formats</strong>
Content-Disposition: attachment; filename=19-09-2025-09-17-16-2160046.png<br>
<pre>blob:http:// ....</pre>

<br><br>
<strong>Create PDF from scanned images</strong><br>
Resource URI: [URL]/api/DoPDF<br>
Metod: POST<br>
Description: Returns PDF file.<br>
<br>
<strong>Headers Information</strong><br>
Content-Type: application/json;<br>
<br>
<strong>Request Information</strong><br> 
<br>
(Body - raw - JSON)<br>
<pre>
{
"filelist":
	[
		{"filename":"22-09-2025-11-45-32-1729418.png","ls":false},
		{"filename":"22-09-2025-11-45-57-2696859.png","ls":true},
		{"filename":"22-09-2025-11-46-30-5452102.png","ls":false}
	]
}
</pre>
<strong>Response Formats</strong>
Content-Disposition: attachment; filename=file.pdf<br>
<pre>blob:http:// ....</pre>

<br><br>
<strong>Sample JavaScript</strong>
<pre>
//GetScannerParameters
const url="http://localhost:8080/api/";
const postData = async (url = '', data = {}) => {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            return response.json();
        }

const jsonData = { "method": "GetScannerParameters", "sourceIndex": 0 };
      postData(url + "GetScannerParameters", jsonData)
            .then((data) => {
                  console.log(data);
                });	


//Scanning
const scanWIA = async (url = '', data = {}) => {
            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            return response;
        }
var scanParam = {
		"method":"Scan",
		"source": "0",
		"dpi":"200",
		"ColorMode":4,
		"FFormat":"PNG",
		"PageFormat":{
				"Width": 8.27,
				"Height": 5.83,
				"Name": "A5 LS"
			},
		"Brightness":-400,
		"Contrast":0
		}

//add div id ="Scanned"
scanWIA(url + "Scan", scanParam)
                .then((data) => {
                    let Scanned = document.getElementById("Scanned");
                    let img = document.createElement("IMG");
                    if (Scanned.children.length > 0) {
                        Scanned.replaceChildren();
                    }
                    data.blob().then(function (blob) {
                        fileDate = blob;
                        objectURL = URL.createObjectURL(fileDate);
                        img.src = objectURL;
                        Scanned.appendChild(img);
                    })
                });

// Save or ...
// add button id="ButtonSave"
// fileName - according to your request, change in the program
        var ButtSave = document.getElementById("ButtonSave"); 
        ButtSave.addEventListener('click', SaveClick, false);
        function SaveClick() {
            let fileName = "file."+ scanParam.FFormat.toLowerCase(); ; 
            let link = document.createElement('a');
            link.download = fileName;
            link.href = URL.createObjectURL(fileDate);
            link.click();
            URL.revokeObjectURL(link.href);
        }				
</pre>