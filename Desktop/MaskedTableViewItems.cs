using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    public class MaskedTableViewItem
    {
        public string OriginalValue { get; set; }
        public string MaskedValue { get; set; }
        public MaskedTableViewItem()
        {
            OriginalValue = "";
            MaskedValue = "";
        }
        public MaskedTableViewItem(string aOriginalValue, string aMaskedValue)
        {
            OriginalValue = aOriginalValue;
            MaskedValue = aMaskedValue;
        }
    }
}
