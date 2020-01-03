using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsdolgTools;
using LARP.Science.Database;

namespace LARP.Science.Economics
{
    public class PrimaryAugment : EjectedAugment
    {
        private string Slot;
        private string Race;
        private string Gender;
        private Dictionary<string, string> CustomParams;

        public PrimaryAugment() { }

        public PrimaryAugment(Augment source)
        {
            Name = source.Name;
            Description = source.Description;
            Slot = ((Character.BodyPartSlot.SlotType)(source.Slot)).GetDescription();
            Race = source.Race == null ? "" : ((Character.RaceType)(source.Race)).GetDescription();
            Gender = source.Gender == null ? "" : ((Character.GenderType)(source.Gender)).GetDescription();
            CustomParams = source.AllCustomParameters;
        }

        public override object ConvertToScienceObject()
        {
            throw new NotImplementedException();
        }
    }
}