﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using AsdolgTools;

namespace LARP.Science.Economics
{
    public static class Exchange
    {
        public static readonly Uri AdminURL = new Uri("https://rpg.x-serv.ru/api/");
        public static readonly Uri UserURL = new Uri("https://sw.x-serv.ru/api/");
        public static readonly string APIKey = "";

        /// <param name="Type">0 - all items [OBSOLETE];
        /// 1 - only organs;
        /// 2 - only augments [OBSOLETE];
        /// 3 - only primary augs;
        /// 4 - only auxilary augs;</param>
        public static async Task<List<Database.BodyPart>> GetUserItems(int Type, Database.Character.BodyPartSlot.SlotType? Slot = null, Database.Character.RaceType? Race = null)
        {
            switch (Type)
            {
                case 1: // Organs
                    {
                        HttpResponseMessage response = await Database.Controller.Client.GetAsync("player/medicine/getUserOrgans");
                        if (response.IsSuccessStatusCode)
                        {
                            string responseString = JsonConverter(await response.Content.ReadAsStringAsync(), false);
                            File.WriteAllText("temp.json", JToken.Parse(responseString).ToString(Formatting.Indented));
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<EjectedOrgan>));
                            MemoryStream stream = new MemoryStream(File.ReadAllBytes("temp.json")) { Position = 0 };
                            List<EjectedOrgan> items = serializer.ReadObject(stream) as List<EjectedOrgan>;

                            if (Slot != null)
                            {
                            restart:
                                foreach (var item in items)
                                    if (CustomEnum.GetValueFromDescription<Database.Character.BodyPartSlot.SlotType>(item.slot) != Slot)
                                    {
                                        items.Remove(item);
                                        goto restart;
                                    }
                            }
                            if (Race != null)
                            {
                            restart:
                                foreach (var item in items)
                                    if (CustomEnum.GetValueFromDescription<Database.Character.RaceType>(item.race) != Race)
                                    {
                                        items.Remove(item);
                                        goto restart;
                                    }
                            }

                            List<Database.Organ> result = new List<Database.Organ>();
                            foreach (var item in items)
                            {
                                int count = item.count;
                                Database.Organ scienceOrgan = item.ConvertToScienceObject();
                                for (int i = 0; i < count; i++)
                                    result.Add(scienceOrgan);
                            }
                            return result.Cast<Database.BodyPart>().ToList();
                        }
                        else throw new HttpRequestException("Сервер возвратил ошибку " + response.StatusCode.ToString() + " по причине " + response.ReasonPhrase + ".");
                    }
                case 2: throw new ArgumentException();
                case 3: // Primary augs
                    {
                        HttpResponseMessage response = await Database.Controller.Client.GetAsync("player/medicine/getUserAugments");
                        if (response.IsSuccessStatusCode)
                        {
                            string responseString = JsonConverter(await response.Content.ReadAsStringAsync(), false);
                            File.WriteAllText("temp.json", JToken.Parse(responseString).ToString(Formatting.Indented));
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<PrimaryAugment>));
                            MemoryStream stream = new MemoryStream(File.ReadAllBytes("temp.json")) { Position = 0 };
                            List<PrimaryAugment> items = serializer.ReadObject(stream) as List<PrimaryAugment>;

                            if (Slot != null)
                            {
                            restart:
                                foreach (var item in items)
                                    if (CustomEnum.GetValueFromDescription<Database.Character.BodyPartSlot.SlotType>(item.slot) != Slot)
                                    {
                                        items.Remove(item);
                                        goto restart;
                                    }
                            }

