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
        [DataMember] public Statistics Stat { get; set; }

        // Primary organs/augments methods
        public Organ GetOrgan(BodyPartSlot.SlotType? slot) => Organs.Keys.Contains(slot) ? Organs[slot] : null;
        public List<Organ> GetOrgansList() => Organs.Values.ToList();
        public List<Augment> GetPrimaryAugments()
        {
            List<Augment> res = new List<Augment>();
            foreach (Organ organ in Organs.Values)
                if (organ.IsAugmented()) res.Add(organ.AugmentEquivalent);
            return res;
        }

        public Organ InstallOrgan(Organ newOrgan)
        {
            BodyPartSlot.SlotType? slot = newOrgan.Slot;
            Organ removed = GetOrgan(slot);
            Organs[slot] = newOrgan;

            return removed;
        }
        public List<Organ> InstallOrgansRange(List<Organ> organs)
        {
            List<Organ> ejectedOrgans = new List<Organ>();
            foreach (var item in organs)
            {
                Organ replacing = InstallOrgan(item);
                if (replacing != null) ejectedOrgans.Add(replacing);
            }
            return ejectedOrgans;
        }

        public Augment InstallAugmentToOrganSlot(Augment augment)
        {
            // Store installed augment if present
            Augment removed = null;
            Organ organ = GetOrgan(augment.GetDestinationSlot());
            if (organ.IsAugmented())
                removed = organ.AugmentEquivalent;

            // Install new augment
            organ.AugmentEquivalent = augment;

            return removed;
        }

        public Organ EjectOrgan(BodyPartSlot.SlotType slot)
        {
            Organ removed = GetOrgan(slot);
            Organs[slot] = null;
            return removed;
        }
        public Augment EjectAugmentFromOrganSlot(BodyPartSlot.SlotType slot) => GetOrgan(slot).EjectAugment();

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
            ID = id == "0000" ? Controller.GetAndRegisterNewCharacterID() : id;
            Name = name;
            Gender = gender;
            Race = race;
            Description = Controller.UnknownDataTemplate;
            Stat = statistics == null ? new Statistics() : statistics;

            // Install default organs list
            this.InstallOrgansRange(BodyPartSlot.GetOrgansListForCharacter(race, gender));

            this.ValidateCharacter();
            Controller.RegisterCharacter(this);
        }

        private void ValidateCharacter()
        {
            Console.WriteLine("NotImplemented: Валидация готового персонажа перед регистрацией.");
        }
    }
}