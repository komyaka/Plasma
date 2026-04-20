using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;
using System.Linq;
using System.Web;
using static Plazma.Controllers.Users;
using System.Web.Mvc;

namespace Plazma.Controllers
{
    //Контроллер управления отгрузками, включает методы:
    // View - просмотр всех отгрузок за период;
    // Apply - отметить отгрузку
    //         список деталей напротив каждой позиции дропбох с заполненым 
    //         от 1- максимального количества деталей. и одной кнопкой  отправить.
    // 
    public class SHIPMENTController : Controller
    {
        // GET: SHIPMENT
        public Users users = new Users();
        // Создать объект cookie-набора
        public _user currentuser;
        PartsClass parts = new PartsClass();
        public ActionResult ViewShip(int periodindays = 180)
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & parts.getChapterCode("viewShip")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            parts.readShipments();
            ViewBag.shipments = parts.Shipments;
            return View();
        }
        public ActionResult ShipOnePosition(string partName="",int Quantity=0)
        {
            //insert into SHIPMENT (PARTID,SHIPED,SHIPTIME,ORDERNAME) select ID as PARTID,QUANTITYSUMM as SHIPED,CAST('" + DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("fr-FR")).ToString().Replace("/", ".") + "'AS datetime2) as SHIPTIME, '" + ordername + "' AS ORDERNAME from PARTS where name like'%" + ordername + "%'
            // Нужно написать запрос на добавление 1 детали в отгрузку по названию. 
            String req = "insert into SHIPMENT (PARTID,SHIPED,SHIPTIME,ORDERNAME) values ((Select id from PARTS where name='" + partName + "')," + Quantity.ToString() +
                ",CURRENT_TIMESTAMP,'" + partName.Substring(0, partName.LastIndexOf(" т")) + "')";
            int res = parts.FreeRequestToBD(req);
            return Json(new { status = "ok", Ret = res }, behavior: JsonRequestBehavior.AllowGet); ;
        }
        public ActionResult ApplyShip(string Name)
        {
            currentuser = users.getCurrentUser();

            ViewBag.User = currentuser;
            if ((currentuser.Privilegies & 128) != 0) parts.Ship(Name);
            parts.readShipments();
            ViewBag.shipments = parts.Shipments;
            return View("ViewShip");
        }
        
