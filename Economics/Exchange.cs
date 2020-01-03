using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Economics
{
    public static class Exchange
    {
        public static readonly Uri ServerURL = new Uri("https://rpg.x-serv.ru/api/");
        public static readonly string APIKey = "";

        /// <param name="Type">0 - all items
        /// 1 - only organs
        /// 2 - only augments
        /// 3 - only primary augs
        /// 4 - only auxilary augs</param>
        public static async Task<List<Database.BodyPart>> GetUserItems(int Type, Database.Character.BodyPartSlot.SlotType? Slot = null)
        {
            List<Database.BodyPart> result = new List<Database.BodyPart>();
            WPFCustomMessageBox.CustomMessageBox.ShowOK("Это не реализовано, ждём экономику.", "", "Блин, ладно");
            return result;
        }

        public static async Task<List<EjectedOrgan>> GetAbstractOrgans()
        {
            WPFCustomMessageBox.CustomMessageBox.ShowOK("Это не реализовано, ждём экономику.", "", "Блин, ладно");
            return new List<EjectedOrgan>();
        }
        public static async Task<List<EjectedAugment>> GetAbstractAugments<T>()
        {
            WPFCustomMessageBox.CustomMessageBox.ShowOK("Это не реализовано, ждём экономику.", "", "Блин, ладно");
            return new List<EjectedAugment>();
        }

        public static async void AddItem(Database.BodyPart item)
        {
            WPFCustomMessageBox.CustomMessageBox.ShowOK("Это не реализовано, ждём экономику.", "", "Блин, ладно");
        }
        public static async void TakeItem(GenericItem item)
        {
            WPFCustomMessageBox.CustomMessageBox.ShowOK("Это не реализовано, ждём экономику.", "", "Блин, ладно");
        }

        public static bool Auth()
        {
            try
            {
                return AuthWindow.ShowAuthDialog();
            }
            catch (HttpRequestException)
            {
                WPFCustomMessageBox.CustomMessageBox.ShowOK("Не удалось авторизоваться на .", "Ошибка работы с сервером", "Понятно");
                return false;
            }

        }
    }
}