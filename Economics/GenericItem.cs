using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Economics
{
    public abstract class GenericItem
    {
        protected string Name;
        protected string Description;

        public abstract object ConvertToScienceObject();
    }
}