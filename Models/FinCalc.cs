using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using Excel = Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
       //microsoft Excel 14 object in references-> COM tab

namespace Plazma.Controllers
{
    public class FinCalc
    {
        public class FPart : Models.Part
        {
            public float cost;
            public int shiped;
            public DateTime shiptime;
        }
        public List<FPart> Parts = new List<FPart> { };
        public List<Models.Part> orderParts = new List<Models.Part> { };
        public string Ordername;
        public double summ;
        void SetTestParts(List<Models.Part> _orderParts)
        {
            if (orderParts.Count > 0) this.orderParts = _orderParts;
        }
        public static string finepath(string filename)
        {
            string newpath = Constants._plasmaPath;
            // Directory.SetAccessControl(newpath, new System.Security.AccessControl.DirectorySecurity(newpath, System.Security.AccessControl.AccessControlSections.Audit));


            /*            using (System.Security.Principal.WindowsImpersonationContext ctx = System.Security.Principal.WindowsIdentity.Impersonate(userToken))

                        {

                          //  выполнять операции ввода - вывода

            ctx.Undo();

                        }*/
            string[] files= { };
            try
            {
                files = Directory.GetFiles(newpath, filename, SearchOption.AllDirectories);
            }
            catch{ }
            if (files.Length == 0) return "Not Fоund File";
            return files[0];
        }
        public static string getfilefromOrdername(string Ordername)
        {
            string fn = ""; string day="";string Month=""; string oName = ""; string index="";
            try { day = Ordername.Trim().Substring(Ordername.IndexOf(",") + 1) + " "; if (day.IndexOf("(") > 0) { index =" "+ day.Substring(day.IndexOf("(")).Trim(); day = day.Substring(0, day.IndexOf("(") - 1)+" "; }  } catch { }
            try { Month = Ordername.Trim().Substring(Ordername.IndexOf(",") - 2, 2).Trim() + " ";if (Month.Length < 3) Month = "0" + Month; } catch { }
            try { oName = Ordername.Trim().Substring(0, Ordername.IndexOf(",") - 2).Trim(); } catch { oName = Ordername.Trim(); }
            string Year = "202?" + " ";
            fn = "расчёт резки " + Year + day + Month + oName +index+ ".xls";
            fn = finepath(fn.Replace(" ","?").Replace("-","?").Replace("_","?"));
            return fn;
        }
        public void readfromxls(string openFilename)
        {
            // Excel.Application xlApp = new Excel.Application();
            string grabFile = @"D:\PlazmaProgs\tmp-" + DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString() + "-" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString() + "-" + DateTime.Now.Millisecond.ToString() + ".xls";
            if (System.IO.File.Exists(openFilename))
            {
                /* xlWorkbook = xlApp.Workbooks.Open(grabFile, ReadOnly: true);
                 xlWorksheet =(Excel._Worksheet) xlWorkbook.Sheets["Счет зак"];*/
                // xlRange = xlWorksheet.;
                try
                {
                    File.Copy(openFilename, grabFile);
                }
                catch (Exception e) { Console.WriteLine(e.Message); FPart t = new FPart(); return; }
            }
            else { FPart t = new FPart(); return; }
                summ = 0;
            /*------------------------------------------------------------------------------------------------*/
            Net.SourceForge.Koogra.Excel.Workbook wb = new Net.SourceForge.Koogra.Excel.Workbook(grabFile);
            Net.SourceForge.Koogra.Excel.Worksheet ws = wb.Sheets.GetByName("Счет зак");
            /*------------------------------------------------------------------------------------------------*/
            for (int i = 1; i < ws.Rows.MaxRow; i++)
                {
                    Net.SourceForge.Koogra.Excel.Row row = ws.Rows[(uint)i];
                    FPart t = new FPart();
                    t.Id = i - 1;
                    try { t.Name = Convert.ToString(  row.Cells[1].Value); } catch { t.Name = "Неизвестно"; break; }
                    try { t.tickness = (float)Convert.ToDouble(row.Cells[2].Value); } catch { t.tickness = 0; };
                    try { t.Size_X = Convert.ToInt32(row.Cells[3].Value); } catch { t.Size_X = 0; }
                    try { t.Size_Y = Convert.ToInt32(row.Cells[4].Value); } catch { t.Size_Y = 0; }
                    try { t.QuantitySummary = Convert.ToInt32(row.Cells[5].Value); } catch { t.QuantitySummary = 0; }
                    try { t.cost = (float)Convert.ToDouble(row.Cells[8].Value); } catch { t.cost = 0; }
                    summ += t.QuantitySummary * t.cost;
                    if ((t.Name != null) && (t.Name.Length > 0) && (t.Name.IndexOf("Требуется добавить") < 0)) Parts.Add(t);
                }
            try { File.Delete(grabFile); } catch { };
        }
    }
}