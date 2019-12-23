using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using LARP.Science.Operation;
using AsdolgTools;

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

            #region Choose between release or debug interface and database interaction
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

#if DEBUG
            DEBUG_TestDefaultOperation.Visibility = Visibility.Visible;
#else
            DEBUG_btnSerializeTest.Visibility = Visibility.Collapsed;
            DEBUG_btnDeserializeTest.Visibility = Visibility.Collapsed;
            DEBUG_btnClearCharacters.Visibility = Visibility.Collapsed;
            DEBUG_btnCreateTestingDB.Visibility = Visibility.Collapsed;
            DEBUG_TestDefaultOperation.Visibility = Visibility.Collapsed;
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

        #region Navigation
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
            if (datagridPatientsList.SelectedItem is Database.Character character)
            {
                SwitchHealOperationMode(false);
                Database.Controller.SelectedCharacter = character;
                DisplayCharacter(character);
                SwitchPage(tabPatientView);
            };
        }
        #endregion

        #region Debug buttons in Patients tab
        #if DEBUG
        private void Debug_SerializerTest(object sender, RoutedEventArgs e)
        {
        
            // Write objects data into MemoryStream
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Database.Character>));
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, Database.Controller.GetCharacters());

            // Read written data from MemoryStream and write into a file
            stream.Position = 0;
            var streamRead = new StreamReader(stream);
            File.WriteAllText("serializer_test.json", streamRead.ReadToEnd());

            // Dispose streams
            stream.Dispose();
            streamRead.Dispose();

        }   
        private void Debug_DeserializerTest(object sender, RoutedEventArgs e)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Database.Character>));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes("serializer_test.json")) { Position = 0 };
            Database.Controller.SetCharactersDatabase(serializer.ReadObject(stream) as List<Database.Character>);
            UpdateDisplayedTables();
        }
        private void Debug_ClearCharacters(object sender, RoutedEventArgs e)
        {
            Database.Controller.SetCharactersDatabase(new List<Database.Character>());
            UpdateDisplayedTables();
        }
        private void Debug_ReinitializeTestingDatabase(object sender, RoutedEventArgs e)
        {
            Database.Controller.CreateTestDatabase();
            UpdateDisplayedTables();
        }
        private async void Debug_TestDefaultOperation(object sender, RoutedEventArgs e)
        {
            Minesweeper.MinesweeperWindow sweeper = new Minesweeper.MinesweeperWindow(new int[] { 5, 10, 15 } );
            sweeper.ShowDialog();
            MessageBox.Show("Operation quality: " + await sweeper.Execute());

            UpdateDisplayedTables();
        }
#else
        private void Debug_SerializerTest(object sender, RoutedEventArgs e) { }
        private void Debug_DeserializerTest(object sender, RoutedEventArgs e) { }
        private void Debug_ClearCharacters(object sender, RoutedEventArgs e) { }
        private void Debug_ReinitializeTestingDatabase(object sender, RoutedEventArgs e) { }
        private void Debug_TestDefaultOperation(object sender, RoutedEventArgs e) { }
