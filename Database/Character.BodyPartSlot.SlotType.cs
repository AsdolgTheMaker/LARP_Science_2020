using System;

namespace LARP.Science.Database
{
    public partial class Character
    {
        public static partial class BodyPartSlot
        { // SlotType declaration and operations on the type itself
            
            public enum SlotType
            {
                Brain,
                Breath,
                Heart,
                Spleen,
                Liver,
                Stomach,
                Kidneys,
                Intestines,
                Bladder,
                Reproduction,
                Head,
                Body,
                LeftArm,
                RightArm,
                LeftLeg,
                RightLeg
            }

            public static SlotType ParseSlotType(string source) => (SlotType)Enum.Parse(typeof(SlotType), source, true);
            public static string[] GetSlotTypeStrings()
            {
                string[] result = new string[Enum.GetValues(typeof(SlotType)).Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = ((SlotType)i).ToString();
                return result;
            }
        }

    }
}