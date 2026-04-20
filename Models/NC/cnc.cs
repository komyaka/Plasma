using System;
using System.Collections.Generic;
using System.Globalization;
using Plazma.Models;
namespace Plazma.Controllers
{
    public class cnc
    {
        public string fileName="";
        public enum comand {undefined,fastmove,cuteline,cutearc,cutearc2}
        public enum metric {decart,polar }
        public int QuantityCut;
        public bool decartmetric;
        public bool polarmetric;
        private bool absolutecoordinates;
        public bool AbsoluteCoordinates { get { return absolutecoordinates; }set { absolutecoordinates = value; } }
        public metric cncmetric;
        public bool cute = false;
        public int M21count;
        public int Jobtime;
        public comand CurrentCommad=comand.undefined;
        public struct _sheet
        {
            public float minX, maxX, minY, maxY;
            public float Tickness;
        }
        public _sheet Sheet;
        public struct _part
        {
            public string Name;
            //public string Path;
            public int quantity;
            public point size;
            public _part(string _name,int _quantity=0,float _SizeX=0,float _SizeY=0)
            {
                Name = _name;
                quantity = _quantity;
                size.x = _SizeX;
                size.y = _SizeY;
                size.z = 0;
            }
        }
        public List<_part> Parts = new List<_part> { };
        public struct point
        {
            public float x, y,z;
        }
        private point CurPoint;
        public point CurrentPoint { get { return CurPoint; } set { CurPoint = value; } }
        public struct step
        {
            public string textline;
            public comand Comand;
            public float Xstart, Ystart,Zstart,Xcentr,Ycentr,Zcentr, Xend, Yend,Zend, StartAngle, EndAngle, SweepAngle, I, J, K,Length,Radius;
        }
        private step tempstep;
        public List<step> AllPrimitives;
        public enum _Process { PL30, PL50, PL80, PL130, PL200, PL260, Gas };
        public struct _speed
        {
            public _Process Process;
            public int PierceTime;
            public int CuttingSpeed;
            public int FastmoveSpeed;
            public int SetupTime;
            public _speed(_Process process,int pierce,int Cutting,int Fast,int Setup)
            {
                Process = process;
                PierceTime=pierce;
                CuttingSpeed=Cutting;
                FastmoveSpeed=Fast;
                SetupTime = Setup;
            }
        }
        public _speed speed;
        
