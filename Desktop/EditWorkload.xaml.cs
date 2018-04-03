using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for EditWorkload.xaml
    /// </summary>
    public partial class EditWorkload : Window
    {
        private string workloadPath;
        public bool IsEdited = false;
        public EditWorkload(string path)
        {
            InitializeComponent();
            workloadPath = path;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string text = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            using (StreamWriter file = new StreamWriter(workloadPath))
            {
                file.WriteLine(text);
            }
            IsEdited = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
