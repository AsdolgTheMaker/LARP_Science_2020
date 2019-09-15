using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    public class Augment
    {
        public string Name;
        public string Description;

        public Augment(string name, string description = "...")
        {
            Name = name;
            Description = description;
        }
    }
}
