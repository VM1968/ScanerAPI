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
(Body - raw - JSON)<br>
<pre>{
    "method":"GetScannerParameters",
    "sourceIndex": 0
}
</pre>
<strong>Response Formats</strong>
<pre>{
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
<pre>{
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
</pre>
<br>
<strong>Response Formats</strong><br>
Content-Type: image/[png,jpg,tif,bmp,gif]<br>
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

scanWIA(url + "Scan", scanParam)
                .then((data) => {
                    //<div id ="Scanned"></div>
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

//Save or ...
        var ButtSave = document.getElementById("ButtonSave"); //add button id="ButtonSave"
        ButtSave.addEventListener('click', SaveClick, false);
        function SaveClick() {
            let fileName = "file.png" ; /// according to your request, change in the program
            let link = document.createElement('a');
            link.download = fileName;
            link.href = URL.createObjectURL(fileDate);
            link.click();
            URL.revokeObjectURL(link.href);
        }				
				
	
</pre>


