using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    [DataContract]
    public class Augment : BodyPart
    {
        [DataMember] private readonly Dictionary<string, string> CustomParameters = new Dictionary<string, string>();
        [DataMember] private readonly bool Visible;

        // Custom params methods
        public bool DoesParameterExist(string param) => CustomParameters.Keys.Contains(param);
        public bool AddCustomParameter(string paramName, string paramValue)
        {
            if (DoesParameterExist(paramName)) return false;

            CustomParameters.Add(paramName, paramValue);
            return true;
        }
        public bool RemoveCustomParameter(string paramName) => CustomParameters.Remove(paramName);

        // Getters
        public bool IsVisible() => Visible;
        public Dictionary<string, string> GetAllCustomParameters() => CustomParameters;
        public Character.BodyPartSlot.SlotType? GetDestinationSlot() => Slot;

        // Primary augment constructor
        public Augment(string name, Character.BodyPartSlot.SlotType slot, string image, string description = "", Dictionary<string, string> customParams = null) 
            : base(name, slot, image, description)
        {
            CustomParameters = customParams ?? new Dictionary<string, string>();
            Visible = true;
        }

        // Secondary augment constructor
        public Augment(string name, string image, string description = "", Dictionary<string, string> customParams = null) 
            : base(name, image, description)
        {
            CustomParameters = customParams ?? new Dictionary<string, string>();
            Visible = false;
        }
    }
}