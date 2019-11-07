using System;

namespace SQliteToXML
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var dm = new DataModel("template.db");
            dm.GetTables();
            dm.SaveXml(@"C:\Temp\template.xml");
            Console.ReadKey();
        }
    }
}
