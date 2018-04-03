using ParsedQueryStructure;
using QueryParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using DatabaseConnection;
using System.Data;

namespace QueryParserImplementation
{
    public class AntlrTsqlListener : TSqlParserBaseListener
    {
        public List<ParsedQueryStructure.Table> AnalyzedWorkload { get; set; }
        private Dictionary<string, string> AliasDic { get; set; } = new Dictionary<string, string>();
        private string CurrentTable = "";
        public DatabaseConnection.Database Database { get; set; }
        private List<string> mMentionedTables = new List<string>();
        private FunctionEnum mAggregateFunction;

        public AntlrTsqlListener()
        {
            AnalyzedWorkload = new List<ParsedQueryStructure.Table>();
            Database = new DatabaseConnection.Database();
            GetDbSchema();
        }
        public event EventHandler<string> OnAnalyzingError;
        public void GetDbSchema()
        {
            DatabaseHelper.GetDBSchema();
            Database = DatabaseHelper.Database;
        }
        public override void EnterQuery_expression([NotNull] TSqlParser.Query_expressionContext context)
        {
            AliasDic = new Dictionary<string, string>();
            mMentionedTables = new List<string>();
            CurrentTable = "";
        }
        public override void EnterTable_name([NotNull] TSqlParser.Table_nameContext context)
        {
            CurrentTable = context.GetText().ToLower();
            if (Database.Tables.FirstOrDefault(aR => aR.Name.ToLower() == CurrentTable) != null)
            {
                if (AnalyzedWorkload.FirstOrDefault(aR => aR.Name.ToLower() == CurrentTable) == null)
                {
                    AnalyzedWorkload.Add(new ParsedQueryStructure.Table(context.GetText()));
                }
                if (mMentionedTables.FirstOrDefault(aR => aR == context.GetText()) == null)
                {
                    mMentionedTables.Add(context.GetText());
                }
            }
        }
        public override void EnterTable_alias([NotNull] TSqlParser.Table_aliasContext context)
        {
            AliasDic.Add(CurrentTable, context.GetText());
            //Console.WriteLine($"{CurrentTable}    {context.GetText()}");
        }
        public override void EnterJoin_part([NotNull] TSqlParser.Join_partContext context)
        {
            //Console.WriteLine(context.GetText());
        }
        public override void EnterSearch_condition([NotNull] TSqlParser.Search_conditionContext context)
        {
            //    Condition lCondition = ParseCondition.Parse(context.GetText().ToLower());
            //    int lColumnCount = 0;
            //    string lTableName = "";
            //    if (AliasDic.Count == 0)
            //    {
            //        foreach (var nTable in Database.Tables)
            //        {//TODO: kdyz neni alias, asi nastavit promenou s aliasy pokud neni tak tohle pokud je tak podle aliasu

            //            if (lCondition.ColumnName != null || lCondition.Value != null || lCondition.Operator != null)
            //            {
            //                if (nTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCondition.ColumnName.ToLower()) != null)
            //                {
            //                    if (mMentionedTables.Contains(nTable.Name))
            //                    {
            //                        lColumnCount++;
            //                    }
            //                    lTableName = nTable.Name;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if (lCondition.ColumnName != null || lCondition.Value != null || lCondition.Operator != null)
            //        {
            //            Tuple<string, string> lColNameParts = ParseCondition.SplitAlias(lCondition.ColumnName);
            //            if (lColNameParts != null)
            //            {
            //                string lTableFromDic = AliasDic.Where(aR => aR.Value == lColNameParts.Item1).Select(aR => aR.Key).FirstOrDefault();
            //                if (Database.Tables.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower()) != null)
            //                {
            //                    ParsedQueryStructure.Table lTable = Tables.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower());
            //                    if (lTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower()) != null)
            //                    {
            //                        Tables.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCondition.ColumnName.ToLower())
            //                            .Conditions.Add(new Condition(lColNameParts.Item2, lCondition.Operator, lCondition.Value));
            //                    }
            //                    else
            //                    {
            //                        Tables.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.Add(new ParsedQueryStructure.Column(lColNameParts.Item2));
            //                        Tables.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower())
            //                            .Conditions.Add(new Condition(lColNameParts.Item2, lCondition.Operator, lCondition.Value));
            //                    }
            //                }
            //            }

            //        }
            //    }
            //    if (AliasDic.Count == 0)
            //    {
            //        if (lColumnCount > 1) { /*TODO: abmbigous column name*/ return; }
            //        if (lColumnCount == 0) { /*TODO: column not found*/return; }
            //        if (lColumnCount == 1)
            //        {
            //            ParsedQueryStructure.Table lTable = Tables.FirstOrDefault(aR => aR.Name == lTableName);
            //            if (lTable.Columns.FirstOrDefault(aR => aR.Name == lCondition.ColumnName) != null)
            //            {
            //                Tables.FirstOrDefault(aR => aR.Name == lTableName).Columns.FirstOrDefault(aR => aR.Name == lCondition.ColumnName)
            //                    .Conditions.Add(new Condition(lCondition.ColumnName, lCondition.Operator, lCondition.Value));
            //            }
            //            else
            //            {
            //                Tables.FirstOrDefault(aR => aR.Name == lTableName).Columns.Add(new ParsedQueryStructure.Column(lCondition.ColumnName));
            //                Tables.FirstOrDefault(aR => aR.Name == lTableName).Columns.FirstOrDefault(aR => aR.Name == lCondition.ColumnName)
            //                    .Conditions.Add(new Condition(lCondition.ColumnName, lCondition.Operator, lCondition.Value));
            //            }
            //        }
            //    }



        }
        public override void EnterAggregate_windowed_function([NotNull] TSqlParser.Aggregate_windowed_functionContext context)
        {
            mAggregateFunction = ParseCondition.NotifyAggregateFunction(context.GetText());
        }
        //TODO: agregacni funkce resit asi jenom COUNT,MIN, MAX... sum a avg nema smysl nemohl bych maskovat nic, 
        //zjistim data zjistim hodnoty pro ktere plati podminka v havingu a ty nebudu maskovat
        public override void EnterSearch_condition_and([NotNull] TSqlParser.Search_condition_andContext context)
        {
            try
            {
                foreach (var nChild in context.children)
                {
                    Condition lCondition = ParseCondition.Parse(nChild.GetText().ToLower());
                    if (lCondition.ColumnName == null || lCondition.Value == null || lCondition.Operator == null)
                        continue;                  
                    int lColumnCount = 0;
                    string lTableName = "";
                    if (AliasDic.Count == 0)
                    {
                        foreach (var nTable in Database.Tables)
                        {
                            if (mMentionedTables.FirstOrDefault(aR => aR.ToLower() == nTable.Name.ToLower()) != null)
                            {
                                if (nTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCondition.ColumnName.ToLower()) != null
                                    && !nTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCondition.ColumnName.ToLower()).PrimaryKey)
                                {
                                    if (mMentionedTables.Contains(nTable.Name))   
                                        lColumnCount++;                                  
                                    lTableName = nTable.Name;
                                }
                            }
                        }
                        if (lColumnCount > 1)
                        {
                            if (OnAnalyzingError != null)
                                OnAnalyzingError(this, "Ambiguous column name. Some conditions are not involved");
                            continue;
                        }
                        if (lColumnCount == 0)
                        {
                            if (OnAnalyzingError != null)
                                OnAnalyzingError(this, "Column not found. Some conditions are not involved");
                            continue;
                        }
                        if (lColumnCount == 1)
                        {
                            ParsedQueryStructure.Table lTable = AnalyzedWorkload.FirstOrDefault(aR => aR.Name == lTableName);
                            foreach (var nCondition in ChangeCondition(lCondition, lTableName))
                            {
                                ParsedQueryStructure.Condition lNewCondition = new Condition(nCondition.ColumnName, nCondition.Operator, nCondition.Value);
                                if (lTable.Columns.FirstOrDefault(aR => aR.Name == lCondition.ColumnName) != null)
                                {
                                    if (lTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCondition.ColumnName.ToLower()).Conditions.FirstOrDefault(aR => Condition.IsSameCondition(aR, lNewCondition)) != null)
                                        continue;
                                    AnalyzedWorkload.FirstOrDefault(aR => aR.Name == lTableName).Columns.FirstOrDefault(aR => aR.Name == lCondition.ColumnName)
                                        .Conditions.Add(lNewCondition);
                                }
                                else
                                {
                                    AnalyzedWorkload.FirstOrDefault(aR => aR.Name == lTableName).Columns.Add(new ParsedQueryStructure.Column(lCondition.ColumnName));
                                    AnalyzedWorkload.FirstOrDefault(aR => aR.Name == lTableName).Columns.FirstOrDefault(aR => aR.Name == lCondition.ColumnName)
                                        .Conditions.Add(lNewCondition);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var nCondition in ChangeCondition(lCondition, lTableName))
                        {
                            Tuple<string, string> lColNameParts = ParseCondition.SplitAlias(nCondition.ColumnName);
                            if (lColNameParts != null)
                            {
                                string lTableFromDic = AliasDic.Where(aR => aR.Value == lColNameParts.Item1).Select(aR => aR.Key).FirstOrDefault();
                                var lDBTable = Database.Tables.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower());
                                if (lDBTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower()) != null
                                    && !lDBTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower()).PrimaryKey)
                                {
                                    ParsedQueryStructure.Table lTable = AnalyzedWorkload.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower());
                                    ParsedQueryStructure.Condition lNewCondition = new Condition(lColNameParts.Item2, nCondition.Operator, nCondition.Value);
                                    if (lTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower()) != null)
                                    {
                                        if (lTable.Columns.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Conditions.FirstOrDefault(aR => Condition.IsSameCondition(aR, lNewCondition)) != null)
                                            continue;
                                        AnalyzedWorkload.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCondition.ColumnName.ToLower())
                                            .Conditions.Add(lNewCondition);
                                    }
                                    else
                                    {
                                        AnalyzedWorkload.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.Add(new ParsedQueryStructure.Column(lColNameParts.Item2));
                                        AnalyzedWorkload.FirstOrDefault(aR => aR.Name.ToLower() == lTableFromDic.ToLower()).Columns.FirstOrDefault(aR => aR.Name.ToLower() == lColNameParts.Item2.ToLower())
                                            .Conditions.Add(lNewCondition);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }

        }
        public override void EnterSearch_condition_not([NotNull] TSqlParser.Search_condition_notContext context)
        {
            //base.EnterSearch_condition_not(context);
        }
        public List<Condition> ChangeCondition(Condition aCondition, string aTableName)
        {
            string lColumnType = Database.Tables.FirstOrDefault(aR => aR.Name.ToLower() == aTableName.ToLower())
                .Columns.FirstOrDefault(aR => aR.Name.ToLower() == aCondition.ColumnName.ToLower()).Type;
            List<Condition> lConditions = new List<Condition>();
            //toto nene kdyz bude sum nebo avg tak to asi odstranit uplne; jinak nonsense podminka
            if (aCondition.Aggregate == FunctionEnum.NONE || aCondition.Aggregate == FunctionEnum.SUM || aCondition.Aggregate == FunctionEnum.AVG)
            {
                if (IsCorrectType(lColumnType, aCondition.Value))
                {
                    lConditions.Add(aCondition);
                }

                return lConditions;
            }

            string[] lData = Database.Tables.FirstOrDefault(aR => aR.Name == aTableName).Data.Tables[0].AsEnumerable().ToArray()
                .Select(aR => aR[aCondition.ColumnName].ToString()).ToArray();
            List<string> lToConditionList = ParseCondition.OperatorConvertion(lData, aCondition.Operator, aCondition.Aggregate, aCondition.Value);
            foreach (var nToCondition in lToConditionList)
            {

                if (IsCorrectType(lColumnType, aCondition.Value))
                {
                    lConditions.Add(new Condition(aCondition.ColumnName, "=", nToCondition));
                }

            }
            return lConditions;
        }
        public bool IsCorrectType(string aType, string aValue)
        {
            switch (aType)
            {
                case "int":
                    {
                        int lIntValue = 0;
                        return Int32.TryParse(aValue, out lIntValue);
                    }

                case "varchar":
                case "nvarchar":
                case "varchar2":
                case "text":
                    {
                        return true;
                    }
                case "date":
                    {
                        DateTime lIntValue = new DateTime();
                        return DateTime.TryParse(aValue, out lIntValue);
                    }
                default:
                    return false;

            }
        }

    }
}
