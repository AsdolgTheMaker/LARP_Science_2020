﻿using System;
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

        // Properties
        public bool IsVisible => Visible;
        public Dictionary<string, string> AllCustomParameters => CustomParameters;
        public Character.BodyPartSlot.SlotType? DestinationSlot => Slot;
        public bool IsReplacement => CustomParameters.ContainsKey("Полная замена");

        // Economics converter
        public Economics.EjectedAugment ConvertToEjectedAugment()
        {
            if (IsVisible) return new Economics.PrimaryAugment(this);
            else return new Economics.AuxilaryAugment(this);
        }

        // Valid primary augment constructor
        public Augment(string name, string description,
            Character.BodyPartSlot.SlotType slot, Character.RaceType race, Character.GenderType gender,
            Dictionary<string, string> customParams = null)
            : base(name, slot, string.Empty, description)
        {
            CustomParameters = customParams ?? new Dictionary<string, string>();
            Visible = true;

            Race = race;
            Gender = gender;

            if (CustomParameters.Count > 0 && CustomParameters.Keys.Contains("Полная замена"))
                ImagePath = Character.BodyPartSlot.GetSlotPicture(slot, race, gender, 2);
            else ImagePath = Character.BodyPartSlot.GetSlotPicture(slot, race, gender, 1);
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