using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models.NC
{
    public class NCreader
    {
        public CNCHead head;
        public CncInfo cnc;
        public NestInfo Nest;
        public NCreader()
        {
            head = new CNCHead();
            cnc = new CncInfo();
        }
        public NCreader(string fileName)
        {
            head = new CNCHead(fileName);
            cnc = new CncInfo(fileName);
            Nest = new NestInfo(fileName);
        }
    }
}