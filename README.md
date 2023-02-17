# ScanerAPI
 WIA Scan in the browser


URL: "http://localhost:8080"

<strong>Query scanner settings</strong>
Resource URI: [URL]/api/GetScannerParameters
Metod: POST
Description: Returns some scanner settings.

<strong>Headers Information</strong>
Content-Type: application/json;

<strong>Request Information</strong> 
not necessary
(Body - raw - JSON) ()
{
    "method":"GetScannerParameters",
    "sourceIndex": 0
}

<strong>Response Formats</strong>
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




