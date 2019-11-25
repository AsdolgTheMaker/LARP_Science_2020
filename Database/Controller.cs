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

namespace LARP.Science.Database
{
    // Этот класс можно назвать главным бэк-эндом всего нашего бэк-энда.
    public static class Controller
    {
        public static readonly string CurrentExecutableDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static readonly string NotFoundImagePath = "\\Resources\\NotFound.png";
        public static readonly string UnknownDataTemplate = "[ДАННЫЕ УДАЛЕНЫ]";

        public static readonly string CharactersDatabaseFile = "characters.json";
        private static readonly List<Character> Characters = new List<Character>();
        public static List<string> CharacterIDs = new List<string>() { "0000" };
        public static Character SelectedCharacter = null;

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

        public static void Initialize(System.Windows.Controls.DataGrid patientsList = null)
        {
            if (patientsList == null) throw new ArgumentNullException("patientsList");
        }

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
                MemoryStream stream = new MemoryStream(File.ReadAllBytes(CharactersDatabaseFile));
                stream.Position = 0;
                SetCharactersDatabase(serializer.ReadObject(stream) as List<Character>);
            }
        }

        public static void SaveCharacters()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Character>));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, GetCharacters());
            stream.Position = 0;
            var streamRead = new StreamReader(stream);
            File.WriteAllText(CharactersDatabaseFile, streamRead.ReadToEnd());

            stream.Dispose();
            streamRead.Dispose();
        }

        #if DEBUG // Create testing database for debug configs
        public static void CreateTestDatabase()
        {
            // A human with standart organs set
            Character tempContainer = new Character(
                name: "Обычный Джо",
                gender: Character.GenderType.Male,
                race: Character.RaceType.Human,
                description: "Это самый обычный Джо, которого только можно вообразить.");

            // A human with augmented left leg
            tempContainer = new Character(
                name: "Одноногий Бача",
                gender: Character.GenderType.Male,
                race: Character.RaceType.Human,
                description: "Бача с рождения служил на флоте, а ногу потерял после неудачного знакомства с ролевиками.");
            tempContainer.InstallAugmentToOrganSlot(new Augment("Стальной протез левой ноги", Character.BodyPartSlot.SlotType.LeftLeg, ""));

            // Unknown race humanoid with cybernetic body
            tempContainer = new Character(
                name: "Терминатор", 
                gender: Character.GenderType.Male,
                race: Character.RaceType.Undefined,
                description: "Прислан Скайнет из будущего чтобы остановить ролёвку про киберпанк, зашедшую слишком далеко.");
            tempContainer.InstallAugmentToOrganSlot(new Augment("Процессор Intel Core i9 серии X", Character.BodyPartSlot.SlotType.Brain, "", "Мощь."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Мифриловое покрытие", Character.BodyPartSlot.SlotType.Body, "", "Выковано древними гномьими мастерами."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Гидравлический пресс «ORA-HAN-D»", Character.BodyPartSlot.SlotType.LeftArm, "", "Сожмёт и адаптируется."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Отбойный молоток класса WK", Character.BodyPartSlot.SlotType.RightArm, "", "Советуем обратиться в Фонд."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Валенок левый", Character.BodyPartSlot.SlotType.LeftLeg, "", "Хороший тёплый валенок вашего дедушки."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Костыль", Character.BodyPartSlot.SlotType.RightLeg, "", "На самом деле всё тело можно было сделать из таких костылей."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Насос из колодца на даче", Character.BodyPartSlot.SlotType.Heart, "Этот ржавый насос видел Сталина."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Замкнутая система жизнеобеспечения БИОС-3", Character.BodyPartSlot.SlotType.Breath, ""));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Какая-то хрень", Character.BodyPartSlot.SlotType.Spleen, "", "Неизвестно что это, но по документации должно быть здесь"));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Печень, распечатанная на 3D-принтере", Character.BodyPartSlot.SlotType.Liver, "Пластик был не очень качественный."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Топливный бак от девятки", Character.BodyPartSlot.SlotType.Stomach, "", "На сорок литров."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Водопроводная система из сантехнического ларька", Character.BodyPartSlot.SlotType.Intestines, "", "Вообще изначально эти трубы купили для сантехсабера, но что-то пошло не так в процессе сборки."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Почки от дуба", Character.BodyPartSlot.SlotType.Kidneys, ""));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Пылевой мешок от пылесоса", Character.BodyPartSlot.SlotType.Bladder, "", "Потому что какая разница, куда собирать отходы."));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Система клонирвоания Grineer", Character.BodyPartSlot.SlotType.Reproduction, "", "Ох уж эти гриндилки."));
        }
        #endif
    }
}