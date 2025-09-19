// import "./locale/locale"
$(document).ready(function () {
    $("#scanFormWindow").draggable({
        handle: ".modal-header" // Указываем, что перетаскивать можно за заголовок
    });
    renderFileName()
    GetScanner()
});

//из отдельного расположения
//const url = "http://localhost:8080/api/";
//со встроенного сервера 
const url = getCookie("server_url") || "api/";
var server_url = document.getElementById("server_url");
server_url.value = url;
server_url.addEventListener("change", saveUrl);

function saveUrl(evt) {
    setCookie("server_url", evt.target.value);
    location.reload();
}


// Определяем функцию которая принимает в качестве параметров url и данные которые необходимо обработать:
const postData = async (url = '', data = {}) => {
    // Формируем запрос
    const response = await fetch(url, {
        // Метод, если не указывать, будет использоваться GET
        method: 'POST',
        // Заголовок запроса
        headers: {
            'Content-Type': 'application/json'
        },
        // Данные
        body: JSON.stringify(data)
    });
    return response.json();
}
const scanWIA = async (url = '', data = {}) => {
    // Формируем запрос
    const response = await fetch(url, {
        // Метод, если не указывать, будет использоваться GET
        method: 'POST',
        // Заголовок запроса
        headers: {
            'Content-Type': 'application/json'
        },
        // Данные
        body: JSON.stringify(data)
    });
    return response;
}

var Dpi = document.getElementById("Dpi");
var Color = document.getElementById("Color");

// Поля формы настройки сканера
var FFormat = document.getElementById("FFormat");
var PFormat = document.getElementById("PFormat");
var Brightness = document.getElementById("Brightness");
var Brightnessnum = document.getElementById("Brightnessnum");
var Contrast = document.getElementById("Contrast");
var Contrastnum = document.getElementById("Contrastnum");
//Регулироака сканирования    
//Яркость
Brightnessnum.value = 0;
Brightness.addEventListener('change', onBrChange, false);
Brightnessnum.addEventListener('change', onBrnChange, false);


//Список сканов 
var fileList = JSON.parse(localStorage.getItem('fileList')) || [];
var i = 0;

function GetScanner() {
    //загрузка параметров сканера
    const jsonData = { "method": "GetScannerParameters", "sourceIndex": 0 };
    postData(url + "GetScannerParameters", jsonData)
        .then((data) => {
            // console.log(data);
            //Сканер
            var Source = document.getElementById("Source");
            var option = document.createElement("option");
            option.text = data.sources.sourcesList[0].value;
            option.value = 0;
            Source.add(option);
            //Формат страницы
            //var PFormat = document.getElementById("PFormat");
            data.allowedFormats.forEach(el => {
                var option = document.createElement("option");
                option.text = el.Name;
                option.value = JSON.stringify({ Width: el.Width, Height: el.Height, Name: el.Name });
                PFormat.add(option);
            });
            //Цвет
            //var Color = document.getElementById("Color");
            data.pixelTypes.forEach(el => {
                var option = document.createElement("option");
                option.text = el.Description;
                option.value = el.Id;
                Color.add(option);
            });
            //DPI
            //var Dpi = document.getElementById("Dpi");
            data.flatbedResolutions.forEach(el => {
                var option = document.createElement("option");
                option.text = el;
                option.value = el;
                Dpi.add(option);
            });
            //Формат файла
            //var FFormat = document.getElementById("FFormat");
            data.fileFormats.forEach(el => {
                var option = document.createElement("option");
                option.text = el.Name;
                option.value = el.Name;
                FFormat.add(option);
            });

            //восстановить последние установки сканирования
            PFormat.selectedIndex = getCookie("PageFormat");
            Color.selectedIndex = getCookie("ColorMode");
            Dpi.selectedIndex = getCookie("dpi");
            FFormat.selectedIndex = getCookie("FFormat");
            Brightness.value = getCookie("Brightness");
            Brightnessnum.value = getCookie("Brightness");
            Contrast.value = getCookie("Contrast");
            Contrastnum.value = getCookie("Contrast");

        })
        .catch((reason) => {
            // отказ
            // console.log(reason)
            const toastLiveExample = document.getElementById('liveToast')

            // if (toastTrigger) {
            const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastLiveExample)
            //   toastTrigger.addEventListener('click', () => {
            toastBootstrap.show()
            //   })
            // }
        });
}

