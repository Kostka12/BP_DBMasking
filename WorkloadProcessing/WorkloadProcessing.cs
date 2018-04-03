using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseConnection;
using Microsoft.SqlServer.Management.SqlParser.Parser;
using Antlr4.Runtime;
using QueryParser;
using QueryParserImplementation;

namespace WorkloadProcessing
{
    public class WorkloadProcessing
    {
        //private const string mScriptPath = @"C:\workload\workload.txt";
        public List<ParsedQueryStructure.Table> Tables { get; set; }

        public WorkloadProcessing()
        {
        }
        public void LoadWorkload(string aScriptPath, EventHandler<string> aOnAnalyzingError)
        {
            Tables = null;
            ProcessWorkload(aScriptPath, aOnAnalyzingError);
        }
        public void ProcessWorkload(string aScriptPath, EventHandler<string> aOnAnalyzingError)
        {
            string lText = ReadFile(aScriptPath);
            StringReader lReader = new StringReader(lText);
            AntlrInputStream lInput = new AntlrInputStream(lReader);
            TSqlLexer lLexer = new TSqlLexer(new CaseChangingCharStream(lInput, true));
            CommonTokenStream lTokens = new CommonTokenStream(lLexer);
            TSqlParser lParser = new TSqlParser(lTokens);
            TSqlParser.Tsql_fileContext Tsql_fileContext1 = lParser.tsql_file();

            Antlr4.Runtime.Tree.ParseTreeWalker lWalker = new Antlr4.Runtime.Tree.ParseTreeWalker();
            AntlrTsqlListener lListener = new AntlrTsqlListener();
            lListener.OnAnalyzingError += aOnAnalyzingError;
            lWalker.Walk(lListener, Tsql_fileContext1);
            Tables = lListener.AnalyzedWorkload;
        }
        private static string ReadFile(string aPath)
        {
            return File.ReadAllText(aPath);
        }
    }
}
