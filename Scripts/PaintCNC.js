// ============================================================================
// PaintCNC.js — Визуализация CNC-раскладки на Canvas и SVG
// ============================================================================
// Отрисовывает контуры деталей и траекторию резки на элементе <canvas>.
// Поддерживает команды G-кода: G00 (холостой ход), G01 (линия),
// G02/G03 (дуга по/против часовой стрелки).
// Также может строить SVG-представление (функция drawCNC2).
// ============================================================================

var currentcanva; // ID текущего элемента canvas

/**
 * Запросить данные раскладки с сервера и отрисовать.
 * @param {number} cncid   - ID CNC-программы
 * @param {string} canvaid - ID элемента canvas на странице
 */
function visual(cncid, canvaid) {
    currentcanva = canvaid;
    MyRequest("getIMAGE?cncID=" + cncid, readshowcnc);
}

/**
 * Обработчик ответа сервера — парсинг JSON и отрисовка.
 */
function readshowcnc() {
    if (xmlRequest.readyState == 4) {
        if (xmlRequest.status == 200) {
            var response = JSON.parse(xmlRequest.responseText);
            drawcnc(response, currentcanva);
        }
    }
}

// === Масштабирование координат ===

var absscale; // Абсолютный масштаб (пикселей на мм)
var xofs;     // Смещение по X (минимальная координата)
var yofs;     // Смещение по Y (минимальная координата)
var ch;       // Высота canvas

/**
 * Преобразование координаты X из мм в пиксели canvas.
 * @param {number} x - Координата X в мм
 * @returns {number} Координата X в пикселях
 */
function mapcoordx(x) {
    return (x - xofs) / absscale + 5;
}

/**
 * Преобразование координаты Y из мм в пиксели canvas.
 * @param {number} y - Координата Y в мм
 * @returns {number} Координата Y в пикселях
 */
function mapcoordy(y) {
    return (y - yofs) / absscale + 5;
}

/**
 * Рассчитать масштаб для вписывания раскладки в canvas.
 * @param {number} xmin, ymin - Минимальные координаты раскладки
 * @param {number} xmax, ymax - Максимальные координаты раскладки
 * @param {number} canvawidth, canvaheigth - Размеры canvas в пикселях
 */
function calcscale(xmin, ymin, xmax, ymax, canvawidth, canvaheigth) {
    var scaleX = (xmax - xmin) / (canvawidth - 10);
    var scaleY = (ymax - ymin) / (canvaheigth - 10);
    // Берём больший масштаб, чтобы раскладка вписалась целиком
    absscale = (scaleX > scaleY) ? scaleX : scaleY;
    xofs = xmin;
    yofs = ymin;
    ch = canvaheigth;
}

var xmin = 0, xmax = 6000, ymin = 0, ymax = 2000;

/**
 * Отрисовать массив примитивов CNC-раскладки на canvas.
 * Последний элемент массива содержит габариты листа.
 *
 * Типы команд (поле Comand):
 *   1 = fastmove (холостой ход) — красный пунктир
 *   2 = cuteline (рез по прямой) — синяя линия
 *   3 = cutearc  (дуга G02, по часовой) — синяя дуга
 *   4 = cutearc2 (дуга G03, против часовой) — синяя дуга
 *
 * @param {Array} a   - Массив примитивов (step) от сервера
 * @param {string} can - ID элемента canvas
 */
function drawcnc(a, can) {
    var lbl = document.getElementById(can);
    lbl.style.display = 'block';
    var c = lbl.getContext("2d");

    // Установить размер canvas по его CSS-размерам
    lbl.width = lbl.offsetWidth;
    lbl.height = lbl.offsetHeight;

    // Перевернуть координатную систему (Y вверх, как в CNC)
    c.translate(0, lbl.height);
    c.scale(1, -1);

    // Рассчитать масштаб по габаритам из последнего элемента массива
    var bounds = a[a.length - 1];
    calcscale(bounds.Xstart, bounds.Ystart, bounds.Xend, bounds.Yend, lbl.width, lbl.height);
    xmin = bounds.Xstart; ymin = bounds.Ystart;
    xmax = bounds.Xend;   ymax = bounds.Yend;

    c.lineWidth = 1;
    c.strokeStyle = "blue";
    c.beginPath();

    for (var i = 0; i < a.length; i++) {
        if (a[i].Comand == 2) {
            // Рез по прямой (G01)
            c.lineTo(mapcoordx(a[i].Xend), mapcoordy(a[i].Yend));
            c.stroke();
        }
        else if ((a[i].Comand == 3) || (a[i].Comand == 4)) {
            // Дуга (G02 или G03)
            c.closePath();
            var Cx = mapcoordx(a[i].Xcentr);
            var Cy = mapcoordy(a[i].Ycentr);
            var A1 = a[i].StartAngle / 180 * Math.PI;
            var A2 = (A1 == a[i].EndAngle / 180 * Math.PI)
                ? A1 + Math.PI * 2
                : a[i].EndAngle / 180 * Math.PI;
            var R = a[i].Radius / absscale;
            var anticlockwise = (a[i].Comand == 3);
            c.beginPath();
            c.arc(Cx, Cy, R, A1, A2, anticlockwise);
            c.stroke();
        }
        else if (a[i].Comand == 1) {
            // Холостой ход (G00) — красный пунктир
            c.closePath();
            c.beginPath();
            c.strokeStyle = "red";
            c.setLineDash([2, 4]);
            c.moveTo(mapcoordx(a[i].Xstart), mapcoordy(a[i].Ystart));
            c.lineTo(mapcoordx(a[i].Xend), mapcoordy(a[i].Yend));
            c.stroke();
            // Вернуть стиль для линий реза
            c.closePath();
            c.beginPath();
            c.strokeStyle = "blue";
            c.setLineDash([1, 0]);
            c.moveTo(mapcoordx(a[i].Xend), mapcoordy(a[i].Yend));
        }
    }
    c.stroke();
}

/**
 * Обработчик ответа для SVG-визуализации.
 */
function svgShow() {
    if (xmlRequest.readyState == 4) {
        if (xmlRequest.status == 200) {
            var response = JSON.parse(xmlRequest.responseText);
            drawCNC2(response, currentcanva);
        }
    }
}

/**
 * Создать SVG-элемент с контурами раскладки.
 * Альтернативный способ визуализации (вместо canvas).
 * Получает готовый SVG path-data от сервера.
 *
 * @param {Object} a   - Объект с полями: path, minX, minY, maxX, maxY
 * @param {string} can - ID элемента canvas (SVG вставляется рядом)
 */
function drawCNC2(a, can) {
    var s = document.getElementById(can).parentNode;
    var svgns = 'http://www.w3.org/2000/svg';

    // Создать корневой SVG-элемент
    var sh = document.createElementNS(svgns, 'svg');
    sh.setAttributeNS(null, "width", "600");
    sh.setAttributeNS(null, "height", "250");
    sh.setAttribute("xmlns", svgns);
    sh.setAttribute("version", "1.1");
    sh.setAttributeNS(null, "viewBox", a.minX + " " + a.minY + " " + a.maxX + " " + a.maxY);
    sh.setAttributeNS(null, "id", "SVG_" + can);
    sh.setAttribute("overflow", "visible");
    s.append(sh);

    // Создать path-элемент с контурами деталей
    var b = document.getElementById("SVG_" + can);
    var heart = document.createElementNS(svgns, "path");
    heart.setAttributeNS(null, "d", a.path);
    b.append(heart);
}
