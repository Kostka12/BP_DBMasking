using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsedQueryStructure
{
    public class Column
    {
        public string Name { get; set; }
        public List<Condition> Conditions { get; set; }

        public Column(string aName)
        {
            Conditions = new List<Condition>();
            Name = aName;
        }
    }
}
