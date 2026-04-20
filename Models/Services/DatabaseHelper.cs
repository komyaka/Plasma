using System;
using System.Data.SqlClient;
using Plazma.Controllers;

namespace Plazma.Models.Services
{
    public class DatabaseHelper
    {
        private readonly PartsClass _parts;

        public DatabaseHelper(PartsClass parts)
        {
            _parts = parts;
        }

        public int FreeRequestToBd(string request, params SqlParameter[] parameters)
        {
            return _parts.FreeRequestToBD(request, parameters);
        }

        public DateTime GetLastUpdateTime(string tableName)
        {
            return _parts.GetLastUpdateTime(tableName);
        }

        public string GetUser()
        {
            return _parts.GetUser();
        }
    }
}
