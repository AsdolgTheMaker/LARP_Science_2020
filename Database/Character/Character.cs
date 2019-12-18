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
        [DataMember] public bool Alive { get; set; } = true;

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

        /// <summary>
        /// Установка органа с заменой в подходящий слот
        /// </summary>
        /// <param name="newOrgan"></param>
        /// <returns></returns>
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

        public Organ EjectOrgan(BodyPartSlot.SlotType? slot)
        {
            if (slot == null) return null;

            Organ removed = GetOrgan(slot);
            Organs[slot] = null;
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