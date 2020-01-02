using System.ComponentModel;
using AsdolgTools;

namespace LARP.Science.Database
{
    public partial class Character
    {
        public enum RaceType
        {
            [Description("Человек")] Human,
            [Description("Забрак")] Zabrak,
            [Description("Тогрута")] Togruta,
            [Description("Тви'лек")] Tvilek,
            [Description("Зелтрон")] Zeltron,
            [Description("Чисса")] Chissa,
            [Description("Киффар")] Kiffar,
            [Description("Катар")] Katar,
            [Description("Кел-дор")] Keldor,
            [Description("Умбара")] Umbara,
            [Description("Нагаи")] Nagai,
            [Description("Наутоланец")] Nautolan,
            [Description("Нурианец")] Nurian,
            [Description("Мириаланец")] Mirialan,
            [Description("Миралукка")] Miralukka,
            [Description("Миккианец")] Mikkian,
            [Description("Мрлсси")] Mrlssi,
            [Description("Панторианец")] Pantorian,
            [Description("Пау'ан")] Pauan,
            [Description("Врунианец")] Vrunian,
            [Description("Вуки")] Wookie,
            [Description("Фаргул")] Fargul,
            [Description("Родианец")] Rodian,
            [Description("Фоллинец")] Follin
        }

        public string GetRace => GetRaceByEnum(Race);
        public static string GetRaceByEnum(RaceType input) => input.GetDescription();
    }
}