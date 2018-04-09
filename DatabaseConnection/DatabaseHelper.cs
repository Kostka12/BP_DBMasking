using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnection
{
    public static class DatabaseHelper
    {
        public static Database Database { get; set; }

        public static string GetDBSchema()
        {
            try
            {
                Database = new Database();
                Database.Tables = Database.Select();

                foreach (var nTable in Database.Tables)
                {
                    nTable.Columns = new List<Column>();
                    nTable.Columns = Table.Select(nTable.Name);
                    nTable.Data = Table.SelectData(nTable.Name);
                }
                return "";
            }
            catch(Exception ex)
            {
                return "Database schema not loaded";
            }
           
        }

    }
}
