using System;

namespace SQliteToXML
{
    class Program
    {
        static void Main(string[] args)
        {
            var dm = new DataModel("projects.db");
            dm.GetTables();
        }
    }
}
