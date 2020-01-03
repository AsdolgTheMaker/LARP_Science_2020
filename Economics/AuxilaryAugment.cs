using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LARP.Science.Database;

namespace LARP.Science.Economics
{
    public class AuxilaryAugment : EjectedAugment
    {
        public string slot { get; set; } // serves only to determine that this is not actually auxilary augment
        public AuxilaryAugment() { }

        public AuxilaryAugment(Augment source)
        {
            name = source.Name;
            description = source.Description;
            customParams = source.AllCustomParameters;
        }

        public Augment ConvertToScienceObject() 
            => new Augment(name, "", description, customParams) { Id = id };
    }
}
