using Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    public class TableMaskedEventArgs
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public List<MaskedTableViewItem> MaskedTableItems { get; set; } = new List<MaskedTableViewItem>();

        public TableMaskedEventArgs(string aTableName, string aColumnName, List<MaskedTableViewItem> aMaskedTableItems)
        {
            TableName = aTableName;
            ColumnName = aColumnName;
            MaskedTableItems = aMaskedTableItems;
        }

    }
}
