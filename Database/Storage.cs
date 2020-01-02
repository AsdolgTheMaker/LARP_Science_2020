using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    [Obsolete("C данными необходимо работать через интеграцию с экономикой (см. пространство имён Economics).", true)]
    [DataContract]
    [KnownType(typeof(Organ))]
    [KnownType(typeof(Augment))]
    public class Storage
    {
        [DataMember] private List<BodyPart> Items { get; set; } = new List<BodyPart>();

        public List<BodyPart> GetAll() => Items;

        public List<Organ> GetOrgans()
        {
            List<Organ> result = new List<Organ>();
            foreach (BodyPart item in Items)
                if (item is Organ organ)
                    result.Add(organ);
            return result;
        }

        public List<Organ> GetOrgans(Character.BodyPartSlot.SlotType slot)
        {
            List<Organ> result = new List<Organ>();
            foreach (BodyPart item in Items)
                if (item is Organ organ && organ.Slot == slot)
                    result.Add(organ);
            return result;
        }

        public List<Augment> GetAugments()
        {
            List<Augment> result = new List<Augment>();
            foreach (BodyPart item in Items)
                if (item is Augment aug)
                    result.Add(aug);
            return result;
        }

        public List<Augment> GetAugments(Character.BodyPartSlot.SlotType slot)
        {
            List<Augment> result = new List<Augment>();
            foreach (BodyPart item in Items)
                if (item is Augment aug && aug.Slot == slot)
                    result.Add(aug);
            return result;
        }

        public void TakeItem(BodyPart item) => Items.Remove(item);
        public void AddItem(BodyPart item) => Items.Add(item);

        public Storage() { }
    }
}