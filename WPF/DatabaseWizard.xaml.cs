using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LARP.Science.Database;
using System.Runtime.Serialization.Json;
using AsdolgTools;
using LARP.Science.Operation;

namespace LARP.Science
{
    /// <summary>
    /// Логика взаимодействия для DatabaseWizard.xaml
    /// </summary>
    public partial class DatabaseWizard : Window
    {
        private List<Character> chars;
        private Character newCharacter;

        public DatabaseWizard(string databaseFile)
        {
            InitializeComponent();

            chars = new List<Character>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Character>));
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(databaseFile)) { Position = 0 };
            chars = serializer.ReadObject(stream) as List<Character>;

            foreach (Character.GenderType gender in Enum.GetValues(typeof(Character.GenderType)))
                BoxGender.Items.Add(gender.GetDescription());

            foreach (Character.RaceType race in Enum.GetValues(typeof(Character.RaceType)))
                BoxRace.Items.Add(race.GetDescription());

            BoxAlive.Items.Add("Да");
            BoxAlive.Items.Add("Нет");

            DefaultCharacter_Click(this, new RoutedEventArgs());

            UpdateGrids();
        }

        private void UpdateGrids()
        {
            UpdateCharsDatagrid();
            UpdateOrgansDatagrid();
        }

        private void UpdateCharsDatagrid()
        {
            DatagridCharacters.Items.Clear();
            foreach (Character player in chars)
                DatagridCharacters.Items.Add(player);
        }

        private void UpdateOrgansDatagrid()
        {
            DatagridOrgans.Items.Clear();
            foreach (Organ organ in newCharacter.OrgansList)
                DatagridOrgans.Items.Add(organ);
        }

        private void DeleteCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridCharacters.SelectedItem is Character character)
            {
                chars.Remove(character);
                UpdateGrids();
            }
        }

        private void CreateCharacter_Click(object sender, RoutedEventArgs e)
        {
            newCharacter.Name = BoxName.Text;
            newCharacter.Description = BoxDescription.Text;
            newCharacter.Gender = CustomEnum.GetValueFromDescription<Character.GenderType>(BoxGender.Text);
            newCharacter.Race = CustomEnum.GetValueFromDescription<Character.RaceType>(BoxRace.Text);

            chars.Add(newCharacter);
            DefaultCharacter_Click(this, new RoutedEventArgs());
            UpdateGrids();
        }

        private async void Augmentate_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridOrgans.SelectedItem is Organ organ)
            {
                try
                {
                    AugmentRequest augWindow = await AugmentRequest.CreateInstance(1, organ.Slot);
                    if (augWindow.ShowDialog().GetValueOrDefault(false))
                        new AugmentationDetails(AugmentationType.Primary, AugmentationAction.Install, _implant: augWindow.Selection).Execute();
                }
                catch (OperationCanceledException) { }
            }
            UpdateGrids();
        }

        private void Deaugmentate_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridOrgans.SelectedItem is Organ organ)
                try { new AugmentationDetails(AugmentationType.Primary, AugmentationAction.Remove, _target: organ).Execute(); }
                catch (OperationCanceledException) { }
            UpdateGrids();
        }

        private void SwitchOrgan_Click(object sender, RoutedEventArgs e)
        {
            if (DatagridOrgans.SelectedItem is Organ organ)
                if (organ.Virtual) new AugmentationDetails(AugmentationType.Organ, AugmentationAction.Install, _implant: organ).Execute();
                else new AugmentationDetails(AugmentationType.Organ, AugmentationAction.Remove, _target: organ).Execute();
            UpdateGrids();
        }

        private void DefaultCharacter_Click(object sender, RoutedEventArgs e)
        {
            BoxGender.Text = "Мужской";
            BoxRace.Text = "Человек";
            BoxAlive.Text = "Да";

            newCharacter = new Character("", Character.GenderType.Male, Character.RaceType.Human);
            Controller.SelectedCharacter = newCharacter;
        }

        private void RefillOrgans() => newCharacter.OrgansList = Character.BodyPartSlot.GetOrgansListForCharacter(newCharacter.Race, newCharacter.Gender);

        private void BoxGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SaveDatabase_Click(object sender, RoutedEventArgs e)
        {
            Controller.SetCharactersDatabase(chars);
            Controller.SaveCharacters();
            if (WPFCustomMessageBox.CustomMessageBox.ShowYesNo("База сохранена. Закрыть всё наконец?", "", "Да", "Нет, хочу работат") == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }
    }
}