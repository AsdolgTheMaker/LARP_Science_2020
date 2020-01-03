using System.ComponentModel;

namespace LARP.Science.Database
{
    public partial class Character
    {
        public enum GenderType
        {
            [Description("Мужской")] Male,
            [Description("Женский")] Female
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