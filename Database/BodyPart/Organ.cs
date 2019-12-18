using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LARP.Science.Database
{
    [DataContract]
    public partial class Organ : BodyPart
    {
        [DataMember] public Augment AugmentEquivalent = null;

        public bool IsAugmented()
        {
            if (AugmentEquivalent == null)
                return false;
            else
                return true;
        }

        public Augment EjectAugment()
        {
            if (IsAugmented())
            {
                Augment aug = AugmentEquivalent;
                AugmentEquivalent = null;
                return aug;
            }
            else return null;
        }

        public BitmapImage GetImageEntity()
        {
            // First, decide if we will use Organ's image or its Augment's one
            string path = IsAugmented() ? (string.IsNullOrEmpty(AugmentEquivalent.ImagePath) ? ImagePath : AugmentEquivalent.ImagePath) : ImagePath;
            if (!File.Exists(path)) path = Character.BodyPartSlot.GetDefaultSlotPicture(this.Slot);

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            fileStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = fileStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        public Organ(string name, Character.BodyPartSlot.SlotType slot, string image, string description = "") : base(name, slot, image, description) {}
    }
}