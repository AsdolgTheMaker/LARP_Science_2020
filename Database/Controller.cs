using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Data;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json.Linq;


namespace LARP.Science.Database
{
    // Этот класс можно назвать главным бэк-эндом всего нашего бэк-энда.
    public static class Controller
    {
        #region // Global variables
        private static readonly List<Character> Characters = new List<Character>();

        public static List<string> CharacterIDs = new List<string>() { "0000" };
        public static Character SelectedCharacter = null;
        #endregion

        #region // Global constants
        public static readonly string CurrentExecutableDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static readonly string NotFoundImagePath = "\\Resources\\NotFound.png";
        public static readonly string UnknownDataTemplate = "[ДАННЫЕ УДАЛЕНЫ]";
        public static readonly string CharactersDatabaseFile = "characters.json";
        #endregion

        #region // Storage manipulations
        public static void AddToStorage(BodyPart item)
        {
            Console.WriteLine(item.Name + " «добавлено» в хранилище.");
        }
        #endregion

        // Я не знаю зачем это, но если это убрать - всё перестаёт работать
        public static void Initialize(System.Windows.Controls.DataGrid patientsList = null)
        {
            if (patientsList == null) throw new ArgumentNullException("patientsList");
        }

        #region // Characters database manipulations
        public static string GetNewCharacterID()
        {
            string id = "0000";
            Random random = new Random();
            while (CharacterIDs.Contains(id))
                id = random.Next(1000, 9999).ToString();
            return id;
        }
        public static void RegisterCharacter(Character character)
        {
            if (!CharacterIDs.Contains(character.ID))
                CharacterIDs.Add(character.ID);
            Characters.Add(character);
        }
        public static List<Character> GetCharacters() => Characters;
        public static void SetCharactersDatabase(List<Character> input)
        {
            Characters.Clear();
            foreach (Character character in input)
                RegisterCharacter(character);
        }


        public static void ReadCharacters()
        {
            if (File.Exists(CharactersDatabaseFile))
            { // read database from file
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Character>));
                MemoryStream stream = new MemoryStream(File.ReadAllBytes(CharactersDatabaseFile)) { Position = 0 };
                SetCharactersDatabase(serializer.ReadObject(stream) as List<Character>);
            }
        }
        public static void SaveCharacters()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Character>));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, GetCharacters());
            stream.Position = 0;
            StreamReader streamRead = new StreamReader(stream);
            File.WriteAllText(CharactersDatabaseFile, JToken.Parse(streamRead.ReadToEnd()).ToString(Newtonsoft.Json.Formatting.Indented));
            
            stream.Dispose();
            streamRead.Dispose();
        }
        #endregion

#if DEBUG // Create testing database for debug configs
        public static void CreateTestDatabase()
        {
            // A human with standart organs set
            new Character(name: "Обычный Джо",
                                                    gender: Character.GenderType.Male,
                                                    race: Character.RaceType.Human,
                                                    description: "Это самый обычный Джо, которого только можно вообразить.");

            // A human with augmented left leg
            Character tempContainer = new Character(name: "Одноногий Бача",
                                          gender: Character.GenderType.Male,
                                          race: Character.RaceType.Human,
                                          description: "Бача с рождения служил на флоте, а ногу потерял после неудачного знакомства с ролевиками.");
            tempContainer.InstallAugmentToOrganSlot(new Augment("Стальной протез левой ноги", "Хороший металлический протез, по всем киберпанковским канонам проверку проходит.", Character.BodyPartSlot.SlotType.LeftLeg, tempContainer.Race, tempContainer.Gender));

            // Unknown race humanoid with cybernetic body
            tempContainer = new Character(name: "Терминатор", gender: Character.GenderType.Male,
                                          race: Character.RaceType.Human,
                                          description: "Прислан Скайнет из будущего чтобы остановить ролёвку про киберпанк, зашедшую слишком далеко.");
            tempContainer.InstallAugmentToOrganSlot(new Augment("Процессор Intel Core i9 серии X", "Мощь.", Character.BodyPartSlot.SlotType.Brain, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Мифриловое покрытие", "Выковано древними гномьими мастерами.", Character.BodyPartSlot.SlotType.Body, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Гидравлический пресс «ORA-HAN-D»", "Сожмёт и адаптируется.", Character.BodyPartSlot.SlotType.LeftArm, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Отбойный молоток класса WK", "Советуем обратиться в Фонд.", Character.BodyPartSlot.SlotType.RightArm, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Валенок левый", "Хороший тёплый валенок вашего дедушки.", Character.BodyPartSlot.SlotType.LeftLeg, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Костыль", "На самом деле всё тело можно было сделать из таких костылей.", Character.BodyPartSlot.SlotType.RightLeg, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Насос из колодца на даче", "Этот ржавый насос видел Сталина.", Character.BodyPartSlot.SlotType.Heart, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Замкнутая система жизнеобеспечения БИОС-3", "", Character.BodyPartSlot.SlotType.Breath, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Какая-то хрень", "Неизвестно что это, но по документации должно быть здесь", Character.BodyPartSlot.SlotType.Spleen, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Печень, распечатанная на 3D-принтере", "Пластик был не очень качественный.", Character.BodyPartSlot.SlotType.Liver, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Топливный бак от девятки", "На сорок литров.", Character.BodyPartSlot.SlotType.Bladder, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Водопроводная система из сантехнического ларька", "Вообще изначально эти трубы купили для сантехсабера, но что-то пошло не так в процессе сборки.", Character.BodyPartSlot.SlotType.Intestines, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Почки от дуба", "", Character.BodyPartSlot.SlotType.Kidneys, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Пылевой мешок от пылесоса", "Потому что какая разница, куда собирать отходы.", Character.BodyPartSlot.SlotType.Stomach, tempContainer.Race, tempContainer.Gender));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Система клонирвоания Grineer", "Ох уж эти гриндилки.", Character.BodyPartSlot.SlotType.Reproduction, tempContainer.Race, tempContainer.Gender));
        }
#endif
    }
}