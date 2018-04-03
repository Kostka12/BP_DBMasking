
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using QueryParser;
using QueryParserImplementation;
using System;
using System.Collections.Generic;
using System.IO;
using Antlr4.Runtime.Misc;

namespace TestingConsoleApp
{

    class Program
    {
        static void Main(string[] args)
        {
            string lQuery = "SELECT Jmeno, Prijmeni FROM Osoba, Film  WHERE Stat LIKE 'Kanada' OR Rok_natoceni >1995 UNION "+
                "(SELECT Jmeno, Prijmeni FROM Uzivatel WHERE Jmeno = 'ads' AND Prijmeni = 'asd');";
            string lQuery2 = "SElect jmeno, nazev_cz FROM Osoba os JOIN Osoba_Film osf ON os.idO = osf.Osoba_idO JOIN Film f ON f.idF = osf.Film_idF Where f.Rok_natoceni > 1995;";
            string lQuery1 = "select * from table1 t1, t2 where t1.neco = 1";
            string text = ReadFile($@"C:\Users\Lukáš\Desktop\doc\Workload_bp.txt");
            StringReader reader = new StringReader(text);
            AntlrInputStream input = new AntlrInputStream(reader);
            TSqlLexer lexer = new TSqlLexer(new CaseChangingCharStream(input, true));
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            TSqlParser parser = new TSqlParser(tokens);
            TSqlParser.Tsql_fileContext Tsql_fileContext1 = parser.tsql_file();
            //Console.WriteLine("Tsql_fileContext1.ChildCount = " + Tsql_fileContext1.ChildCount.ToString());

            Antlr4.Runtime.Tree.ParseTreeWalker walker = new Antlr4.Runtime.Tree.ParseTreeWalker();
            AntlrTsqlListener listener = new AntlrTsqlListener();
            walker.Walk(listener, Tsql_fileContext1);

            foreach(var nTable in listener.AnalyzedWorkload)
            {
                Console.WriteLine("Tabulka "+nTable.Name);
                foreach(var nColumn in nTable.Columns)
                {
                    foreach(var nCondition in nColumn.Conditions)
                    {
                        Console.WriteLine($" col { nCondition.ColumnName} operator { nCondition.Operator} val {nCondition.Value}");
                    }
                }
            }
            Console.ReadKey();
        }
        private static string ReadFile(string aPath)
        {
            return File.ReadAllText(aPath);
        }
    }
}