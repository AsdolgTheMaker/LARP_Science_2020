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
        public List<Augment> Augments;
        public Statistics Stat = new Statistics();

        private static readonly List<Organ> DefaultOrgansPreset = new List<Organ>()
        {
            Organ.Presets["Глаз"], Organ.Presets["Глаз"],
            Organ.Presets["Легкое"], Organ.Presets["Легкое"],
            Organ.Presets["Сердце"], Organ.Presets["Кожа"]
        };

        public Character(string name,
            RaceType race,
            string id = "0000",
            string description = "",
            List<Organ> organs = null,
            List<Augment> augments = null)
        {
            ID = id == "0000" ? Controller.GetFreeCharacterID() : id;
            Name = name;
            Race = race;
            Description = description;
            Organs = organs == null ? DefaultOrgansPreset : organs;
            Augments = augments == null ? new List<Augment>() : augments;
            Controller.NewCharacter(this);
        }
    }
}