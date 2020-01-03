using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    [DataContract]
    public partial class Character
    {
        [DataMember] public string ID { get; set; }
        [DataMember] public string Name { get; set; }
        [DataMember] public GenderType Gender { get; set; }
        [DataMember] public RaceType Race { get; set; }
        [DataMember] public string Description { get; set; }
        [DataMember] private Dictionary<BodyPartSlot.SlotType?, Organ> Organs { get; set; } = new Dictionary<BodyPartSlot.SlotType?, Organ>();
        [DataMember] private readonly List<Augment> SecondaryAugments = new List<Augment>();
        [DataMember] public Statistics Stat { get; set; } = new Statistics();
        [DataMember] public bool Alive { get; set; } = true;

        // Primary organs/augments 
        public Organ GetOrgan(BodyPartSlot.SlotType? slot) => Organs.Keys.Contains(slot) ? Organs[slot] : null;
        public List<Organ> OrgansList
        {
            get
            {
                List<Organ> organs = Organs.Values.ToList();
                while (organs.Contains(null)) organs.Remove(null);
                return organs;
            }
            set => OrgansList = value;
        }

        public List<Augment> PrimaryAugments
        {
            get
            {
                List<Augment> res = new List<Augment>();
                foreach (Organ organ in Organs.Values)
                    if (organ.IsAugmented) res.Add(organ.AugmentEquivalent);
                return res;
            }
        }

        /// <summary>
        /// Установка органа с заменой в подходящий слот
        /// </summary>
        /// <param name="newOrgan"></param>
        /// <returns></returns>
        public bool InstallOrgan(BodyPartSlot.SlotType? Slot)
        {
            if (Organs[Slot].Virtual == true)
            {
                Organs[Slot].Virtual = false;
                return true;
            }
            else return false;

        }

        public void InstallOrgansRange(List<Organ> organs)
        {
            foreach (Organ item in organs)
                Organs.Add(item.Slot, item);
        }

        public Augment InstallAugmentToOrganSlot(Augment augment)
        {
            // Store installed augment if present
            Augment removed = null;
            Organ organ = GetOrgan(augment.DestinationSlot);
            if (organ.IsAugmented)
                removed = organ.AugmentEquivalent;

            // Install new augment
            organ.AugmentEquivalent = augment;

            if (augment.IsReplacement) organ.Virtual = false;

            return removed;
        }

        public Organ EjectOrgan(BodyPartSlot.SlotType? slot)
        {
            if (slot == null) return null;

            Organ removed = GetOrgan(slot);
            Organs[slot].Virtual = true;
            return removed;
        }
        public Augment EjectAugmentFromOrganSlot(BodyPartSlot.SlotType? slot) => slot == null ? null : GetOrgan(slot).EjectAugment();

        // Augments methods
        public void AddAugment(Augment aug) => SecondaryAugments.Add(aug);
        public void AddAugmentsRange(List<Augment> augs)
        {
            foreach (Augment aug in augs)
                SecondaryAugments.Add(aug);
        }
        public bool EjectAugment(Augment aug) => SecondaryAugments.Remove(aug);
        public List<Augment> GetSecondaryAugments() => SecondaryAugments;

        // Main constructor
        public Character(string name,
            GenderType gender,
            RaceType race,
            string id = "0000",
            string description = "",
            Statistics statistics = null)
        {
            ID = id == "0000" ? Controller.GetNewCharacterID() : id;
            Name = name;
            Gender = gender;
            Race = race;
            Description = description == "" ? Controller.UnknownDataTemplate : description;
            Stat = statistics ?? new Statistics();

            // Install default organs list
            this.InstallOrgansRange(BodyPartSlot.GetOrgansListForCharacter(race, gender));

            this.ValidateCharacter();
            Controller.RegisterCharacter(this);
        }

        private void ValidateCharacter()
        {
            Console.WriteLine("Сделать валидацию персонажа при регистрации. Разобрать подробнее процесс - может, получится совсем опустить этот этап.");
        }
    }
}