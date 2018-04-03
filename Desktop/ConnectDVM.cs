using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Desktop
{
    public class ConnectDvm : INotifyPropertyChanged
    {
        private string _fServer;
        public string Server
        {
            get
            {
                return this._fServer;
            }
            set
            {
                this._fServer = value;
                this.OnPropertyChanged("Server");
            }
        }

        private string _fDatabase;
        public string Database {
            get
            {
                return this._fDatabase;
            }
            set
            {
                this._fDatabase = value;
                this.OnPropertyChanged("Database");
            }
        }

        private string _fUser;
        public string User {
            get
            {
                return this._fUser;
            }
            set
            {
                this._fUser = value;
                this.OnPropertyChanged("User");
            }
        }

        private string _fPassword;
        public string Password {
            get
            {
                return this._fPassword;
            }
            set
            {
                this._fPassword = value;
                this.OnPropertyChanged("Password");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool TestConnect(string conString)
        {
            using (SqlConnection connection = new SqlConnection())
            {
                try
                {
                    connection.ConnectionString = conString;
                    connection.Open();
                    connection.Close();
                    return true;

                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

    
        public bool SaveConnection()
        {
            XmlDocument xDoc = new XmlDocument();
           
            string connectionString = "server=" + Server + ";database=" + Database + ";user=" +
                                       User + ";password=" + Password + ";";
            string result = "";
            if(TestConnect(connectionString))
            {
                result = "Connected";
                XmlNode declaration = xDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                xDoc.AppendChild(declaration);
                XmlNode root = xDoc.CreateElement("connString");
                xDoc.AppendChild(root);
                XmlNode sBranch = xDoc.CreateElement("ServerName");
                root.AppendChild(sBranch);
                sBranch.InnerText = Server;
                XmlNode dbBranch = xDoc.CreateElement("DbName");
                root.AppendChild(dbBranch);
                dbBranch.InnerText = Database;
                XmlNode uBranch = xDoc.CreateElement("UserName");
                root.AppendChild(uBranch);
                uBranch.InnerText = User;
                XmlNode pBranch = xDoc.CreateElement("UserPassword");
                root.AppendChild(pBranch);
                pBranch.InnerText = Password;
                xDoc.Save(@"C:\DbMasking\dbconnect\connection_string.xml");
                return true;
            }
            else
            {
                result = "Connection cannot be established";
                return false;
            }         
        }
    }
}