        public cnc()
        {
            
            AllPrimitives = new List<step> { };
            Sheet.maxX = 0;
            Sheet.maxY = 0;
            Sheet.minX = 0;
            Sheet.minY = 0;
            Sheet.Tickness = 0;
        }
        public cnc(string Filename)
        {
            string line;
            fileName = Filename;
            AllPrimitives = new List<step> { };
            M21count = 0;
            absolutecoordinates = true;
            Sheet.maxX = 0;
            Sheet.maxY = 0;
            Sheet.minX = 0;
            Sheet.minY = 0;
            Sheet.Tickness = 0;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(Filename, System.Text.Encoding.Default);
                while ((line = file.ReadLine()) != null)
                {
                    //  Form1.addnewstring(line);
                    addline(line);
                }
            

            if (Sheet.Tickness == 0) try { Sheet.Tickness = (float)Convert.ToDouble(gettiknessfromname( Filename.Replace("_", "."))); }
                catch (Exception e)
                {
                    // MessageBox.Show(e.Message, e.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Sheet.Tickness = 0;
                }
               

            file.Close();
            }
            catch { }
            Jobtime =CalculateJobTime();
        }
        public _speed GetRecomendetSpeed(string tickn)
        {
            if (tickn == "1") return new _speed(_Process.PL30 ,8,5000,12000,240);
            if (tickn == "1,2") return new _speed(_Process.PL50, 8, 4150, 12000, 240); 
            if (tickn == "1_2") return new _speed(_Process.PL50, 8, 4150, 12000, 240);
            if (tickn == "1,5") return new _speed(_Process.PL50, 8, 3200, 12000, 240);
            if (tickn == "1_5") return new _speed(_Process.PL50, 8, 32000, 12000, 240);
            if (tickn == "2") return new _speed(_Process.PL50, 8, 9800, 12000, 240);
            if (tickn == "3") return new _speed(_Process.PL80, 8, 6145, 12000, 240);
            if (tickn == "4") return new _speed(_Process.PL80, 8, 3670, 12000, 240);
            if (tickn == "5") return new _speed(_Process.PL80, 8, 4760, 12000, 240);
            if (tickn == "6") return new _speed(_Process.PL130, 8, 4035, 12000, 240);
            if (tickn == "7") return new _speed(_Process.PL130, 8, 3700, 12000, 240);
            if (tickn == "8") return new _speed(_Process.PL200, 8, 3360, 12000, 240);
            if (tickn == "9") return new _speed(_Process.PL200, 8, 4000, 12000, 240);
            if (tickn == "10") return new _speed(_Process.PL200, 8, 3460, 12000, 240);
            if (tickn == "11") return new _speed(_Process.PL200, 8, 3200, 12000, 240);
            if (tickn == "12") return new _speed(_Process.PL200, 8, 3060, 12000, 240);
            if (tickn == "14") return new _speed(_Process.PL200, 8, 2800, 12000, 240);
            if (tickn == "15") return new _speed(_Process.PL200, 8, 2275, 12000, 240);
            if (tickn == "16") return new _speed(_Process.PL200, 8, 2050, 12000, 240);
            if (tickn == "18") return new _speed(_Process.PL200, 8, 1900, 12000, 240);
            if (tickn == "20") return new _speed(_Process.PL200, 8, 1575, 12000, 240);
            if (tickn == "22") return new _speed(_Process.PL200, 12, 1400, 12000, 240);
            if (tickn == "25") return new _speed(_Process.Gas, 12, 550, 12000, 240);
            if (tickn == "28") return new _speed(_Process.Gas, 12, 550, 12000, 240);
            if (tickn == "28") return new _speed(_Process.Gas, 12, 550, 12000, 240);
            if (tickn == "30") return new _speed(_Process.Gas, 12, 500, 12000, 240);
            if (tickn == "32") return new _speed(_Process.Gas, 15, 490, 12000, 240);
            if (tickn == "36") return new _speed(_Process.Gas, 15, 460, 12000, 240);
            if (tickn == "40") return new _speed(_Process.Gas, 15, 450, 12000, 240);
            if (tickn == "45") return new _speed(_Process.Gas, 18, 450, 12000, 240);
            if (tickn == "50") return new _speed(_Process.Gas, 25, 420, 12000, 240);
            if (tickn == "60") return new _speed(_Process.Gas, 25, 320, 12000, 240);
            if (tickn == "70") return new _speed(_Process.Gas, 30, 310, 12000, 240);
            if (tickn == "80") return new _speed(_Process.Gas, 30, 280, 12000, 240);
            if (tickn == "90") return new _speed(_Process.Gas, 40, 280, 12000, 240);
            if (tickn == "100") return new _speed(_Process.Gas, 40, 240, 12000, 240);
            if (tickn == "110") return new _speed(_Process.Gas, 50, 240, 12000, 240);
            if (tickn == "120") return new _speed(_Process.Gas, 60, 240, 12000, 240);
            if (tickn == "130") return new _speed(_Process.Gas, 70, 200, 12000, 240);
            if (tickn == "140") return new _speed(_Process.Gas, 90, 200, 12000, 240);
            if (tickn == "150") return new _speed(_Process.Gas, 100, 180, 12000, 240);
            if (tickn == "160") return new _speed(_Process.Gas, 120, 180, 12000, 240);
            return new _speed(_Process.PL200, 12, 3000, 12000, 240);
        }

