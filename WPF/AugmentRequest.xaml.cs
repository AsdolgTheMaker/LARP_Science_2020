using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

namespace LARP.Science
{
    /// <summary>
    /// Логика взаимодействия для AugmentRequest.xaml
    /// </summary>
    public partial class AugmentRequest : Window
    {
        private List<Database.Augment> AugsOnStorage;
        public Database.Augment Selection;

        /// <param name="Type">
        /// <= 0 - get all;
        /// == 1 - primary only;
        /// >= 2 - auxilary only</param>
        private AugmentRequest()
        {
            InitializeComponent();
            ButtonConfirm.IsEnabled = false;
        }

        private void ButtonConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridList.SelectedItem != null)
            {
                Selection = DataGridList.SelectedItem as Database.Augment;
                DialogResult = true;
                Close();
            }
            else
            {
                WPFCustomMessageBox.CustomMessageBox.ShowOK("Сначала следует выбрать нужный аугмент.", "", "Хорошо");
                ButtonConfirm.IsEnabled = false;
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DataGridList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ButtonConfirm.IsEnabled = true;
                DataGridCustomParams.Items.Clear();
                foreach (KeyValuePair<string, string> pair in (DataGridList.SelectedItem as Database.Augment).AllCustomParameters)
                    DataGridCustomParams.Items.Add(pair);
            }
        }

        /// <param name="Type">0 - all, 1 - primary, 2 - auxilary</param>
        public static async Task<AugmentRequest> CreateInstance(int Type, Database.Character.BodyPartSlot.SlotType? Slot = null, bool seekReplacements = false)
        {
            AugmentRequest window = new AugmentRequest();
            if (Type <= 0) // request all
                window.AugsOnStorage = (await Economics.Exchange.GetUserItems(2, Slot)).Cast<Database.Augment>().ToList();
            else if (Type == 1) // request primary
                window.AugsOnStorage = (await Economics.Exchange.GetUserItems(3, Slot)).Cast<Database.Augment>().ToList();
            else if (Type == 2) // request auxilary
                window.AugsOnStorage = (await Economics.Exchange.GetUserItems(4, Slot)).Cast<Database.Augment>().ToList();

            if (seekReplacements)
                window.AugsOnStorage.RemoveAll((Database.Augment augment) => { return !augment.IsReplacement; });

            foreach (Database.Augment aug in window.AugsOnStorage)
                window.DataGridList.Items.Add(aug);
            return window;
        }
    }
}