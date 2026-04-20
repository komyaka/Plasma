using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Threading;
using Plazma.Models;

namespace Plazma.Controllers
{
    public class PartsClass
    {
        //object block = new object { };
        public string[] materials = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", };
        public struct _CNC
        {
            public int Id;
            public string FileName;
            public string OriginalName;
            public int RunTimeOneSheet;
            public int Quantity;
            public int QuantityDone;
            public string realrickness;
            public string tickness;
            public int RunTimeAll;
            public DateTime AddedTime;
            public DateTime FileCreatedTime;
            public string Sheets;
            public int SheetWidth;
            public int SheetHeigh;
            public int Material;
            public string Reserve1;
            public string Reserve2;
            public string Reserve3;
            public _CNC(string Filename, string OriginalFile, int RuntimeOneSheet = 0, int quantity = 1, int Quantytidone = 0, DateTime? addedTime = null, DateTime? Filecreatedtime = null, string Realtickness = "0", string Tickness = "0", string sheets = "", int material = -1, int width = -1, int heigth = -1, string reserve1 = "", string reserve2 = "", string reserve3 = "", int _id = 0)
            {
                Id = _id;
                FileName = Filename;
                OriginalName = OriginalFile;
                FileName = OriginalFile;
                RunTimeOneSheet = RuntimeOneSheet;
                Quantity = quantity;
                QuantityDone = Quantytidone;
                RunTimeAll = (Quantity * RuntimeOneSheet);
                if (!addedTime.HasValue)
                {
                    addedTime = DateTime.Now;
                }
                AddedTime = addedTime.Value;
                FileCreatedTime = DateTime.Now;
                if (!Filecreatedtime.HasValue)
                {
                    Filecreatedtime = AddedTime;
                }
                else FileCreatedTime = Filecreatedtime.Value;
                realrickness = Realtickness;
                tickness = Tickness;
                Sheets = sheets;
                SheetWidth = width;
                SheetHeigh = heigth;
                Material = material < 0 ? GetMaterialFromName(Filename) : material;
                Reserve1 = reserve1;
                Reserve2 = reserve2;
                Reserve3 = reserve3;
            }
        }
        public struct _sheet
        {
            public int Id;
            public string Name;
            public int Matherial;
            public float Tickness;
            public int Width;
            public int Heigth;
            public int Quantity;
            public string CNCFILE;
            public string Parts;
            public string Owner;
            public string Status;
            public string Reserve1;
            public string Reserve2;
            public string Reserve3;
            public string Document;
            public DateTime Date;
            public _sheet(string name, int matherial, float tickness, int width, int heigth, int quantity, string CNCFile, string parts, string owner, string status, string reserve1 = "", string reserve2 = "", string reserve3 = "", string document = "", DateTime? date = null)
            {
                Id = 0;
                Name = name;
                Matherial = matherial;
                Tickness = tickness;
                Width = width;
                Heigth = heigth;
                Quantity = quantity;
                CNCFILE = CNCFile;
                Parts = parts;
                Owner = owner;
                Status = status;
                Reserve1 = reserve1;
                Reserve2 = reserve2;
                Reserve3 = reserve3;
                Document = document;
                if (!date.HasValue)
                {
                    date = DateTime.Now;
                }
                Date = date.Value;
            }
        }
        public struct _Part
        {
            public int Id;
            public string Name;
            public int Quantity;
            public int QuantitySummary;
            public int QuantityCutted;
            public int Shipped;
            public string CNCID;
            public string tickness;
            public string Size_X;
            public string Size_Y;
            public string Reserve1;
            public string Reserve2;
            public string Reserve3;
            public _Part(string name, int quantity = 1, int quantitysum = 1, int Quantitycutted = 0, int shipped = 0, string Tickness = "0", string SizeX = "0", string SizeY = "0", string CncId = "", string reserve1 = "", string reserve2 = "", string reserve3 = "")
            {
                Id = 0;
                Name = name;
                Quantity = quantity;
                QuantitySummary = quantitysum;
                QuantityCutted = Quantitycutted;
                Shipped = shipped;
                tickness = Tickness;
                Size_X = SizeX;
                Size_Y = SizeY;
                CNCID = CncId;
                Reserve1 = reserve1;
                Reserve2 = reserve2;
                Reserve3 = reserve3;
            }

        }
        public struct _Order
        {
            public string Name;
            public int all;
            public int cutted;
            public DateTime Date;
            public _Order(string name, int qtyAll, int qtyCutted, DateTime _Date)
            {
                Name = name;
                all = qtyAll;
                cutted = qtyCutted;
                Date = _Date;
            }
        }
        public struct _Shipment
        {
            public int id;
            public int partId;
            public float tikcness;
            public int width;
            public int heigth;
            public int Shiped;
            public DateTime Shiptime;
            public string orderName;
            public _Shipment(int partId = -1, int shiped = -1, DateTime? shiptime = null, string orderName = "") : this()
            {
                this.partId = partId;
                Shiped = shiped;
                if (!shiptime.HasValue) Shiptime = DateTime.Parse("1996/04/26 01:23:47");
                else Shiptime = shiptime.Value;
                this.orderName = orderName;
            }
        }
        public struct Chapter
        {
            public string Name;
            public int accessCode;
            public Chapter(string _Name = "", int _accessCode = 0)
            {
                Name = _Name;
                accessCode = _accessCode;
            }
        }
        public List<_CNC> CNCs = new List<_CNC> { };
        public List<_sheet> Sheets = new List<_sheet> { };
        public List<_Part> Parts = new List<_Part> { };
        public List<_Order> NORDER = new List<_Order> { };
        public List<_Shipment> Shipments = new List<_Shipment> { };
        public List<Chapter> chapters = new List<Chapter> { };
        public Dictionary<string,int> Dchapters = new Dictionary<string, int> { };
        public void showError(Exception e) { Logger.Error("PartsClass error", e); }  // ИСПРАВЛЕНО: логирование ошибок
        public int getChapterCode(string _Name)
        {
            int id = Math.Abs(chapters.FindIndex(X => X.Name.Trim().ToUpper() == _Name.Trim().ToUpper()));

            return chapters[id].accessCode;
        }
        SqlConnection sqlconnection = new SqlConnection(Constants.bdconnectionstring);
        //sqlconnection;
        public void CloseSqlConnection()
        {
            if (!(sqlconnection.State == System.Data.ConnectionState.Closed)) { sqlconnection.Close(); }
            //  while (!(sqlconnection.State == System.Data.ConnectionState.Closed)) { }
        }
        public void ConnectDB()
        {
            if (sqlconnection.State != System.Data.ConnectionState.Open)
                sqlconnection.Open();
            //while (!(sqlconnection.State == System.Data.ConnectionState.Open)) { }
        }
        public void clearbase()
        {
            ConnectDB();

            string comm = "DROP TABLE [dbo].[CNCFILES]";

            SqlCommand command = new SqlCommand(comm, sqlconnection);
            try
            {
                command.ExecuteNonQuery();
                comm = "DROP TABLE [dbo].[PARTS]";
                command = new SqlCommand(comm, sqlconnection);
                command.ExecuteNonQuery();
                comm = "DROP TABLE [dbo].[SHEETS]";
                command = new SqlCommand(comm, sqlconnection);
                command.ExecuteNonQuery();
            }
            catch { }
            CloseSqlConnection();
            ConnectDB();
            comm = "CREATE TABLE [dbo].[CNCFILES] ([ID] INT IDENTITY(1, 1) NOT NULL,[FILENAME] NVARCHAR(255) NULL,[ORIGINALFILENAME] NVARCHAR(255) NULL,[RUNTIMEONESHEET] INT NULL,[QUANTITY] INT NULL,[QUANTITYDONE] INT NULL,[RUNTIMEALL] INT NULL,[ADDEDTIME] DATETIME2(7) NULL,[FILECREATEDTIME] DATETIME2(7) NULL,[REALTICKNESS] NVARCHAR(5)  NULL,[TICKNESS] NVARCHAR(5)  NULL,[SHEETS] TEXT NULL, [RESERVE1] TEXT NULL, [RESERVE2] TEXT NULL,[RESERVE3] TEXT NULL,PRIMARY KEY CLUSTERED([ID] ASC));";
            command = new SqlCommand(comm, sqlconnection);
            command.ExecuteNonQuery();
            comm = "CREATE TABLE [dbo].[PARTS] ([ID] INT IDENTITY(1, 1) NOT NULL,[NAME] NVARCHAR(255) NULL, [QUANTITY] INT NULL,[QUANTITYSUMM] INT NULL,[QUANTITYCUTTED] INT NULL,[TICKNESS] NVARCHAR (5) NULL,[SIZE_X] NVARCHAR (10) NULL,[SIZE_Y] NVARCHAR (10) NULL,[CNCID] INT NULL,[RESERVE1] TEXT NULL,[RESERVE2] TEXT NULL,[RESERVE3] TEXT NULL, PRIMARY KEY CLUSTERED([Id] ASC))";
            command = new SqlCommand(comm, sqlconnection);
            command.ExecuteNonQuery();
            comm = "CREATE TABLE [dbo].[SHEETS] ([ID] INT IDENTITY(1, 1) NOT NULL,[NAME] NVARCHAR(255) NULL,  [MATHERIAL] INT NULL,[TICKNESS]  REAL NULL, [WIDTH] INT NULL,[HEIGTH] INT NULL,[CNCFILE] NVARCHAR(50) NULL, [PARTS] TEXT NULL,[OWNER]  NVARCHAR(50) NULL, [STATUS] NCHAR(10) NULL,[RESERVE1] TEXT NULL,[RESERVE2]  TEXT NULL,[RESERVE3] TEXT NULL,PRIMARY KEY CLUSTERED([Id] ASC))";
            command = new SqlCommand(comm, sqlconnection);
            command.ExecuteNonQuery();
            CloseSqlConnection();
        }
        public string GetUser()
        {
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT CURRENT_USER as 'user'", sqlconnection);
            TableReader = command.ExecuteReader();
            if (TableReader.Read()) return Convert.ToString(TableReader["user"]);
            return "No_User";
        }
        public static int GetMaterialFromName(string fname)
        {
            if (fname.ToUpper().IndexOf("09G2S") > 0) return 2;
            if (fname.ToUpper().IndexOf("NERJ") > 0) return 3;
            if (fname.ToUpper().IndexOf("RIFL_R") > 0) return 5;
            if (fname.ToUpper().IndexOf("RIFL") > 0) return 4;
            if (fname.ToUpper().IndexOf("HSND") > 0) return 6;
            return 1;
        }
        private void readCNC()
        {
            /*sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            await sqlconnection.OpenAsync();
            */

            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM CNCFILES WHERE ARHIVE IS NULL", sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())// lock (block)
                {
                    if (!Convert.ToBoolean(TableReader["ARHIVE"] == DBNull.Value ? false : TableReader["ARHIVE"]))
                    {
                        _CNC newcnc;
                        newcnc.Id = Convert.ToInt32(TableReader["ID"] == DBNull.Value ? 0 : TableReader["ID"]);
                        newcnc.FileName = Convert.ToString(TableReader["FILENAME"] == DBNull.Value ? "UNCNOVNFILE" : TableReader["FILENAME"]);
                        newcnc.OriginalName = Convert.ToString(TableReader["ORIGINALFILENAME"] == DBNull.Value ? "UNCNOVNFILE" : TableReader["ORIGINALFILENAME"]);
                        newcnc.RunTimeOneSheet = Convert.ToInt32(TableReader["RUNTIMEONESHEET"] == DBNull.Value ? 0 : TableReader["RUNTIMEONESHEET"]);
                        newcnc.Quantity = Convert.ToInt32(TableReader["QUANTITY"] == DBNull.Value ? 0 : TableReader["QUANTITY"]);
                        newcnc.QuantityDone = Convert.ToInt32(TableReader["QUANTITYDONE"] == DBNull.Value ? 0 : TableReader["QUANTITYDONE"]);
                        newcnc.realrickness = Convert.ToString(TableReader["REALTICKNESS"] == DBNull.Value ? "0" : TableReader["REALTICKNESS"]);
                        newcnc.tickness = Convert.ToString(TableReader["TICKNESS"] == DBNull.Value ? "0" : TableReader["TICKNESS"]);
                        newcnc.RunTimeAll = Convert.ToInt32(TableReader["RUNTIMEALL"] == DBNull.Value ? 0 : TableReader["RUNTIMEALL"]);
                        newcnc.AddedTime = Convert.ToDateTime(TableReader["ADDEDTIME"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["ADDEDTIME"]);
                        newcnc.FileCreatedTime = Convert.ToDateTime(TableReader["FILECREATEDTIME"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["FILECREATEDTIME"]);
                        newcnc.Sheets = Convert.ToString(TableReader["SHEETS"] == DBNull.Value ? "" : TableReader["SHEETS"]);
                        newcnc.SheetWidth = Convert.ToInt32(TableReader["SHEETWIDTH"] == DBNull.Value ? 0 : TableReader["SHEETWIDTH"]);
                        newcnc.SheetHeigh = Convert.ToInt32(TableReader["SHEETHEIGTH"] == DBNull.Value ? 0 : TableReader["SHEETHEIGTH"]);
                        newcnc.Material = Convert.ToInt32(TableReader["MATERIAL"] == DBNull.Value ? -1 : TableReader["MATERIAL"]);
                        newcnc.Reserve1 = Convert.ToString(TableReader["RESERVE1"] == DBNull.Value ? "" : TableReader["RESERVE1"]);
                        newcnc.Reserve2 = Convert.ToString(TableReader["RESERVE2"] == DBNull.Value ? "" : TableReader["RESERVE2"]);
                        newcnc.Reserve3 = Convert.ToString(TableReader["RESERVE3"] == DBNull.Value ? "" : TableReader["RESERVE3"]);
                        CNCs.Add(newcnc);
                    }
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        public void readCNC(string querry)
        {
            /*sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            await sqlconnection.OpenAsync();
            */
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand(querry, sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                int id = 1;
                while (TableReader.Read())
                    if (!Convert.ToBoolean(TableReader["ARHIVE"] == DBNull.Value ? false : TableReader["ARHIVE"]))
                    {
                        _CNC newcnc;
                        try { newcnc.Id = Convert.ToInt32(TableReader["ID"] == DBNull.Value ? 0 : TableReader["ID"]); } catch { newcnc.Id = id; id++; }
                        try { newcnc.FileName = Convert.ToString(TableReader["FILENAME"] == DBNull.Value ? "UNCNOVNFILE" : TableReader["FILENAME"]); } catch { newcnc.FileName = ""; }
                        try { newcnc.OriginalName = Convert.ToString(TableReader["ORIGINALFILENAME"] == DBNull.Value ? "UNCNOVNFILE" : TableReader["ORIGINALFILENAME"]); } catch { newcnc.OriginalName = ""; }
                        try { newcnc.RunTimeOneSheet = Convert.ToInt32(TableReader["RUNTIMEONESHEET"] == DBNull.Value ? 0 : TableReader["RUNTIMEONESHEET"]); } catch { newcnc.RunTimeOneSheet = 0; }
                        try { newcnc.Quantity = Convert.ToInt32(TableReader["QUANTITY"] == DBNull.Value ? 0 : TableReader["QUANTITY"]); } catch { newcnc.Quantity = 0; }
                        try { newcnc.QuantityDone = Convert.ToInt32(TableReader["QUANTITYDONE"] == DBNull.Value ? 0 : TableReader["QUANTITYDONE"]); } catch { newcnc.QuantityDone = 0; }
                        try { newcnc.realrickness = Convert.ToString(TableReader["REALTICKNESS"] == DBNull.Value ? "0" : TableReader["REALTICKNESS"]); } catch { newcnc.realrickness = "0"; }
                        try { newcnc.tickness = Convert.ToString(TableReader["TICKNESS"] == DBNull.Value ? "0" : TableReader["TICKNESS"]); } catch { newcnc.tickness = "0"; }
                        try { newcnc.RunTimeAll = Convert.ToInt32(TableReader["RUNTIMEALL"] == DBNull.Value ? 0 : TableReader["RUNTIMEALL"]); } catch { newcnc.RunTimeAll = 0; }
                        try { newcnc.AddedTime = Convert.ToDateTime(TableReader["ADDEDTIME"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["ADDEDTIME"]); } catch { newcnc.AddedTime = DateTime.Parse("1996/04/26 01:23:47"); }
                        try { newcnc.FileCreatedTime = Convert.ToDateTime(TableReader["FILECREATEDTIME"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["FILECREATEDTIME"]); } catch { newcnc.FileCreatedTime = DateTime.Parse("1996/04/26 01:23:47"); }
                        try { newcnc.Sheets = Convert.ToString(TableReader["SHEETS"] == DBNull.Value ? "" : TableReader["SHEETS"]); } catch { newcnc.Sheets = ""; }
                        try { newcnc.SheetWidth = Convert.ToInt32(TableReader["SHEETWIDTH"] == DBNull.Value ? 0 : TableReader["SHEETWIDTH"]); } catch { newcnc.SheetWidth = -1; }
                        try { newcnc.SheetHeigh = Convert.ToInt32(TableReader["SHEETHEIGTH"] == DBNull.Value ? 0 : TableReader["SHEETHEIGTH"]); } catch { newcnc.SheetHeigh = -1; }
                        try { newcnc.Material = Convert.ToInt32(TableReader["MATERIAL"] == DBNull.Value ? GetMaterialFromName(newcnc.FileName) : TableReader["MATERIAL"]); } catch { newcnc.Material = -1; }
                        try { newcnc.Reserve1 = Convert.ToString(TableReader["RESERVE1"] == DBNull.Value ? "" : TableReader["RESERVE1"]); } catch { newcnc.Reserve1 = ""; }
                        try { newcnc.Reserve2 = Convert.ToString(TableReader["RESERVE2"] == DBNull.Value ? "" : TableReader["RESERVE2"]); } catch { newcnc.Reserve2 = ""; }
                        try { newcnc.Reserve3 = Convert.ToString(TableReader["RESERVE3"] == DBNull.Value ? "" : TableReader["RESERVE3"]); } catch { newcnc.Reserve3 = ""; }
                        CNCs.Add(newcnc);
                    }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        public string GetNameCNC(int ID)
        {
            int t = CNCs.FindIndex(x => x.Id == ID);
            if (t > 0) return CNCs[t].FileName; else return "Не известно";
        }
        private int _InsertCNC(_CNC OnecNC)
        {
            int returnedID = 0;
           // SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("INSERT INTO [CNCFILES] (FILENAME,ORIGINALFILENAME,RUNTIMEONESHEET,QUANTITY,RUNTIMEALL,ADDEDTIME,FILECREATEDTIME,REALTICKNESS,TICKNESS,SHEETS,RESERVE1,RESERVE2,RESERVE3) OUTPUT INSERTED.ID VALUES (@FILENAME,@ORIGINALFILENAME,@RUNTIMEONESHEET,@QUANTITY,@RUNTIMEALL,@ADDEDTIME,@FILECREATEDTIME,@REALTICKNESS,@TICKNESS,@SHEETS,@RESERVE1,@RESERVE2,@RESERVE3);SELECT IDENT_CURRENT('CNCFILES')", sqlconnection);
            command.Parameters.AddWithValue("FILENAME", OnecNC.FileName);
            command.Parameters.AddWithValue("ORIGINALFILENAME", OnecNC.OriginalName);
            command.Parameters.AddWithValue("RUNTIMEONESHEET", OnecNC.RunTimeOneSheet);
            command.Parameters.AddWithValue("QUANTITY", OnecNC.Quantity);
            command.Parameters.AddWithValue("QUANTITYDONE", OnecNC.QuantityDone);
            command.Parameters.AddWithValue("RUNTIMEALL", OnecNC.RunTimeAll);
            command.Parameters.AddWithValue("ADDEDTIME", OnecNC.AddedTime);
            command.Parameters.AddWithValue("FILECREATEDTIME", OnecNC.FileCreatedTime);
            command.Parameters.AddWithValue("REALTICKNESS", OnecNC.realrickness);
            command.Parameters.AddWithValue("TICKNESS", OnecNC.tickness);
            command.Parameters.AddWithValue("SHEETS", OnecNC.Sheets);
            command.Parameters.AddWithValue("RESERVE1", OnecNC.Reserve1);
            command.Parameters.AddWithValue("RESERVE2", OnecNC.Reserve2);
            command.Parameters.AddWithValue("RESERVE3", OnecNC.Reserve3);
            try
            {
                returnedID = (int)command.ExecuteScalar();
                OnecNC.Id = returnedID;
                CNCs.Add(OnecNC);
            }
            catch (Exception e)
            {
                showError(e);
            }

            return returnedID;
        }
        public int AddCNCtoBD(_CNC CNCRecord)
        {
            return _InsertCNC(CNCRecord);
        }
        public int AddCNCtoBD(string Filename, string OriginalFile, int RuntimeOneSheet = 0, int Quantity = 1, int Quantitydone = 1, DateTime? AddedTime = null, DateTime? FileCreatedTime = null, string realtickness = "0", string tickness = "0", string Sheets = "", string Reserve1 = "", string Reserve2 = "", string Reserve3 = "")
        {
            _CNC CNCRecord = new _CNC { };
            CNCRecord.Id = 0;
            CNCRecord.FileName = Filename;
            CNCRecord.OriginalName = OriginalFile;
            CNCRecord.FileName = OriginalFile;
            CNCRecord.RunTimeOneSheet = RuntimeOneSheet;
            CNCRecord.Quantity = Quantity;
            CNCRecord.QuantityDone = Quantitydone;
            CNCRecord.RunTimeAll = (Quantity * RuntimeOneSheet);
            if (!AddedTime.HasValue)
            {
                AddedTime = DateTime.Now;
            }
            CNCRecord.AddedTime = AddedTime.Value;
            if (!FileCreatedTime.HasValue)
            {
                FileCreatedTime = DateTime.Now;
            }
            CNCRecord.FileCreatedTime = AddedTime.Value;
            CNCRecord.realrickness = realtickness;
            CNCRecord.tickness = tickness;
            CNCRecord.Sheets = Sheets;
            CNCRecord.Reserve1 = Reserve1;
            CNCRecord.Reserve2 = Reserve2;
            CNCRecord.Reserve3 = Reserve3;
            return _InsertCNC(CNCRecord);
        }
        private void ReadSheets()
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open(); 
            Sheets.Clear();
            //    while (!(sqlconnection.State == System.Data.ConnectionState.Open)) { }
            // ***********************    Этот запрос не позволяет получить имена документов и даты прихода
            //SqlDataReader TableReader = null;                                                                  //        ,DOKUMENT,cast(DATE as date) as DATE                                                                           ,DOKUMENT,cast(DATE as date)
            //SqlCommand command = new SqlCommand("select NAME,MATHERIAL,TICKNESS,WIDTH,HEIGTH,COUNT(QUANTITY) AS QTY,OWNER                                     from plasma.dbo.sheets group by MATHERIAL,TICKNESS,WIDTH,HEIGTH,NAME,OWNER                             ORDER BY NAME,MATHERIAL,TICKNESS,WIDTH,HEIGTH,QTY", sqlconnection);
            // ***********************    Этот запрос не позволяет Группировать все не начатые листы т.к. документы и даты разные
            SqlDataReader TableReader = null;                                                                  //                                                                                   
            SqlCommand command = new SqlCommand("select NAME,MATHERIAL,TICKNESS,WIDTH,HEIGTH,COUNT(QUANTITY) AS QTY,COALESCE(NAME,OWNER) as OWNER,COALESCE(NAME,Null) as DOCUMENT,COALESCE(NAME,null) as DATE from plasma.dbo.sheets group by MATHERIAL,TICKNESS,WIDTH,HEIGTH,NAME,OWNER,COALESCE(NAME,Null),COALESCE(NAME,null) ORDER BY NAME,MATHERIAL,TICKNESS,WIDTH,HEIGTH,QTY", sqlconnection);
            int pos = 0;
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())// lock(block)
                //if (!Convert.ToBoolean(TableReader["ARHIVE"] == DBNull.Value ? false : TableReader["ARHIVE"]))
                {
                    pos++;
                    _sheet newsheet;
                    try { newsheet.Id = pos;/*Convert.ToInt32(TableReader["ID"] == DBNull.Value ? 0 : TableReader["ID"]);*/} catch { newsheet.Id = 0; }
                    try { newsheet.Name = Convert.ToString(TableReader["NAME"] == DBNull.Value ? "" : TableReader["NAME"]); } catch { newsheet.Name = ""; }
                    try { newsheet.Matherial = Convert.ToInt32(TableReader["MATHERIAL"] == DBNull.Value ? 0 : TableReader["MATHERIAL"]); } catch { newsheet.Matherial = 0; }
                    try { newsheet.Tickness = (float)Convert.ToDouble(TableReader["TICKNESS"] == DBNull.Value ? 0 : TableReader["TICKNESS"]); } catch { newsheet.Tickness = 0; }
                    try { newsheet.Width = Convert.ToInt32(TableReader["WIDTH"] == DBNull.Value ? 6000 : TableReader["WIDTH"]); } catch { newsheet.Width = 0; }
                    try { newsheet.Heigth = Convert.ToInt32(TableReader["HEIGTH"] == DBNull.Value ? 1500 : TableReader["HEIGTH"]); } catch { newsheet.Heigth = 0; }
                    try { newsheet.Quantity = Convert.ToInt32(TableReader["QTY"] == DBNull.Value ? 1500 : TableReader["QTY"]); } catch { newsheet.Quantity = 0; }
                    try { newsheet.CNCFILE = "";/*Convert.ToString(TableReader["CNCFILE"] == DBNull.Value ? "" : TableReader["CNCFILE"]);*/} catch { newsheet.CNCFILE = ""; }
                    try { newsheet.Parts = "";/*Convert.ToString(TableReader["PARTS"] == DBNull.Value ? "" : TableReader["PARTS"]);*/} catch { newsheet.Parts = ""; }
                    try { newsheet.Owner = Convert.ToString(TableReader["OWNER"] == DBNull.Value ? "" : TableReader["OWNER"]); } catch { newsheet.Owner = ""; }
                    try { newsheet.Status = "";/*Convert.ToString(TableReader["STATUS"] == DBNull.Value ? "" : TableReader["STATUS"]);*/} catch { newsheet.Status = ""; }
                    try { newsheet.Reserve1 = "";/*Convert.ToString(TableReader["RESERVE1"] == DBNull.Value ? "" : TableReader["RESERVE1"]);*/} catch { newsheet.Reserve1 = ""; }
                    try { newsheet.Reserve2 = "";/* Convert.ToString(TableReader["RESERVE2"] == DBNull.Value ? "" : TableReader["RESERVE2"]);*/} catch { newsheet.Reserve2 = ""; }
                    try { newsheet.Reserve3 = "";/* Convert.ToString(TableReader["RESERVE3"] == DBNull.Value ? "" : TableReader["RESERVE3"]);*/} catch { newsheet.Reserve3 = ""; }
                    try { newsheet.Date = DateTime.Parse(Convert.ToString(TableReader["DATE"] == DBNull.Value ? "" : TableReader["DATE"])); } catch { newsheet.Date = DateTime.Parse("12-12-2112"); }
                    try { newsheet.Document = Convert.ToString(TableReader["DOKUMENT"] == DBNull.Value ? "" : TableReader["DOKUMENT"]); } catch { newsheet.Document = "=X-X-X="; }
                    Sheets.Add(newsheet);
                    newsheet.Date.ToString("s").Substring(0, newsheet.Date.ToString("s").IndexOf("T"));
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        public void ReadSheets(string Querry)
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open();
            //    while (!(sqlconnection.State == System.Data.ConnectionState.Open)) { }
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand(Querry, sqlconnection);
            Sheets.Clear();
            try
            {
                TableReader = command.ExecuteReader();
                int id = 0;
                while (TableReader.Read())
                //if (!Convert.ToBoolean(TableReader["ARHIVE"] == DBNull.Value ? false : TableReader["ARHIVE"]))
                {
                    _sheet newsheet;
                    try { newsheet.Id = Convert.ToInt32(TableReader["ID"] == DBNull.Value ? 0 : TableReader["ID"]); } catch { newsheet.Id = id; id++; }
                    try { newsheet.Name = Convert.ToString(TableReader["NAME"] == DBNull.Value ? "" : TableReader["NAME"]); } catch { newsheet.Name = ""; }
                    try { newsheet.Matherial = Convert.ToInt32(TableReader["MATHERIAL"] == DBNull.Value ? 0 : TableReader["MATHERIAL"]); } catch { newsheet.Matherial = 0; }
                    try { newsheet.Tickness = (float)Convert.ToDouble(TableReader["TICKNESS"] == DBNull.Value ? 0 : TableReader["TICKNESS"]); } catch { newsheet.Tickness = 0; }
                    try { newsheet.Width = Convert.ToInt32(TableReader["WIDTH"] == DBNull.Value ? 6000 : TableReader["WIDTH"]); } catch { newsheet.Width = -1; }
                    try { newsheet.Heigth = Convert.ToInt32(TableReader["HEIGTH"] == DBNull.Value ? 1500 : TableReader["HEIGTH"]); } catch { newsheet.Heigth = -1; }
                    try { newsheet.Quantity = Convert.ToInt32(TableReader["QUANTITY"] == DBNull.Value ? 1500 : TableReader["QUANTITY"]); } catch { newsheet.Quantity = 1; }
                    try { newsheet.CNCFILE = Convert.ToString(TableReader["CNCFILE"] == DBNull.Value ? "" : TableReader["CNCFILE"]); } catch { newsheet.CNCFILE = ""; }
                    try { newsheet.Parts = Convert.ToString(TableReader["PARTS"] == DBNull.Value ? "" : TableReader["PARTS"]); } catch { newsheet.Parts = ""; }
                    try { newsheet.Owner = Convert.ToString(TableReader["OWNER"] == DBNull.Value ? "" : TableReader["OWNER"]); } catch { newsheet.Owner = "МОНТАЖНИК"; }
                    try { newsheet.Status = Convert.ToString(TableReader["STATUS"] == DBNull.Value ? "" : TableReader["STATUS"]); } catch { newsheet.Status = ""; }
                    try { newsheet.Reserve1 = Convert.ToString(TableReader["RESERVE1"] == DBNull.Value ? "" : TableReader["RESERVE1"]); } catch { newsheet.Reserve1 = ""; }
                    try { newsheet.Reserve2 = Convert.ToString(TableReader["RESERVE2"] == DBNull.Value ? "" : TableReader["RESERVE2"]); } catch { newsheet.Reserve2 = ""; }
                    try { newsheet.Reserve3 = Convert.ToString(TableReader["RESERVE3"] == DBNull.Value ? "" : TableReader["RESERVE3"]); } catch { newsheet.Reserve3 = ""; }
                    try { newsheet.Date = Convert.ToDateTime(TableReader["DATE"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["DATE"], CultureInfo.CreateSpecificCulture("en-US")); } catch { newsheet.Date = DateTime.Parse("1996/04/26 01:23:47"); }
                    try { newsheet.Document = Convert.ToString(TableReader["DOKUMENT"] == DBNull.Value ? "" : TableReader["DOKUMENT"]); } catch { newsheet.Document = "---"; }
                    //string d = newsheet.Date.Date.ToString('d',DateTimeFormatInfo.InvariantInfo).Replace("/",".");
                    Sheets.Add(newsheet);
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        private void ReadMaterials()
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open(); 
            //    while (!(sqlconnection.State == System.Data.ConnectionState.Open)) { }
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM MATERIALS", sqlconnection);
            int count = 0;
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())//lock(block)
                {
                    materials[count] = Convert.ToString(TableReader["NAME"] == DBNull.Value ? "" : TableReader["NAME"]);
                    count++;
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        private void ReadChapters()
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open(); 
            //    while (!(sqlconnection.State == System.Data.ConnectionState.Open)) { }
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM Chapters", sqlconnection);
            //int count = 0;
            Chapter curChap = new Chapter();
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())//lock(block)
                {
                    curChap.Name = Convert.ToString(TableReader["CHAPTER"] == DBNull.Value ? "" : TableReader["CHAPTER"]).Trim().ToLower();
                    curChap.accessCode = Convert.ToInt32(TableReader["BINCODE"] == DBNull.Value ? 0 : TableReader["BINCODE"]);
                    chapters.Add(curChap);
                    Dchapters.Add(curChap.Name, curChap.accessCode);
                }
                //chapters.Sort((a,b) => a.accessCode>b.accessCode?1:-1);
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        private void _InsertSheet(_sheet OneSheet)
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open();
            SqlCommand command = new SqlCommand("INSERT INTO [SHEETS] (MATHERIAL,TICKNESS,WIDTH,HEIGTH,CNCFILE,PARTS,OWNER,STATUS,RESERVE1,RESERVE2,RESERVE3,QUANTITY,DOKUMENT,DATE) VALUES (@MATHERIAL,@TICKNESS,@WIDTH,@HEIGTH,@CNCFILE,@PARTS,@OWNER,@STATUS,@RESERVE1,@RESERVE2,@RESERVE3,@QUANTITY,@DOKUMENT,@DATE)", sqlconnection);
            //command.Parameters.AddWithValue("NAME", null);
            command.Parameters.AddWithValue("MATHERIAL", OneSheet.Matherial);
            command.Parameters.AddWithValue("TICKNESS", OneSheet.Tickness);
            command.Parameters.AddWithValue("WIDTH", OneSheet.Width);
            command.Parameters.AddWithValue("HEIGTH", OneSheet.Heigth);
            command.Parameters.AddWithValue("CNCFILE", OneSheet.CNCFILE);
            command.Parameters.AddWithValue("PARTS", OneSheet.Parts);
            command.Parameters.AddWithValue("OWNER", OneSheet.Owner);
            command.Parameters.AddWithValue("STATUS", OneSheet.Status);
            command.Parameters.AddWithValue("RESERVE1", OneSheet.Reserve1);
            command.Parameters.AddWithValue("RESERVE2", OneSheet.Reserve2);
            command.Parameters.AddWithValue("RESERVE3", OneSheet.Reserve3);
            command.Parameters.AddWithValue("QUANTITY", 1);
            command.Parameters.AddWithValue("DOKUMENT", OneSheet.Document);
            command.Parameters.AddWithValue("DATE", OneSheet.Date.ToString("d", CultureInfo.CreateSpecificCulture("fr-FR")));
            command.ExecuteNonQuery();
            CloseSqlConnection();
        }
        public void AddSheettoBD(_sheet SHEETRecord)
        {
            _InsertSheet(SHEETRecord);
        }
        public void AddSheettoBD(string Name, int Matherial, float Tickness, int Width, int Heigth, string CNCFILE, string Parts, string Owner, string Status, string Reserve1 = "", string Reserve2 = "", string Reserve3 = "")
        {
            _sheet SHEETRecord = new _sheet { };
            SHEETRecord.Id = 0;
            SHEETRecord.Name = Name;
            SHEETRecord.Matherial = Matherial;
            SHEETRecord.Tickness = Tickness;
            SHEETRecord.Width = Width;
            SHEETRecord.Heigth = Heigth;
            SHEETRecord.CNCFILE = CNCFILE;
            SHEETRecord.Parts = Parts;
            SHEETRecord.Owner = Owner;
            SHEETRecord.Status = Status;
            SHEETRecord.Reserve1 = Reserve1;
            SHEETRecord.Reserve2 = Reserve2;
            SHEETRecord.Reserve3 = Reserve3;
            _InsertSheet(SHEETRecord);
        }

        public void ReadParts(string extquest = "Select* from parts where ARHIVE is null order by id")
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open();
            //     while (!(sqlconnection.State == System.Data.ConnectionState.Open)) { }
            //  if (extquest.Length == 0) extquest = "SELECT * FROM PARTS";
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand(extquest, sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                Parts.Clear();
                while (TableReader.Read())
                    if (!Convert.ToBoolean(TableReader["ARHIVE"] == DBNull.Value ? false : TableReader["ARHIVE"]))
                    {
                        _Part NewPart = new _Part { };
                        try { NewPart.Id = Convert.ToInt32(TableReader["ID"] == DBNull.Value ? "" : TableReader["ID"]); } catch { NewPart.Id = -1; }
                        try { NewPart.Name = Convert.ToString(TableReader["NAME"] == DBNull.Value ? "NONAMEPART" : TableReader["NAME"]); } catch { NewPart.Name = "-X-"; }
                        try { NewPart.Quantity = Convert.ToInt32(TableReader["QUANTITY"] == DBNull.Value ? 1 : TableReader["QUANTITY"]); } catch { NewPart.Quantity = 0; }
                        try { NewPart.QuantitySummary = Convert.ToInt32(TableReader["QUANTITYSUMM"] == DBNull.Value ? 1 : TableReader["QUANTITYSUMM"]); } catch { NewPart.QuantitySummary = 0; }
                        try { NewPart.QuantityCutted = Convert.ToInt32(TableReader["QUANTITYCUTTED"] == DBNull.Value ? 0 : TableReader["QUANTITYCUTTED"]); } catch { NewPart.QuantityCutted = 0; }
                        try { NewPart.CNCID = Convert.ToString(TableReader["CNCID"] == DBNull.Value ? "" : TableReader["CNCID"].ToString().Trim()); } catch { NewPart.CNCID = ""; }
                        try { NewPart.tickness = Convert.ToString(TableReader["TICKNESS"] == DBNull.Value ? "0" : TableReader["TICKNESS"]); } catch { NewPart.tickness = ""; }
                        try { NewPart.Size_X = Convert.ToString(TableReader["SIZE_X"] == DBNull.Value ? "0" : TableReader["SIZE_X"]); } catch { NewPart.Size_X = "0"; }
                        try { NewPart.Size_Y = Convert.ToString(TableReader["SIZE_Y"] == DBNull.Value ? "0" : TableReader["SIZE_Y"]); } catch { NewPart.Size_Y = "0"; }
                        try { NewPart.Shipped = Convert.ToInt32(TableReader["Shipped"] == DBNull.Value ? "" : TableReader["Shipped"]); } catch { NewPart.Shipped = 0; }
                        try { NewPart.Reserve1 = Convert.ToString(TableReader["RESERVE1"] == DBNull.Value ? "" : TableReader["RESERVE1"]); } catch { NewPart.Reserve1 = ""; }
                        try { NewPart.Reserve2 = Convert.ToString(TableReader["RESERVE2"] == DBNull.Value ? "" : TableReader["RESERVE2"]); } catch { NewPart.Reserve2 = ""; }
                        try { NewPart.Reserve3 = Convert.ToString(TableReader["RESERVE3"] == DBNull.Value ? "" : TableReader["RESERVE3"]); } catch { NewPart.Reserve3 = ""; }
                        Parts.Add(NewPart);
                    }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        private void _InsertParts(_Part OnePart)
        {
            SqlCommand command1 = new SqlCommand("INSERT INTO [PARTS] (NAME,QUANTITY,QUANTITYSUMM,QUANTITYCUTTED,TICKNESS,SIZE_X,SIZE_Y,CNCID,RESERVE1,RESERVE2,RESERVE3) VALUES (@NAME,@QUANTITY,@QUANTITYSUMM,@QUANTITYCUTTED,@TICKNESS,@SIZE_X,@SIZE_Y,@CNCID,@RESERVE1,@RESERVE2,@RESERVE3)", sqlconnection);
            command1.Parameters.AddWithValue("NAME", OnePart.Name);
            command1.Parameters.AddWithValue("QUANTITY", OnePart.Quantity);
            command1.Parameters.AddWithValue("QUANTITYSUMM", OnePart.QuantitySummary);
            command1.Parameters.AddWithValue("QUANTITYCUTTED", OnePart.QuantityCutted);
            command1.Parameters.AddWithValue("TICKNESS", OnePart.tickness);
            command1.Parameters.AddWithValue("SIZE_X", OnePart.Size_X);
            command1.Parameters.AddWithValue("SIZE_Y", OnePart.Size_Y);
            command1.Parameters.AddWithValue("CNCID", OnePart.CNCID);
            command1.Parameters.AddWithValue("RESERVE1", OnePart.Reserve1);
            command1.Parameters.AddWithValue("RESERVE2", OnePart.Reserve2);
            command1.Parameters.AddWithValue("RESERVE3", OnePart.Reserve3);
            try
            {
                command1.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                showError(e);
            }
        }
        public void AddPartToBD(_Part OnePart)
        {
            if (OnePart.CNCID == "")
            {
                // MessageBox.Show("Попытка добавить деталь на несуществующую раскладку");
            }
            _InsertParts(OnePart);
        }
        public void AddPartToBD(string Name, int Quantity = 1, int QuantityCutted = 0, int QuantitySummary = 1, string tickness = "0", string SizeX = "0", string SizeY = "0", string cncId = "", string Reserve1 = "", string Reserve2 = "", string Reserve3 = "")
        {
            _Part OnePart = new _Part { };
            OnePart.Id = 0;
            OnePart.Name = Name;
            OnePart.Quantity = Quantity;
            OnePart.QuantityCutted = QuantityCutted;
            OnePart.QuantitySummary = QuantitySummary;
            OnePart.tickness = tickness;
            OnePart.CNCID = cncId;
            OnePart.Size_X = SizeX;
            OnePart.Size_Y = SizeY;
            OnePart.Reserve1 = Reserve1;
            OnePart.Reserve2 = Reserve2;
            OnePart.Reserve3 = Reserve3;
            _InsertParts(OnePart);
        }
        // ИСПРАВЛЕНО: логирование + убрана повторная попытка выполнения при ошибке
        public int FreeRequestToBD(string request)
        {
            int qty = 0;
            Logger.Sql(request);
            if (sqlconnection.State != System.Data.ConnectionState.Open) ConnectDB();
            SqlCommand command1 = new SqlCommand(request, sqlconnection);
            try
            {
                qty = command1.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Logger.Error("FreeRequestToBD failed: " + request, e);
            }
            CloseSqlConnection();
            return qty;
        }
        public List<string> changestatus(int ID, int QuantDone = -1)
        {
            if (sqlconnection.State != System.Data.ConnectionState.Open) ConnectDB();
            SqlCommand command1;
            if (QuantDone == -1) command1 = new SqlCommand("UPDATE CNCFILES SET QUANTITYDONE=QUANTITY WHERE ID=" + ID.ToString(), sqlconnection);
            else command1 = new SqlCommand("UPDATE CNCFILES SET QUANTITYDONE=" + QuantDone.ToString() + " WHERE ID=" + ID.ToString(), sqlconnection);
            try
            {
                command1.ExecuteNonQuery();
                int s = CNCs.FindIndex(x => x.Id == ID);
                CNCs[s] = new _CNC(Filename: CNCs[s].FileName, OriginalFile: CNCs[s].OriginalName, RuntimeOneSheet: CNCs[s].RunTimeOneSheet, quantity: CNCs[s].Quantity, Quantytidone: QuantDone, addedTime: CNCs[s].AddedTime, Filecreatedtime: CNCs[s].FileCreatedTime, Realtickness: CNCs[s].realrickness, Tickness: CNCs[s].tickness, sheets: CNCs[s].Sheets, reserve1: CNCs[s].Reserve1, reserve2: CNCs[s].Reserve2, reserve3: CNCs[s].Reserve3, _id: CNCs[s].Id);
            }
            catch (Exception e)
            {
                showError(e);
            }
            if (QuantDone == -1) command1 = new SqlCommand("UPDATE PARTS SET QUANTITYCUTTED=QUANTITYSUMM WHERE CNCID=" + ID.ToString(), sqlconnection);
            else command1 = new SqlCommand("UPDATE PARTS SET QUANTITYCUTTED=QUANTITY*" + QuantDone.ToString() + " WHERE CNCID=" + ID.ToString(), sqlconnection);
            try
            {
                command1.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                showError(e);
            }
            CloseSqlConnection();
            return checkOrdersDone(ID);
        }
        public bool checkOrdersDoneBD(string order)
        {
            bool result = false;
            return result;
        }
        public List<string> checkOrdersDone(int cncID)
        {
            bool result = false;
            //  Проверка закрытия заказов
            //Получить список заказов в раскладке.
            List<string> orders_h =new List<string> { };
            List<string> ordersDone = new List<string>{ };
            ReadParts("select * from parts where cncid=" + cncID.ToString());
            // Parts.ForEach((x) => x.Name = x.Name.Substring(0, x.Name.IndexOf(" т")));
            for (var n = 0; n < Parts.Count; n++)
            {
                int Io = Parts[n].Name.IndexOf(" т");
                if (Io>=0)
                    orders_h.Add(Parts[n].Name.Substring(0,Io));
                else
                    orders_h.Add(Parts[n].Name);
            }
            //отбросить дубликаты
            for (int i = 0; i < orders_h.Count; i++)
                for (int j = i + 1; j < orders_h.Count; j++)
                    if (orders_h[i] == orders_h[j])  orders_h.RemoveAt(j--); 
            //Получить список заказов в раскладке.
            // пройтись по всем заказам и проверить данные калькуляций с количеством готовых деталей
            foreach (var s in orders_h)
            {
                result = true;
                //*********************************************************************************
                //              ПРОВЕРКА ГОТОВНОСТИ ЗАКАЗА ПО БД
                //    ЕСЛИ В БД СТОИТ ГОТОВО ТО ПЕРЕЙТИ К СЛЕДУЮЩЕМУ ЗАКАЗУ
                //*********************************************************************************
                if (!checkOrdersDoneBD(s))
                {
                    string xlsName;
                    FinCalc list = new FinCalc();
                    xlsName = FinCalc.getfilefromOrdername(s);
                    list.readfromxls(xlsName);
                    if (list.Parts.Count > 0) // Если удалось получить файл и список позиций в заявке то продолжить                        
                    {
                        ReadParts("select * from parts where name like '%" + s+"%' and Arhive is null");
                        if (list.Parts.Count <= Parts.Count) // Если вырезано позиций меньше чем в расчете то не продолжать.
                        {
                            // сумма всех деталей в заказе Xls
                            int sum1 = 0; foreach (var v in list.Parts) sum1 += v.QuantitySummary;
                            // сумма всех вырезанных деталей
                            int sum2 = 0; foreach (var v in Parts) sum2 += v.QuantityCutted;
                            if (sum2 >= sum1) // Если суммарно вырезано больше чем сумма всех заказанных деталей то перейти к построчному сравнению
                            {
                                bool eq = true;
                                foreach (var v in list.Parts) 
                                {
                                    if (v.Size_Y > v.Size_X) { int _1T = v.Size_X; v.Size_X = v.Size_Y;v.Size_Y = _1T; }
                                    //если найденная в расчете позиция количественно больше найденной позиции в деталях то заказ не выполнен до конца.
                                    string query = "select '" + s + "' as name,sum([QUANTITY]) as QUANTITY,sum([QUANTITYSUMM])as QUANTITYSUMM,sum([QUANTITYCUTTED]) as QUANTITYCUTTED,[SIZE_X],[SIZE_Y],sum(isnull(Defect,0)) as Defect,ARHIVE from parts where  name like '%" + s +
                                        "%' and ARHIVE is null and (CAST(SIZE_X as float) BETWEEN " + v.Size_X.ToString() + "-2 AND " + v.Size_X.ToString() +
                                        "+2) AND (CAST(SIZE_Y as float) BETWEEN " + v.Size_Y.ToString() + "-2 AND " + v.Size_Y.ToString() + "+2) and (CAST(TICKNESS as float) between " + v.tickness.ToString() +
                                        "-0.2 and " + v.tickness.ToString() + "+0.2) group by SIZE_X,SIZE_Y,ARHIVE";
                                    ReadParts(query);
                                    if (Parts.Count==0) 
                                        eq = false; 
                                    else
                                    if (v.QuantitySummary > Parts[Parts.Count-1].QuantityCutted) 
                                        eq = false;
                                }
                                if (eq) ordersDone.Add(s);
                                result = eq;
                            }
                        }
                    }
                    // получить список деталей к этому заказу
                }
                //ViewBag.FinParts = list.Parts;
            }
            //===================================================
            return ordersDone;
        }
        public void SetQuantity(int ID, int quantity)
        {
            if (sqlconnection.State != System.Data.ConnectionState.Open) ConnectDB();
            SqlCommand command1 = new SqlCommand("UPDATE CNCFILES SET QUANTITY=" + quantity.ToString() + " WHERE ID=" + ID.ToString(), sqlconnection);
            try
            {
                command1.ExecuteNonQuery();
                int s = CNCs.FindIndex(x => x.Id == ID);
                CNCs[s] = new _CNC(Filename: CNCs[s].FileName, OriginalFile: CNCs[s].OriginalName, RuntimeOneSheet: CNCs[s].RunTimeOneSheet, quantity: quantity, Quantytidone: CNCs[s].QuantityDone, addedTime: CNCs[s].AddedTime, Filecreatedtime: CNCs[s].FileCreatedTime, Realtickness: CNCs[s].realrickness, Tickness: CNCs[s].tickness, sheets: CNCs[s].Sheets, reserve1: CNCs[s].Reserve1, reserve2: CNCs[s].Reserve2, reserve3: CNCs[s].Reserve3, _id: CNCs[s].Id);
            }
            catch (Exception e)
            {
                showError(e);
            }
            command1 = new SqlCommand("UPDATE PARTS SET QUANTITYSUMM=" + quantity.ToString() + "*QUANTITY WHERE CNCID=" + ID.ToString(), sqlconnection);
            try
            {
                command1.ExecuteNonQuery();
                ReadParts();
            }
            catch (Exception e)
            {
                showError(e);
            }
            ReadParts();
            CloseSqlConnection();
        }
        // ИСПРАВЛЕНО: TableReader использовался до ExecuteReader
        public void DeleteCNC(int ID)
        {
            if (sqlconnection.State != System.Data.ConnectionState.Open) ConnectDB();
            string fname = "";
            SqlDataReader TableReader = null;
            SqlCommand command1 = new SqlCommand("Select ORIGINALFILENAME FROM CNCFILES WHERE ID=" + ID.ToString(), sqlconnection);
            try { TableReader = command1.ExecuteReader(); if (TableReader.Read()) fname = Convert.ToString(TableReader["ORIGINALFILENAME"] == DBNull.Value ? "" : TableReader["ORIGINALFILENAME"]); } catch { fname = ""; }
            if (TableReader != null) TableReader.Close();
            if (fname.Length > 5) File.Delete(fname);

            command1 = new SqlCommand("DELETE FROM CNCFILES WHERE ID=" + ID.ToString(), sqlconnection);
            try
            {
                command1.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                showError(e);
            }
            command1 = new SqlCommand("DELETE FROM PARTS WHERE CNCID=" + ID.ToString(), sqlconnection);
            try
            {
                command1.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                showError(e);
            }
            CloseSqlConnection();
        }
        public List<float> ticnesslist()
        {
            List<float> t = new List<float> { };
            foreach (_CNC s in CNCs) if (!t.Contains((float)Convert.ToDouble(s.tickness))) t.Add((float)Convert.ToDouble(s.tickness));
            t.Sort((x, y) => x.CompareTo(y));
            return t;
        }
        public void readorders(int timeindays = 180)
        {
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open(); 
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM ORDERS WHERE готово<всего or lastdate>GETDATE()-" + timeindays.ToString() + " order by (всего-готово) desc,lastdate desc", sqlconnection);
            NORDER.Clear();
            _Order TempOrder;
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())
                {
                    //_sheet newsheet;
                    TempOrder.Name = Convert.ToString(TableReader["заказ"] == DBNull.Value ? 0 : TableReader["заказ"]);
                    TempOrder.all = Convert.ToInt32(TableReader["всего"] == DBNull.Value ? "" : TableReader["всего"]);
                    TempOrder.cutted = Convert.ToInt32(TableReader["готово"] == DBNull.Value ? "" : TableReader["готово"]);
                    TempOrder.Date = Convert.ToDateTime(TableReader["lastdate"] == DBNull.Value ? DateTime.Parse("1/1/2000") : TableReader["lastdate"]);
                    NORDER.Add(TempOrder);
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        public DateTime GetLastUpdateTime(string TableName)
        {
            DateTime lastupd = DateTime.Parse("01-01-2019");
            sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            sqlconnection.Open();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT top(1)  OBJECT_NAME(OBJECT_ID) AS TableName, MAX(last_user_update) AS LastUpdateDate FROM sys.dm_db_index_usage_stats Where object_id = OBJECT_ID('" + TableName + "') GROUP BY OBJECT_NAME(OBJECT_ID) ORDER BY MAX(last_user_update) DESC", sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                if (TableReader.Read())
                {

                    lastupd = Convert.ToDateTime(TableReader["LastUpdateDate"] == DBNull.Value ? "" : TableReader["LastUpdateDate"]);
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();

            return lastupd;
        }
        public void readShipments()
        {
            /*sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            await sqlconnection.OpenAsync();
            */
            Shipments.Clear();
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT ROW_NUMBER() over(order by ordername)  as Id,count(partid) as partid,sum([SHIPED]) as shiped,cast([SHIPTIME] as date) as shiptime ,[ORDERNAME],0 as SIZE_X,0 as SIZE_Y,0 as TICKNESS FROM [PLASMA].[dbo].[SHIPMENT],[PLASMA].[dbo].[PARTS] WHERE [PLASMA].[dbo].[SHIPMENT].[PARTID]=[PLASMA].[dbo].[PARTS].[ID] group by ordername,cast([SHIPTIME] as date) order by shiptime desc", sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())
                {
                    _Shipment newShipment;
                    newShipment.id = Convert.ToInt32(TableReader["ID"] == DBNull.Value ? 0 : TableReader["ID"]);
                    newShipment.partId = Convert.ToInt32(TableReader["PARTID"] == DBNull.Value ? 0 : TableReader["PARTID"]);
                    newShipment.Shiped = Convert.ToInt32(TableReader["SHIPED"] == DBNull.Value ? 0 : TableReader["SHIPED"]);
                    newShipment.orderName = Convert.ToString(TableReader["ORDERNAME"] == DBNull.Value ? "ЗАКАЗ-0" : TableReader["ORDERNAME"]);
                    newShipment.Shiptime = Convert.ToDateTime(TableReader["SHIPTIME"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["SHIPTIME"]);
                    try { newShipment.width = (int)Convert.ToDouble(TableReader["SIZE_X"] == DBNull.Value ? 0 : TableReader["SIZE_X"]); } catch { newShipment.width = 0; }
                    try { newShipment.heigth = (int)Convert.ToDouble(TableReader["SIZE_Y"] == DBNull.Value ? 0 : TableReader["SIZE_Y"]); } catch { newShipment.heigth = 0; }
                    try { newShipment.tikcness = (float)Convert.ToDouble(TableReader["TICKNESS"] == DBNull.Value ? 0 : TableReader["TICKNESS"]); } catch { newShipment.tikcness = 0; }
                    Shipments.Add(newShipment);
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        public void Ship(string ordername)
        {
            FreeRequestToBD("insert into SHIPMENT (PARTID,SHIPED,SHIPTIME,ORDERNAME) select parts.ID as PARTID,(parts.QUANTITYSUMM-COALESCE(shipment.SHIPED,0)) as SHIPED,CAST('" + DateTime.Now.ToString("g", CultureInfo.CreateSpecificCulture("fr-FR")).ToString().Replace("/", ".") + "'AS datetime2) as SHIPTIME, '" + ordername + "' AS ORDERNAME from PARTS left join SHIPMENT on PARTS.id=SHIPMENT.PARTID where (parts.QUANTITYSUMM-COALESCE(shipment.SHIPED,0))>0 and PARTS.ARHIVE is null and parts.NAME like'" + ordername + "%'");
        }
        public List<_Shipment> getListPartialShip(string OrderName)
        {
            List<_Shipment> shippedlist = new List<_Shipment> { };
            Shipments.Clear();
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT ROW_NUMBER() over(order by ordername) as Id,SUM(QUANTITYSUMM) as partid,TICKNESS,SIZE_X,SIZE_Y,sum([SHIPED]) as shiped,[SHIPTIME],[ORDERNAME] FROM [PLASMA].[dbo].[SHIPMENT],[PLASMA].[dbo].[PARTS] WHERE [PLASMA].[dbo].[SHIPMENT].[ORDERNAME] like '%" + OrderName + "%' and [PLASMA].[dbo].[SHIPMENT].[PARTID]=[PLASMA].[dbo].[PARTS].[ID] group by ordername,shiptime,TICKNESS,SIZE_X,SIZE_Y", sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())
                {
                    _Shipment newShipment;
                    newShipment.id = Convert.ToInt32(TableReader["ID"] == DBNull.Value ? 0 : TableReader["ID"]);
                    newShipment.partId = Convert.ToInt32(TableReader["PARTID"] == DBNull.Value ? 0 : TableReader["PARTID"]);
                    newShipment.Shiped = Convert.ToInt32(TableReader["SHIPED"] == DBNull.Value ? 0 : TableReader["SHIPED"]);
                    newShipment.orderName = Convert.ToString(TableReader["ORDERNAME"] == DBNull.Value ? "ЗАКАЗ-0" : TableReader["ORDERNAME"]);
                    newShipment.Shiptime = Convert.ToDateTime(TableReader["SHIPTIME"] == DBNull.Value ? DateTime.Parse("1996/04/26 01:23:47") : TableReader["SHIPTIME"]);
                    try { newShipment.width = (int)Convert.ToDouble(TableReader["SIZE_X"] == DBNull.Value ? 0 : TableReader["SIZE_X"]); } catch { newShipment.width = 0; }
                    try { newShipment.heigth = (int)Convert.ToDouble(TableReader["SIZE_Y"] == DBNull.Value ? 0 : TableReader["SIZE_Y"]); } catch { newShipment.heigth = 0; }
                    try { newShipment.tikcness = (float)Convert.ToDouble(TableReader["TICKNESS"] == DBNull.Value ? 0 : TableReader["TICKNESS"]); } catch { newShipment.tikcness = 0; }
                    shippedlist.Add(newShipment);
                }
            }
            catch (Exception e)
            {
                showError(e);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();

            return shippedlist;
        }
        void readallbd()
        {
            readCNC();
            ReadSheets();
            ReadMaterials();
            readorders(30);
            ReadChapters();            
        }
        public PartsClass(int readall = 1)
        {
            if (readall == 1)
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                readallbd();
            }
        }
    }
}


