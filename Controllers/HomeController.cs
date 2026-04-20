using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Text.Json;
using Newtonsoft.Json;
using System.DirectoryServices.AccountManagement;
using static Plazma.Controllers.Users;
using Plazma.Models;
using Plazma.Models.NC;
using GoogleSpreadSheetsWorking;

namespace Plazma.Controllers
{
    /* Привилегии :
     0 - любой пользователь.
     2 - управление программами (добавить, удалить, изменить количество, заменить)
     4 - управление деталями изменить количество
     8 - добавить листы.
     16 - отметить листы
     32 - сформировать заказ, редактировать заказы.
     64 - Просмотр отгрузок
     128 - Управление отгрузками
     256 - просмотр калькуляций
     512
     1024
     2048
     4096
     8192
     16384
     32768 - управление привилегиями.
         */
    public class HomeController : Controller
    {
        /*   public struct _returnmarksheetresponse
           {
               public int cncid;
               public int sheetid;
               public bool result;
               public string user;
           }
     /*      public struct _SVGAnswer
           {
               public string path;
               public int minX, minY, maxX, maxY;
           }*/
        // Создать объект cookie-набора
        public Users users = new Users();
        // Создать объект cookie-набора
        public _user currentuser;
        public PartsClass partsClass = new PartsClass();
        [HttpGet]
        public ActionResult Index(string sort = "TICKNESS", string psort = "", bool cb1 = false)
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("index")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "About" }));
            ViewBag.User = currentuser;

            ViewBag.SORT = sort;
            ViewBag.CB1 = cb1 ? "ON" : "";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            if (sort == "ID")
            {
                if (psort == "ID") { partsClass.CNCs.Sort((b, a) => a.Id.CompareTo(b.Id)); ViewBag.SORT = "ID_R"; }
                else partsClass.CNCs.Sort((a, b) => a.Id.CompareTo(b.Id));
            }

            else if (sort == "NAME")
            {
                //                if (psort == "NAME") { partsClass.CNCs.Sort((b, a) => (a.tickness == b.tickness) ? (a.OriginalName.CompareTo(b.OriginalName)) : Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness))); ViewBag.SORT = "NAME_R"; }
                //                else partsClass.CNCs.Sort((a, b) => (a.tickness == b.tickness) ? (a.OriginalName.CompareTo(b.OriginalName)) : Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness)));
                if (psort == "NAME") { partsClass.CNCs.Sort((b, a) => (a.tickness == b.tickness) ? (a.OriginalName.CompareTo(b.OriginalName)) : (a.OriginalName.CompareTo(b.OriginalName))); ViewBag.SORT = "NAME_R"; }
                else partsClass.CNCs.Sort((a, b) => (a.tickness == b.tickness) ? (a.OriginalName.CompareTo(b.OriginalName)) : (a.OriginalName.CompareTo(b.OriginalName)));
            }
            else if (sort == "TICKNESS")
            {
                if (psort == "TICKNESS") { partsClass.CNCs.Sort((b, a) => Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness))); ViewBag.SORT = "TICKNESS_R"; }
                else partsClass.CNCs.Sort((a, b) => Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness)));
            }
            else if (sort == "QTY")
            {
                if (psort == "QTY") { partsClass.CNCs.Sort((b, a) => a.Quantity.CompareTo(b.Quantity)); ViewBag.SORT = "QTY_R"; }
                else partsClass.CNCs.Sort((a, b) => a.Quantity.CompareTo(b.Quantity));
            }
            ViewBag.CNCs = partsClass.CNCs;
            ViewBag.currenttime = DateTime.Now;
            return View();
        }
        public ActionResult About()
        {
            currentuser = users.getCurrentUser();
            ViewBag.Message = "В доработке";
            ViewBag.User = currentuser;

            return View();
        }
        public ActionResult Contact()
        {
            currentuser = users.getCurrentUser();
            ViewBag.Message = "В доработке";
            ViewBag.User = currentuser;
            return View();
        }
        public ActionResult CNCQR(int ID = -1)
        {
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            if (ID > 0)
            {
                ViewBag.Message = "Установка статуса программы";
                int Index = partsClass.CNCs.FindIndex(x => x.Id == ID);
                if (Index >= 0) { ViewBag.CNC = partsClass.CNCs[Index]; ViewBag.found = true; }  // ИСПРАВЛЕНО: >= 0, т.к. FindIndex возвращает 0 для первого элемента else { ViewBag.found = false; }

                return View();
            }
            else
            {
                string[] rrr = { "Программы с ID=" + ID + ", В базе данных нет" };
                return View(rrr);
            }
        }
        public ActionResult CNCViewJnVUE()
        {
            ViewBag.User = users.getCurrentUser();
            return View();
        }

        public ActionResult Sheets(string sort = "ID")
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("viewSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            ViewBag.Message = "Учет листов.";
            //ViewBag.Sheets = partsClass.Sheets;
            ViewBag.m = partsClass.materials;
            if (sort == "ID") partsClass.Sheets.Sort((a, b) => a.Id.CompareTo(b.Id));
            else if ((sort == "STEEL")) partsClass.Sheets.Sort((a, b) => a.Matherial.CompareTo(b.Matherial));
            else if ((sort == "TICKNESS")) partsClass.Sheets.Sort((a, b) => Convert.ToDouble(a.Tickness).CompareTo(Convert.ToDouble(b.Tickness)));
            else if ((sort == "SIZE")) partsClass.Sheets.Sort((a, b) => Convert.ToDouble(a.Width).CompareTo(Convert.ToDouble(b.Width)) != 0 ? Convert.ToDouble(a.Width).CompareTo(Convert.ToDouble(b.Width)) : Convert.ToDouble(a.Heigth).CompareTo(Convert.ToDouble(b.Heigth)));
            ViewBag.Sheets = partsClass.Sheets;
            ViewBag.SORT = sort;
            ViewBag.currenttime = DateTime.Now;
            return View();
        }
        public ActionResult SheetsArrival(string sort = "DATE", float tickness = -1, int material = -1, string datestart = "", string dateend = "", string Document = "", int Width = -1, int Heigth = -1)
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("arrivalSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            PartsClass SpartsClass = new PartsClass(0);
            //Подготовка sql-запроса
            string qwestion = "SELECT [MATHERIAL],[TICKNESS],[WIDTH],[HEIGTH],sum([QUANTITY]) as Quantity,[DOKUMENT],[DATE],[OWNER] FROM [PLASMA].[dbo].[SHEETS] where [TICKNESS]>0";
            if (tickness > 0) qwestion += " and TICKNESS=" + tickness.ToString();
            if (material > 0) qwestion += " and MATHERIAL=" + material.ToString();
            if (datestart.Length > 0) qwestion += " AND DATE>'" + datestart + "'";  // ИСПРАВЛЕНО: пробел перед AND + кавычки
            if (dateend.Length > 0) qwestion += " AND DATE<'" + dateend + "'";  // ИСПРАВЛЕНО: пробел перед AND + кавычки
            if (Document.Length > 0) qwestion += " AND DOKUMENT like '%" + Document + "%'";  // ИСПРАВЛЕНО: пробел перед AND
            if (Width > 0) qwestion += " AND WIDTH=" + Width.ToString();
            if (Heigth > 0) qwestion += " AND HEIGTH=" + Heigth.ToString();
            qwestion += "group by [OWNER], DOKUMENT,[DATE],MATHERIAL,TICKNESS,WIDTH,HEIGTH "; // ORDER BY DATE DESC,MATHERIAL,TICKNESS,HEIGTH,WIDTH,DOKUMENT ";
            if (sort == "DATE") qwestion += " Order by DATE DESC";
            else if (sort == "QTY") qwestion += " Order by Quantity";
            else if (sort == "TIKN") qwestion += " Order by TICKNESS";
            else if (sort == "MATHERIAL") qwestion += " Order by MATHERIAL";
            else if (sort == "WIDTH") qwestion += " Order by WIDTH";
            else if (sort == "HEIGTH") qwestion += " Order by HEIGTH";
            SpartsClass.ReadSheets(qwestion);
            ViewBag.Message = "Приход металла";
            //ViewBag.Sheets = partsClass.Sheets;
            ViewBag.m = partsClass.materials;
            ViewBag.t = partsClass.ticnesslist();
            ViewBag.Sheets = SpartsClass.Sheets;
            ViewBag.currenttime = DateTime.Now;
            return View();
        }
        public ActionResult Parts(string sort = "ID", string psort = "", string split = "off", string _Name = "*", string tikn = "*", int minwidth = 0, int maxwidth = 100000, int minheigth = 0, int maxheigth = 100000, string hdone = "off", string hcute = "on")
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("Parts")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
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
            if (_Name != "*") qname = "s1.NAME LIKE '%" + _Name + "%'";
            if (tikn != "*" && tikn != "Любая") qtikn = "s1.TICKNESS='" + tikn + "'";
            string finnalyquestion = "";
            if (qwidth.Length + qheigth.Length + qname.Length + qtikn.Length > 0)
                finnalyquestion += " WHERE ";
            if (qname != "") finnalyquestion += qname;
            if ((qtikn != "") && (finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND "; finnalyquestion += qtikn;
            if ((qwidth != "") && (finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND "; finnalyquestion += qwidth;
            if ((qheigth != "") && (finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND "; finnalyquestion += qheigth;
            if ((finnalyquestion.Length - finnalyquestion.IndexOf("WHERE") > 6) && (finnalyquestion.Length - finnalyquestion.IndexOf("AND") > 4)) finnalyquestion += " AND s1.ARHIVE IS NULL ";
            if (split != "off") { partsClass.ReadParts("SELECT * FROM PARTS s1" + finnalyquestion); ViewBag.split = true; }
            else {
                //finnalyquestion = "select ROW_NUMBER() OVER(ORDER BY NAME ASC) AS ID,NAME,   SUM(QUANTITY)AS QUANTITY,SUM(QUANTITYCUTTED)AS QUANTITYCUTTED,SUM(QUANTITYSUMM)AS QUANTITYSUMM,TICKNESS,SIZE_X,SIZE_Y,(select                                                                                      CAST(CNCID as varchar(5)) + '; ' from PLASMA.dbo.PARTS s2 where s2.NAME=s1.NAME FOR XML PATH('')) as CNCID, ARHIVE from PLASMA.dbo.PARTS s1" + finnalyquestion+                                               " GROUP BY ARHIVE, NAME ,  TICKNESS,SIZE_X,SIZE_Y";
                finnalyquestion = "select ROW_NUMBER() OVER(ORDER BY NAME ASC) AS ID,NAME,SUM(s1.QUANTITY)AS QUANTITY,SUM(QUANTITYCUTTED)AS QUANTITYCUTTED,SUM(QUANTITYSUMM)AS QUANTITYSUMM,s1.TICKNESS,(select IIF((select '-' from CNCFILES where id = s2.CNCID and QUANTITYDONE >= QUANTITY) IS NULL,'','-')+CAST(CNCID as varchar(7)) + ';'  from PLASMA.dbo.PARTS s2 where s2.NAME=s1.NAME FOR XML PATH('')) as CNCID, SIZE_X, SIZE_Y,s1.ARHIVE,max(ADDEDTIME) as 'addate' from PARTS s1,CNCFILES" + finnalyquestion + (finnalyquestion.IndexOf("WHERE") > 0 ? "and (s1.CNCID=CNCFILES.ID)" : "") + "GROUP BY s1.ARHIVE,NAME ,s1.TICKNESS,SIZE_X,SIZE_Y";
                partsClass.ReadParts(finnalyquestion);
                ViewBag.split = false;
            }


            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            if (sort == "ID")
            {
                if (psort != "ID") partsClass.Parts.Sort((a, b) => a.Id.CompareTo(b.Id));
                else partsClass.Parts.Sort((b, a) => a.Id.CompareTo(b.Id));
            }
            else if (sort == "NAME")
            {
                if (psort != "NAME") partsClass.Parts.Sort((a, b) => a.Name.CompareTo(b.Name));
                else partsClass.Parts.Sort((b, a) => a.Name.CompareTo(b.Name));
            }
            else if (sort == "TICKNESS")
            {
                if (psort != "TICKNESS") partsClass.Parts.Sort((a, b) => Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness)));
                else partsClass.Parts.Sort((b, a) => Convert.ToDouble(a.tickness).CompareTo(Convert.ToDouble(b.tickness)));
            }
            else if (sort == "SIZE")
            {
                if (psort != "SIZE") partsClass.Parts.Sort((a, b) => Convert.ToDouble(a.Size_X).CompareTo(Convert.ToDouble(b.Size_X)) != 0 ? Convert.ToDouble(a.Size_X).CompareTo(Convert.ToDouble(b.Size_X)) : Convert.ToDouble(a.Size_Y).CompareTo(Convert.ToDouble(b.Size_Y)));
                else partsClass.Parts.Sort((b, a) => Convert.ToDouble(a.Size_X).CompareTo(Convert.ToDouble(b.Size_X)) != 0 ? Convert.ToDouble(a.Size_X).CompareTo(Convert.ToDouble(b.Size_X)) : Convert.ToDouble(a.Size_Y).CompareTo(Convert.ToDouble(b.Size_Y)));
            }
            else if (sort == "QTY") {
                if (psort != "QTY") partsClass.Parts.Sort((a, b) => a.Quantity.CompareTo(b.Quantity));
                else partsClass.Parts.Sort((b, a) => a.Quantity.CompareTo(b.Quantity));
            }
            ViewBag.Parts = partsClass.Parts;
            if (sort != psort) ViewBag.SORT = sort; else ViewBag.SORT = sort + "_r";
            ViewBag.t = partsClass.ticnesslist();
            ViewBag.hcute = hidecute;
            ViewBag.hdone = hidedone;
            ViewBag.currenttime = DateTime.Now;
            return View();
        }
        public ActionResult PartsList(int CNCID = -1)
        {
            //Составить SQL запрос для выборки деталей
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & (partsClass.getChapterCode("addSheet") + partsClass.getChapterCode("viewOrders"))) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            partsClass.ReadParts("SELECT * FROM PARTS where CNCID=" + CNCID);
            ViewBag.Parts = partsClass.Parts;
            int cncindex = partsClass.CNCs.FindIndex(x => x.Id == CNCID);
            NCreader nc = new NCreader();
            try
            {
                nc = new NCreader(partsClass.CNCs[cncindex].FileName);
            }
            catch
            {

                return View("Error", new Error { number = 7, text = "Файл CNC Не найден или перенесён в архив" });
            }
            ViewBag.info = nc;
            return View();
        }
        public ActionResult Calculation(string Name = "")
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("calculation")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            string xlsName;
            if ((currentuser.Privilegies & 64) != 0)
            {
                FinCalc list = new FinCalc();
                xlsName = FinCalc.getfilefromOrdername(Name);
                ViewBag.Fname = Name;
                try
                {
                    list.readfromxls(xlsName);
                }
                catch (Exception e) {
                    return View(Error(24,e.Message));
                }
                ViewBag.FinParts = list.Parts;
                ViewBag.User = currentuser;
            }
            else { FinCalc list = new FinCalc(); ViewBag.FinParts = list.Parts; }

            return View();
        }
        public ActionResult NewSheet()
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("addSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            ViewBag.t = partsClass.ticnesslist();
            ViewBag.mt = partsClass.materials;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            ViewBag.now = DateTime.Now.ToString("d").ToString().Replace("-", ".");
            return View();
        }
        public ActionResult NewSheetonVue()
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("addSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            ViewBag.t = partsClass.ticnesslist();
            ViewBag.mt = partsClass.materials;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            ViewBag.now = DateTime.Now.ToString("d").ToString().Replace("-", ".");
            return View();
        }
        public ActionResult MarkSheet()
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("markSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            PartsClass spartsClass = new PartsClass();
            List<float> tickness = partsClass.ticnesslist();
            spartsClass.CNCs.Clear();
            spartsClass.Sheets.Clear();

            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 // было            \/ and not ARHIVE is null                                                     
                spartsClass.readCNC(@"select [ID],replace(replace(FileName,'D:\PlazmaProgs\',''),'.CNC','') as filename,[ORIGINALFILENAME],[RUNTIMEONESHEET],[QUANTITY],[QUANTITYDONE],[RUNTIMEALL],[REALTICKNESS],[TICKNESS],[SHEETS],[ADDEDTIME],[FILECREATEDTIME],[SHEETWIDTH],[MATERIAL],[SHEETHEIGTH],[ARHIVE],[DONETIME] from CNCFILES where id not in ( SELECT [ID] FROM [CNCFILES] where arhive is null and trim(REPLACE(Replace([FILENAME],'D:\PlazmaProgs\',''),'.CNC','')) in (select trim(name)  FROM [SHEETS] where not name is null and ARHIVE is null ) ) and not FILENAME like '%(N%' and not SHEETS like '%NO_SHEET%' order by ADDEDTIME, TICKNESS, FILENAME");
                //spartsClass.readCNC(@"SELECT [ID],replace(replace(FileName,'D:\PlazmaProgs\',''),'.CNC','') as FILENAME,[QUANTITY],[QUANTITYDONE],[TICKNESS],[SHEETS],[ADDEDTIME],[FILECREATEDTIME],[SHEETWIDTH],[SHEETHEIGTH],[ARHIVE],[DONETIME] FROM [PLASMA].[dbo].[CNCFILES] where TICKNESS=cast(" + tik.ToString()+@" as real) and not FILENAME like ' % (N % ' and not FILENAME like ' % 09G2S % ' and not replace(replace(FileName,'D:\PlazmaProgs\',''),'.CNC','') in (SELECT [NAME]  FROM [PLASMA].[dbo].[SHEETS]  where not name is null and TICKNESS=cast("+tik.ToString()+" as real) and OWNER='МОНТАЖНИК' and MATHERIAL=1 and HEIGTH=1500) order by FILENAME");
                spartsClass.ReadSheets("select * from SHEETS where  ( NAME is null)  order by TICKNESS");
            }
            ViewBag.m = spartsClass.materials;
            ViewBag.CNC = spartsClass.CNCs;
            ViewBag.Sheets = spartsClass.Sheets;
            Thread.CurrentThread.CurrentCulture = new CultureInfo("nl-NL");
            ViewBag.now = DateTime.Now.ToString("d").ToString().Replace("-", ".");
            return View();
        }
        [HttpGet]
        public ActionResult MarkSheet2(int cncId, int sheetId = -7)
        {
            //int result = 0;
            _returnmarksheetresponse MarkSheet = new _returnmarksheetresponse();
            MarkSheet.cncid = cncId;
            MarkSheet.sheetid = sheetId;
            MarkSheet.result = false;
            MarkSheet.user = partsClass.GetUser();
            string querry = "";
            if (sheetId == -1) return Json(MarkSheet);
            if (sheetId == -7)
            {
                querry = "UPDATE CNCFILES SET SHEETS='NO_SHEET' WHERE ID=" + cncId.ToString();
            }
            else querry = @"UPDATE SHEETS SET NAME=(SELECT replace(replace(FileName,'D:\PlazmaProgs\',''),'.CNC','') FROM CNCFILES WHERE CNCFILES.ID=" + cncId + ") WHERE SHEETS.ID=" + sheetId;
            MarkSheet.result = partsClass.FreeRequestToBD(querry) > 0 ? true : false;
            return Json(MarkSheet, behavior: JsonRequestBehavior.AllowGet);
        }
        /* Отправить массив примитивов в клиент   */
        //[AcceptVerbs(HttpVerbs.Post)]
        public ActionResult getIMAGE(int cncID)
        {
            cnc cncfile = new cnc(partsClass.CNCs[partsClass.CNCs.FindIndex(x => x.Id == cncID)].FileName);

            cncfile.AllPrimitives.Add(new cnc.step { textline = "", Xstart = cncfile.Sheet.minX, Ystart = cncfile.Sheet.minY, Xend = cncfile.Sheet.maxX, Yend = cncfile.Sheet.maxY });
            return Json(cncfile.AllPrimitives, behavior: JsonRequestBehavior.AllowGet);
        }
        public ActionResult getIMAGE_SVG(int cncID)
        {
            cnc cncfile = new cnc(partsClass.CNCs[partsClass.CNCs.FindIndex(x => x.Id == cncID)].FileName);
            cncfile.AllPrimitives.Add(new cnc.step { textline = "", Xstart = cncfile.Sheet.minX, Ystart = cncfile.Sheet.minY, Xend = cncfile.Sheet.maxX, Yend = cncfile.Sheet.maxY });
            _SVGAnswer SVGAnswer = new _SVGAnswer();
            SVGAnswer.minX = (int)cncfile.Sheet.minX;
            SVGAnswer.minY = (int)cncfile.Sheet.minY;
            SVGAnswer.maxX = (int)cncfile.Sheet.maxX;
            SVGAnswer.maxY = (int)cncfile.Sheet.maxY;
            string svg = "";
            foreach (cnc.step s in cncfile.AllPrimitives)
            {
                switch (s.Comand)
                {
                    case cnc.comand.fastmove: svg += " M" + s.Xend.ToString() + " " + s.Yend.ToString(); break;
                    case cnc.comand.cuteline: svg += " L" + s.Xend.ToString() + " " + s.Yend.ToString(); break;
                    case cnc.comand.cutearc: svg += " A" + s.Radius.ToString() + " " + s.Radius.ToString() + " 0 " + (s.SweepAngle < 180 ? "0 0 " : "1 0 ") + (s.Xend + 0.1).ToString() + " " + (s.Yend + 0.1).ToString(); break;
                    case cnc.comand.cutearc2: svg += " A" + s.Radius.ToString() + " " + s.Radius.ToString() + " 0 " + (s.SweepAngle < 180 ? "0 1 " : "1 1 ") + (s.Xend + 0.1).ToString() + " " + (s.Yend + 0.1).ToString(); break;
                    default: break;
                }

            }
            SVGAnswer.path = svg;
            return Json(SVGAnswer, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckUpdateTable(string tablename)
        {
            DateTime updatetime = partsClass.GetLastUpdateTime(tablename);
            if (updatetime > DateTime.Now.AddMinutes(-2))
            {
                GSSW gssw = new GSSW();
                            
            }
            return Json(updatetime);
        }
        public ActionResult CurentOrders(string date = "-1")
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("viewOrders")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            date.Replace(".", "/");
            int diff;
            if (date == "-1") { diff = 31; }
            else
            {
                DateTime date1 = DateTime.Parse(date, CultureInfo.CreateSpecificCulture("fr-FR"));
                DateTime date2 = DateTime.Now;
                diff = (date2 - date1).Days;
            }
            partsClass.readorders(diff);
            ViewBag.orders = partsClass.NORDER;
            ViewBag.currenttime = DateTime.Now;
            ViewBag.chapters_calculation = partsClass.getChapterCode("calculation");
            ViewBag.chapters_ShipManaged = partsClass.getChapterCode("ShipManaged");
            ViewBag.now = DateTime.Now.AddDays(-diff).ToString("d", CultureInfo.CreateSpecificCulture("fr-FR")).ToString().Replace("/", ".");
            return View();
        }
        public ActionResult GetAlCncTable()
        {
            return Json(partsClass.CNCs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadCNC()
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("AddCNC")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            return View();
        }

        public ActionResult NewSheet2(int mat, string tikn, int WIDTH, int HEIGTH, int Quantity = 1, string OWNER = "МОНТАЖНИК", string DOC = "", string Date = "")
        {
            currentuser = users.getCurrentUser();
            if (((currentuser.Privilegies & partsClass.getChapterCode("addSheet")) == 0)) return Redirect(Url.RouteUrl(new { controller = "HOME", action = "Index" }));
            ViewBag.User = currentuser;
            PartsClass._sheet nSheet;
            nSheet.Matherial = mat;
            nSheet.Id = 0;
            nSheet.Name = "NULL";
            nSheet.Owner = OWNER;
            nSheet.Parts = "";
            nSheet.Quantity = 1;
            nSheet.Reserve1 = "";
            nSheet.Reserve2 = "";
            nSheet.Reserve3 = "";
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            nSheet.Tickness = (float)Convert.ToDouble(tikn.Replace(",", "."));
            nSheet.CNCFILE = "";
            nSheet.Heigth = HEIGTH;
            nSheet.Width = WIDTH;
            nSheet.Status = "";
            if (Date.Length < 5)
            {
                nSheet.Date = DateTime.Now;
            } else
                nSheet.Date = DateTime.Parse(Date.Replace(".", "/"), CultureInfo.CreateSpecificCulture("fr-FR"));

            nSheet.Document = DOC;
            for (int i = 1; i <= Quantity; i++) { partsClass.AddSheettoBD(nSheet); }
            ViewBag.sh = nSheet;
            ViewBag.q = Quantity;
            ViewBag.mt = partsClass.materials;
            return View();
        }
        public ActionResult EditSheetArrival(
            string DateOld = "",
            string DocumentOld = "~",
            int MatherialOld = -1,
            float ticknessOld = -1,
            int QuantityOld = -1,
            string ownerOld = "МОНТАЖНИК",
            int WidthOld = 0,
            int heigthOld = 0,
            string DateNew = "",
            string DocumentNew = "~",
            int MatherialNew = -1,
            float ticknessNew = -1,
            int QuantityNew = -1,
            string ownerNew = "МОНТАЖНИК",
            int WidthNew = 0,
            int heigthNew = 0
            )
        {
            int Del = 0;
            int change=0;
            int ins = 0;
            //если произошли изменения по листам то нужно установить правильное количество:
            //1) Удалить лишние листы
            if (QuantityOld > QuantityNew) Del=partsClass.FreeRequestToBD("delete from sheets where id in (select top(" + (QuantityOld - QuantityNew).ToString() +
                  ") id From SHEETS where NAME is null and MATHERIAL=" + MatherialOld.ToString() +
                  " and TICKNESS = " + ticknessOld.ToString() +
                  " and WIDTH = " + WidthOld.ToString() +
                  " and HEIGTH = " + heigthOld.ToString() +
                  " and DOKUMENT like '%" + DocumentOld.Trim() +
                  "%' and OWNER like '%" + ownerOld.Trim() +
                  "%' and cast(DATE as Date) = cast('" + DateOld.Trim() + "' as date))");
            //1) Добавить листы partClass.AddSheets()
            if (QuantityOld < QuantityNew) for (int i = 0; i < (QuantityNew - QuantityOld); i++) {
                   /* PartsClass._sheet nSheet = new PartsClass._sheet();
                    nSheet.Matherial = MatherialOld;
                    nSheet.Id = 0;
                    nSheet.Name = "NULL";
                    nSheet.Owner = ownerOld;
                    nSheet.Parts = "";
                    nSheet.Quantity = 1;
                    nSheet.Reserve1 = "";
                    nSheet.Reserve2 = "";
                    nSheet.Reserve3 = "";
                    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                    nSheet.Tickness = ticknessOld;
                    nSheet.CNCFILE = "";
                    nSheet.Heigth = heigthOld;
                    nSheet.Width = WidthOld;
                    nSheet.Status = "";*/
                    partsClass.FreeRequestToBD("INSERT INTO[SHEETS](MATHERIAL, TICKNESS, WIDTH, HEIGTH, OWNER, QUANTITY, DOKUMENT, DATE) VALUES(" +
                                                                    MatherialOld.ToString() + "," + 
                                                                    ticknessOld.ToString() + "," + 
                                                                    WidthOld.ToString() + "," + 
                                                                    heigthOld.ToString() + ",'" + 
                                                                    ownerOld.Trim() + "',1,'" + 
                                                                    DocumentOld.Trim() + "','" + 
                                                                    DateOld.Trim() + "')");
                    //partsClass.AddSheettoBD(nSheet);
                    ins++;
                }
            // 2)привести все остальные параметры к нужным значениям
            change = partsClass.FreeRequestToBD("update sheets set "+
                "MATHERIAL = " + MatherialNew.ToString()+
                  ", TICKNESS = " + ticknessNew.ToString() +
                  " , WIDTH = " + WidthNew.ToString() +
                  " , HEIGTH = " + heigthNew.ToString() +
                  " , DOKUMENT = '" + DocumentNew.Trim() +
                  "', OWNER = '" + ownerNew.Trim() +
                  "' ,DATE  = cast('" + DateNew.Trim() + "' as date)"+
                  " where id in ( select id From SHEETS where MATHERIAL=" + MatherialOld.ToString() +
                  " and TICKNESS = " + ticknessOld.ToString() +
                  " and WIDTH = " + WidthOld.ToString() +
                  " and HEIGTH = " + heigthOld.ToString() +
                  " and DOKUMENT like '%" + DocumentOld.Trim() +
                  "%' and OWNER like '%" + ownerOld.Trim() +
                  "%' and cast(DATE as Date) = cast('" + DateOld.Trim() + "' as date))");
            return Json(new { operation=true, deleted=Del, inserted=ins,changed= change}, behavior: JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditSheetArrivalMinQty(
            string DateOld = "",
            string DocumentOld = "~",
            int MatherialOld = -1,
            float ticknessOld = -1,
            int QuantityOld = -1,
            string ownerOld = "МОНТАЖНИК",
            int WidthOld = 0,
            int heigthOld = 0
            )
        {
            var R = partsClass.Sheets.Where(
                (x) => (
                x.Document.IndexOf(DocumentOld) >= 0 &&
                x.Matherial == MatherialOld &&
                x.Tickness == (float)Convert.ToDouble(ticknessOld) &&
                x.Owner.IndexOf(ownerOld) >= 0 &&
                x.Width == WidthOld &&
                x.Heigth == heigthOld &&
                x.Name==""
                )).ToList();

            return Json(new { quantity = R.Count,id=0 }, behavior: JsonRequestBehavior.AllowGet) ;
        }
        public ActionResult Error(int err=-1, string Text="Упс... Что-то пошло не так...")
        {
            Error Err=new Error();
            Err.text = Text;
            Err.number = err;
            return View(Err);
        }
    }
}