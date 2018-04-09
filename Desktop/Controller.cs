using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DatabaseConnection;
using System.Data;
using System.IO;

namespace Desktop
{
    public class Controller : INotifyPropertyChanged
    {
        private ObservableCollection<string> mConstraint;
        private ObservableCollection<string> mMaskList;
        private Column mCurrentColumn;
        private Table mCurrentTable;
        public ObservableCollection<ColumnView> mColumns;
        public string mFunctionName;

        public Database Database;
        public bool WorkloadLoaded = false;
        public bool MaskTechnique = false;
        public bool AlreadyMasked = false;
        public ObservableCollection<String> Tables { get; set; }
        public WorkloadProcessing.WorkloadProcessing WorkloadProcessing;
        public ObservableCollection<string> Constraint {
            get
            {
                return this.mConstraint;

            }
            set
            {
                this.mConstraint = value;
                this.OnPropertyChanged("Constraint");
            }
        }
        public ObservableCollection<string> MaskList
        {
            get
            {
                return this.mMaskList;
                
            }
            set
            {
                this.mMaskList = value;
                this.OnPropertyChanged("MaskList");
            }
        }
        public Table CurrentTable {
            get
            {
                return this.mCurrentTable;
            }
            set
            {
                this.mCurrentTable = value;
                this.OnPropertyChanged("CurrentTable");
            }
        }
        public Column CurrentColumn
        {
            get
            {
                return this.mCurrentColumn;
            }
            set
            {
                this.mCurrentColumn = value;
                this.OnPropertyChanged("CurrentColumn");
            }
        }
        public ObservableCollection<ColumnView> Columns
        {
            get
            {
                return this.mColumns;
            }
            set
            {
                this.mColumns = value;
                this.OnPropertyChanged("Columns");
            }
        }
        public string FunctionName
        {
            get
            {
                return this.mFunctionName;

            }
            set
            {
                this.mFunctionName = value;
                this.OnPropertyChanged("FunctionName");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Controller()
        {
            Tables = new ObservableCollection<String>();
            CurrentTable = new Table();
            CurrentColumn = new Column();
            Database = null;
            MaskList = new ObservableCollection<string>();
            WorkloadProcessing = new WorkloadProcessing.WorkloadProcessing();
            //LoadSubstitutionValues(@"C:\Data\names.txt");
        }
        public event EventHandler<TableMaskedEventArgs> OnTableMasked;
        public string ApplyMasking(string aFirstParameter, string aSecondParameter)
        {
            try
            {
                bool lNoWorkload = false;
                string lInfoLabelString = "";
                List<MaskedTableViewItem> lMaskedTableItems = new List<MaskedTableViewItem>();
                string lFunctionName = FunctionName;
                int lCurrentTableID = CurrentTable.TableId;
                string lCurrentTableName = CurrentTable.Name;
                string lCurrentColumnName = CurrentColumn.Name;
                int lCurrentColumnId = CurrentColumn.ColumnId;
                string lType = Database.Tables[lCurrentTableID].Columns[lCurrentColumnId].Type;
                Random lRand = new Random();
                if (WorkloadLoaded)
                {
                    if (WorkloadProcessing.Tables.Count == 0)
                    {
                        lNoWorkload = true;
                    }
                }
                if (MaskTechnique)
                {
                    try
                    {
                        if (!IsKey(lCurrentTableID, lCurrentColumnName))
                        {

                            var lDataCount = Database.Tables.FirstOrDefault(aR => aR.Name == lCurrentTableName).Data.Tables[0].AsEnumerable().ToArray()
                                .Select(aR => aR[lCurrentColumnName].ToString()).ToArray().GroupBy(aR => aR).Count();
                            //   var lTMP = Database.Tables[lCurrentTableID].Data.Tables[0].
                            if (!Mask.Mask.SubValuesLoaded(lFunctionName))
                            {
                                lInfoLabelString = "Substitution list is not loaded";
                                return lInfoLabelString;
                            }
                            if (!Mask.Mask.EnoughSubValues(lDataCount, lFunctionName))
                            {
                                lInfoLabelString = "Substitution list is not large enough";
                                return lInfoLabelString;
                            }
                            DataTable origVals = Database.Tables[lCurrentTableID].Data.Tables[0].Copy();
                            for (int i = 0; i < Database.Tables[lCurrentTableID].Data.Tables[0].Rows.Count; i++)
                            {
                                MaskedTableViewItem lMaskedItem = new MaskedTableViewItem();
                                var lOriginalValue = origVals.Rows[i][lCurrentColumnName].ToString();
                                lMaskedItem.OriginalValue = lOriginalValue.ToString();
                                if (WorkloadLoaded)
                                {
                                    if (WorkloadProcessing.Tables.FirstOrDefault(aR => aR.Name.ToLower() == lCurrentTableName.ToLower()) == null)
                                    {
                                        string lValue = Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName].ToString();
                                        string lMaskedValue = Mask.Mask.ApplyMasking(lValue, lType, lFunctionName, aFirstParameter, aSecondParameter);
                                        lMaskedItem.MaskedValue = lMaskedValue.ToString();
                                        Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName] = lMaskedValue;
                                    }
                                    else if (WorkloadProcessing.Tables.FirstOrDefault(aR => aR.Name.ToLower() == lCurrentTableName.ToLower())
                                        .Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCurrentColumnName.ToLower()) == null)
                                    {
                                        string lValue = Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName].ToString();
                                        string lMaskedValue = Mask.Mask.ApplyMasking(lValue, lType, lFunctionName, aFirstParameter, aSecondParameter);
                                        lMaskedItem.MaskedValue = lMaskedValue.ToString();
                                        Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName] = lMaskedValue;
                                    }
                                    else
                                    {
                                        var lColumn = WorkloadProcessing.Tables.FirstOrDefault(aR => aR.Name.ToLower() == lCurrentTableName.ToLower())
                                        .Columns.FirstOrDefault(aR => aR.Name.ToLower() == lCurrentColumnName.ToLower());
                                        string lValue = Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName].ToString();
                                        string lMaskedValue = Mask.Mask.ApplyMasking(lValue, lType, lFunctionName, aFirstParameter, aSecondParameter, lColumn.Conditions);
                                        lMaskedItem.MaskedValue = lMaskedValue.ToString();
                                        Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName] = lMaskedValue;
                                    }
                                }
                                else
                                {
                                    lNoWorkload = true;
                                }
                                if (lNoWorkload)
                                {
                                    string lValue = Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName].ToString();
                                    string lMaskedValue = Mask.Mask.ApplyMasking(lValue, lType, lFunctionName, aFirstParameter, aSecondParameter);
                                    lMaskedItem.MaskedValue = lMaskedValue.ToString();
                                    Database.Tables[lCurrentTableID].Data.Tables[0].Rows[i][lCurrentColumnName] = lMaskedValue;
                                }
                                lMaskedTableItems.Add(lMaskedItem);
                                //Table.BuildInsertCommand(dVM.db.Tables[ctable].Data, dVM.db.Tables[ctable].Name);
                                //Table.DisableConstrains();
                                //Table.DeleteData(dVM.db.Tables[ctable].Name);
                                //Table.InsertData(dVM.db.Tables[ctable].Data, dVM.db.Tables[ctable].Name);
                                //Table.EnableConstrains();
                                Trace.Flush();
                            }

