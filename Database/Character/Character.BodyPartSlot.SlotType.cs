using System;
using System.ComponentModel;

namespace LARP.Science.Database
{
    public partial class Character
    {
        public static partial class BodyPartSlot
        { // SlotType declaration and operations on the type itself

            public enum SlotType
            {
                [Description("Мозг")] Brain,
                [Description("Дыхательная система")] Breath,
                [Description("Сердце")] Heart,
                [Description("Селезёнка")] Spleen,
                [Description("Печень")] Liver,
                [Description("Желудок")] Stomach,
                [Description("Выделительная система")] Kidneys,
                [Description("Кишечник")] Intestines,
                [Description("Мочевой пузырь")] Bladder,
                [Description("Репродуктивная система")] Reproduction,
                [Description("Голова")] Head,
                [Description("Туловище")] Body,
                [Description("Левая рука")] LeftArm,
                [Description("Правая рука")] RightArm,
                [Description("Левая нога")] LeftLeg,
                [Description("Правая нога")] RightLeg
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