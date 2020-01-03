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
        public string slot { get; set; } 
        public string race { get; set; }
        public Character.GenderType? gender { get; set; }
        public Character.GenderType genderInverted
        {
            get
            {
                if (gender == Character.GenderType.Male)
                    return Character.GenderType.Female;
                else return Character.GenderType.Male;
            }
        }

        public EjectedOrgan() : base() { }

        public EjectedOrgan(Organ organ)
        {
            name = organ.Name;
            description = organ.Description;
            slot = organ.Slot.GetDescription();
            race = organ.Race.GetDescription();
            gender = organ.Gender == Character.GenderType.Male ? Character.GenderType.Female : Character.GenderType.Male;
            id = organ.Id;
        }

        public Organ ConvertToScienceObject()
        {
            var Slot = CustomEnum.GetValueFromDescription<Character.BodyPartSlot.SlotType>(slot);
            return new Organ(name, Slot,
                Character.BodyPartSlot.GetSlotPicture(Slot, CustomEnum.GetValueFromDescription<Character.RaceType>(race), genderInverted),
                description)
            { 
                Id = id,
                Gender = genderInverted,
                Race = CustomEnum.GetValueFromDescription<Character.RaceType>(race)
            };
        }
    }
}