using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    [DataContract]
    public class Augment
    {
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] private readonly Dictionary<string, string> CustomParameters;
        [DataMember] private readonly Character.OrganSlot.SlotType DestinationSlot;
        [DataMember] private readonly bool Visible;

        // Methods
        public bool DoesParameterExist(string param)
        {
            if (CustomParameters.Keys.Contains(param)) return true; 
            else return false;
        }

        // Getters
        public bool IsVisible() => Visible;
        public Character.OrganSlot.SlotType GetDestinationSlot() => DestinationSlot;

        // Setters
        public bool AddCustomParameter(string paramName, string paramValue)
        {
            if (DoesParameterExist(paramName)) return false;

            CustomParameters.Add(paramName, paramValue);
            return true;
        }
        public bool RemoveCustomParameter(string paramName) => CustomParameters.Remove(paramName);

        // Primary augment constructor
        public Augment(string name, Character.OrganSlot.SlotType slot, string description = "", Dictionary<string, string> customParams = null)
        {
            Name = name;
            Description = description;
            CustomParameters = customParams;
            DestinationSlot = slot;
            Visible = true;
        }

        // Secondary augment constructor
        public Augment(string name, string description = "", Dictionary<string, string> customParams = null) 
        {
            Name = name;
            Description = description;
            CustomParameters = customParams;
            Visible = false;
        }
    }
}
