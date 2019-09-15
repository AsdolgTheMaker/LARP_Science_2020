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

        public bool IsAugment()
        {
            if (AugmentEquivalent == null)
                return false;
            else
                return true;
        }

        public static Dictionary<string, Organ> Presets = new Dictionary<string, Organ>
        {
            ["Сердце"] = new Organ("Сердце", "Полый фиброзно-мышечный орган, обеспечивающий посредством повторных ритмичных сокращений ток крови по кровеносным сосудам. Присутствует у всех живых организмов с развитой кровеносной системой, включая всех позвоночных. Сердце позвоночных состоит главным образом из сердечной, эндотелиальной и соединительной ткани. При этом сердечная мышца представляет собой особый вид поперечно-полосатой мышечной ткани, встречающейся исключительно в сердце."),
            ["Легкое"] = new Organ("Легкое", "Орган воздушного дыхания у всех млекопитающих, птиц, пресмыкающихся, большинства земноводных, а также у некоторых рыб. В лёгких осуществляется газообмен между воздухом, находящимся в паренхиме лёгких, и кровью, протекающей по лёгочным капиллярам."),
            ["Кожа"] = new Organ("Кожа", "Наружный покров тела. Кожа защищает тело от широкого спектра внешних воздействий, участвует в дыхании, терморегуляции, обменных и многих других процессах. Кроме того, кожа представляет массивное рецептивное поле различных видов поверхностной чувствительности (боли, давления, температуры и т. д.).",
                customParams: new Dictionary<string, string>() { ["Цвет"] = "Белесый" }),
            ["Глаз"] = new Organ("Глаз", "Сенсорный орган (орган зрительной системы), обладающий способностью воспринимать электромагнитное излучение в световом диапазоне длин волн и обеспечивающий функцию зрения.")
        };

        public Organ(string name, string description = "", Augment augment = null, Dictionary<string, string> customParams = null)
        {
            id = Controller.NewOrgan();
            Name = name;
            Description = description;
            AugmentEquivalent = augment;
            CustomParameters = customParams;
        }
    }
}