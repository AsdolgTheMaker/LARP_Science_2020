using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace LARP.Science
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Database.Controller.Initialize(patientsList: datagridPatientsList);

            // Hide tabs
            foreach (TabItem tab in tabctrlRootNavigator.Items)
                tab.Visibility = Visibility.Collapsed;

            #region // Choose the way database will be read or written depending on current config
#if DBWRITE 
            Database.Controller.CreateTestDatabase();
#elif DBREAD
            if (File.Exists(Database.Controller.CharactersDatabaseFile))
            { // read database from file
                Database.Controller.ReadCharacters();
            }
            else
            {
                Database.Controller.CreateTestDatabase();
                Database.Controller.SaveCharacters();
            }
#else // If it's RELEASE config - <TODO>
            
#endif

#if !DEBUG
            DEBUG_btnSerializeTest.Visibility = Visibility.Collapsed;
            DEBUG_btnDeserializeTest.Visibility = Visibility.Collapsed;
            DEBUG_btnClearCharacters.Visibility = Visibility.Collapsed;
#endif
            #endregion
            
            UpdateDisplayedTables();
        }

        // Data updater
        private void UpdateDisplayedTables()
        {
            datagridPatientsList.Items.Clear();
            foreach (Database.Character item in Database.Controller.GetCharacters())
                datagridPatientsList.Items.Add(item);
            Database.Controller.SaveCharacters();
        }

        #region // Navigation
        public void SwitchPage(TabItem target) 
        {
            UpdateDisplayedTables();
            tabctrlRootNavigator.SelectedItem = target; 
        }
        private void BtnNavMenuPatients_Click(object sender, RoutedEventArgs e) => SwitchPage(tabPatients);
        private void BtnNavMenuJournal_Click(object sender, RoutedEventArgs e) => SwitchPage(tabJournal);
        private void BtnNavMenuStorage_Click(object sender, RoutedEventArgs e) => SwitchPage(tabStorage);
        private void BtnNavMenuEconomics_Click(object sender, RoutedEventArgs e) => SwitchPage(tabEconomics);
        private void BtnNavMenuOther_Click(object sender, RoutedEventArgs e) => SwitchPage(tabEconomics);
        private void BtnBackToNavMenu_Click(object sender, RoutedEventArgs e) => SwitchPage(tabNavigation);
        private void BtnNavViewPatients_Click(object sender, RoutedEventArgs e)
        {
            Database.Character character = datagridPatientsList.SelectedItem as Database.Character;
            if (character != null)
            { 
                DisplayOrgans(character);
                SwitchPage(tabPatientView);
            };
        }
        #endregion

        #region // Debug buttons in Patients tab (OnClick implementations)
        private void Debug_SerializerTest(object sender, RoutedEventArgs e)
        {
#if DEBUG
            // Write objects data into MemoryStream
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Database.Character>));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, Database.Controller.GetCharacters());

            // Read written data from MemoryStream and write into a file
            stream.Position = 0;
            var streamRead = new StreamReader(stream);
            File.WriteAllText("serializer_test.json", streamRead.ReadToEnd());

            System.Diagnostics.Process.Start("explorer.exe", "-select " + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + "serializer_test.json");

            // Dispose streams
            stream.Dispose();
            streamRead.Dispose();
#endif
        }
        private void Debug_DeserializerTest(object sender, RoutedEventArgs e)
        {
#if DEBUG
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Database.Character>));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes("serializer_test.json"));
            stream.Position = 0;
            Database.Controller.SetCharactersDatabase(serializer.ReadObject(stream) as List<Database.Character>);
            UpdateDisplayedTables();
#endif
        }
        private void Debug_ClearCharacters(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Database.Controller.SetCharactersDatabase(new List<Database.Character>());
            UpdateDisplayedTables();
#endif
        }
        private void Debug_ReinitializeTestingDatabase(object sender, RoutedEventArgs e)
        {
            Database.Controller.CreateTestDatabase();
            UpdateDisplayedTables();
        }
        #endregion

        // Image displayer block
        private void DisplayOrgans(Database.Character character)
        {
            foreach (Database.Organ organ in character.GetOrgansList())
            {
                Image image = GetImageEntity(organ.Slot);
                var imgSource = organ.GetImageSource();
                image.Source = imgSource;
            }
        }

        private Image GetImageEntity(Database.Character.OrganSlot.SlotType slot)
        {
            switch (slot)
            {
                case Database.Character.OrganSlot.SlotType.Brain: return imgPatientViewBrain;
                case Database.Character.OrganSlot.SlotType.Heart: return imgPatientViewHeart;
                case Database.Character.OrganSlot.SlotType.Breath: return imgPatientViewBreath;
                case Database.Character.OrganSlot.SlotType.Head: return imgPatientViewHead;
                case Database.Character.OrganSlot.SlotType.Body: return imgPatientViewBody;
                case Database.Character.OrganSlot.SlotType.LeftArm: return imgPatientViewArmLeft;
                case Database.Character.OrganSlot.SlotType.RightArm: return imgPatientViewArmRight;
                case Database.Character.OrganSlot.SlotType.LeftLeg: return imgPatientViewLegLeft;
                case Database.Character.OrganSlot.SlotType.RightLeg: return imgPatientViewLegRight;
                case Database.Character.OrganSlot.SlotType.Spleen: return imgPatientViewSpleen;
                case Database.Character.OrganSlot.SlotType.Liver: return imgPatientViewLiver;
                case Database.Character.OrganSlot.SlotType.Stomach: return imgPatientViewStomach;
                case Database.Character.OrganSlot.SlotType.Kidneys: return imgPatientViewKidneys;
                case Database.Character.OrganSlot.SlotType.Intestines: return imgPatientViewIntestines;
                case Database.Character.OrganSlot.SlotType.Bladder: return imgPatientViewBladder;
                case Database.Character.OrganSlot.SlotType.Reproduction: return imgPatientViewReproduction;
                default: throw new ArgumentException("Ошибка: аргумент передан не с типом SlotType.", "slot");
            }
        }
    }
}