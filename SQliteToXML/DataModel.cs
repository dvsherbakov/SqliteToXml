using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;

namespace SQliteToXML
{
    class DataModel
    {
        string ConnectionString { get; set; }
        List<LocalDataTable> Tables { get; set; }
        DataSet ds { get; set; }

        public DataModel(string fileName)
        {
            ConnectionString = $"Data Source=.\\{fileName};Version=3;";
            ds = new DataSet();
        }

        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public void GetTables()
        {
            using (IDbConnection cnn = CreateConnection())
            {
                Tables = cnn.Query<LocalDataTable>("SELECT * FROM sqlite_master WHERE type = 'table' ORDER BY 1", new DynamicParameters()).ToList();
                foreach (var tab in Tables)
                {
                    GetTableFields(tab.name);
                }
                using (StreamWriter sr = new StreamWriter(Path.Combine("C:\\Temp", "tables.xml")))
                {
                    ds.WriteXml(sr);
                }
            }
        }

        public void GetTableFields(string tableName)
        {
            using (SQLiteConnection cnn = new SQLiteConnection(ConnectionString))
            {
                //var outp = cnn.Query<DataField>($"PRAGMA table_info({tableName})", new DynamicParameters()).ToList();

                cnn.Open();
                SQLiteCommand cmd = new SQLiteCommand($"select * from {tableName}", cnn);
                SQLiteDataReader dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    DataTable dt = new DataTable();
                    dt.Load(dataReader);
                    ds.Tables.Add(dt);
                    using (StreamWriter fs = new StreamWriter(Path.Combine("C:\\Temp", tableName + ".xml"))) // XML File Path
                    {
                        dt.WriteXml(fs);
                    }
                }
                cnn.Close();
            }
        }


    }

    class LocalDataTable
    {
        public string name { get; set; }
    }

    class DataField
    {
        public string name { get; set; }
        public string type { get; set; }
    }
}
