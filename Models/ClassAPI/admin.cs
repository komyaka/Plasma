using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Plazma.Models.ClassAPI
{
    public class admin
    {
        int addSheet(int mat, float tikn, int WIDTH, int HEIGTH, int Quantity = 1, string OWNER = "МОНТАЖНИК", string DOC = "", string Date = "")
        {
            int result = 0;

            return (result);
        }
        public string setindex(string Name, int index) 
        { 
            return Name.Substring(0,Name.LastIndexOf(".")) + "(N" + index.ToString() + ")" + Name.Substring(Name.LastIndexOf("."), Name.Length - Name.LastIndexOf(".")); 
        }
        
        public string getNewFileName(string originalName,string path) 
        {
            int count = 1;
            if (path.Substring(path.Length - 1) != "/") path += "/";
            if (!File.Exists(path + originalName)) return (originalName);
            while (File.Exists(path + setindex(originalName,count)))
            { 
                count++; 
            }
            return (setindex(originalName, count));
        }
        int addCNC(string fname, string pathDestination) 
        {
            int result = 0;
             
            return (result);
        }
    }
}