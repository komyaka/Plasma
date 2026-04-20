using Plazma.Models;
using Plazma.Models.ClassAPI;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;


namespace Plazma.Controllers
{

    public class APIController : Controller
    {
        // GET: API
        //[AcceptVerbs(HttpVerbs.Post)]
        protected struct cncNames
        {
            public string newFileName;
            public string oldFileName;
            public cncNames(string newName, string oldName = "")
            {
                newFileName = newName;
                oldFileName = (oldName == "") ? newName : oldName;
            }
        }
        protected admin ADM=new admin { };
        public static PartsClass parts = new PartsClass();
        public List<string> filelist = new List<string> { }; // Хранит имена файлов при загрузке на сервер.
        /*
        public static string getNewFileName(string originalName, string path)
        {
            int count = 0;
            if (path.Substring(path.Length - 1) != "/") path += "/";
            if (!File.Exists(path + originalName)) return (originalName);
            while (!File.Exists(path + originalName + "N" + (count++).ToString())) { }
            return (originalName + "N" + (count).ToString());
        }
        */
        public ActionResult ticknessList()
        {
            return Json(parts.ticnesslist());
        }
        public ActionResult materialList()
        {
            return Json(parts.materials);
        }
        public ActionResult programmList()
        {
            return Json(parts.CNCs);
        }
        public ActionResult partList()
        {
            return Json(parts.Parts);
        }
        public ActionResult sheetList()
        {
            return Json(parts.Sheets);
        }
        public ActionResult orderList()
        {
            return Json(parts.NORDER);
        }
        public ActionResult sheepmentList()
        {
            return Json(parts.Shipments);
        }
        [HttpPost]
        public ActionResult newCNC2(IEnumerable<HttpPostedFileBase> uploads)
        {
            int result = 0;
            List<cncNames> files = new List<cncNames> { };
            files.Clear();
            string tmpfile = "";
            string path = AppConfig.CNCUploadPath;  // ИСПРАВЛЕНО: путь из конфига
            foreach (var file in uploads)
            {
                if (file != null)
                {
                    // получаем имя файла
                    string fileName = System.IO.Path.GetFileName(file.FileName);
                    // сохраняем файл в папку Files в проекте
                    //file.SaveAs(Server.MapPath("~/Files/" + fileName));
                    
                    tmpfile = ADM.getNewFileName(fileName, path);

                    file.SaveAs(path+tmpfile);
                    files.Add(new cncNames(fileName, tmpfile));
                }
            }
            foreach (cncNames s in files)
            {
                cnc Fcnc = new cnc(s.newFileName);
                PartsClass._CNC nCNC = new PartsClass._CNC(
                    Filename: s.newFileName,
                    OriginalFile: s.oldFileName,
                    quantity: Fcnc.QuantityCut,
                    Tickness: cnc.gettiknessfromname(s.oldFileName),
                    Realtickness: cnc.gettiknessfromname(s.oldFileName),
                    material: PartsClass.GetMaterialFromName(s.oldFileName),
                    // ширина листа ???
                    width: (int)(Fcnc.Sheet.maxX - Fcnc.Sheet.minX),
                    heigth: (int)(Fcnc.Sheet.maxY - Fcnc.Sheet.minY)
                    );
                int id = parts.AddCNCtoBD(nCNC);
                foreach (cnc._part p in Fcnc.Parts)
                {
                    PartsClass._Part newPart = new PartsClass._Part(
                        name: p.Name,
                        quantity: p.quantity,
                        quantitysum: p.quantity * Fcnc.QuantityCut,
                        Quantitycutted: 0,
                        Tickness: cnc.gettiknessfromname(s.oldFileName),
                        SizeX: p.size.x.ToString(),
                        SizeY: p.size.y.ToString(),
                        CncId: id.ToString());
                    parts.AddPartToBD(newPart);
                }
            }
            return Json(files[0],JsonRequestBehavior.AllowGet);
        }
        public ActionResult deleteCNC()
        {
            int result = 0;
            return Json(result);
        }
        public ActionResult NewSheet()
        {
            int result = 0;
            return Json(result);
        }
        public ActionResult DeleteSheet()
        {
            int result = 0;
            return Json(result);
        }
        public ActionResult MarkSheet()
        {
            int result = 0;
            return Json(result);
        }
        public ActionResult PartListInCNC(int cncId)
        {
            int result = 0;
            return Json(result);
        }
        public ActionResult markCNC(int cncId = -1, int Quantity = 1)
        {
            int result = 0;
            return Json(result);
        }
        public ActionResult getIMAGE(int cncID)
        {
            cnc cncfile = new cnc(parts.CNCs[parts.CNCs.FindIndex(x => x.Id == cncID)].FileName);

            cncfile.AllPrimitives.Add(new cnc.step { textline = "", Xstart = cncfile.Sheet.minX, Ystart = cncfile.Sheet.minY, Xend = cncfile.Sheet.maxX, Yend = cncfile.Sheet.maxY });
            return Json(cncfile.AllPrimitives);
        }
        public ActionResult getIMAGE_SVG(int cncID)
        {
            cnc cncfile = new cnc(parts.CNCs[parts.CNCs.FindIndex(x => x.Id == cncID)].FileName);
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
                    case cnc.comand.cutearc: svg += " A" + s.Radius.ToString() + " " + s.Radius.ToString() + " 0 " + (s.SweepAngle < 180 ? "0 0 " : "1 0 ") + s.Xend.ToString() + " " + s.Yend.ToString(); break;
                    case cnc.comand.cutearc2: svg += " A" + s.Radius.ToString() + " " + s.Radius.ToString() + " 0 " + (s.SweepAngle < 180 ? "0 1 " : "1 1 ") + s.Xend.ToString() + " " + s.Yend.ToString(); break;
                    default: break;
                }

            }
            SVGAnswer.path = svg;
            return Json(SVGAnswer, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CheckUpdateTable(string tablename)
        {
            DateTime updatetime = parts.GetLastUpdateTime(tablename);
            return Json(updatetime);
        }
    }
}