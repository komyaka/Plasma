using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class XLSMaterial
    {
        public double tickness { get; set; }
        public int XLSType { get; set; }
        public int pLasmaType { get; set; }
        XLSMaterial(double _tickness, int _XLSType)
        {
            tickness = _tickness;
            XLSType = _XLSType;
            pLasmaType = retype(_XLSType);
        }
        int retype(int T)
        {
            int mat = -1;
            if (T == 1) mat = 1;
            if (T == 2) mat = -1;
            if (T == 3) mat = 5;
            if (T == 4) mat = 2;
            if (T == 5) mat = 6;
            if (T == 6) mat = 3;
            return mat;
        }
    }
}