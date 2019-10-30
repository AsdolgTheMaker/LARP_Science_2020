using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LARP.Science.Database
{
    [DataContract]
    public partial class Organ
    {
        [DataMember] public string Name;
        [DataMember] public string Description;
        [DataMember] public Augment AugmentEquivalent;
        [DataMember] public Character.OrganSlot.SlotType Slot;
        [DataMember] public string ImagePath; 

        public bool IsAugmented()
        {
            if (AugmentEquivalent == null)
                return false;
            else
                return true;
        }

        public ImageSource GetImageSource()
        {
            System.Windows.Media.Imaging.BitmapImage tempImg = new System.Windows.Media.Imaging.BitmapImage();
            tempImg.BeginInit();
            tempImg.UriSource = new Uri(Controller.CurrentExecutableDirectory + "\\" + ImagePath, UriKind.Relative);
            tempImg.EndInit();
            return tempImg;
        }

        public Organ(
            string name,
            Character.OrganSlot.SlotType slot,
            string image,
            string description = "")
        {
            Name = name;
            Description = description;
            Slot = slot;
            ImagePath = image;
        }
    }
}