using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace LARP.Science.Database
{
    public static class Controller
    {
        public static List<Character> Characters;
        public static List<string> CharacterIDs = new List<string>() { "0000" };
        public static string GetFreeCharacterID()
        {
            string id = "0000";
            while (CharacterIDs.Contains(id))
                id = new Random().Next(1000, 9999).ToString();
            CharacterIDs.Add(id);

            return id;
        }
        public static void NewCharacter(Character character)
        {
            Characters.Add(character);
        }

        public static int GlobalOrganCounter = 0;
        public static int NewOrgan()
        {
            GlobalOrganCounter++;
            return GlobalOrganCounter;
        }

        public static void Initialize(System.Windows.Controls.DataGrid patientsList = null)
        {
            patientsList.ItemsSource = Characters;
        }

        public static void CreateTestDatabase()
        {
            Characters = new List<Character>()
            {
                // Человек со стандартным набором органов
                new Character(name: "Обычный Джо", description: "Это самый обычный Джо, которого только можно вообразить."),

                // Человек с протезом левой ноги
                new Character(name: "Одноногий Бача", description: "Бача с рождения служил на флоте, а ногу потерял после неудачного опыта с ролевиками.",
                    organs: new List<Organ>()
                    {
                        Organ.Presets["Кожа"],
                        Organ.Presets["Мозг"],
                        Organ.Presets["Левый глаз"], Organ.Presets["Правый глаз"],
                        Organ.Presets["Левая рука"], Organ.Presets["Правая рука"],
                        Organ.Presets["Сердце"],
                        Organ.Presets["Легкое"], Organ.Presets["Легкое"],
                        new Organ("Левая нога", Organ.PresetDescriptions["Нога"], new Augment("Протез левой ноги", "Протез из обработанного воронежскими электриками металла. ")),
                        Organ.Presets["Правая нога"]
                    })

            };
        }
    }
}