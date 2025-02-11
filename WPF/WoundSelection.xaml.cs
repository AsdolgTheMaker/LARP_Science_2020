﻿using System;
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

namespace LARP.Science.Operation
{
    /// <summary>
    /// Логика взаимодействия для WoundSelection.xaml
    /// </summary>
    public partial class WoundSelection : Window
    {
        private HealingDetails.DamageType selection;

        private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();

        #region Selection buttons event handlers
        private void ButtonBlade_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.Blade;
            DialogResult = true;
            Close();
        }

        private void ButtonBlunt_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.Blunt;
            DialogResult = true;
            Close();
        }

        private void ButtonElectro_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.Electro;
            DialogResult = true;
            Close();
        }

        private void ButtonBlaster_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.Blaster;
            DialogResult = true;
            Close();
        }

        private void ButtonSaber_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.Saber;
            DialogResult = true;
            Close();
        }

        private void ButtonPoison_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.Poison;
            DialogResult = true;
            Close();
        }

        private void ButtonBodyHole_Click(object sender, RoutedEventArgs e)
        {
            selection = HealingDetails.DamageType.BodyHole;
            DialogResult = true;
            Close();
        }
        #endregion

        private WoundSelection() => InitializeComponent();

        public static HealingDetails.DamageType SelectWound()
        {
            WoundSelection window = new WoundSelection();
            if (window.ShowDialog().GetValueOrDefault(false))
                return window.selection;
            else
                throw new OperationCanceledException();
        }
    }
}