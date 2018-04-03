using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnection
{
    public class Database
    {
        private static string _sqlSelect = "SELECT object_id, name FROM sys.tables";

        public List<Table> Tables { get; set; }
        public List<Relation> Relations { get; set; }
        private static DatabaseConnection db;
        public static List<Table> Select(DatabaseConnection pDb = null)
        {
            List<Table> tables = new List<Table>();
            if (pDb == null)
            {
                db = new DatabaseConnection();
                db.Connect();
            }
            else
            {
                db = (DatabaseConnection)pDb;
            }
            try
            {
                SqlCommand command = db.CreateCommand(_sqlSelect);
                SqlDataReader reader = db.Select(command);

                tables = Read(reader);
                reader.Close();
            }
            catch (Exception e)
            {
                Trace.TraceInformation("Connection not created");
            }
            if (pDb == null)
            {
                db.Close();
            }

            return tables;
        }
        private static List<Table> Read(SqlDataReader reader)
        {
            List<Table> tables = new List<Table>();

            while (reader.Read())
            {
                int i = -1;
                Table table = new Table();
                table.ObjectId = reader.GetInt32(++i);
                table.Name = reader.GetString(++i);
                tables.Add(table);
            }
            return tables;
        }

    }
}
