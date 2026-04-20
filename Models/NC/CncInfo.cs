using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class CncInfo
    {
        public struct _Nest 
        {
            public _Nest(int width, int heigth)
            {
                Width = width;
                Heigth = heigth;
            }

            public int Width { get; set; }
            public int Heigth { get; set; }
        }
        public string Filename { get; set; }
        public int M21Count { get; set; }
        public int cutlengh { get; set; }
        public int fastmovelength { get; set; }
        public int cuttingtime { get; set; }
        public int CutTimes { get; set; }
        public Sheet sheet;
        private CNCHead head;
        public _Nest Nest;
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
        public Dictionary<int, _Material> materials = new Dictionary<int, _Material> { { 0, new _Material( "NONAME", "Неизвестный материал" ) }
                                                                                    ,{ 1, new _Material( "", "Сталь3" ) }
                                                                                    ,{ 2, new _Material( "09G2S", "09Г2С" ) }
                                                                                    ,{ 3, new _Material( "NERJ", "Нержавейка" ) }
                                                                                    ,{ 4, new _Material( "RIFL", "Рифл.(Чечевица)" ) }
                                                                                    ,{ 5, new _Material( "RIFL_R", "Рифл.(Ромб)" ) }
                                                                                    ,{ 6, new _Material("10HSND","10ХСНД" ) }
};

        public void readFromCNC(string Filename) 
        {
            head = new CNCHead(Filename);
            
        }
        public CncInfo() 
        {
            sheet = new Sheet();
            Nest = new _Nest();
        }
        public CncInfo(string Filename)
        {
            Nest = new _Nest();
            sheet = new Sheet();
            //sheet = head.sheet;
            readFromCNC(Filename);
            this.Filename = Filename;
            
            
        }

    }
}