                            if (Race != null)
                            {
                            restart:
                                foreach (var item in items)
                                    if (CustomEnum.GetValueFromDescription<Database.Character.RaceType>(item.race) != Race)
                                    {
                                        items.Remove(item);
                                        goto restart;
                                    }
                            }
                            List<Database.Augment> result = new List<Database.Augment>();
                            foreach (var item in items)
                            {
                                int count = item.count;
                                Database.Augment scienceAugment = item.ConvertToScienceObject();
                                for (int i = 0; i < count; i++)
                                    result.Add(scienceAugment);
                            }
                            return result.Cast<Database.BodyPart>().ToList();
                        }
                        else throw new HttpRequestException("Сервер возвратил ошибку " + response.StatusCode.ToString() + " по причине " + response.ReasonPhrase + ".");
                    }
                case 4: // Auxilary augs
                    {
                        HttpResponseMessage response = await Database.Controller.Client.GetAsync("player/medicine/getUserAugments");
                        if (response.IsSuccessStatusCode)
                        {
                            string responseString = JsonConverter(await response.Content.ReadAsStringAsync(), false);
                            File.WriteAllText("temp.json", JToken.Parse(responseString).ToString(Formatting.Indented));
                            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<AuxilaryAugment>));
                            MemoryStream stream = new MemoryStream(File.ReadAllBytes("temp.json")) { Position = 0 };
                            List<AuxilaryAugment> items = serializer.ReadObject(stream) as List<AuxilaryAugment>;

                            List<Database.Augment> result = new List<Database.Augment>();
                            foreach (var item in items)
                                if (item.slot == "") // Auxilary aug = aug without slot
                                {
                                    int count = item.count;
                                    Database.Augment scienceAugment = item.ConvertToScienceObject();
                                    for (int i = 0; i < count; i++)
                                        result.Add(scienceAugment);
                                }
                            return result.Cast<Database.BodyPart>().ToList();
                        }
                        else throw new HttpRequestException("Сервер возвратил ошибку " + response.StatusCode.ToString() + " по причине " + response.ReasonPhrase + ".");
                    }
                default: throw new ArgumentException("Передан неверный тип предметов при запросе на экономический сервер: " + Type.ToString(), "Type");
            }
        }

        public static async Task<List<EjectedOrgan>> GetAbstractOrgans()
        {
            HttpResponseMessage response = await Database.Controller.Client.GetAsync("player/medicine/getAllOrgansList");

            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<List<EjectedOrgan>>(await response.Content.ReadAsStringAsync());
            else return null;
        }

        /// <param name="type">0 - all;
        /// 1 - primary; 
        /// 2 - auxilary;</param>
        public static async Task<List<PrimaryAugment>> GetAbstractAugments()
        {
            HttpResponseMessage response = await Database.Controller.Client.GetAsync("player/medicine/getAllAugmentsList");

            if (response.IsSuccessStatusCode)
            {
                string responseString = JsonConverter(await response.Content.ReadAsStringAsync(), false);
                File.WriteAllText("temp.json", JToken.Parse(responseString).ToString(Formatting.Indented));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<PrimaryAugment>));
                MemoryStream stream = new MemoryStream(File.ReadAllBytes("temp.json")) { Position = 0 };

                return serializer.ReadObject(stream) as List<PrimaryAugment>;
            }
            else return null;
        }

        // Add one item of given id to Economics storage
        public static async Task<bool> AddItem(GenericItem item) => await AddItem(item.id.ToString());
        public static async Task<bool> AddItem(Database.BodyPart item)
        {
            if (item.Id != null) return await AddItem(item.Id.ToString());
            else return await AddItem(await TryFindId(item));
        }
        public static async Task<bool> AddItem(string id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "player/medicine/addItem");

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("id", id));

            request.Content = new FormUrlEncodedContent(keyValues);
            HttpResponseMessage response = await Database.Controller.Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                if (await response.Content.ReadAsStringAsync() == id) return true;
                else return false;
            }
            else return false;
        }

        // Remove one item of given id to Economics storage
        public static async Task<bool> TakeItem(GenericItem item) => await TakeItem(item.id.ToString());
        public static async Task<bool> TakeItem(Database.BodyPart item)
        {
            if (item.Id != null) return await TakeItem(item.Id.ToString());
            else return await TakeItem(await TryFindId(item));
        }
        public static async Task<bool> TakeItem(string id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "player/medicine/takeItem");

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("id", id));

            request.Content = new FormUrlEncodedContent(keyValues);
            HttpResponseMessage response = await Database.Controller.Client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                if (await response.Content.ReadAsStringAsync() == id) return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Last resort method, meant to prevent critical error (when something doesn't have ID)
        /// </summary>
        private async static Task<GenericItem> TryFindId(Database.BodyPart item)
        {
            // if some of our items do not have ID - this is critical, we cant send requests because of that
            // so we will try to find item which will fit to us
            {
                if (item is Database.Organ)
                {
                    List<EjectedOrgan> abstracts = await GetAbstractOrgans();
                    foreach (EjectedOrgan v in abstracts)
                    {
                        if (v.slot == item.SlotString
                            && v.race == item.Race.GetDescription()
                            && v.genderInverted == item.Gender)
                        {
                            item.Id = v.id; // fix this organ for future
                            return v;
                        }
                    }
                }
                else
                {
                    List<PrimaryAugment> abstracts = await GetAbstractAugments();
                    foreach (PrimaryAugment v in abstracts)
                    {
                        if (v.slot == item.SlotString
                            && v.race == item.Race.GetDescription()
                            && v.genderInverted == item.Gender)
                        {
                            item.Id = v.id; // fix this augment for future
                            return v;
                        }
                    }
                }
                // and if we fail to find that item - nothing to do, this is critical error, throw exception
                throw new InvalidOperationException("Критическая ошибка работы базы данных: Непроидентифицированный BodyPart. Не удалось разрешить проблему перебором данных сервера.");
            }
        }

        public static bool Auth()
        {
            try
            {
                return AuthWindow.ShowAuthDialog();
            }
            catch (HttpRequestException)
            {
                WPFCustomMessageBox.CustomMessageBox.ShowOK("Не удалось авторизоваться на экономическом сервере.", "Ошибка работы с сервером", "Понятно");
                return false;
            }

        }

        public static string JsonConverter(string json, bool toEconomics)
        {
            if (toEconomics)
            {
                json = json.Replace("\"Key\"", "\"title\"");
                json = json.Replace("\"Value\"", "\"value\"");
            }
            else
            {
                // Dictionary conversion
                json = json.Replace("\"title\"", "\"Key\"");
                json = json.Replace("\"value\"", "\"Value\"");
            }

            return json;
        }
    }
}