const translations = {
    en: {
        scanbtn: "Scan",
        scanFormWindowLabel: "Scanning",
        closebtn: "Close",
        labelSource: "Scanner:",
        labelPFormat: "Page format:",
        labelColor: "Color:",
        labelDpi: "Resolution(DPI):",
        labelFFormat: "File type:",
        labelBrightness: "Brightness:",
        labelContrast: "Contrast:",
        ButtonSave: "Save",
        ButtonSelect:"Enable highlighting",
        ButtonCrop:"Trim to selection",
        pdfbtn: "to PDF",
        editfilebtn: "View",
        delfilebtn: "Delete",
        fileth:"File",
        lsth:"Landscape"
    },
    ru: {
        scanbtn: "Сканировать",
        scanFormWindowLabel: "Сканирование",
        closebtn: "Закрыть",
        labelSource: "Сканер:",
        labelPFormat: "Формат страницы:",
        labelColor: "Цвет:",
        labelDpi: "Разрешение(DPI):",
        labelFFormat: "Тип файла:",
        labelBrightness: "Яркость:",
        labelContrast: "Контрастность:",
        ButtonSave: "Сохранить",
        ButtonSelect:"Включить выделение",
        ButtonCrop:"Обрезать до выделения",
        pdfbtn: "в PDF",
        editfilebtn: "Просмотр",
        delfilebtn: "Удалить",
        fileth:"Файл",
        lsth:"Альбомный"
    }
};

function setLang(lang) {
    localStorage.setItem('lang', lang);
    translateDocument();
}
document.getElementById('langSelect').onchange = function () {
    setLang(this.value);
};

window.onload = function () {
    const savedLang = localStorage.getItem('lang') || 'ru';
    document.getElementById('langSelect').value = savedLang;
    translateDocument();
};

function translateDocument() {
    // Получаем все элементы на странице
    const lang = localStorage.getItem('lang') || 'ru';
    const allElements = document.querySelectorAll('*');

    // if (lang!='en') {
    allElements.forEach(element => {
        // Перебираем всех детей элемента
        for (let i = 0; i < element.childNodes.length; i++) {
            const node = element.childNodes[i];

            // Проверяем, если это текстовый узел (TEXT_NODE)
            if (node.nodeType === Node.TEXT_NODE) {
                // console.log(node)
                const textforelement = node.parentNode.getAttribute("textforelement"); // Получаем текст и убираем пробелы по краям
                // console.log(node)
                // Если есть перевод для этого текста в нашем словаре
                if (textforelement) {
                    // console.log(node)
                    node.textContent = translations[lang][textforelement]; // Заменяем на перевод
                }
            }
            //Для всплывающих элементов tooltip
            if (node.parentNode.getAttribute("data-bs-toggle") == "tooltip") {
                if (translations[lang][node.parentNode.id]) {
                    node.parentNode.setAttribute("title", translations[lang][node.parentNode.id])
                }
                if (node.parentNode.getAttribute("textforelement")) {
                    // console.log(node.parentNode.getAttribute("textforelement"));
                    node.parentNode.setAttribute("title", translations[lang][node.parentNode.getAttribute("textforelement")])
                }
            }
        }
    });
    // }
}