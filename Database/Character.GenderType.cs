namespace LARP.Science.Database
{
    public partial class Character
    {
        public enum GenderType
        {
            Male, // Паркет
            Female // Плитка (ничего не спрашивайте)
        }

        public string GetGender => GetGenderByEnum(Gender);
        public static string GetGenderByEnum(GenderType gender)
        {
            switch (gender)
            {
                case GenderType.Male:
                    return "Мужской";
                case GenderType.Female:
                    return "Женский";
                default:
                    throw new System.ArgumentException();
            }
        }
    }
}