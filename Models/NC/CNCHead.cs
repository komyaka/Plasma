using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class CNCHead
    {
        #region структуры
        public struct _Nest
        {
            public _Nest(int width, int heigth, int cutTimes)
            {
                Width = width;
                Heigth = heigth;
                CutTimes = cutTimes;
            }

            public int Width { get; set; }
            public int Heigth { get; set; }
            public int CutTimes { get; set; }
        }
        public struct _Material
        {
            public string name;
            public string about;

            public _Material(string name, string about)
            {
                this.name = name;
                this.about = about;
            }
        }
        public struct _Part
        {
            public string Name;
            public int Quantity;
            public int Width;
            public int Heigth;

            public _Part(string name, int quantity, int width, int heigth)
            {
                Name = name;
                Quantity = quantity;
                Width = width;
                Heigth = heigth;
            }
        }
        #endregion
        public Sheet sheet;
        public _Nest Nest;
        public string Filename = "";
        public Dictionary<int, _Material> materials = new Dictionary<int, _Material> {
            { 0, new _Material( "NONAME", "Неизвестный материал" ) }
           ,{ 1, new _Material( "", "Сталь3" ) }
           ,{ 2, new _Material( "09G2S", "09Г2С" ) }
           ,{ 3, new _Material( "NERJ", "Нержавейка" ) }
           ,{ 4, new _Material( "RIFL", "Рифл.(Чечевица)" ) }
           ,{ 5, new _Material( "RIFL_R", "Рифл.(Ромб)" ) }
           ,{ 6, new _Material("HSND","10ХСНД" ) }
        };
        public List<_Part> Parts = new List<_Part> { };

        public CNCHead()
        {
}
        public CNCHead(string Filename)
        {
            string line;
            sheet = new Sheet();
            sheet.Tickness = 0;
            sheet.Material = -1;
            this.Filename = Filename.Substring(Filename.LastIndexOf("\\")+1,Filename.ToUpper().IndexOf(".CNC")-Filename.LastIndexOf("\\") - 1);
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(Filename, System.Text.Encoding.Default);
                while (((line = file.ReadLine()) != null)&&(!parseHead(line))){}
                if (sheet.Tickness == 0) 
                    try 
                    { 
                        sheet.Tickness = (float)Convert.ToDouble(gettiknessfromname(Filename.Replace("_", ".").Replace(",","."))); 
                    }
                    catch (Exception e)
                    {
                        sheet.Tickness = 0;
                    }


                file.Close();
            }
            catch { }
            if (sheet.Material<0) sheet.Material  = getmatherialFromname(Filename);
            

        }

        private string gettiknessfromname(string fname)
        {
            string tik = "";
            tik = fname.Substring(fname.LastIndexOf("\\"));
            char ch = tik[0];
            while ((tik.Length > 0) && (!char.IsDigit(tik[0])))
            {
                tik = tik.Substring(1);
            }
            tik = tik.Substring(0, tik.IndexOf("-"));
            return tik;
        }
        private int getmatherialFromname(string fname)
        {
            int mat = 1;
            for (int i = materials.Count - 1; i > 1; i--) if (fname.ToUpper().LastIndexOf(materials[i].name) > 0) mat = i;
            return mat;
        }
        public bool parseHead(string line)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string m = line.ToUpper().Trim();
            if ((m.Length > 7) && (m.Substring(0, 7) == "(PART#:"))
            {
                string _Name = line.Substring(0, m.IndexOf("QTY"));
                if (line.IndexOf("<") > 0)
                {
                    string sX = "0";
                    string sY = "0";
                    int sx = 0;
                    int sy = 0;
                    int partqty = 0;
                    try { sx =(int) Convert.ToDouble(line.Substring(line.LastIndexOf("<") + 1, line.LastIndexOf("X") - line.LastIndexOf("<") - 1)); } catch { }
                    try { sy =(int) Convert.ToDouble( line.Substring(line.LastIndexOf("X") + 1, line.LastIndexOf(">") - line.LastIndexOf("X") - 1)); } catch { }
                    try { partqty = (Convert.ToInt32(line.Substring(line.IndexOf("QTY") + 3, line.LastIndexOf("<") - line.IndexOf("QTY") - 3))); } catch { }
                    Parts.Add(new _Part(line.Substring(7, line.IndexOf("QTY") - 7),partqty, sx, sy));
                }
                else
                {
                    Parts.Add(new _Part(line.Substring(7, line.IndexOf("QTY") - 7), Convert.ToInt32(line.Substring(line.IndexOf("QTY") + 3, line.LastIndexOf(")") - line.IndexOf("QTY") - 3)), 0, 0));
                }
            }
            if ((m.Length > 11) && (m.Substring(0, 11) == "(THICKNESS="))
            {
                sheet.Tickness = (float)Convert.ToDouble(m.Substring(11, m.Length - 12).Replace(".",","));
            }
            if ((m.Length > 10) && (m.Substring(0, 10) == "(QTYTIMES "))
            {
                Nest.CutTimes = Convert.ToInt32(m.Substring(10, m.Length - 11));
            }
            if ((m.Length > 6) && (m.Substring(0, 6) == "(Y_DIM")) this.sheet.Heigth = (int)Convert.ToDouble(m.Substring(m.IndexOf("=")+1, m.IndexOf(")") - m.IndexOf("=") - 1));
            if ((m.Length > 6) && (m.Substring(0, 6) == "(X_DIM")) this.sheet.Width  = (int)Convert.ToDouble(m.Substring(m.IndexOf("=")+1, m.IndexOf(")") - m.IndexOf("=") - 1));

            if (m[0].ToString() == "(") { return false; }
            if (m.IndexOf("M21") >= 0)  { return true; }
            return false;
        }
    }
}
