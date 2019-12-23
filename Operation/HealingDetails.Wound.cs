using AsdolgTools;

namespace LARP.Science.Operation
{
    public partial class HealingDetails
    {
        public class Wound
        {
            public readonly Database.Organ Organ;
            public readonly DamageType Damage;

            public string OrganSlot { get; set; }
            public string DamageName { get; set; }

            public Wound(Database.Organ Organ, DamageType Damage)
            {
                this.Organ = Organ;
                this.Damage = Damage;

                OrganSlot = Database.Character.BodyPartSlot.GetSlotName((Database.Character.BodyPartSlot.SlotType)Organ.Slot);
                DamageName = Damage.GetDescription();
            }
        }
    }
}