using LARP.Science.Operation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Windows;
using System.Windows.Controls;

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
            Database.Controller.Initialize();
            Database.Controller.LogOutputDuringOperation = TextBoxPatientViewJournal;

            // Hide tabs
            foreach (TabItem tab in tabctrlRootNavigator.Items)
                tab.Visibility = Visibility.Collapsed;

            #region Database creators for debug configs.
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
#endif

#if DEBUG
            //DEBUG_TestDefaultOperation.Visibility = Visibility.Visible;
#else
            DEBUG_btnSerializeTest.Visibility = Visibility.Collapsed;
            DEBUG_btnDeserializeTest.Visibility = Visibility.Collapsed;
            DEBUG_btnClearCharacters.Visibility = Visibility.Collapsed;
            DEBUG_btnCreateTestingDB.Visibility = Visibility.Collapsed;
            DEBUG_TestDefaultOperation.Visibility = Visibility.Collapsed;
#endif
            #endregion
        }

        #region Displayed data updaters
        private void UpdateDisplayedTables()
        {
            UpdatePatientsTable();
            UpdateStorageTable();
            UpdateJournalBox();
        }

        private void UpdatePatientsTable()
        {
            Database.Controller.ReadCharacters();
            datagridPatientsList.Items.Clear();
            foreach (Database.Character item in Database.Controller.GetCharacters())
                datagridPatientsList.Items.Add(item);
        }

        private async void UpdateStorageTable()
        {
            datagridStorage.Items.Clear();
            foreach (Database.BodyPart item in await Economics.Exchange.GetUserItems(0))
                datagridStorage.Items.Add(item);
        }

        private void UpdateJournalBox()
        {
            TextBoxJournal.Text = string.Empty;
            string[] output = Database.Journal.GetOutput;
            for (int i = 0; i < output.Length; i++)
                TextBoxJournal.Text += output[i] + "\n";
        }
        #endregion

        #region Navigation
        public void SwitchPage(TabItem target) => tabctrlRootNavigator.SelectedItem = target;
        private void BtnNavMenuPatients_Click(object sender, RoutedEventArgs e)
        {
            UpdatePatientsTable();
            SwitchPage(tabPatients);
        }
        private void BtnNavMenuJournal_Click(object sender, RoutedEventArgs e)
        {
            UpdateJournalBox();
            SwitchPage(tabJournal);
        }
        private void BtnNavMenuStorage_Click(object sender, RoutedEventArgs e)
        {
            SwitchPage(tabStorage);
            UpdateStorageTable();
        }
        private void BtnNavMenuEconomics_Click(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start("https://rpg.x-serv.ru/");
        private void BtnNavViewPatients_Click(object sender, RoutedEventArgs e)
        {
            if (datagridPatientsList.SelectedItem is Database.Character character)
            {
                Database.Controller.SelectedCharacter = character;
                SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
                DisplayCharacter(character);
                SwitchPage(tabPatientView);
            };
        }
        private void BtnBackToNavMenu_Click(object sender, RoutedEventArgs e) => SwitchPage(tabNavigation);
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
            UpdatePatientsTable();
        }
        private void Debug_ClearCharacters(object sender, RoutedEventArgs e)
        {
            Database.Controller.SetCharactersDatabase(new List<Database.Character>());
            Database.Controller.SaveCharacters();
            UpdatePatientsTable();
        }
        private void Debug_ReinitializeTestingDatabase(object sender, RoutedEventArgs e)
        {
            Database.Controller.CreateTestDatabase();
            Database.Controller.SaveCharacters();
            UpdatePatientsTable();
        }
        private async void Debug_TestDefaultOperation(object sender, RoutedEventArgs e)
        {
            Minesweeper.MinesweeperWindow sweeper = new Minesweeper.MinesweeperWindow(new int[] { 5, 10, 15 });
            sweeper.ShowDialog();
            MessageBox.Show("Operation quality: " + await sweeper.Execute());
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

        private enum OrganClickAction
        {
            DisplayOrganInfo,
            WoundInput,
            RemoveOrgan,
            ReplaceOrgan,
            AugmentateOrgan
        }
        private OrganClickAction CurrentOrganClickAction = OrganClickAction.DisplayOrganInfo;
        private List<HealingDetails.Wound> wounds;
        Database.Organ selectedOrgan;

        #region Patient View - Displayer
        private void DisplayCharacter() => DisplayCharacter(Database.Controller.SelectedCharacter);
        private void DisplayCharacter(Database.Character character)
        {
            foreach (Database.Organ organ in character.OrgansList)
                GetImageEntity(organ.Slot).Source = organ.GetImage();

            textblockPatientName.Text = character.Name;
            textblockPatientRace.Text = character.GetRace;
            textblockPatientGender.Text = character.GetGender;
            textblockPatientDescr.Text = character.Description;

            rectanglePatientOrgan.Visibility = Visibility.Visible;
            rectanglePatientOrganAug.Visibility = Visibility.Visible;
            rectanglePatientAuxilaryAugs.Visibility = Visibility.Collapsed;
            rectanglePatientAuxilaryAugParams.Visibility = Visibility.Visible;

            if (character.Alive)
            {
                imgPatientViewDead.Visibility = Visibility.Collapsed;
                if (CurrentOrganClickAction == OrganClickAction.DisplayOrganInfo) groupBoxSelectOperationType.Visibility = Visibility.Visible;
                buttonHeal.Visibility = Visibility.Visible;
            }
            else
            {
                imgPatientViewDead.Visibility = Visibility.Visible;
                groupBoxPrepareHealOperation.Visibility = Visibility.Collapsed;
                buttonHeal.Visibility = Visibility.Collapsed;
            }

            Database.Controller.SaveCharacters();
        }
        #endregion
        #region Patient View - Organ Click handler
        private async void OrganClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Database.Organ organ = Database.Controller.SelectedCharacter.GetOrgan(GetSlotByImageEntity(sender as Image));
            selectedOrgan = organ;
            try
            {
                switch (CurrentOrganClickAction)
                {
                    case OrganClickAction.DisplayOrganInfo:
                        textblockPatientOrganName.Text = organ.Name;
                        textblockPatientOrganDescription.Text = organ.Description;

                        // If there is no organ, we should be able to install it
                        if (organ.Virtual) ButtonPatientViewInstallOrgan.Visibility = Visibility.Visible;
                        else ButtonPatientViewInstallOrgan.Visibility = Visibility.Collapsed;

                        if (organ.IsAugmented)
                        {
                            Database.Augment aug = organ.AugmentEquivalent;
                            textblockPatientOrganAugName.Text = aug.Name;
                            textblockPatientOrganAugDescription.Text = aug.Description;
                            textblockPatientOrganAugSlotType.Text = organ.Name;
                            datagridPatientOrganAugParams.Items.Clear();
                            foreach (KeyValuePair<string, string> pair in aug.AllCustomParameters ?? new Dictionary<string, string>())
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
                        break;
                    case OrganClickAction.WoundInput:
                        wounds.Add(new HealingDetails.Wound(organ, WoundSelection.SelectWound()));
                        UpdateDataGridPatientViewHealOperationData();
                        ButtonPatientViewHealOperationBegin.IsEnabled = true;
                        break;
                    case OrganClickAction.RemoveOrgan:
                        if (organ.Virtual)
                            WPFCustomMessageBox.CustomMessageBox.ShowOK("Здесь нет органа.", "", "ОК");
                        else if (organ.IsAugmented)
                        {
                            if (organ.AugmentEquivalent.IsReplacement)
                            {
                                if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Удалить выбранный протез?", "", "Да", "Нет") == MessageBoxResult.Yes)
                                {
                                    new AugmentationDetails(AugmentationType.Primary, AugmentationAction.Remove, _target: organ).Execute();
                                    SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
                                }
                                else WPFCustomMessageBox.CustomMessageBox.ShowOK("Нельзя удалить аугментированный орган. Сначала удалите аугмент.", "", "Понятно");
                            }
                            DisplayCharacter(Database.Controller.SelectedCharacter);
                        }
                        else if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Удалить выбранный орган?", "", "Да", "Нет") == MessageBoxResult.Yes)
                        {
                            new AugmentationDetails(AugmentationType.Organ, AugmentationAction.Remove, _target: organ).Execute();
                            DisplayCharacter();
                            SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
                        }
                        break;
                    case OrganClickAction.AugmentateOrgan:
                        if (organ.Virtual) // No organ
                            WPFCustomMessageBox.CustomMessageBox.ShowOK("Здесь нет органа.", "", "ОК");
                        else if (organ.IsAugmented) // Either augmented or replaced organ
                        {
                            if (organ.AugmentEquivalent.IsReplacement)
                                WPFCustomMessageBox.CustomMessageBox.ShowOK("Протез извлечь нельзя. Необходимо воспользоваться операцией удаления органа.", "", "Понятно");
                            else if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Извлечь аугмент из органа?", "", "Да", "Назад") == MessageBoxResult.Yes)
                            {
                                new AugmentationDetails(AugmentationType.Primary, AugmentationAction.Remove, _target: organ).Execute();
                                DisplayCharacter();
                                SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
                            }
                        }
                        else // Just an organ
                        {
                            if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Выберите аугмент для установки.", "", "ОК", "Отменить")
                                == MessageBoxResult.Yes)
                            {
                                AugmentRequest augWindow = await AugmentRequest.CreateInstance(1, selectedOrgan.Slot);
                                bool result = augWindow.ShowDialog().GetValueOrDefault(false);
                                if (result) new AugmentationDetails(AugmentationType.Primary, AugmentationAction.Install, _implant: augWindow.Selection).Execute();
                                else WPFCustomMessageBox.CustomMessageBox.ShowOK("Установка аугмента отменена.", "", "ОК");
                                DisplayCharacter();
                            }
                            SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
                        }
                        break;
                    case OrganClickAction.ReplaceOrgan:
                        break;
                }
            }
            catch (OperationCanceledException) { }
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
            if (Database.Controller.SelectedCharacter.Alive)
            {
                if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Для начала операции необходимо провести обследование самостоятельно и указать, куда и какие повреждения были нанесены пациенту. Для указания нажмите на часть тела.", "", "Понятно", "Отмена")
                        == MessageBoxResult.Yes)
                    SwitchOperationMode(OrganClickAction.WoundInput);
            }
            else WPFCustomMessageBox.CustomMessageBox.ShowOK("Мёртвого уже не вылечить.", "", "Понятно");
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
            throw new NotImplementedException("Требуется реализация класса Storage.");
        }

        private void ButtonPatientViewHealOperationBegin_Click(object sender, RoutedEventArgs e)
        {
            List<HealingDetails.DamageType> damages = new List<HealingDetails.DamageType>();
            foreach (var wound in wounds)
                damages.Add(wound.Damage);
            HealingDetails operation = new HealingDetails(damages);
            operation.Execute();

            

            SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
        }

        private void DataGridPatientViewHealOperationWounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ButtonPatientViewHealOperationReplaceOrgan.IsEnabled = ButtonPatientViewHealOperationRemoveWound.IsEnabled = e.AddedItems.Count > 0;

        private void ButtonPatientViewHealOperationCancel_Click(object sender, RoutedEventArgs e)
        {
            if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Отменить подготовку к операции?", "", "Да", "Нет") == MessageBoxResult.Yes)
                SwitchOperationMode(OrganClickAction.DisplayOrganInfo);
        }

        private void UpdateDataGridPatientViewHealOperationData()
        {
            DataGridPatientViewHealOperationWounds.Items.Clear();
            for (int i = 0; wounds != null && i < wounds.Count; i++)
                DataGridPatientViewHealOperationWounds.Items.Add(wounds[i]);
        }
        #endregion
        #region Patient View - Augmentation buttons
        private void ButtonRemoveOrgan_Click(object sender, RoutedEventArgs e) => SwitchOperationMode(OrganClickAction.RemoveOrgan);
        
        private void ButtonAugmentateOrgan_Click(object sender, RoutedEventArgs e) => SwitchOperationMode(OrganClickAction.AugmentateOrgan);

        private void ButtonCancelGenericOperation_Click(object sender, RoutedEventArgs e) => SwitchOperationMode(OrganClickAction.DisplayOrganInfo);

        private async void ButtonPatientViewInstallOrgan_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = WPFCustomMessageBox.CustomMessageBox.ShowYesNoCancel("Провести установку органа или протеза?", "", "Орган", "Протез", "Отменить установку");
            switch (result)
            {
                case MessageBoxResult.Yes: // Install organ
                    List<Database.Organ> organsList = (await Economics.Exchange.GetUserItems(1, selectedOrgan.Slot)).Cast<Database.Organ>().ToList();
                    if (organsList.Count <= 0)
                    {
                        if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("К сожалению, на складе нет подходящих органов. Просмотреть протезы?", "", "Да", "Нет, отменить")
                            == MessageBoxResult.Yes)
                            goto case MessageBoxResult.No;
                    }
                    else
                    {
                        if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("Есть подходящий орган. Провести установку?", "", "Да", "Нет, отменить")
                            == MessageBoxResult.Yes)
                        {
                            AugmentationDetails augmentation = new AugmentationDetails(AugmentationType.Organ, AugmentationAction.Install, _implant: organsList[0]);
                            augmentation.Execute();
                        }
                    }
                    break;
                case MessageBoxResult.No: // Install augment
                    AugmentRequest augWindow = await AugmentRequest.CreateInstance(1, selectedOrgan.Slot);
                    bool requestResult = augWindow.ShowDialog().GetValueOrDefault(false);
                    if (requestResult)
                    {
                        AugmentationDetails augmentation = new AugmentationDetails(AugmentationType.Primary, AugmentationAction.Install, _implant: augWindow.Selection);
                        augmentation.Execute();
                    }
                    else WPFCustomMessageBox.CustomMessageBox.ShowOK("Установка протеза отменена.", "", "ОК");
                    break;
            }
        }

        private async void ButtonPatientViewAddImplant_Click(object sender, RoutedEventArgs e)
        {
            AugmentRequest augWindow = await AugmentRequest.CreateInstance(2);
            bool result = augWindow.ShowDialog().GetValueOrDefault(false);
            if (result) new AugmentationDetails(AugmentationType.Auxilary, AugmentationAction.Install, _implant: augWindow.Selection).Execute();
            else WPFCustomMessageBox.CustomMessageBox.ShowOK("Установка импланта отменена.", "", "ОК");
        }

        private void ButtonPatientViewRemoveImplant_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridPatientAuxilaryAugs.Items.Count <= 0)
                WPFCustomMessageBox.CustomMessageBox.ShowOK("Нет имплантов для удаления.", "", "ОК");
            else if (dataGridPatientAuxilaryAugs.SelectedItem == null)
                WPFCustomMessageBox.CustomMessageBox.ShowOK("Сначала выберите имплант из списка.", "", "ОК");
            else new AugmentationDetails(AugmentationType.Auxilary, AugmentationAction.Remove, dataGridPatientAuxilaryAugs.SelectedItem as Database.Augment).Execute();
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

        private void SwitchOperationMode(OrganClickAction action)
        {
            CurrentOrganClickAction = action;
            switch (action)
            {
                case OrganClickAction.WoundInput:
                    wounds = new List<HealingDetails.Wound>();
                    ButtonPatientViewHealOperationBegin.IsEnabled = false;
                    ButtonPatientViewHealOperationReplaceOrgan.IsEnabled = false;
                    ButtonPatientViewHealOperationRemoveWound.IsEnabled = false;

                    groupBoxPrepareHealOperation.Visibility = Visibility.Visible;
                    groupBoxSelectOperationType.Visibility = Visibility.Collapsed;
                    ButtonCancelGenericOperation.Visibility = Visibility.Collapsed;
                    UpdateDataGridPatientViewHealOperationData();
                    break;
                case OrganClickAction.ReplaceOrgan:
                    break;
                case OrganClickAction.RemoveOrgan:
                    WPFCustomMessageBox.CustomMessageBox.ShowOK("Выберите орган для удаления.", "", "Хорошо");
                    ButtonCancelGenericOperation.Visibility = Visibility.Visible;
                    groupBoxSelectOperationType.Visibility = Visibility.Collapsed;
                    break;
                case OrganClickAction.AugmentateOrgan:
                    WPFCustomMessageBox.CustomMessageBox.ShowOK("Выберите орган для аугментации.", "", "Хорошо");
                    ButtonCancelGenericOperation.Visibility = Visibility.Visible;
                    groupBoxSelectOperationType.Visibility = Visibility.Collapsed;
                    break;
                case OrganClickAction.DisplayOrganInfo:
                    wounds = null;
                    groupBoxSelectOperationType.Visibility = Visibility.Visible;
                    groupBoxPrepareHealOperation.Visibility = Visibility.Collapsed;
                    ButtonCancelGenericOperation.Visibility = Visibility.Collapsed;
                    break;
            }
            DisplayCharacter(Database.Controller.SelectedCharacter);
        }

        #endregion

        private void ButtonJournalAddRecord_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxJournalNew.Text.Length > 0)
            {
                Database.Journal.AddRecord(TextBoxJournalNew.Text);
                TextBoxJournalNew.Text = string.Empty;
                Database.Journal.SaveJournal();
                UpdateJournalBox();
            }
        }

        private void TextBoxJournalNew_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                ButtonJournalAddRecord_Click(sender, e);
        }
    }
}