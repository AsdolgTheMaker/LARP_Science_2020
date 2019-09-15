using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    public partial class Character
    {
        public string ID;
        public string Name;
        public RaceType Race;
        public string Description;
        public List<Organ> Organs;
        public Statistics Stat = new Statistics();

        // Internal method works through static method
        public Organ GetOrganByName(string name, bool ignoreAugments = false) => GetOrganByName(name, Organs, ignoreAugments);
        public static Organ GetOrganByName(string name, List<Organ> organs, bool ignoreAugments = false)
        {
            foreach (Organ organ in organs)
                if (organ.Name == name && (ignoreAugments ? organ.IsAugment() : true)) return organ;
            return null;
        }

        /// <summary>
        /// Стандартный набор органов: кожа, мозг, 2 глаза, 2 руки, сердце, 2 легких, 2 ноги
        /// </summary>
        public static readonly List<Organ> DefaultOrgansPreset = new List<Organ>()
        {
            Organ.Presets["Кожа"],
            Organ.Presets["Мозг"],
            Organ.Presets["Левый глаз"], Organ.Presets["Правый глаз"],
            Organ.Presets["Левая рука"], Organ.Presets["Правая рука"],
            Organ.Presets["Сердце"],
            Organ.Presets["Легкое"], Organ.Presets["Легкое"],
            Organ.Presets["Нога"], Organ.Presets["Нога"]
        };

        public Character(string name,
            RaceType race = RaceType.Human,
            string id = "0000",
            string description = "",
            List<Organ> organs = null)
        {
            ID = id == "0000" ? Controller.GetFreeCharacterID() : id;
            Name = name;
            Race = race;
            Description = description;
            Organs = organs == null ? DefaultOrgansPreset : organs;

            // Go through validity pass to ensure we have everything under control.
            ValidateCharacter(this);

            Controller.NewCharacter(this);
        }

        // Constructor validity tools
        private void CheckOrganDuplicates()
        {
            for (int i = 0; i < Organs.Count - 1; i++)
            {
                string comparer = Organs[i].Name;
                for (int j = i + 1; j < Organs.Count; j++)
                    if (comparer == Organs[j].Name) throw new Organ.DuplicateOrgansException("Ошибка персонажа " + this.Name + ":\nОбнаружены дублирующиеся органы.");
            }
        }
        private static void ValidateCharacter(Character character)
        {
            character.CheckOrganDuplicates();
        }

    }
}