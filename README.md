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
    "method":"GetScannerParameters",<br>
    "sourceIndex": 0<br>
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
<strong>Scanning</strong>
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


