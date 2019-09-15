using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;
using System.Data;

namespace LARP.Science.Database
{
    public static class Controller
    {
        public static List<Character> Characters;
        public static List<string> CharacterIDs = new List<string>() { "0000" };
        public static string GetFreeCharacterID()
        {
            string id = "0000";
            while (CharacterIDs.Contains(id))
                id = new Random().Next(1000, 9999).ToString();
            CharacterIDs.Add(id);

            return id;
        }
        public static void NewCharacter(Character character)
        {
            Characters.Add(character);
        }

        public static int GlobalOrganCounter = 0;
        public static int NewOrgan()
        {
            GlobalOrganCounter++;
            return GlobalOrganCounter;
        }

        public static void Initialize(System.Windows.Controls.DataGrid patientsList = null)
        {
            patientsList.ItemsSource = Characters;
        }

        public static void CreateTestingDatabase()
        {
            
        }
    }
}