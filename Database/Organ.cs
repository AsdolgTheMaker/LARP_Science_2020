using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    public class Organ
    {
        public int id;
        public string Name;
        public string Description;
        public Dictionary<string, string> CustomParameters;
        public Augment AugmentEquivalent;
        public BodyAlign Align;

        // Это очень важные параметры - именно srcName отвечает за слот, в который попадёт орган при отображении.
        internal string srcName;
        internal string srcDescr;

        public string GetNativeName() => srcName;
        public string GetNativeDescription() => srcDescr;

        public enum BodyAlign
        {
            Left,
            Middle,
            Right,
            None
        }

        public bool IsAugment()
        {
            if (AugmentEquivalent == null)
                return false;
            else
                return true;
        }

        public bool InsertAugment(Augment augment)
        {
            if (AugmentEquivalent == null)
            {
                AugmentEquivalent = augment;

                Name = augment.Name;
                Description = augment.Description;

                return true;
            }
            else return false;
        }

        public bool RemoveAugment()
        {
            if (AugmentEquivalent != null)
            {
                AugmentEquivalent = null;

                Name = srcName;
                Description = srcDescr;

                return true;
            }
            else return false;
        }

        /// <summary>
        /// Готовые описания: Мозг, Сердце, Легкое, Кожа, Глаз, Рука, Нога
        /// </summary>
        public static Dictionary<string, string> PresetDescriptions = new Dictionary<string, string>
        {
            ["Мозг"] = "Центральный отдел нервной системы, обычно расположенный в головном (переднем) отделе тела и представляющий собой компактное скопление нервных клеток и их отростков-дендритов. У многих животных содержит также глиальные клетки, может быть окружен оболочкой из соединительной ткани. У позвоночных животных (в том числе и у человека) различают головной мозг, размещённый в полости черепа, и спинной, находящийся в позвоночном канале.",
            ["Сердце"] = "Полый фиброзно-мышечный орган, обеспечивающий посредством повторных ритмичных сокращений ток крови по кровеносным сосудам. Присутствует у всех живых организмов с развитой кровеносной системой, включая всех позвоночных. Сердце позвоночных состоит главным образом из сердечной, эндотелиальной и соединительной ткани. При этом сердечная мышца представляет собой особый вид поперечно-полосатой мышечной ткани, встречающейся исключительно в сердце.",
            ["Легкое"] = "Орган воздушного дыхания у всех млекопитающих, птиц, пресмыкающихся, большинства земноводных, а также у некоторых рыб. В лёгких осуществляется газообмен между воздухом, находящимся в паренхиме лёгких, и кровью, протекающей по лёгочным капиллярам.",
            ["Кожа"] = "Наружный покров тела. Кожа защищает тело от широкого спектра внешних воздействий, участвует в дыхании, терморегуляции, обменных и многих других процессах. Кроме того, кожа представляет массивное рецептивное поле различных видов поверхностной чувствительности (боли, давления, температуры и т. д.).",
            ["Глаз"] = "Сенсорный орган (орган зрительной системы), обладающий способностью воспринимать электромагнитное излучение в световом диапазоне длин волн и обеспечивающий функцию зрения.",
            ["Рука"] = "Верхняя конечность опорно двигательного аппарата, одна из главнейших частей тела. С помощью рук гуманоид может выполнять множество действий, основным из которых является возможность захватывать предметы.",
            ["Нога"] = "Нога анатомически состоит из трёх основных частей: бедра, голени и стопы. Бедро образовано бедренной костью (самой массивной и прочной из человеческих костей) и надколенником, защищающим коленный сустав. Надколенник обеспечивает блок при разгибании голени. Голень образуют большая и малая берцовые кости. Стопу образует множество мелких костей."
        };

        /// <summary>
        /// Готовые наборы: Мозг, Сердце, [Левое/Правое] легкое, Кожа, Левый/Правый глаз, Левая/Правая рука, Левая/Правая нога
        /// </summary>
        public static Dictionary<string, Organ> Presets = new Dictionary<string, Organ>
        {
            ["Мозг"] = new Organ("Мозг", PresetDescriptions["Мозг"], align: BodyAlign.Middle),
            ["Сердце"] = new Organ("Сердце", PresetDescriptions["Сердце"], align: BodyAlign.Left),

            ["Легкое"] = new Organ("Легкое", PresetDescriptions["Легкое"]),
            ["Левое легкое"] = new Organ("Легкое", PresetDescriptions["Легкое"], align: BodyAlign.Left),
            ["Правое легкое"] = new Organ("Легкое", PresetDescriptions["Легкое"], align: BodyAlign.Right),

            ["Кожа"] = new Organ("Кожа", PresetDescriptions["Кожа"], customParams: new Dictionary<string, string>() { ["Цвет"] = "Белесый" }),

            ["Левый глаз"] = new Organ("Глаз", PresetDescriptions["Глаз"], align: BodyAlign.Left),
            ["Правый глаз"] = new Organ("Глаз", PresetDescriptions["Глаз"], align: BodyAlign.Right),

            ["Левая рука"] = new Organ("Рука", PresetDescriptions["Рука"], align: BodyAlign.Left),
            ["Правая рука"] = new Organ("Рука", PresetDescriptions["Рука"], align: BodyAlign.Right),
            ["Левая нога"] = new Organ("Нога", PresetDescriptions["Нога"], align: BodyAlign.Left),
            ["Правая нога"] = new Organ("Нога", PresetDescriptions["Нога"], align: BodyAlign.Right)
        };

        public Organ(
            string name,
            string description,
            Augment augment = null,
            Dictionary<string, string> customParams = null,
            BodyAlign align = BodyAlign.None)
        {
            srcName = name;
            srcDescr = description;

            AugmentEquivalent = augment;
            Name = augment == null ? name : augment.Name;
            Description = augment == null ? description : augment.Description;
            CustomParameters = customParams;
            Align = align;

            id = Controller.NewOrgan();
        }
    }
}