        private bool compare(float x, float y) => (((Math.Abs(x - y) / y)<0.01&&(Math.Abs(x-y)<3))|| Math.Abs(x - y)<3);
        private FinCalc machdata(List<PartsClass._Shipment> sdata, FinCalc calcdata)
        {
            FinCalc maches = new FinCalc();
            FinCalc nomaches = new FinCalc();
            int ind = -1;
            int count = 0;
            int nomachcount = 0;
            foreach (PartsClass._Shipment s in sdata) 
            {
                ind=calcdata.Parts.FindIndex(x => ((x.tickness == s.tikcness)&&((compare(x.Size_X,s.width) && compare(x.Size_Y , s.heigth))|| (compare(x.Size_X , s.heigth) && compare(x.Size_Y , s.width)))  ));
                if (ind >= 0)
                {
                    count++;
                    FinCalc.FPart newShipment = new FinCalc.FPart();
                    newShipment.Id = count;
                    newShipment.shiped = s.Shiped;
                    newShipment.Name = s.orderName;
                    newShipment.shiptime = s.Shiptime;
                    newShipment.Size_X = s.width;
                    newShipment.Size_Y = s.heigth;
                    newShipment.tickness = s.tikcness;
                    newShipment.QuantitySummary = calcdata.Parts[ind].QuantitySummary;
                    newShipment.cost = calcdata.Parts[ind].cost;
                    newShipment.shiptime = s.Shiptime;// calcdata.Parts[ind].shiptime;
                    maches.Parts.Add(newShipment);
                    calcdata.Parts.Remove(calcdata.Parts[ind]);
                }
                
                else//foreach (FinCalc.FPart sd in calcdata.Parts)
                {
                    nomachcount++;
                    FinCalc.FPart newShipment = new FinCalc.FPart();
                    newShipment.Id = nomachcount;
                    newShipment.shiped = s.Shiped;
                    newShipment.Name = "!"+s.orderName+"!";
                    newShipment.shiptime = s.Shiptime;
                    newShipment.Size_X = s.width;
                    newShipment.Size_Y = s.heigth;
                    newShipment.tickness = s.tikcness;
                    newShipment.QuantitySummary = 0;
                    newShipment.cost = -1;
                    newShipment.shiptime = s.Shiptime;// DateTime.Parse("26-08-1996");
                    maches.Parts.Add(newShipment);
                    //calcdata.Parts.Remove(calcdata.Parts[ind]);
                }
            }
//            if (nomaches.Parts.Count>0) foreach (FinCalc.FPart x in nomaches.Parts) { maches.Parts.Add(x); }
            return maches;
        }
        public ActionResult PartialShip(string Name)
        {
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            ViewBag.chapters_ShipManaged = parts.getChapterCode("ShipManaged");
            string hdone = "off";
            string hcute = "off";
            int minwidth = 0;
            int maxwidth = 100000;
            int minheigth = 0;
            int maxheigth = 100000;
            string tikn = "*";
            string split = "off";
            string sort = "TICKNESS";

            bool hidedone = true;
            bool hidecute = false;
            if (hdone == "on") hidedone = true; else hidedone = false;
            if (hcute == "on") hidecute = true; else hidecute = false;
            //Составить SQL запрос для выборки деталей
            string qwidth = "";
            string qheigth = "";
            string qname = "";
            string qtikn = "";
            if (minwidth > maxwidth) { int a = maxwidth; minwidth = maxwidth; maxwidth = a; }
            if (minheigth > maxheigth) { int a = maxheigth; minheigth = maxheigth; maxheigth = a; }
            if (maxwidth < 0)
                if (minwidth < 0) qwidth = "";
                else qwidth = "WIDTH='" + minwidth.ToString() + "'";
            else qwidth = "(CAST(s1.SIZE_X as float) BETWEEN " + minwidth.ToString() + " AND " + maxwidth.ToString() + ")";
            if (maxheigth < 0)
                if (minheigth < 0) qheigth = "";
                else qheigth = "heigth='" + minheigth.ToString() + "'";
            else qheigth = "(CAST(s1.SIZE_Y as float) BETWEEN " + minheigth.ToString() + " AND " + maxheigth.ToString() + ")";
            if (Name != "*") qname = "s1.NAME LIKE '%" + Name + "%'";
            if (tikn != "*" && tikn != "Любая") qtikn = "s1.TICKNESS='" + tikn + "'";
            string finnalyquestion = "";
            if (qwidth.Length + qheigth.Length + qname.Length + qtikn.Length > 0)
                finnalyquestion += " WHERE ";
            if (qname != "") finnalyquestion += qname;
            if ((qtikn != "") && (finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND "; finnalyquestion += qtikn;
            if ((qwidth != "") && (finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND "; finnalyquestion += qwidth;
            if ((qheigth != "") && (finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND "; finnalyquestion += qheigth;
            if (split != "off") { parts.ReadParts("SELECT * FROM PARTS s1" + finnalyquestion); ViewBag.split = true; }
            else
            {
                //finnalyquestion = "select ROW_NUMBER() OVER(ORDER BY NAME ASC) AS ID,NAME,   SUM(QUANTITY)AS QUANTITY,SUM(QUANTITYCUTTED)AS QUANTITYCUTTED,SUM(QUANTITYSUMM)AS QUANTITYSUMM,TICKNESS,SIZE_X,SIZE_Y,(select                                                                                      CAST(CNCID as varchar(5)) + '; ' from PLASMA.dbo.PARTS s2 where s2.NAME=s1.NAME FOR XML PATH('')) as CNCID, ARHIVE from PLASMA.dbo.PARTS s1" + finnalyquestion+                                               " GROUP BY ARHIVE, NAME ,  TICKNESS,SIZE_X,SIZE_Y";
                finnalyquestion = "select ROW_NUMBER() OVER(ORDER BY NAME ASC) AS ID,NAME,SUM(s1.QUANTITY)AS QUANTITY,SUM(QUANTITYCUTTED)AS QUANTITYCUTTED,SUM(QUANTITYSUMM)AS QUANTITYSUMM,s1.TICKNESS,(select IIF((select '-' from CNCFILES where id = s2.CNCID and QUANTITYDONE >= QUANTITY) IS NULL,'','-')+CAST(CNCID as varchar(5)) + ';'  from PLASMA.dbo.PARTS s2 where s2.NAME=s1.NAME FOR XML PATH('')) as CNCID, SIZE_X, SIZE_Y,s1.ARHIVE,max(ADDEDTIME) as 'addate',(select DISTINCT Sum(shiped) From SHIPMENT where PARTID in (Select id from parts where NAME=s1.name)) as Shipped from PARTS s1,CNCFILES" + finnalyquestion + (finnalyquestion.IndexOf("WHERE") > 0 ? "and (s1.CNCID=CNCFILES.ID)" : "") + "GROUP BY s1.ARHIVE,NAME ,s1.TICKNESS,SIZE_X,SIZE_Y";
                parts.ReadParts(finnalyquestion);
                ViewBag.split = false;
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            if (sort == "ID") parts.Parts.Sort((a, b) => a.Id.CompareTo(b.Id));
            else if ((sort == "NAME")) parts.Parts.Sort((a, b) => a.Name.CompareTo(b.Name));
            else if ((sort == "TICKNESS")) parts.Parts.Sort((a, b) => Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness)));
            else if ((sort == "SIZE")) parts.Parts.Sort((a, b) => Convert.ToDouble(a.Size_X).CompareTo(Convert.ToDouble(b.Size_X)) != 0 ? Convert.ToDouble(a.Size_X).CompareTo(Convert.ToDouble(b.Size_X)) : Convert.ToDouble(a.Size_Y).CompareTo(Convert.ToDouble(b.Size_Y)));
            else if ((sort == "QTY")) parts.Parts.Sort((a, b) => a.Quantity.CompareTo(b.Quantity));
            ViewBag.Parts = parts.Parts;
            ViewBag.SORT = sort;
            ViewBag.t = parts.ticnesslist();
            ViewBag.hcute = hidecute;
            ViewBag.hdone = hidedone;
            ViewBag.currenttime = DateTime.Now;
            return View();
        }
        public ActionResult XLS(string OrderName)
        {
        // link="http://localhost:50386/SHIPMENT/xls?OrderName=%D0%A2%D0%B5%D1%85%D0%BD%D0%BE%D0%BB%D0%BE%D0%B3%D0%B8%D0%B8%20%D0%9E%D0%9E%D0%9E%2020,05"
        // Deeplink http://localhost:50386/SHIPMENT/xls?OrderName=%D0%A4%D0%B8%D0%BB%D0%B8%D0%BF%D0%BF%D0%BE%D0%B2%2015,04
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            FinCalc list = new FinCalc();
            ViewBag.Fname = FinCalc.getfilefromOrdername(OrderName);
            return View(); 
        }
    }
}