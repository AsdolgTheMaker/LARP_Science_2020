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
        public static readonly string CharactersDatabaseFile = "characters.json";
        public static readonly string UnknownDataTemplate = "[ДАННЫЕ УДАЛЕНЫ]";
        private static readonly List<Character> Characters = new List<Character>();
        public static List<string> CharacterIDs = new List<string>() { "0000" };

        public static string GetAndRegisterNewCharacterID()
        {
            string id = "0000";
            Random random = new Random();
            while (CharacterIDs.Contains(id))
                id = random.Next(1000, 9999).ToString();
            CharacterIDs.Add(id);

            return id;
        }
        public static void RegisterCharacter(Character character) => Characters.Add(character);
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
            tempContainer.InstallAugmentToOrganSlot(new Augment("Стальной протез левой ноги", Character.OrganSlot.SlotType.LeftLeg));

            // Unknown race humanoid with cybernetic body
            tempContainer = new Character(
                name: "Терминатор", 
                gender: Character.GenderType.Male,
                race: Character.RaceType.Undefined,
                description: "Прислан Скайнет из будущего чтобы остановить ролёвку про киберпанк, зашедшую слишком далеко.");
            tempContainer.InstallAugmentToOrganSlot(new Augment("Процессор Intel Core i9 серии X", Character.OrganSlot.SlotType.Brain));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Мифриловое покрытие", Character.OrganSlot.SlotType.Body));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Гидравлический пресс «ORA-HAN-D»", Character.OrganSlot.SlotType.LeftArm));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Отбойный молоток класса WK", Character.OrganSlot.SlotType.RightArm));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Валенок левый", Character.OrganSlot.SlotType.LeftLeg));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Костыль", Character.OrganSlot.SlotType.RightLeg));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Насос из колодца на даче", Character.OrganSlot.SlotType.Heart));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Замкнутая система жизнеобеспечения БИОС-3", Character.OrganSlot.SlotType.Breath));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Неизвестно что это, но по документации должно быть здесь", Character.OrganSlot.SlotType.Spleen));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Печень, распечатанная на 3D-принтере", Character.OrganSlot.SlotType.Liver));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Топливный бак от девятки", Character.OrganSlot.SlotType.Stomach));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Водопроводная система из сантехнического ларька", Character.OrganSlot.SlotType.Intestines));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Почки от дуба", Character.OrganSlot.SlotType.Kidneys));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Пылевой мешок от пылесоса", Character.OrganSlot.SlotType.Bladder));
            tempContainer.InstallAugmentToOrganSlot(new Augment("Система клонирвоания Grineer", Character.OrganSlot.SlotType.Reproduction));
        }
        #endif
    }
}