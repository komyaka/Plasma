using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Plazma.Models.NC;

namespace Plazma.Models
{
    public class NestInfo
    {
        public NestInfo(string fileName)
        {
            CNCParsebody(fileName);
        }


        public int Width { get; set; }
        public int Heigth { get; set; }
        public int M21Count { get; set; }
        public int fastmovelength { get; set; }
        public int cutlength { get; set; }
        public int Process { get; set; }// заменить на _Speed
        public int CutTimes { get; set; }
        public int Jobtime { get; set; }
        public string JobTime { get; set; }
        public enum comand { undefined, fastmove, cuteline, cutearc, cutearc2 }
        public class  _Sheet:Sheet 
        {
            public float minX, minY, maxX, maxY;
        }
        //public int nestWidth;
       // public int nestHeigth;
        public struct step
        {
            public string textline;
            public comand Comand;
            public float Xstart, Ystart, Zstart, Xcentr, Ycentr, Zcentr, Xend, Yend, Zend, StartAngle, EndAngle, SweepAngle, I, J, K, Length, Radius;
        }
        private bool cute = false;
        private step tempstep;
        private bool absolutecoordinates;
        public bool AbsoluteCoordinates { get { return absolutecoordinates; } set { absolutecoordinates = value; } }
        private comand CurrentCommad = comand.undefined;
        public struct point
        {
            public float x, y, z;
        }
        private point CurPoint;
        public point CurrentPoint { get { return CurPoint; } set { CurPoint = value; } }
        public List<step> AllPrimitives;
        public _Sheet sheet = new _Sheet();