                            MaskTechnique = false;
                            lInfoLabelString = "Data in column " + lCurrentColumnName + " in table " + lCurrentTableName + " was masked";
                            if (OnTableMasked != null)
                                OnTableMasked(this, new TableMaskedEventArgs(lCurrentTableName, lCurrentColumnName, lMaskedTableItems));
                        }
                        else
                            lInfoLabelString = "Primary keys, Foreign keys and Unique columns cannot be masked!";
                    }
                    catch
                    {

                    }

                }

                else
                    lInfoLabelString = "Mask technique was not selected";
                return lInfoLabelString;
            }
            catch
            {
                string lInfoLabelString = "Nothing to mask";
                return lInfoLabelString;
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string LoadDatabase()
        {
            string lResult = "";
            try
            {
                lResult = DatabaseHelper.GetDBSchema();
                Database = DatabaseHelper.Database;
                FillTablesTable();
                return lResult;
            }
            catch
            {
                return lResult;
            }
            
        }
        private void FillTablesTable()
        {
            if(Database.Tables != null)
            {
                foreach (var nTable in Database.Tables)
                {
                    Tables.Add(nTable.Name);
                }
            }
        }
        public string GetColumnType(int aTableIndex, int aColumnIndex)
        {
            try
            {
                return Database.Tables[aTableIndex].Data.Tables[0].Columns[aColumnIndex].DataType.ToString();
            }
            catch
            {
                return "";
            }
        }
        public void FillMaskList()
        {
            try
            {
                MaskList.Clear();
                string lType = Database.Tables[CurrentTable.TableId].Columns[CurrentColumn.ColumnId].Type;
                if (lType.Equals("int"))
                {
                    MaskList.Add("Hash");
                    MaskList.Add("Random Value");
                    MaskList.Add("RangedValue");
                    MaskList.Add("Offset");
                    MaskList.Add("Shuffle");
                }
                else if (lType.Equals("varchar") || lType.Equals("nvarchar") || lType.Equals("varchar2") || lType.Equals("text"))
                {
                    MaskList.Add("Random String Part");
                    // MaskList.Add("Replace String Part");
                    MaskList.Add("Substitution");
                }
                else if (lType.Equals("date") || lType.Equals("datetime") || lType.Equals("datetime2"))
                {
                    MaskList.Add("Hash date");
                }
                else if (lType.Equals("bit"))
                {
                    MaskList.Add("Random Value");
                }
                else if (lType.Equals("float") || lType.Equals("double"))
                {
                    MaskList.Add("Hash");
                }
            }
            catch
            {

            }
            
        }
        public void GetColumns(string aTableName)
        {
            try
            {
                Table lSelectedTable = Database.Tables.FirstOrDefault(aR => aR.Name == aTableName);
                Columns = new ObservableCollection<ColumnView>();
                Constraint = new ObservableCollection<string>();
                ObservableCollection<Column> lColumns = new ObservableCollection<Column>(lSelectedTable.Columns);
                foreach (var nColumn in lColumns)
                {
                    ColumnView lColumnView = new ColumnView();
                    lColumnView.Name = nColumn.Name;
                    if (nColumn.ForeignKey)
                        lColumnView.Constraint += "F ";
                    if (nColumn.PrimaryKey)
                        lColumnView.Constraint += "P ";
                    if (nColumn.Unique)
                        lColumnView.Constraint += "U ";
                    Columns.Add(lColumnView);
                }
            }
            catch
            {

            }
           
        }
        public void LoadSubstitutionValues(string aPath)
        {
            try
            {
                Mask.Mask.SubstitutionValues = File.ReadLines(aPath).ToList();
            }
            catch
            {

            }
        }
        public bool IsKey(int aTableIndex, string aColumnName)
        {
            try
            {
                Column lColumn = Database.Tables[aTableIndex].Columns.Find(x => x.Name == aColumnName);
                //Database.Tables.FirstOrDefault(aR => aR.TableId == aTableIndex).Columns.FirstOrDefault(aR => aR.Name == aColumnName);
                if (lColumn.PrimaryKey || lColumn.ForeignKey || lColumn.Unique)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
          
        }

    }
}
