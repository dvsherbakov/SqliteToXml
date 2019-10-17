using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Dapper;

namespace SQliteToXML
{
    class DataModel
    {
        string ConnectionString { get; set; }
        List<LocalDataTable> Tables { get; set; }
        XDocument XDoc { get; set; }
        XElement Database { get; set; }

        public DataModel(string fileName)
        {
            ConnectionString = $"Data Source=.\\{fileName};Version=3;";
            XDoc = new XDocument();
            
            Database = new XElement("DataBase");
            XDoc.Add(Database);
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

                if (dataReader.HasRows)
                {
                    var TableElement = new XElement(tableName);

                    var columns = new List<string>();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        columns.Add(dataReader.GetName(i));
                    }

                    while (dataReader.Read())
                    {
                        var Row = new XElement("Row");
                        
                        foreach( string col in columns)
                        {
                            Row.Add(new XAttribute(col, dataReader[col].ToString()));
                        }
                        TableElement.Add(Row);
                    }
                    Database.Add(TableElement);

                }
                cnn.Close();
            }
        }

        public void SaveXml(string fileName)
        {
            XDoc.Save(fileName);
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