#endif
        #endregion

        #region Patient View logic 

        private bool isInWoundInputMode = false;
        private List<HealingDetails.Wound> wounds;

        #region Patient View - Displayer
        private void DisplayCharacter(Database.Character character)
        {
            foreach (Database.Organ organ in character.GetOrgansList())
                GetImageEntity(organ.Slot).Source = organ.GetImageEntity();

            textblockPatientName.Text = character.Name;
            textblockPatientRace.Text = character.GetRace;
            textblockPatientGender.Text = character.GetGender;
            textblockPatientDescr.Text = character.Description;

            rectanglePatientOrgan.Visibility = Visibility.Visible;
            rectanglePatientOrganAug.Visibility = Visibility.Visible;
            rectanglePatientAuxDetails.Visibility = Visibility.Visible;
        }
        #endregion
        #region Patient View - Organ Click handler
        private void OrganClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Database.Organ organ = Database.Controller.SelectedCharacter.GetOrgan(GetSlotByImageEntity(sender as Image));
            textblockPatientOrganName.Text = organ.Name;
            textblockPatientOrganDescription.Text = organ.Description;
            if (organ.IsAugmented())
            {
                Database.Augment aug = organ.AugmentEquivalent;
                textblockPatientOrganAugName.Text = aug.Name;
                textblockPatientOrganAugDescription.Text = aug.Description;
                textblockPatientOrganAugSlotType.Text = organ.Name;
                datagridPatientOrganAugParams.Items.Clear();
                foreach (KeyValuePair<string, string> pair in aug.GetAllCustomParameters() ?? new Dictionary<string, string>())
                    datagridPatientOrganAugParams.Items.Add(pair);

                textblockPatientOrganAugmentNotifier.Visibility = Visibility.Visible;
                rectanglePatientOrganAug.Visibility = Visibility.Collapsed;
            }
            else
            {
                textblockPatientOrganAugmentNotifier.Visibility = Visibility.Collapsed;
                rectanglePatientOrganAug.Visibility = Visibility.Visible;
            }
            rectanglePatientOrgan.Visibility = Visibility.Collapsed;

            if (isInWoundInputMode) try 
            { 
                wounds.Add(new HealingDetails.Wound(organ, WoundSelection.SelectWound()));
                UpdateDataGridPatientViewHealOperationData();
                ButtonPatientViewHealOperationBegin.IsEnabled = true;
            } catch (OperationCanceledException) { }
        }
        #endregion
        #region Patient View - Animation handlers
        private static readonly System.Windows.Media.Effects.DropShadowEffect hoverEffect = new System.Windows.Media.Effects.DropShadowEffect()
        {
            Color = System.Windows.Media.Color.FromRgb(25, 255, 0),
            ShadowDepth = 7
        };

        private void EnableDropShadow(Image image) => image.Effect = hoverEffect;
        private void ClearEffect(Image image) => image.Effect = null;

        private void PatientImageMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => EnableDropShadow(sender as Image);
        private void PatientImageMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) => ClearEffect(sender as Image);
