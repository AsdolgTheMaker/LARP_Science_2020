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

#if DEBUG // Это нам в релизе не потребуется. Я на это очень надеюсь.
        public static void CreateTestDatabase()
        {
            Characters = new List<Character>();

            // Человек со стандартным набором органов
            new Character(name: "Обычный Джо", description: "Это самый обычный Джо, которого только можно вообразить.");

            // Человек с протезом левой ноги
            new Character(name: "Одноногий Бача", description: "Бача с рождения служил на флоте, а ногу потерял после неудачного знакомства с ролевиками.",
                organs: new List<Organ>()
                {
                    Organ.Presets["Кожа"],
                    Organ.Presets["Мозг"],
                    Organ.Presets["Левый глаз"], Organ.Presets["Правый глаз"],
                    Organ.Presets["Левая рука"], Organ.Presets["Правая рука"],
                    Organ.Presets["Сердце"],
                    Organ.Presets["Левое легкое"], Organ.Presets["Правое легкое"],
                    new Organ("Левая нога", Organ.PresetDescriptions["Нога"],
                        new Augment("Протез левой ноги", "Протез собран из кусков высокотехнологичного мусора, которым изобилуют нижние уровни Корусанта.")),
                    Organ.Presets["Правая нога"]
                });

            // Гуманоид неизвестной расы с кибернетическим телом.
            new Character(name: "Терминатор", description: "Прислан Скайнет из будущего чтобы остановить ролёвку про киберпанк, зашедшую слишком далеко.", race: Character.RaceType.Undefined,
                organs: new List<Organ>()
                {
                    new Organ("Кожа", Organ.PresetDescriptions["Кожа"],
                        new Augment("Мифриловый покров", "Практически неотличим от настоящей кожи. Нижние слои покрова выкованы древнейшими гномами-мастерами.")),
                    new Organ("Мозг", Organ.PresetDescriptions["Мозг"],
                        new Augment("Процессор Intel® Core™ i9-9820X серии X", "В семействе процессоров Intel® Core™ серии X разблокирован множитель для обеспечения дополнительного запаса производительности. Среди новых функций: возможность оверклокинга каждого ядра в отдельности, управление коэффициентом AVX для повышения стабильности, а также управление напряжением VccU в экстремальных сценариях. В сочетании с такими инструментами, как Intel® Extreme Tuning Utility (Intel® XTU) и Intel® Extreme Memory Profile (Intel® XMP) вы получаете мощный набор для достижения максимальной производительности.")),
                    new Organ("Левый глаз", Organ.PresetDescriptions["Глаз"], align: Organ.BodyAlign.Left,
                        augment: new Augment("Камера от iPhone 11", "В iPhone 11 используется новая система двух камер — широкоугольной и сверхширокоугольной. Благодаря тесной интеграции с iOS 13 она позволяет снимать видео высочайшего качества и значительно расширяет функциональность, доступную при фотосъёмке." +
                        "\nВидеозаписи, снятые на камеру iPhone 11, отличаются удивительной чёткостью. Кроме того, обе камеры поддерживают съёмку видео 4K с расширенным динамическим диапазоном, хорошо проработанными светлыми участками и кинематографической стабилизацией видео. Сверхширокоугольная камера с более широким углом обзора и большой фокусной плоскостью отлично подходит для съёмки сцен с движением." +
                        "\nПереключаться между двумя камерами очень легко, а функция аудиозума сопоставляет источник звука с тем, что вы видите в кадре, приглушая посторонние шумы. В iOS 13 каждому доступны мощные инструменты редактирования видео. Новый интерфейс приложения «Камера» позволяет поворачивать и обрезать кадр, увеличивать экспозицию и мгновенно применять фильтры.")),
                    new Organ("Правый глаз", Organ.PresetDescriptions["Глаз"], align: Organ.BodyAlign.Right,
                        augment: new Augment("Окуляр маски Корво Аттано", "Система тройного окуляра маски Корво из Dishonored позволяет рассматривать даже сильно отдалённые объекты.")),
                    new Organ("Сердце", Organ.PresetDescriptions["Сердце"], align: Organ.BodyAlign.Middle,
                        augment: new Augment("Водяной насос из OBI", "Насос повышенной мощности, приобретенный в OBI в секции дачной техники.")),
                    new Organ("Левая рука", Organ.PresetDescriptions["Рука"], align: Organ.BodyAlign.Left,
                        augment: new Augment("Рука Супер Тенген Топпа Гуррен-Лаганна", "Рука самого громадного существа за всю история кинематографа. Как это вообще присобачено к телу?")),
                    new Organ("Правая рука", Organ.PresetDescriptions["Рука"], align: Organ.BodyAlign.Right,
                        augment: new Augment("Протез в виде Стар Платинума", "Выплавленный из сплава стали, мифрила, спиральной энергии, хренулима и ещё бог знает чего, этот протез воплощает секретную технику легендарного джедая Джотаро Куджо. Может действовать без участия воли владельца.")),
                    new Organ("Левая нога", Organ.PresetDescriptions["Нога"], align: Organ.BodyAlign.Left,
                        augment: new Augment("Складной миниган", "Это миниган, который складывается пополам (типа коленный сустав). Т.е. тут просто миниган вместо ноги, вообще ничего интересного.")),
                    new Organ("Правая нога", Organ.PresetDescriptions["Нога"], align: Organ.BodyAlign.Right,
                        augment: new Augment("Боль программиста", "Протез, выглядящий как плавающие в воздухе нули и единицы. Если поднести к ним ухо, можно услышать плач программиста."))
                });
        }
#endif
    }
}