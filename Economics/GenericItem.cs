using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Economics
{
    public abstract class GenericItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int count { get; set; } = 0;
    }
}