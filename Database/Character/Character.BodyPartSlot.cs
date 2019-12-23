using System;
using System.Collections.Generic;
using System.Linq;

namespace LARP.Science.Database
{
    public partial class Character
    {
        /// <summary>
        /// Defines available organ slots. Also features default organs data.
        /// </summary>
        public static partial class BodyPartSlot
        {
            #region // Default organs data
            public static List<Organ> GetOrgansListForCharacter(RaceType race, GenderType gender)
            {
                List<Organ> res = new List<Organ>();
                foreach (SlotType slot in Enum.GetValues(typeof(SlotType))) res.Add(GetSlotOrgan(slot, race, gender));
                return res;
            }
            public static Organ GetSlotOrgan(SlotType slot, RaceType race, GenderType gender)
                => new Organ(GetSlotName(slot), slot, GetSlotPicture(slot, race, gender), GetSlotDescription(slot));
            public static string GetSlotName(SlotType slot)
            {
                switch (slot)
                {
                    case SlotType.Brain:
                        return "Мозг";
                    case SlotType.Heart:
                        return "Сердце";
                    case SlotType.Breath:
                        return "Дыхательная система";
                    case SlotType.Body:
                        return "Туловище";
                    case SlotType.LeftArm:
                        return "Левая рука";
                    case SlotType.RightArm:
                        return "Правая рука";
                    case SlotType.LeftLeg:
                        return "Левая нога";
                    case SlotType.RightLeg:
                        return "Правая нога";
                    case SlotType.Spleen:
                        return "Селезёнка";
                    case SlotType.Liver:
                        return "Печень";
                    case SlotType.Stomach:
                        return "Желудок";
                    case SlotType.Kidneys:
                        return "Почки";
                    case SlotType.Intestines:
                        return "Кишечник";
                    case SlotType.Bladder:
                        return "Мочевой пузырь";
                    case SlotType.Reproduction:
                        return "Половая система";
                    case SlotType.Head:
                        return "Голова";
                    default:
                        throw new ArgumentException("slot");
                }
            }

            public static string GetSlotDescription(SlotType slot)
            {
                if (PresetDescriptions.Keys.Contains(slot)) return PresetDescriptions[slot];
                else return Controller.UnknownDataTemplate;
            }

            public static string GetDefaultSlotPicture(SlotType? slot) 
                => slot == null ? Controller.NotFoundImagePath : GetSlotPicture(slot, RaceType.Human, GenderType.Male);

            public static string GetSlotPicture(SlotType? slot, RaceType race, GenderType gender, byte augLevel)
            {
                string result = "Resources\\Bodies\\" + race.ToString() + "\\" + gender.ToString() + "\\" + slot.ToString();
                switch (augLevel)
                {
                    case 0: break;
                    case 1:
                        {
                            result += "_Mod";
                            break;
                        }
                    case 2:
                    default:
                        {
                            result += "_Aug";
                            break;
                        }
                }
                return result += ".png";
            }
            public static string GetSlotPicture(SlotType? slot, RaceType race, GenderType gender)
                => GetSlotPicture(slot, race, gender, 0);
            #endregion

            private static readonly IReadOnlyDictionary<SlotType, string> PresetDescriptions = new Dictionary<SlotType, string>()
            {
                [SlotType.Brain] = "Центральный отдел нервной системы, обычно расположенный в головном (переднем) отделе тела и представляющий собой компактное скопление нервных клеток и их отростков-дендритов. У многих животных содержит также глиальные клетки, может быть окружен оболочкой из соединительной ткани. У позвоночных животных (в том числе и у человека) различают головной мозг, размещённый в полости черепа, и спинной, находящийся в позвоночном канале.",
                [SlotType.Heart] = "Полый фиброзно-мышечный орган, обеспечивающий посредством повторных ритмичных сокращений ток крови по кровеносным сосудам. Присутствует у всех живых организмов с развитой кровеносной системой, включая всех позвоночных. Сердце позвоночных состоит главным образом из сердечной, эндотелиальной и соединительной ткани. При этом сердечная мышца представляет собой особый вид поперечно-полосатой мышечной ткани, встречающейся исключительно в сердце.",
                [SlotType.LeftArm] = "Верхняя конечность опорно двигательного аппарата, одна из главнейших частей тела. С помощью рук гуманоид может выполнять множество действий, основным из которых является возможность захватывать предметы.",
                [SlotType.RightArm] = "Верхняя конечность опорно двигательного аппарата, одна из главнейших частей тела. С помощью рук гуманоид может выполнять множество действий, основным из которых является возможность захватывать предметы.",
                [SlotType.LeftLeg] = "Нога анатомически состоит из трёх основных частей: бедра, голени и стопы. Бедро образовано бедренной костью (самой массивной и прочной из человеческих костей) и надколенником, защищающим коленный сустав. Надколенник обеспечивает блок при разгибании голени. Голень образуют большая и малая берцовые кости. Стопу образует множество мелких костей.",
                [SlotType.RightLeg] = "Нога анатомически состоит из трёх основных частей: бедра, голени и стопы. Бедро образовано бедренной костью (самой массивной и прочной из человеческих костей) и надколенником, защищающим коленный сустав. Надколенник обеспечивает блок при разгибании голени. Голень образуют большая и малая берцовые кости. Стопу образует множество мелких костей."
            };
        }
    }
}