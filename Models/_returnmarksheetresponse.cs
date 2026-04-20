using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plazma.Models
{
    public class _returnmarksheetresponse
    {
        public int cncid;
        public int sheetid;
        public bool result;
        public string user;

        public _returnmarksheetresponse(bool result=false)
        {
            this.result = result;
        }
    }
}