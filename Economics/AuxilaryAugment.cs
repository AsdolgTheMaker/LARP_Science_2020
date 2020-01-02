using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Economics
{
    public class AuxilaryAugment : EjectedAugment
    {
        private Dictionary<string, string> CustomParams;

        public AuxilaryAugment() { }

        public AuxilaryAugment(Database.Augment source)
        {
            this.Name = source.Name;
            this.Description = source.Description;
            this.CustomParams = source.AllCustomParameters;
        }

        public override object ConvertToScienceObject()
        {
            throw new NotImplementedException();
        }
    }
}
