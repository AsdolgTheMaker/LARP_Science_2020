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
using System.Windows.Navigation;
using System.Windows.Shapes;

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

            #if DEBUG
                Database.Controller.CreateTestingDatabase();
            #else
                // TODO - Read initial data from... somewhere.
            #endif
        }

        public void SwitchPage(TabItem target)
        {
            tabctrlRootNavigator.SelectedItem = target;
        }

        // Navigation
        private void BtnNavMenuPatients_Click(object sender, RoutedEventArgs e) => SwitchPage(tabPatients);
        private void BtnNavMenuJournal_Click(object sender, RoutedEventArgs e) => SwitchPage(tabJournal);
        private void BtnNavMenuStorage_Click(object sender, RoutedEventArgs e) => SwitchPage(tabPatients);
        private void BtnNavMenuEconomics_Click(object sender, RoutedEventArgs e) => SwitchPage(tabEconomics);
        private void BtnNavMenuOther_Click(object sender, RoutedEventArgs e) => SwitchPage(tabEconomics);
        private void BtnBackToNavMenu_Click(object sender, RoutedEventArgs e) => SwitchPage(tabNavigation);
    }
}