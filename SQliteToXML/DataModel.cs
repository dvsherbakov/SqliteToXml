using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Dapper;

namespace SQliteToXML
{
    internal class DataModel
    {
        private string ConnectionString { get; set; }
        private List<LocalDataTable> Tables { get; set; }
        private XDocument XDoc { get; set; }
        private XElement Database { get; set; }

        public DataModel(string fileName)
        {
            ConnectionString = $"Data Source=.\\{fileName};Version=3;";
            XDoc = new XDocument();

            Database = new XElement("DataBase");
            XDoc.Add(Database);
        }

        private SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public void GetTables()
        {
            using (IDbConnection cnn = CreateConnection())
            {
                Tables = cnn.Query<LocalDataTable>("SELECT * FROM sqlite_master WHERE type = 'table' ORDER BY 1",
                    new DynamicParameters()).ToList();
                foreach (var tab in Tables)
                {
                    GetTableFields(tab.Name);
                }

            }
        }

        private void GetTableFields(string tableName)
        {
            Example(tableName);
            
            using (var cnn = new SQLiteConnection(ConnectionString))
            {
                cnn.Open();
                var cmd = new SQLiteCommand($"select * from {tableName}", cnn);
                var dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    var tableElement = new XElement(tableName);

                    var columns = new List<string>();
                    for (var i = 0; i < dataReader.FieldCount; i++)
                    {
                        columns.Add(dataReader.GetName(i));
                    }

                    while (dataReader.Read())
                    {
                        var row = new XElement("Row");

                        foreach (var col in columns)
                        {
                            row.Add(new XAttribute(col, dataReader[col].ToString()));
                        }

                        tableElement.Add(row);
                    }

                    Database.Add(tableElement);
                    Console.WriteLine($"Added table {tableName}");
                }

                cnn.Close();
            }
        }

        public void SaveXml(string fileName)
        {
            XDoc.Save(fileName);
        }

       
        private static async void Example(string tableName)
        {
            var t = await Task.Run(Allocate);
            Console.WriteLine($"{tableName}: {t}");
        }

        private static int Allocate()
        {
            // Compute total count of digits in strings.
            var size = 0;
            for (var z = 0; z < 1000; z++)
            {
                for (var i = 0; i < 1000; i++)
                {
                    var value = i.ToString();
                    size += value.Length;
                }
            }
            return size;
        }

    }

    internal class LocalDataTable
    {
        public string Name { get; set; }
    }
}
