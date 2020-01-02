using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LARP.Science.Database
{
    public static class Journal
    {
        private static List<string> Records = new List<string>();

        public static string[] GetOutput { get => Records.ToArray(); }

        public static void AddRecord(string log)
        {
            Records.Add(FormatMessage(log));
            SaveJournal();
        }
        public static void AddRecord(string log, System.Windows.Controls.TextBox output)
        {
            string message = FormatMessage(log);
            output.AppendText("\n" + message);
            Records.Add(message);
            SaveJournal();
        }

        public static void SaveJournal() => File.WriteAllLines(Controller.JournalDatabaseFile, Records.ToArray());
        public static void ReadJournal() => Records = File.ReadAllLines(Controller.JournalDatabaseFile).ToList();

        private static string FormatMessage(string message) => DateTime.Now.TimeOfDay.ToString() + " || " + message;
    }
}
