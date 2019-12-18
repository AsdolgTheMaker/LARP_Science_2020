namespace LARP.Science.Database
{
    public partial class Character
    {
        public enum RaceType
        {
            Human,
            Zabrak,
            Undefined
        }

        public string GetRace => GetRaceByEnum(Race);
        public static string GetRaceByEnum(RaceType input)
        {
            switch (input)
            {
                case RaceType.Human:
                    return "Человек";
                case RaceType.Zabrak:
                    return "Забрак";
                case RaceType.Undefined:
                default: return "Неопределено";
            }
        }
    }
}