        float getValuefromLine(string Symbol, ref string line)
        {
            if (!(line.IndexOf(Symbol) < 0))
            {
                string m = line.Substring(line.IndexOf(Symbol) + 1);
                line = line.Substring(0, line.IndexOf(Symbol));
                if ((Symbol == "Y") || (Symbol == "J")) { return (float)Convert.ToDouble(m, CultureInfo.InvariantCulture); }
                else
                {
                    return (float)Convert.ToDouble(m, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                return 0;
            }
        }
        float Angle(float x0, float y0, float x1, float y1)
        {
            float dx = (x1 - x0);
            float dy = (y1 - y0);
            float L = (float)Math.Sqrt(dx * dx + dy * dy);
            if (dy < 0)
            {
                return (float)(90 - (Math.Asin(-dx / L)) * 180 / Math.PI);
            }
            else
            {
                return (float)(360 - ((Math.Asin(dx / L)) * 180 / Math.PI + 90));
            }
        }

        public void addline(string line)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string m = line.ToUpper().Trim();//.Replace(".", ",");
            tempstep.textline = line;
            float tempvalueX, tempvalueY, tempvalueZ, tempvalueI, tempvalueJ, tempvalueK;
            if (m[0].ToString() == "(") { return; }
            if (m.IndexOf("M21") >= 0) { this.M21Count++; cute = true; }
            if (m.IndexOf("M21") >= 0) { cute = false; }
            if (m.IndexOf("G90") >= 0) { absolutecoordinates = true; }
            if (m.IndexOf("G91") >= 0) { absolutecoordinates = false; }

            if ((m.IndexOf("X") >= 0) || 
                (m.IndexOf("Y") >= 0) || 
                (m.IndexOf("I") >= 0) || 
                (m.IndexOf("J") >= 0) || 
                (m.IndexOf("G00") >= 0) || 
                (m.IndexOf("G01") >= 0) || 
                (m.IndexOf("G02") >= 0) || 
                (m.IndexOf("G03") >= 0))
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
                tempstep.Length = (float)Math.Sqrt((tempstep.Xend - tempstep.Xstart) * (tempstep.Xend - tempstep.Xstart) + (tempstep.Yend - tempstep.Ystart) * (tempstep.Yend - tempstep.Ystart));
                tempstep.Radius = (float)Math.Sqrt(tempvalueI * tempvalueI + tempvalueJ * tempvalueJ);
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
                            { tempstep.SweepAngle = (tempstep.EndAngle - tempstep.StartAngle) - 360; }
                        }
                    }
                    else
                    {
                        tempstep.StartAngle = 0;
                        tempstep.EndAngle = 0;
                        tempstep.SweepAngle = 360;
                    }
                    tempstep.Length =(float) Math.Abs( 2*tempstep.Radius * Math.PI* tempstep.SweepAngle/360);
                    for (int count = 0; count < 9; count++)
                    {
                        float zX = (float)(CurPoint.x + tempstep.I + tempstep.Radius * Math.Cos((tempstep.StartAngle + count * (tempstep.SweepAngle) / 8) * Math.PI / 180));
                        float zY = (float)(CurPoint.y + tempstep.J + tempstep.Radius * Math.Sin((tempstep.StartAngle + count * (tempstep.SweepAngle) / 8) * Math.PI / 180));
                        if (zX < sheet.minX) { sheet.minX = zX; }
                        if (zY < sheet.minY) { sheet.minY = zY; }
                        if (zX > sheet.maxX) { sheet.maxX = zX; }
                        if (zY > sheet.maxY) { sheet.maxY = zY; }
                    }
                }
                if ((CurrentCommad != comand.cuteline) || (CurrentCommad != comand.fastmove))
                {
                    if (tempstep.Xend < sheet.minX) { sheet.minX = tempstep.Xend; }
                    if (tempstep.Yend < sheet.minY) { sheet.minY = tempstep.Yend; }
                    if (tempstep.Xend > sheet.maxX) { sheet.maxX = tempstep.Xend; }
                    if (tempstep.Yend > sheet.maxY) { sheet.maxY = tempstep.Yend; }
                }
                if (CurrentCommad != comand.undefined) { AllPrimitives.Add(tempstep); }

            }

        }
        private float calcFastMovesLength(int ind) => (ind) < 1 ? 0 : AllPrimitives[ind].Comand == comand.fastmove ? AllPrimitives[ind].Length + calcFastMovesLength(ind - 1) : calcFastMovesLength(ind - 1);
        private    float   calcCuteLength(int ind) => (ind) < 1 ? 0 : AllPrimitives[ind].Comand != comand.fastmove ? AllPrimitives[ind].Length + calcFastMovesLength(ind - 1) : calcCuteLength(ind - 1);
        private int CalculateJobTime(string fileName)
        {
            int time = 0;
            speed Speed = new speed(sheet.Tickness == 0 ? gettiknessfromname(fileName) : sheet.Tickness.ToString());
            foreach (step s in AllPrimitives) 
            {
                if (s.Comand == comand.fastmove) fastmovelength +=(int) s.Length; else cutlength += (int) s.Length; 
            }
     //       this.fastmovelength = (int) calcFastMovesLength(AllPrimitives.Count - 1);
       //     this.cutlength = (int)calcCuteLength(AllPrimitives.Count - 1);
            time = (this.fastmovelength*60 / Speed.FastmoveSpeed + this.cutlength*60 / Speed.CuttingSpeed)+Speed.PierceTime+M21Count+Speed.SetupTime;
            return time;
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
        private void CNCParsebody(string Filename)
        {
            string line;
            AllPrimitives = new List<step> { };
            M21Count = 0;
            absolutecoordinates = true;
            sheet.maxX = 0;
            sheet.maxY = 0;
            sheet.minX = 0;
            sheet.minY = 0;
            sheet.Tickness = 0;
            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(Filename, System.Text.Encoding.Default);
                while ((line = file.ReadLine()) != null)
                {
                    //  Form1.addnewstring(line);
                    addline(line);
                }
                file.Close();
            }
            catch (Exception e) {
                  string t= e.Message;
                string r = e.Source;
                    }
            Jobtime = CalculateJobTime(Filename);
            TimeSpan ts = TimeSpan.FromSeconds(Jobtime*1.55);
            JobTime = (ts.Hours==0?"":(ts.Hours + "ч:")) + ts.Minutes + "мин";
            Heigth =(int) Math.Abs(sheet.maxY - sheet.minY);
            Width = (int) Math.Abs(sheet.maxX - sheet.minX);
        }
    }
}
