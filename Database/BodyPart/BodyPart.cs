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
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public string ImagePath;
        [DataMember] public readonly Character.BodyPartSlot.SlotType? Slot = null;

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