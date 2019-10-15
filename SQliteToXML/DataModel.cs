using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Dapper;

namespace SQliteToXML
{
    class DataModel
    {
        string ConnectionString { get; set; }
        List<LocalDataTable> Tables { get; set; }
        public DataModel(string fileName)
        {
            ConnectionString = $"Data Source=.\\{fileName};Version=3;";
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
                
                while (dataReader.Read())
                {
                    var vls = dataReader.GetValues();
                    foreach (string st in vls)
                    {
                        var tp = dataReader[st].GetType();
                        Console.WriteLine(dataReader[st]);
                        switch (tp.Name)
                        {
                            case "String" :
                                //var xc = dataReader[st].ToString();
                                //var xt = dataReader[xc];
                                //Console.WriteLine(dataReader[st]);
                                break;
                        }
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
