using System;
using System.Collections.Generic;
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
    /// Interaction logic for ConnectionWin.xaml
    /// </summary>
    public partial class ConnectionWin : Window
    {
        private ConnectDvm _cDvm = new ConnectDvm();
        public ConnectionWin()
        {
            InitializeComponent();
        }

        private void ConnectBut_OnClick(object sender, RoutedEventArgs e)
        {
            _cDvm.Server = ServerTextBox.Text;
            _cDvm.Database = DbTextBox.Text;
            _cDvm.User = UserTextBox.Text;
            _cDvm.Password = PasswordBox.Password;
            if (_cDvm.SaveConnection())
            {
                ConLabel.Content = "Connected";
            }
            else
            {
                ConLabel.Content = "Connection cannot be established!";
            }
        }
    }
}
