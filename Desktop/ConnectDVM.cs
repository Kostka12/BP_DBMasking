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

    
        public bool SaveConnection(bool aIntegratedSecurity)
        {
            XmlDocument lXDoc = new XmlDocument();
            try
            {
                if (aIntegratedSecurity)
                {
                    string lConnectionString = "Data Source=" + Database + ";Initial Catalog=" +  Server + ";Integrated Security=True";

                    if (TestConnect(lConnectionString))
                    {
                        System.IO.Directory.CreateDirectory(@"C:\DbMasking\dbconnect");
                        System.IO.File.WriteAllText(@"C:\DbMasking\dbconnect\connection_string.txt", lConnectionString);
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    string lConnectionString = "server=" + Server + ";database=" + Database + ";user=" +
                                           User + ";password=" + Password + ";";

                    if (TestConnect(lConnectionString))
                    {
                        //XmlNode declaration = lXDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                        //lXDoc.AppendChild(declaration);
                        //XmlNode root = lXDoc.CreateElement("connString");
                        //lXDoc.AppendChild(root);
                        //XmlNode sBranch = lXDoc.CreateElement("ServerName");
                        //root.AppendChild(sBranch);
                        //sBranch.InnerText = Server;
                        //XmlNode dbBranch = lXDoc.CreateElement("DbName");
                        //root.AppendChild(dbBranch);
                        //dbBranch.InnerText = Database;
                        //XmlNode uBranch = lXDoc.CreateElement("UserName");
                        //root.AppendChild(uBranch);
                        //uBranch.InnerText = User;
                        //XmlNode pBranch = lXDoc.CreateElement("UserPassword");
                        //root.AppendChild(pBranch);
                        //pBranch.InnerText = Password;
                        System.IO.Directory.CreateDirectory(@"C:\DbMasking\dbconnect");
                        System.IO.File.WriteAllText(@"C:\DbMasking\dbconnect\connection_string.txt", lConnectionString);
                        //lXDoc.Save();
                        return true;
                    }
                    else
                    {

                        return false;
                    }
                }


            }
            catch
            {
                return false;
            }
            
        }
    }
}
