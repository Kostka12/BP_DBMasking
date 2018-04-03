using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DatabaseConnection;
using Mask;

using Table = DatabaseConnection.Table;
using WorkloadProcessing;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Controller mController;
        private WorkloadProcessing.WorkloadProcessing mWorkloadProcessing;
        private string mWorkloadFile;

        public MainWindow()
        {
            mWorkloadProcessing = new WorkloadProcessing.WorkloadProcessing();
            mController = new Controller();
            mController.OnTableMasked += OnMaskedTable;
            DataContext = mController; //někde musi byt chyba, DC musi byt až po intcomp
            InitializeComponent();
            HideElements();
        }

        private void AddMaskedTableView(string aTableName, List<MaskedTableViewItem> aMaskedTableItems)
        {
            MaskedTableView lMaskedTableView = new MaskedTableView(aMaskedTableItems);
            TabItem lTabItem = new TabItem();
            lTabItem.Header = aTableName;
            lTabItem.Content = lMaskedTableView;
            MaskedTableViewsTabControl.Items.Add(lTabItem);
        }
        private void ParamsControl(string aFunctionName)
        {
            switch (aFunctionName)
            {

                case "Random Value":
                    break;
                case "Offset":
                    FirstParameter.Visibility = Visibility.Visible;
                    SecondParameter.Visibility = Visibility.Visible;
                    FirstParamLabel.Content = "Bottom:";
                    FirstParamLabel.Visibility = Visibility.Visible;
                    SecParamLabel.Content = "Top:";
                    SecParamLabel.Visibility = Visibility.Visible;
                    break;
                case "RangedValue":
                    FirstParameter.Visibility = Visibility.Visible;
                    SecondParameter.Visibility = Visibility.Visible;
                    FirstParamLabel.Content = "From:";
                    FirstParamLabel.Visibility = Visibility.Visible;
                    SecParamLabel.Content = "To:";
                    SecParamLabel.Visibility = Visibility.Visible;
                    break;
                case "Random String Part":
                    FirstParameter.Visibility = Visibility.Visible;
                    SecondParameter.Visibility = Visibility.Visible;
                    FirstParamLabel.Content = "From:";
                    SecParamLabel.Content = "To:";
                    FirstParamLabel.Visibility = Visibility.Visible;
                    SecParamLabel.Visibility = Visibility.Visible;
                    break;
                case "Create Email":
                    break;
                case "Hash":
                case "Hash date":
                    FirstParamLabel.Visibility = Visibility.Hidden;
                    SecParamLabel.Visibility = Visibility.Hidden;
                    FirstParameter.Visibility = Visibility.Hidden;
                    SecondParameter.Visibility = Visibility.Hidden;
                    break;
                default:
                    FirstParamLabel.Visibility = Visibility.Hidden;
                    SecParamLabel.Visibility = Visibility.Hidden;
                    FirstParameter.Visibility = Visibility.Hidden;
                    SecondParameter.Visibility = Visibility.Hidden;
                    break;
            }
        }
        private void HideElements()
        {
            TablesGrid.Visibility = Visibility.Hidden;
            ColumnsGrid.Visibility = Visibility.Hidden;
            MasksComboBox.Visibility = Visibility.Hidden;
            MaskButton.Visibility = Visibility.Hidden;
            LoadWorkload.Visibility = Visibility.Hidden;
            EditWorkload.Visibility = Visibility.Hidden;
            //VievValues.Visibility = Visibility.Hidden;
            FirstParamLabel.Visibility = Visibility.Hidden;
            SecParamLabel.Visibility = Visibility.Hidden;
            FirstParameter.Visibility = Visibility.Hidden;
            SecondParameter.Visibility = Visibility.Hidden;
        }
        private void ShowElements()
        {
            TablesGrid.Visibility = Visibility.Visible;
            ColumnsGrid.Visibility = Visibility.Visible;
            MasksComboBox.Visibility = Visibility.Visible;
            MaskButton.Visibility = Visibility.Visible;
            LoadWorkload.Visibility = Visibility.Visible;
            //FirstParameter.Visibility = Visibility.Visible;
            SecondParameter.Visibility = Visibility.Visible;
            FirstParamLabel.Visibility = Visibility.Visible;
            SecParamLabel.Visibility = Visibility.Visible;
        }
        private void Connect(object sender, RoutedEventArgs e)
        {
            ConnectionWin lConnectionWindow = new ConnectionWin();
            lConnectionWindow.Show();
        }
        private void Disconnect(object sender, RoutedEventArgs e)
        {
            mController.Columns = new ObservableCollection<ColumnView>();
            mController.Tables = new ObservableCollection<string>();
            mController.CurrentColumn = new Column();
            mController.CurrentTable = new Table();
            HideElements();
        }
        private void LoadDbSchema(object sender, RoutedEventArgs e)
        {
            ShowElements();
            mController.LoadDatabase();
        }

        private void TablesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mController.CurrentColumn.ColumnId != 0)
            {
                ColumnsGrid.SelectedIndex = 0;
            }
            mController.CurrentTable.Name = TablesGrid.SelectedCells[0].Item.ToString();
            mController.CurrentTable.TableId = TablesGrid.SelectedIndex;
            mController.GetColumns(mController.CurrentTable.Name);
            mController.CurrentColumn.ColumnId = 0;
        }
        private void MasksComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MasksComboBox.SelectedItem != null)
            {
                mController.FunctionName = MasksComboBox.SelectedItem.ToString();
                mController.MaskTechnique = true;
                FirstParameter.Text = "";
                SecondParameter.Text = "";

            }
            string lFunctionName = mController.FunctionName;
            if (MasksComboBox.SelectedItem == null)
            {
                lFunctionName = "";
            }

            ParamsControl(lFunctionName);
        }
        private void ColumnsGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColumnsGrid.SelectedIndex == -1)
            {
                ColumnsGrid.SelectedIndex = 0;
            }
            else
            {
                ColumnView lColumnView = ColumnsGrid.SelectedItem as ColumnView;
                mController.CurrentColumn.Name = lColumnView.Name;
                mController.CurrentColumn.ColumnId = mController.Database.Tables[mController.CurrentTable.TableId].Columns.FindIndex(x => x.Name == mController.CurrentColumn.Name);
            }
            mController.FillMaskList();
            Trace.Flush();
        }

        private void MaskButton_OnClick(object sender, RoutedEventArgs e)
        {
            InfoLabel.Content = mController.ApplyMasking(FirstParameter.Text, SecondParameter.Text);
            MasksComboBox.SelectedItem = null;
        }
        private void LoadWorkload_OnClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog lOpenFileDialog = new Microsoft.Win32.OpenFileDialog();


            lOpenFileDialog.DefaultExt = ".txt";
            lOpenFileDialog.Filter = "TXT Files (*.txt)|*.txt|SQL Files (*.sql)|*.sql";

            Nullable<bool> result = lOpenFileDialog.ShowDialog();

            if (result == true)
            {
                mWorkloadFile = lOpenFileDialog.FileName;
                mController.WorkloadProcessing.LoadWorkload(mWorkloadFile, OnAnalyzingWorkload);
                InfoLabel.Content = "Workload was loaded";
                mController.WorkloadLoaded = true;
                EditWorkload.Visibility = Visibility.Visible;
            }
            else
            {
                InfoLabel.Content = "Workload was not loaded";
            }
        }
        private void EditWorkload_Click(object sender, RoutedEventArgs e)
        {
            EditWorkload lEditWindow = new EditWorkload(mWorkloadFile);
            lEditWindow.Show();
            string lFileText = File.ReadAllText(mWorkloadFile);
            lEditWindow.richTextBox.Document.Blocks.Clear();
            lEditWindow.richTextBox.Document.Blocks.Add(new Paragraph(new Run(lFileText)));
            if (lEditWindow.IsEdited)
            {
                mWorkloadProcessing.LoadWorkload(mWorkloadFile, OnAnalyzingWorkload);
            }

        }

        private void OnMaskedTable(object sender, TableMaskedEventArgs e)
        {
            AddMaskedTableView($"{e.TableName}: { e.ColumnName}", e.MaskedTableItems);
        }
        private void OnAnalyzingWorkload (object sender, string e)
        {
            InfoLabel.Content = e;
        }
    }
}
