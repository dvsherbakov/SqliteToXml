using System;

namespace SQliteToXML
{
    class Program
    {
        static void Main(string[] args)
        {
            var dm = new DataModel("template.db");
            dm.GetTables();
            dm.SaveXml(@"C:\Temp\template.xml");
            //Console.ReadKey();
        }
    }
}
