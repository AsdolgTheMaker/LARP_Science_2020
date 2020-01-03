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

        public PrimaryAugment() { }

        public PrimaryAugment(Augment source)
        {
            name = source.Name;
            description = source.Description;
            slot = source.Slot.GetDescription();
            race = source.Race == null ? "" : source.Race.GetDescription();
            gender = source.Gender;
            customParams = source.AllCustomParameters;
        }

        public Augment ConvertToScienceObject() => new Augment(
            name, description,
            CustomEnum.GetValueFromDescription<Character.BodyPartSlot.SlotType>(slot),
            CustomEnum.GetValueFromDescription<Character.RaceType>(race),
            genderInverted, customParams)
        {
            Id = id
        };

    }
}