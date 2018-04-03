using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsedQueryStructure
{
    public class Table
    {
        public string Name { get; set; }
        public List<Column> Columns { get; set; }
        public Table(string aName)
        {
            Name = aName;
            Columns = new List<Column>();
        }
    }
}
