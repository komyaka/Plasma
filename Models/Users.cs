using Plazma.Controllers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Plazma.Controllers
{
    public class Users
    {
        public struct _user
        {
            public string name;
            public string domain;
            public int Privilegies { get; set; }
            public _user(string _name, string _domain = "",int _privilegies=0)
            {
                if ((_domain == "") && (_name.IndexOf("\\") > 0))
                {
                    domain = _name.Substring(0, _name.IndexOf("\\"));
                    name = _name.Substring(_name.IndexOf("\\")+1);
                }
                else
                {
                    name = _name;
                    domain = _domain;
                }
                Privilegies = _privilegies;
            }
            public int getprivilegies() => this.Privilegies;
            public void setprivilegies(int _privilegies) { this.Privilegies = _privilegies; }
        }
        public List<string> chapters = new List<string> { }; // Список разделов сайта к которым предоставляется доступ
        public List<_user> userlist = new List<_user> { };   // Список всех пользователей
        System.Data.SqlClient.SqlConnection sqlconnection = new System.Data.SqlClient.SqlConnection(Constants.bdconnectionstring);
        public _user SplitUserName(string name) => new _user(name);
        public _user getCurrentUser() 
        { _user User=SplitUserName(System.Web.HttpContext.Current.User.Identity.Name);
          if (userlist.FindIndex(x => x.name == User.name) < 0) NewUser(User);
            User.setprivilegies(userlist[userlist.FindIndex(x => x.name == User.name)].Privilegies);
            return User;
        }
        private void ConnectDB()
        {
            if (sqlconnection.State != System.Data.ConnectionState.Open)
                sqlconnection.Open(); 
        }
        public void CloseSqlConnection()
        {
            if (!(sqlconnection.State == System.Data.ConnectionState.Closed)) { sqlconnection.Close(); }
        }
        public int gefunctionfromname()
        {
            int role = 0;
            //получить должность, роль из имени.

            return role;
        }
        public int setrole(string Name, int role)
        {
            int result = 0;

            // установить для пользователя его должность и роль
            return result;
        }
        public int NewUser(string Name, int role = 0)
        {
            int result = 0;
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM USERS WHERE NAME=" + Name, sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                if (!TableReader.Read()) return -1;
            }
            catch (Exception e)
            {
                //  MessageBox.Show(e.Message, e.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            command.Dispose();
            command = new SqlCommand("INSERT INTO [USERS] NAME,DOMAIN,FUNCT VALUES (" + Name + "," + SplitUserName(Name).domain + "," + role + ")", sqlconnection);
            CloseSqlConnection();
            // занести
            return result;
        }
        public int NewUser(_user usr)
        {
            int result = 0;
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM USERS WHERE NAME=" + usr.name, sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                if (!TableReader.Read()) return -1;
            }
            catch (Exception e)
            {
                //  MessageBox.Show(e.Message, e.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            command.Dispose();
            command = new SqlCommand("INSERT INTO [USERS] (NAME,DOMAIN,FUNCT) VALUES ('" + usr.name + "','" + usr.domain + "'," + "0" + ")", sqlconnection);
            command.ExecuteNonQuery();
            CloseSqlConnection();
            // занести
            return result;
        }
        public int getchapterlist()
        {
            int result = 0;
            return result;
        }
        public List<string> GetUserChapters(string UserName)
        {
            List<string> chapt = new List<string> { };
            //Получить все разделы сайта к к которым у пользователя есть доступ.

            return chapt;
        }
        public List<string> GetUserChapters(int Userid)
        {
            List<string> chapt = new List<string> { };
            //Получить все разделы сайта к к которым у пользователя есть доступ.

            return chapt;
        }
        public void readAllUsers() 
        {
            /*sqlconnection = new SqlConnection(Constants.bdconnectionstring);
            await sqlconnection.OpenAsync();
            */
            ConnectDB();
            SqlDataReader TableReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM USERS", sqlconnection);
            try
            {
                TableReader = command.ExecuteReader();
                while (TableReader.Read())
                    {
                        _user newuser;
                        newuser.name = Convert.ToString(TableReader["NAME"] == DBNull.Value ? "GUEST" : TableReader["NAME"]);
                        newuser.domain = Convert.ToString(TableReader["DOMAIN"] == DBNull.Value ? "NODOMAIN" : TableReader["DOMAIN"]);
                        int Priv = Convert.ToInt32(TableReader["FUNCT"] == DBNull.Value ? 0 : TableReader["FUNCT"]);
                        userlist.Add(new _user(Convert.ToString(TableReader["NAME"] == DBNull.Value ? "GUEST" : TableReader["NAME"]), Convert.ToString(TableReader["DOMAIN"] == DBNull.Value ? "NODOMAIN" : TableReader["DOMAIN"]),Priv));
                    }
            }
            catch (Exception e)
            {
                //  MessageBox.Show(e.Message, e.Source, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!(TableReader == null)) { TableReader.Close(); }
            }
            CloseSqlConnection();
        }
        public Users()
        {
            readAllUsers();
        }

    }
}
