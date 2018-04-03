using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnection
{
    public class Column
    {
        private static string _sqlSelectPks = "SELECT i.name AS IndexName,OBJECT_NAME(ic.OBJECT_ID) AS TableName," +
                                              "COL_NAME(ic.OBJECT_ID, ic.column_id) AS ColumnName " +
                                              "FROM sys.indexes AS i " +
                                              "INNER JOIN sys.index_columns AS ic ON i.OBJECT_ID = ic.OBJECT_ID " +
                                              "AND i.index_id = ic.index_id WHERE i.is_primary_key = 1 " +
                                              "AND COL_NAME(ic.OBJECT_ID, ic.column_id) = @columnName";

        private static string _sqlSelectFks = "	SELECT c1.name FROM sys.foreign_keys fk" +
                                              " INNER JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id" +
                                              " INNER JOIN sys.columns c1" +
                                              " ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id" +
                                              " WHERE c1.name = @columnName";

        private static string _sqlSelectUq = "SELECT cc.column_Name " +
                                             "FROM information_schema.table_constraints tc " +
                                             "INNER JOIN information_schema.constraint_column_usage cc " +
                                             "ON tc.Constraint_Name = cc.Constraint_Name " +
                                             "WHERE TC.constraint_type = 'Unique' AND cc.column_name = @columnName";

        public string Name { get; set; }
        public string Type { get; set; }
        public short Length { get; set; }
        public bool PrimaryKey { get; set; }
        public bool ForeignKey { get; set; }
        public bool Unique { get; set; }
        public int ColumnId { get; set; }
    
        public bool IsPrimaryKey(string pColumnName, DatabaseConnection pDb = null)
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

            SqlCommand command = db.CreateCommand(_sqlSelectPks);

            command.Parameters.AddWithValue("@columnName", pColumnName);
            SqlDataReader reader = db.Select(command);
            string pk = Read(reader);

            if (pk !=null)
            {
                return true;
            }
            reader.Close();

            if (pDb == null)
            {
                db.Close();
            }
            return false;
        }
        public bool IsForeignKey(string pColumnName, DatabaseConnection pDb = null)
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

            SqlCommand command = db.CreateCommand(_sqlSelectFks);

            command.Parameters.AddWithValue("@columnName", pColumnName);
            SqlDataReader reader = db.Select(command);
            string fk = Read(reader);

            if (fk != null)
            {
                return true;
            }
            reader.Close();

            if (pDb == null)
            {
                db.Close();
            }
            return false;
        }
        public bool IsUnique(string pColumnName, DatabaseConnection pDb = null)
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

            SqlCommand command = db.CreateCommand(_sqlSelectUq);

            command.Parameters.AddWithValue("@columnName", pColumnName);
            SqlDataReader reader = db.Select(command);
            string uq = Read(reader);

            if (uq != null)
            {
                return true;
            }
            reader.Close();

            if (pDb == null)
            {
                db.Close();
            }
            return false;
        }
        private static string Read(SqlDataReader reader)
        {
            string k = null;

            while (reader.Read())
            {
                int i = -1;
                k = reader.GetString(++i);
            }
            return k;
        }

        public void SetPrimaryKey(string pColumnName)
        {
            PrimaryKey = IsPrimaryKey(pColumnName);
        }

        public void SetForeignKey(string pColumnName)
        {
            ForeignKey = IsForeignKey(pColumnName);
        }

        public void SetUnique(string pColumnName)
        {
            Unique = IsUnique(pColumnName);
        }
    }
}
