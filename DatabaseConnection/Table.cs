using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseConnection;

namespace DatabaseConnection
{
    public class Table
    {
        public string Name { get; set; }

        private static string _sqlSelect = "SELECT c.name,ty.name,c.max_length FROM sys.columns c JOIN sys.tables t ON c.object_id = t.object_id " +
                                          "JOIN sys.types ty ON c.system_type_id = ty.system_type_id WHERE t.name = @name";

        private static string _sqlSelectData = "SELECT * FROM ";

        private static string _sqlDeleteData = "DELETE FROM ";

        
        public int ObjectId { get; set; }
        public DataSet Data { get; set; }

        public List<Column> Columns { get; set; }

        public int TableId { get; set; }

        public static List<Column> Select(string pName, DatabaseConnection pDb = null)
        {
            DatabaseConnection db;
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection) pDb;
            }

            SqlCommand command = db.CreateCommand(_sqlSelect);

            command.Parameters.AddWithValue("@name", pName);
            SqlDataReader reader = db.Select(command);

            List<Column> columns = Read(reader);
            reader.Close();

            if (pDb == null)
            {
                db.Close();
            }

            return columns;
        }
        public static DataSet SelectData(string pTable, DatabaseConnection pDb = null)
        {
            DatabaseConnection db;
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection)pDb;
            }
            _sqlSelectData += pTable;
            SqlCommand command = db.CreateCommand(_sqlSelectData);
            DataSet data= db.SelectData(command);
            _sqlSelectData = "SELECT * FROM ";
                
            if (pDb == null)
            {
                db.Close();
            }

            return data;
        }

        public static int InsertData(DataSet dataSet,string pTable, DatabaseConnection pDb = null)
        {
            DatabaseConnection db;
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection) pDb;
            }

            SqlCommand command = db.CreateCommand(BuildInsertCommand(dataSet,pTable));
            int ret = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                db.Close();
            }

            return ret;
        }

        public static int DeleteData(string pTable, DatabaseConnection pDb = null)
        {
            DatabaseConnection db;
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection)pDb;
            }
            SqlCommand command = db.CreateCommand(_sqlDeleteData + pTable);

            int ret = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                db.Close();
            }

            return ret;
        }

        public static int DisableConstrains(DatabaseConnection pDb = null)
        {
            DatabaseConnection db;
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection)pDb;
            }
         var sqlConstrainsDisable = "EXEC sp_MSforeachtable \"ALTER TABLE ? NOCHECK CONSTRAINT all\"";

        
        SqlCommand command = db.CreateCommand(sqlConstrainsDisable);

            int ret = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                db.Close();
            }

            return ret;
        }
        public static int EnableConstrains(DatabaseConnection pDb = null)
        {
            DatabaseConnection db;
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection)pDb;
            }
            var sqlConstrainsEnable = "exec sp_MSforeachtable \"ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all\"";

            SqlCommand command = db.CreateCommand(sqlConstrainsEnable);

            int ret = db.ExecuteNonQuery(command);

            if (pDb == null)
            {
                db.Close();
            }

            return ret;
        }

        public static string BuildInsertCommand(DataSet dataSet, string pTable)
        {
            string SQL_INSERT = "";
            StringBuilder columnsBuilder = new StringBuilder();
            StringBuilder valuesBuilder = new StringBuilder();
            for (int i = 0; i < dataSet.Tables[0].Columns.Count; i++)
            {
                var columnName = dataSet.Tables[0].Columns[i].ColumnName;     
                columnsBuilder.Append(columnName);
                if (i != dataSet.Tables[0].Columns.Count - 1)
                {
                    columnsBuilder.Append(", ");
                }
            }
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                valuesBuilder.Append("(");
                for (int j = 0; j < dataSet.Tables[0].Columns.Count; j++)
                {
                    string type = dataSet.Tables[0].Rows[i][j].GetType().Name;
                    if (dataSet.Tables[0].Rows[i][j].Equals(DBNull.Value))
                    {
                        valuesBuilder.Append("NULL");
                    }
                    else if (type.Equals("String"))
                    {
                        valuesBuilder.Append("'" + dataSet.Tables[0].Rows[i].ItemArray[j] + "'");
                    }
                    else if (type.Equals("Boolean"))
                    {
                        var tmpBool = dataSet.Tables[0].Rows[i].ItemArray[j].ToString().Equals("True") ? "1" : "0";

                        valuesBuilder.Append(tmpBool);
                    }
                    else if (type.Equals("DateTime"))
                    {
                        var tmpDate = DateTime.Parse(dataSet.Tables[0].Rows[i].ItemArray[j].ToString());
                        valuesBuilder.Append("'" + tmpDate.ToString("yyyy-MM-dd HH:mm:ss") + "'");     
                    }
                    else
                    {
                        valuesBuilder.Append(dataSet.Tables[0].Rows[i].ItemArray[j]);
                    }
                    if (j != dataSet.Tables[0].Columns.Count - 1)
                    {
                        valuesBuilder.Append(", ");
                    }
                }
                valuesBuilder.Append(i == dataSet.Tables[0].Rows.Count - 1 ? ")" : "),");
            }
            SQL_INSERT = "INSERT INTO " + pTable + "(" + columnsBuilder + ") VALUES" + valuesBuilder;
            return SQL_INSERT;
        }
        private static List<Column> Read(SqlDataReader reader)
        {
            List<Column> columns = new List<Column>();
            while (reader.Read())
            {
                int i = -1;
                Column column = new Column();
                column.Name = reader.GetString(++i);
                column.Type = reader.GetString(++i);
                column.Length = reader.GetInt16(++i);
                column.SetPrimaryKey(column.Name);
                column.SetForeignKey(column.Name);
                column.SetUnique(column.Name);
                columns.Add(column);
            }
            return columns;
        }
    }
}
