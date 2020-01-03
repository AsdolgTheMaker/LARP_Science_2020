using LARP.Science.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsdolgTools;

namespace LARP.Science.Economics
{
    public class EjectedOrgan : GenericItem
    {
        private string Slot;
        private string Race;
        private string Gender;

        public EjectedOrgan() { }

        public EjectedOrgan(Organ organ)
        {
            Name = organ.Name;
            Description = organ.Description;
            Slot = ((Character.BodyPartSlot.SlotType)(organ.Slot)).GetDescription();
            Race = ((Character.RaceType)(organ.Race)).GetDescription();
            Gender = ((Character.GenderType)(organ.Gender)).GetDescription();
        }

        public override object ConvertToScienceObject()
        {
            throw new NotImplementedException();
        }
    }
}