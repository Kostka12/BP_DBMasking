using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DatabaseConnection
{
    public class DatabaseConnection
    {
        public static SqlConnection Connection { get; set; }
        private SqlTransaction SqlTransaction { get; set; }
        public string Language { get; set; }
        public StringBuilder Message { get; set; }
        private string path = @"C:\DbMasking\dbconnect\connection_string.xml";
        private string ConnectionString { get; set; }

        public DatabaseConnection()
        {
            Load();
            Connection = new SqlConnection();
            Language = "en";
            Message = new StringBuilder();
            Connection.InfoMessage += myConnection_InfoMessage;
            //ConnectionString = "server=DESKTOP-I8MFAAT;database=UDBS;user=udbs_user1;password=okulele;";

           
        }

        public void myConnection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            Message.Clear();
            Message.AppendLine(e.Message);
        }

        public void Load()
        {
            string text = "";
            try
            {
                using (StringReader sr = new StringReader(path))
                {
                    text = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Soubor nebyl nalezen");
            }
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(text);
                XmlNode dbNode = xDoc.SelectSingleNode("//DbName");
                XmlNode serverNode = xDoc.SelectSingleNode("//ServerName");
                XmlNode userNode = xDoc.SelectSingleNode("//UserName");
                XmlNode passNode = xDoc.SelectSingleNode("//UserPassword");
                if (serverNode != null && dbNode != null && userNode != null && passNode != null)
                    ConnectionString = "server=" + serverNode.InnerText + ";database=" + dbNode.InnerText + ";user=" +
                                       userNode.InnerText + ";password=" + passNode.InnerText + ";";
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Soubor nebyl nalezen");
            }
            
          //  Console.WriteLine(dbNode.InnerText + "dsda " + serverNode.InnerText+ " "+ userNode.InnerText + " "+ passNode.InnerText);
        }

        public bool Connect(string conString)
        {
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                Connection.ConnectionString = conString;
                Connection.Open();
            }
            return true;

        }

        public bool Connect()
        {
            bool ret = true;
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                if(ConnectionString!=null)
                    ret = Connect(ConnectionString);
                else
                    Trace.TraceInformation("Cannot create connection");

            }
            return ret;
        }


        public void Close()
        {
            Connection.Close();
        }

        public void BeginTransaction()
        {
            SqlTransaction = Connection.BeginTransaction(IsolationLevel.Serializable);
        }


        public void EndTransaction()
        {
            SqlTransaction.Commit();
            Close();
        }


        public void Rollback()
        {
            SqlTransaction.Rollback();
        }


        public int ExecuteNonQuery(SqlCommand command)
        {
            int rowNumber = 0;
            try
            {
                rowNumber = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw e;
            }
            return rowNumber;
        }

        public SqlCommand CreateCommand(string strCommand)
        {
            SqlCommand command = new SqlCommand(strCommand, Connection);

            if (SqlTransaction != null)
            {
                command.Transaction = SqlTransaction;
            }
            return command;
        }


        public SqlDataReader Select(SqlCommand command)
        {
            SqlDataReader sqlReader = command.ExecuteReader();
            return sqlReader;
        }

        public DataSet SelectData(SqlCommand command)
        {
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(command);
            DataSet dataSet = new DataSet();
            sqlAdapter.Fill(dataSet);
            return dataSet;
        }
        public int GetIdentity(String tableName)
        {
            SqlCommand command = CreateCommand("SELECT IDENT_CURRENT(@tableName)");
            command.Parameters.AddWithValue("@tableName", tableName);
            return Convert.ToInt32(command.ExecuteScalar());
        }
    }
}
