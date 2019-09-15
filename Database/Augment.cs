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
        public Dictionary<string, string> CustomParameters;

        public Augment(string name, string description, Dictionary<string, string> customParams = null)
        {
            Name = name;
            Description = description;
            CustomParameters = customParams;
        }
    }
}
