function visual(cncid, canvaid) {
    currentcanva = canvaid;
    MyRequest("getIMAGE?cncID=" + cncid, readshowcnc);
}

function readshowcnc() {
    // Проверить успешность получения ответа
    if (xmlRequest.readyState == 4) {
        if (xmlRequest.status == 200) {
            var response = JSON.parse(xmlRequest.responseText);
            drawcnc(response, currentcanva);
        }
    }
}
function mapcoordx(x) {
    return (x - xofs) / absscale + 5;
}
function mapcoordy(y) {
    return /*ch-*/((y - yofs) / absscale + 5);//отразить по вертикали но нужно пересчитывать StartAngle И EndAngle
    //270-angle
}
var absscale;
var xofs;//y -offset
var yofs;//x-- offset
var ch; //canvas heigth
function calcscale(xmin, ymin, xmax, ymax, canvawidth, canvaheigth) {
    scaleX = (xmax - xmin) / (canvawidth - 10);
    scaleY = (ymax - ymin) / (canvaheigth - 10);
    if (scaleX > scaleY) { absscale = scaleX; } else { absscale = scaleY; }
    xofs = xmin;
    yofs = ymin;
    ch = canvaheigth;
}
var xmin = 0; xmax = 6000, ymin = 0, ymax = 2000;
//Отрисосывает на канве  can значения берёт из массива примитивов который передаётся в 'a'
function drawcnc(a, can) {
    var lbl = document.getElementById(can);
    lbl.style.display = 'block';
    var c = lbl.getContext("2d");
    lbl.width = lbl.offsetWidth;
    lbl.height = lbl.offsetHeight;
    c.translate(0, lbl.height);
    c.scale(1, -1);
    calcscale(a[a.length - 1].Xstart, a[a.length - 1].Ystart, a[a.length - 1].Xend, a[a.length - 1].Yend, lbl.width, lbl.height);
    let tnp = a[a.length - 1];
    xmin = tnp.Xstart; ymin = tnp.Ystart; xmax = tnp.Xend; ymax = tnp.Yend;
    c.lineWidth = 1;
    c.strokeStyle = "blue";
    c.beginPath();
    for (i = 0; i < a.length; i++) {
        if (a[i].Comand == 2) { c.lineTo(mapcoordx(a[i].Xend), mapcoordy(a[i].Yend)); c.stroke(); }
        else if ((a[i].Comand == 3) || (a[i].Comand == 4)) {
            c.closePath();
            Cx = mapcoordx(a[i].Xcentr);
            Cy = mapcoordy(a[i].Ycentr);
            A1 = a[i].StartAngle / 180 * Math.PI;
            A2 = A1 == a[i].EndAngle / 180 * Math.PI ? A1 + Math.PI * 2 : a[i].EndAngle / 180 * Math.PI;
            R = a[i].Radius / (absscale);
            anclockwise = (a[i].Comand == 3);
            c.beginPath();
            c.arc(Cx, Cy, R, A1, A2, anclockwise);
            c.stroke();
        }
        else if (a[i].Comand == 1) {
            c.closePath();
            c.beginPath();
            c.strokeStyle = "red";
            c.setLineDash([2, 4]);
            c.moveTo(mapcoordx(a[i].Xstart), mapcoordy(a[i].Ystart));
            c.lineTo(mapcoordx(a[i].Xend), mapcoordy(a[i].Yend));
            c.stroke();
            c.closePath();
            c.beginPath();
            c.strokeStyle = "blue";
            c.setLineDash([1, 0]);
            c.moveTo(mapcoordx(a[i].Xend), mapcoordy(a[i].Yend));
        }
    }
    c.stroke();
    //drawCNC2(a, can,xmin,ymin, xmax,ymax);
    //MyRequest("Home/getIMAGE_SVG?cncID=" + can.substring(6),svgShow);
}
function svgShow()
{
    if (xmlRequest.readyState == 4) {
        if (xmlRequest.status == 200) {
            var response = JSON.parse(xmlRequest.responseText);
            drawCNC2(response, currentcanva);
        }
    }
}
function drawCNC2(a, can) {
  //  if (typeof (a) != 'string') return;
    let s = document.getElementById(can).parentNode;
    let w = 500;
    let h = 250;
    var svgns = 'http://www.w3.org/2000/svg';
    var sh = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
    sh.setAttributeNS(null, "width", "600");
    sh.setAttributeNS(null, "height", "250");
    sh.setAttribute("xmlns", "http://www.w3.org/2000/svg");
    sh.setAttribute("version", "1.1");
    sh.setAttribute( "viewBox", "" + a.minX + " " + a.minY + " " + a.maxX + " " + a.maxY + " ");
    sh.setAttributeNS(null, "viewBox", "" + a.minX + " " + a.minY + " " + a.maxX + " " + a.maxY + " ");
    sh.setAttributeNS(null, "id", "SVG_"+can);
    sh.setAttribute("overflow", "visible");
    s.append(sh);
    var b = document.getElementById("SVG_" + can);
    var heart = document.createElementNS(svgns, "path");
    heart.setAttributeNS(null, "d",a.path);
    b.append(heart);
}
