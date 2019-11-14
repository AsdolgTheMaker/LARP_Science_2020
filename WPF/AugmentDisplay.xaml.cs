using System;
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

namespace LARP.Science.WPF
{
    public partial class AugmentDisplay : Window
    {
        private readonly Database.Organ organ;

        public AugmentDisplay(Database.Organ _organ)
        {
            InitializeComponent();
            organ = _organ;

            UpdateDisplayInfo();
        }

        private void OnClick_Eject(object sender, RoutedEventArgs e)
        {
            // <TODO> Send ejected augment to the storage
            organ.EjectAugment();
            UpdateDisplayInfo();
        }
        private void OnClick_Exit(object sender, RoutedEventArgs e) => Close();
        private void OnClick_Install(object sender, RoutedEventArgs e)
        {

            UpdateDisplayInfo();
        }

        private void UpdateDisplayInfo()
        {
            if (organ.IsAugmented())
            {
                Database.Augment augment = organ.AugmentEquivalent;
                labelAugmentName.Content = augment.Name;
                labelAugmentDescription.Content = augment.Description;
                imgAugmentDisplayPicture.ImageSource = organ.GetImageEntity();
                foreach (KeyValuePair<string, string> param in augment.GetAllCustomParameters())
                    datagridCustomParams.Items.Add(param);

                buttonEject.IsEnabled = true;
            }
            else
            {
                labelAugmentName.Content = "[ОТСУТСТВУЕТ]";
                labelAugmentDescription.Content = "[ОТСУТСТВУЕТ]";
                // TODO: Display picture should be replaced to 'OBSOLETE'
                datagridCustomParams.Items.Clear();

                buttonEject.IsEnabled = false;
                buttonInstall.IsEnabled = true; 
            }
        }
    }
}