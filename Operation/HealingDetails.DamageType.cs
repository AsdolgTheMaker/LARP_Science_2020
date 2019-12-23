using System.ComponentModel;

namespace LARP.Science.Operation
{
    public partial class HealingDetails
    {
        public enum DamageType
        {
            [Description("Колото-резаные повреждения")] Blade,
            [Description("Ушибленно-рваные повреждения и переломы")] Blunt,
            [Description("Электрические повреждения")] Electro,
            [Description("Ожоги от плазменных и лазерных зарядов")] Blaster,
            [Description("Повреждение от светового меча")] Saber,
            [Description("Отравление")] Poison,
            [Description("Крупное структурное повреждение")] BodyHole
        }
    }
}