        float Angle(float x0, float y0, float x1, float y1)
        {
            float dx = (x1 - x0);
            float dy = (y1 - y0);
            float L =(float) Math.Sqrt(dx*dx+dy*dy);
            if (dy < 0)
            {
                return (float) (90- (Math.Asin(-dx / L)) * 180 / Math.PI);
            }
            else
            {
                return (float) (360-((Math.Asin(dx / L)) * 180 / Math.PI + 90));
            }
        }
        void updatemaxmin(step prim)
        {
            switch (prim.Comand)
            {
                case comand.cuteline:                  
                    break;
                case comand.cutearc:
                    break;
                case comand.cutearc2:
                    break;
                case comand.fastmove:
                    break;
                default:
                    break;
            }
        }
        float getValuefromLine(string Symbol, ref string line)
        {
            if (!(line.IndexOf(Symbol) < 0))
            {
                string m = line.Substring(line.IndexOf(Symbol) + 1);
                line = line.Substring(0, line.IndexOf(Symbol));
                if ((Symbol == "Y") || (Symbol == "J")) { return (float) Convert.ToDouble(m,CultureInfo.InvariantCulture); }
                else
                {
                    return (float) Convert.ToDouble(m, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                return 0;
            }
        }
        public static string gettiknessfromname(string fname)
        {
            string tik = "";
            fname = fname.Replace("\\", "/");
            tik = fname.LastIndexOf("/")>0? fname.Substring(fname.LastIndexOf("/")):fname;
            char ch = tik[0];
            while ((tik.Length > 0) && (!char.IsDigit(tik[0])))
            {
                tik = tik.Substring(1);
            }
            tik = tik.Substring(0, tik.IndexOf("-"));
            return tik;
        }
        private int CalculateJobTime()
        {
            double jobtime = 0;
            try
            {
                speed = GetRecomendetSpeed(Sheet.Tickness == 0 ? gettiknessfromname(fileName) : Sheet.Tickness.ToString());
            }
            catch 
            { 
            }
            for (int i = 0; i <= AllPrimitives.Count-1; i++)
            {
                try
                {
                    jobtime += AllPrimitives[i].Comand == comand.fastmove ? AllPrimitives[i].Length*60 / speed.FastmoveSpeed : AllPrimitives[i].Length*60 / speed.CuttingSpeed;
                }
                catch (Exception e)
                {
                   // MessageBox.Show(e.Message+"файл("+"),Строка("+i.ToString()+")", e.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Sheet.Tickness = 0;
                }
            }
            try { return Convert.ToInt32(jobtime) + M21count * speed.PierceTime + speed.SetupTime; }
            catch { return 20000; }
        }
        public void addline(string line)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string m = line.ToUpper().Trim();//.Replace(".", ",");
            if ((m.Length>7)&&(m.Substring(0, 7) == "(PART#:"))
            {
                string _Name = line.Substring(0, m.IndexOf("QTY"));
                //int qty = Convert.ToInt32(line.Substring(m.IndexOf("QTY") + 3, m.LastIndexOf(")") - m.IndexOf("QTY") - 3));
                if (line.IndexOf("<") > 0)
                {
                    string sX = "0";
                    string sY = "0";
                    try { sX = line.Substring(line.LastIndexOf("<") + 1, line.LastIndexOf("X") - line.LastIndexOf("<") - 1); } catch { }
                    try { sY = line.Substring(line.LastIndexOf("X") + 1, line.LastIndexOf(">") - line.LastIndexOf("X") - 1); } catch { }
                    Parts.Add(new _part(line.Substring(7, line.IndexOf("QTY") - 7), Convert.ToInt32(line.Substring(line.IndexOf("QTY") + 3, line.LastIndexOf("<") - line.IndexOf("QTY") - 3)), (float)Convert.ToDouble(sX), (float)Convert.ToDouble(sY)));
                }
                else 
                {
                    Parts.Add(new _part(line.Substring(7, line.IndexOf("QTY") - 7), Convert.ToInt32(line.Substring(line.IndexOf("QTY") + 3, line.LastIndexOf(")") - line.IndexOf("QTY") - 3)), 0,0));
                }
            }
            if ((m.Length > 11) && (m.Substring(0, 11) == "(THICKNESS="))
            {
                Sheet.Tickness = (float)Convert.ToDouble(m.Substring(11,m.Length-12));
            }
            if ((m.Length > 10) && (m.Substring(0, 10) == "(QTYTIMES "))
            {
                QuantityCut = Convert.ToInt32(m.Substring(10,m.Length-11));
            }

            if (m[0].ToString()=="(") { return;  }
            tempstep.textline = line;
            float tempvalueX, tempvalueY, tempvalueZ, tempvalueI, tempvalueJ, tempvalueK;
            if (m.IndexOf("M21") >= 0) { M21count++; cute = true; }
            if (m.IndexOf("M21") >= 0) { cute = false; }
            if (m.IndexOf("G90") >= 0) { absolutecoordinates = true; }
            if (m.IndexOf("G91") >= 0) { absolutecoordinates = false; }

            if ((m.IndexOf("X") >= 0) || (m.IndexOf("Y") >= 0) || (m.IndexOf("I") >= 0) || (m.IndexOf("J") >= 0) || (m.IndexOf("G00") >= 0)|| (m.IndexOf("G01") >= 0)|| (m.IndexOf("G02") >= 0)|| (m.IndexOf("G03") >= 0))
            {
                tempvalueK = getValuefromLine("K", ref m);
                tempvalueJ = getValuefromLine("J", ref m);
                tempvalueI = getValuefromLine("I", ref m);
                tempvalueZ = getValuefromLine("Z", ref m);
                tempvalueY = getValuefromLine("Y", ref m);
                tempvalueX = getValuefromLine("X", ref m);
                if (m == "G00") { CurrentCommad = comand.fastmove; }
                if (m == "G01") { CurrentCommad = comand.cuteline; }
                if (m == "G02") { CurrentCommad = comand.cutearc; }
                if (m == "G03") { CurrentCommad = comand.cutearc2; }
                
                    tempstep.Xstart = CurPoint.x;
                    tempstep.Ystart = CurPoint.y;
                    tempstep.Zstart = CurPoint.z;
                if (!absolutecoordinates)
                {
                    tempstep.Xend = CurPoint.x + tempvalueX;
                    tempstep.Yend = CurPoint.y + tempvalueY;
                    tempstep.Zend = CurPoint.z + tempvalueZ;
                }
                else
                {
                    tempstep.Xend = tempvalueX;
                    tempstep.Yend = tempvalueY;
                    tempstep.Zend = tempvalueZ;
                }
                CurPoint.x = tempstep.Xend;
                CurPoint.y = tempstep.Yend;
                CurPoint.z = tempstep.Zend;
                tempstep.Length = (float) Math.Sqrt((tempstep.Xend - tempstep.Xstart) * (tempstep.Xend - tempstep.Xstart) + (tempstep.Yend - tempstep.Ystart) * (tempstep.Yend - tempstep.Ystart));
                tempstep.Radius = (float) Math.Sqrt(tempvalueI * tempvalueI + tempvalueJ * tempvalueJ);
                tempstep.Comand = CurrentCommad;
                
                if ((CurrentCommad == comand.cutearc) || (CurrentCommad == comand.cutearc2))
                {
                    tempstep.I = tempvalueI;
                    tempstep.J = tempvalueJ;
                    tempstep.K = tempvalueK;
                    tempstep.Xcentr = tempstep.Xstart + tempstep.I;
                    tempstep.Ycentr = tempstep.Ystart + tempstep.J;
                    tempstep.Zcentr = tempstep.Zstart + tempstep.K;
                    if (tempstep.Length > 0)
                    {
                        tempstep.StartAngle = Angle(0, 0, tempvalueI, tempvalueJ);
                        tempstep.EndAngle = Angle(tempvalueX, tempvalueY, tempvalueI, tempvalueJ);
                        if ((CurrentCommad == comand.cutearc))
                        {
                            tempstep.SweepAngle = tempstep.EndAngle - tempstep.StartAngle;
                        }
                        else
                        {
                            if (tempstep.StartAngle > tempstep.EndAngle)
                            { tempstep.SweepAngle = -(tempstep.StartAngle - tempstep.EndAngle); }
                            else
                            { tempstep.SweepAngle = ( tempstep.EndAngle-tempstep.StartAngle )-360 ; }
                        }
                    }
                    else
                    {
                        tempstep.StartAngle = 0;
                        tempstep.EndAngle = 0;
                        tempstep.SweepAngle = 360;
                    }
                    for (int count = 0; count < 9; count++)
                    {
                        float zX = (float)(CurPoint.x + tempstep.I + tempstep.Radius * Math.Cos((tempstep.StartAngle + count * (tempstep.SweepAngle) / 8) * Math.PI / 180));
                        float zY = (float)(CurPoint.y + tempstep.J + tempstep.Radius * Math.Sin((tempstep.StartAngle + count * (tempstep.SweepAngle) / 8) * Math.PI / 180));
                        if (zX < Sheet.minX) { Sheet.minX = zX; }
                        if (zY < Sheet.minY) { Sheet.minY = zY; }
                        if (zX > Sheet.maxX) { Sheet.maxX = zX; }
                        if (zY > Sheet.maxY) { Sheet.maxY = zY; }
                    }
                }
                if ((CurrentCommad != comand.cuteline) || (CurrentCommad != comand.fastmove))
                {
                    if (tempstep.Xend < Sheet.minX) { Sheet.minX = tempstep.Xend; }
                    if (tempstep.Yend < Sheet.minY) { Sheet.minY = tempstep.Yend; }
                    if (tempstep.Xend > Sheet.maxX) { Sheet.maxX = tempstep.Xend; }
                    if (tempstep.Yend > Sheet.maxY) { Sheet.maxY = tempstep.Yend; }
                }
                if (CurrentCommad != comand.undefined) { AllPrimitives.Add(tempstep); }

            }

        }
        public void Dispose()
        {
            AllPrimitives.Clear();
        }
    }
}