#endregion
        #region Patient View - Operation Logic
        private void ButtonHeal_Click(object sender, RoutedEventArgs e)
        {
            if (!isInWoundInputMode)
                if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Для начала операции необходимо провести обследование самостоятельно и указать, куда и какие повреждения были нанесены пациенту. Для указания нажмите на часть тела.", "", "Понятно", "Отмена")
                    == MessageBoxResult.Yes)
                    SwitchHealOperationMode(true);
        }

        private void ButtonPatientViewHealOperationRemoveWound_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridPatientViewHealOperationWounds.SelectedItem is HealingDetails.Wound)
            {
                wounds.RemoveAt(DataGridPatientViewHealOperationWounds.SelectedIndex);
                if (wounds.Count == 0) ButtonPatientViewHealOperationBegin.IsEnabled = false;
            }
            UpdateDataGridPatientViewHealOperationData();
        }

        private void ButtonPatientViewHealOperationReplaceOrgan_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonPatientViewHealOperationBegin_Click(object sender, RoutedEventArgs e)
        {
            List<HealingDetails.DamageType> damages = new List<HealingDetails.DamageType>();
            foreach (var wound in wounds)
                damages.Add(wound.Damage);
            HealingDetails operation = new HealingDetails(damages);
            operation.Execute();
        }

        private void DataGridPatientViewHealOperationWounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ButtonPatientViewHealOperationReplaceOrgan.IsEnabled = ButtonPatientViewHealOperationRemoveWound.IsEnabled = e.AddedItems.Count > 0;

        private void ButtonPatientViewHealOperationCancel_Click(object sender, RoutedEventArgs e)
        {
            if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Отменить подготовку к операции?", "", "Да", "Нет") == MessageBoxResult.Yes)
                SwitchHealOperationMode(false);
        }

        private void UpdateDataGridPatientViewHealOperationData()
        {
            DataGridPatientViewHealOperationWounds.Items.Clear();
            for (int i = 0; (wounds != null && i < wounds.Count); i++)
                DataGridPatientViewHealOperationWounds.Items.Add(wounds[i]);
        }

        private void SwitchHealOperationMode(bool isEnabled)
        {
            if (isEnabled)
            {
                wounds = new List<HealingDetails.Wound>();
                groupBoxPrepareHealOperation.Visibility = Visibility.Visible;
                groupBoxSelectOperationType.Visibility = Visibility.Collapsed;
                ButtonPatientViewHealOperationBegin.IsEnabled = false;
                ButtonPatientViewHealOperationReplaceOrgan.IsEnabled = false;
                ButtonPatientViewHealOperationRemoveWound.IsEnabled = false;
                isInWoundInputMode = true;
            }
            else {
                wounds = null;
                groupBoxPrepareHealOperation.Visibility = Visibility.Collapsed;
                groupBoxSelectOperationType.Visibility = Visibility.Visible;
                isInWoundInputMode = false;
            }
            UpdateDataGridPatientViewHealOperationData();
        }
        #endregion

        #region Patient View - Character UI elements
                private Image GetImageEntity(Database.Character.BodyPartSlot.SlotType? slot)
                {
                    switch (slot)
                    {
                        case Database.Character.BodyPartSlot.SlotType.Brain: return imgPatientViewBrain;
                        case Database.Character.BodyPartSlot.SlotType.Heart: return imgPatientViewHeart;
                        case Database.Character.BodyPartSlot.SlotType.Breath: return imgPatientViewBreath;
                        case Database.Character.BodyPartSlot.SlotType.Head: return imgPatientViewHead;
                        case Database.Character.BodyPartSlot.SlotType.Body: return imgPatientViewBody;
                        case Database.Character.BodyPartSlot.SlotType.LeftArm: return imgPatientViewArmLeft;
                        case Database.Character.BodyPartSlot.SlotType.RightArm: return imgPatientViewArmRight;
                        case Database.Character.BodyPartSlot.SlotType.LeftLeg: return imgPatientViewLegLeft;
                        case Database.Character.BodyPartSlot.SlotType.RightLeg: return imgPatientViewLegRight;
                        case Database.Character.BodyPartSlot.SlotType.Spleen: return imgPatientViewSpleen;
                        case Database.Character.BodyPartSlot.SlotType.Liver: return imgPatientViewLiver;
                        case Database.Character.BodyPartSlot.SlotType.Stomach: return imgPatientViewStomach;
                        case Database.Character.BodyPartSlot.SlotType.Kidneys: return imgPatientViewKidneys;
                        case Database.Character.BodyPartSlot.SlotType.Intestines: return imgPatientViewIntestines;
                        case Database.Character.BodyPartSlot.SlotType.Bladder: return imgPatientViewBladder;
                        case Database.Character.BodyPartSlot.SlotType.Reproduction: return imgPatientViewReproduction;
                        default: throw new ArgumentException("Ошибка: передан пустой слот.", "slot");
                    }
                }
                private Database.Character.BodyPartSlot.SlotType? GetSlotByImageEntity(Image image)
                {
                    if (image.Equals(imgPatientViewArmLeft)) return Database.Character.BodyPartSlot.SlotType.LeftArm;
                    else if (image.Equals(imgPatientViewArmRight)) return Database.Character.BodyPartSlot.SlotType.RightArm;
                    else if (image.Equals(imgPatientViewBody)) return Database.Character.BodyPartSlot.SlotType.Body;
                    else if (image.Equals(imgPatientViewLegLeft)) return Database.Character.BodyPartSlot.SlotType.LeftLeg;
                    else if (image.Equals(imgPatientViewLegRight)) return Database.Character.BodyPartSlot.SlotType.RightLeg;
                    else if (image.Equals(imgPatientViewHead)) return Database.Character.BodyPartSlot.SlotType.Head;
                    else if (image.Equals(imgPatientViewHeart)) return Database.Character.BodyPartSlot.SlotType.Heart;
                    else if (image.Equals(imgPatientViewBladder)) return Database.Character.BodyPartSlot.SlotType.Bladder;
                    else if (image.Equals(imgPatientViewBrain)) return Database.Character.BodyPartSlot.SlotType.Brain;
                    else if (image.Equals(imgPatientViewBreath)) return Database.Character.BodyPartSlot.SlotType.Breath;
                    else if (image.Equals(imgPatientViewIntestines)) return Database.Character.BodyPartSlot.SlotType.Intestines;
                    else if (image.Equals(imgPatientViewKidneys)) return Database.Character.BodyPartSlot.SlotType.Kidneys;
                    else if (image.Equals(imgPatientViewLiver)) return Database.Character.BodyPartSlot.SlotType.Liver;
                    else if (image.Equals(imgPatientViewReproduction)) return Database.Character.BodyPartSlot.SlotType.Reproduction;
                    else if (image.Equals(imgPatientViewSpleen)) return Database.Character.BodyPartSlot.SlotType.Spleen;
                    else if (image.Equals(imgPatientViewStomach)) return Database.Character.BodyPartSlot.SlotType.Stomach;
                    else return null;
                }
        #endregion

        #endregion
    }
}