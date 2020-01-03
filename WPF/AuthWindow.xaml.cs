using LARP.Science.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Логика взаимодействия для AuthWindow.xaml
    /// </summary>
    public partial class AuthWindow : Window
    {
        private AuthWindow()
        {
            InitializeComponent();
        }

        private async void ButtonAuth_Click(object sender, RoutedEventArgs e)
        {
            ButtonAuth.IsEnabled = false;
            ButtonCancel.IsEnabled = false;
            Login.IsEnabled = false;
            Password.IsEnabled = false;

            if (Login.Text.Length > 0 && Password.Password.Length > 0)
            {
                if (await Auth())
                {
                    DialogResult = true;
                    Close();
                }
                else WPFCustomMessageBox.CustomMessageBox.ShowOK("Не удалось авторизоваться на экономическом сервере.\nПроверьте введённые логин и пароль и попробуйте ещё раз.", "", "ОК");
            }
            else WPFCustomMessageBox.CustomMessageBox.ShowOK("Для авторизации введите логин и пароль.", "", "ОК");

            ButtonAuth.IsEnabled = true;
            ButtonCancel.IsEnabled = true;
            Login.IsEnabled = true;
            Password.IsEnabled = true;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ButtonAuth_Click(sender, new RoutedEventArgs());
        }

        private async Task<bool> Auth()
        {
            string stringContent = "{  \"login\": \"" + Login.Text + "\",  \"password\": \"" + Password.Password + "\"} ";
            StringContent content = new StringContent(stringContent, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Database.Controller.Client.PostAsync("auth", content);

            Database.Controller.User = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode) return true;
            else return false;
        }

        public static bool ShowAuthDialog()
        {
            AuthWindow auth = new AuthWindow();
            if (auth.ShowDialog().GetValueOrDefault(false)) return true;
            else return false;
        }
    }
}
