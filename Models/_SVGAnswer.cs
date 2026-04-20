using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class _SVGAnswer
    {
        public string path;
        public int minX, minY, maxX, maxY;
        public _SVGAnswer() {
            path = "";
            minX = minY = maxX = maxY = 0;
        }
    }
    
}