using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    [DataContract]
    public abstract class BodyPart
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] public string ImagePath { get; set; }
        [DataMember] public readonly Character.BodyPartSlot.SlotType? Slot = null;
        [DataMember] public readonly Character.RaceType? Race = null;
        [DataMember] public readonly Character.GenderType? Gender = null;

        public string Type
        {
            get
            {
                if (this is Organ) return "Орган";
                else if (this is Augment) return "Аугмент";
                else return "Неизвестно";
            }
        }

        public string SlotString
        {
            get => Slot == null ? "Неизвестно" : Character.BodyPartSlot.GetSlotName((Character.BodyPartSlot.SlotType)Slot);
        }

        public BodyPart(string name, Character.BodyPartSlot.SlotType slot, string image, string description = "")
        {
            Name = name;
            Description = description;
            ImagePath = image;
            Slot = slot;
        }

        public BodyPart(string name, string image, string description = "")
        {
            Name = name;
            Description = description;
            ImagePath = image;
        }
    }
}