function onBrChange() {
    //console.log(Brightness.value);
    Brightnessnum.value = Brightness.value;
}
function onBrnChange() {
    //console.log(Brightness.value);
    Brightness.value = Brightnessnum.value;
}
//Контрастность

Contrastnum.value = 0;
Contrast.addEventListener('change', onCtChange, false);
Contrastnum.addEventListener('change', onCtnChange, false);
function onCtChange() {
    //console.log(Contrast.value);
    Contrastnum.value = Contrast.value;
}
function onCtnChange() {
    //console.log(Contrast.value);
    Contrast.value = Contrastnum.value;
}

//Сканирование
var ButtScan = document.getElementById("ButtonScan");
ButtScan.addEventListener('click', ScanClick, false);
var fileDate = null;

function ScanClick() {
    ButtScan.setAttribute("disabled", "");
    let scanParam = {
        "method": "Scan",
        "source": "0",
        "dpi": Dpi.value,
        "ColorMode": Number(Color.value),
        "FFormat": FFormat.value,
        "PageFormat": JSON.parse(PFormat.value),
        "Brightness": Number(Brightnessnum.value),
        "Contrast": Number(Contrastnum.value)
    }
    setCookie("dpi", Dpi.selectedIndex);
    setCookie("ColorMode", Color.selectedIndex);
    setCookie("FFormat", FFormat.selectedIndex);
    setCookie("PageFormat", PFormat.selectedIndex);
    setCookie("Brightness", Number(Brightnessnum.value));
    setCookie("Contrast", Number(Contrastnum.value));


    //console.log(scanParam);

    scanWIA(url + "Scan", scanParam)
        .then((data) => {
            let Scanned = document.getElementById("Scanned");
            var myimg = document.createElement("IMG");
            myimg.id = "origimg";
            if (Scanned.children.length > 0) {
                Scanned.replaceChildren();
            }

            const disposition = data.headers.get('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]+)/;
                const matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) {
                    const filename = matches[1].replace(/['"]/g, '');
                    addFileName(filename);
                }
            }
            data.blob().then(function (blob) {
                fileDate = blob;
                objectURL = URL.createObjectURL(fileDate);
                myimg.src = objectURL;
                // Scanned.style.width='auto';
                Scanned.appendChild(myimg);
                ButtScan.removeAttribute("disabled");
                ButtSave.removeAttribute("disabled");
                ButtSelect.removeAttribute("disabled");
            })

        }
        );

}

//Скачать файл или ...
var ButtSave = document.getElementById("ButtonSave");
ButtSave.setAttribute("disabled", "");
ButtSave.addEventListener('click', SaveClick, false);
function SaveClick() {
    let fileName = "file." + FFormat.value.toLowerCase();
    let link = document.createElement('a');
    link.download = fileName;
    link.href = URL.createObjectURL(fileDate);
    link.click();
    URL.revokeObjectURL(link.href);
}
//Включить выделение
var ButtSelect = document.getElementById("ButtonSelect");
ButtSelect.setAttribute("disabled", "");
ButtSelect.addEventListener('click', SelectClick, false);
var MySelection;
function SelectClick() {
    $('#origimg').imgAreaSelect({
        handles: true,
        onSelectEnd: function (img, selection) {
            MySelection = selection;
            ButtCrop.removeAttribute("disabled");
        }
    });

}


//Обрезать
var ButtCrop = document.getElementById("ButtonCrop");
ButtCrop.setAttribute("disabled", "");
ButtCrop.addEventListener('click', CropClick, false);
function CropClick() {

    const canvas = document.createElement('canvas');//document.getElementById("mycanvas");
    canvas.width = MySelection.width;
    canvas.height = MySelection.height;
    const ctx = canvas.getContext('2d');
    let img = document.getElementById('origimg');
    let kf1 = img.naturalWidth / img.width;
    let kf2 = img.naturalHeight / img.height;

    //console.log(img);
    ctx.drawImage(img, MySelection.x1 * kf1, MySelection.y1 * kf2, MySelection.width * kf1, MySelection.height * kf2, 0, 0, MySelection.width, MySelection.height);

    $('#origimg').imgAreaSelect({ remove: true });
    let Scanned = document.getElementById("Scanned");
    if (Scanned.children.length > 0) {
        Scanned.replaceChildren();
    }
    Scanned.style.width = MySelection.width + 'px';
    var myimg = document.createElement("IMG");
    //my.img.width=MySelection.width;
    myimg.id = "origimg";
    myimg.src = canvas.toDataURL(fileDate.type);
    Scanned.appendChild(myimg);

    canvas.toBlob((blob) => {
        fileDate = blob;
    });


    ButtCrop.setAttribute("disabled", "");

}

//для хранения последних настроек
function setCookie(key, value) {
    var expires = new Date();
    expires.setTime(expires.getTime() + (1 * 24 * 60 * 60 * 1000));
    document.cookie = key + '=' + value + ';expires=' + expires.toUTCString();
}

function getCookie(key) {
    var keyValue = document.cookie.match('(^|;) ?' + key + '=([^;]*)(;|$)');
    return keyValue ? keyValue[2] : null;
}


function renderFileName() {
    const lang = localStorage.getItem('lang') || 'ru';
    const tbody = document.querySelector('#scanList tbody');
    tbody.innerHTML = '';
    fileList.forEach(file => {
        tbody.innerHTML += `<tr>
                                <td>${file.id}</td>
                                <td>${file.filename}</td>
                                <td><input disabled class="form-check-input" type="checkbox" value="" ${file.ls ? 'checked' : ''}></td>
                                <td>
                                <button class="btn btn-sm desktop-btn editfilebtn"  textforelement="editfilebtn">${translations[lang]['editfilebtn']}</button>
                                <button class="btn btn-danger desktop-btn btn-sm " onclick="deleteFile(${file.id})" textforelement="delfilebtn">${translations[lang]['delfilebtn']}</button>
                                </td>
                            </tr>`;
        i = file.id;
    });

    $('.editfilebtn').on('click', function () {
// console.log(this.parentElement.parentElement)
        let Scanned = document.getElementById("Scanned");
        var myimg = document.createElement("IMG");
        myimg.id = "origimg";
        if (Scanned.children.length > 0) {
            Scanned.replaceChildren();
        }

        let filename = this.parentElement.parentElement.cells[1].textContent
        scanWIA(url + "getImage", { filename: filename })
            .then((data) => {
                data.blob().then(function (blob) {
                    fileDate = blob;
                    objectURL = URL.createObjectURL(fileDate);
                    myimg.src = objectURL;
                    // Scanned.style.width='auto';
                    Scanned.appendChild(myimg);
                    ButtScan.removeAttribute("disabled");
                    ButtSave.removeAttribute("disabled");
                    ButtSelect.removeAttribute("disabled");
                })
            })
     
        scanWindow = new bootstrap.Modal(document.getElementById('scanFormWindow'));
        scanWindow.show();

    })

}
function addFileName(filename) {
    i += 1
    file = { id: i, filename: filename, ls: 0 }
    fileList.push(file)
    const fileListString = JSON.stringify(fileList);
    localStorage.setItem('fileList', fileListString);
    renderFileName()
}

function deleteFile(id) {
    const newArray = fileList.filter(obj => obj.id !== id);
    fileList = newArray
    const fileListString = JSON.stringify(fileList);
    localStorage.setItem('fileList', fileListString);
    renderFileName()
}

$('#PDFbtn').on('click', function () {
    let filelist = []
    fileList.forEach(el => {
        filelist.push(el.filename)
    })
    scanWIA(url + "DoPDF", { filelist: filelist })
        //взвращает response
        .then((data) => {
            // console.log(data)
            let filename = "fff"
            const disposition = data.headers.get('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]+)/;
                const matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) {
                    filename = matches[1].replace(/['"]/g, '');
                }
            }
            data.blob().then(function (blob) {
                // console.log('pdf')
                var file = new Blob([(blob)], {
                    type: 'application/pdf'
                });
                var fileURL = URL.createObjectURL(file);

                let fileName = filename;
                let link = document.createElement('a');
                link.download = fileName;
                link.href = fileURL;
                link.click();
                URL.revokeObjectURL(link.href);
            })
        })

}

)

