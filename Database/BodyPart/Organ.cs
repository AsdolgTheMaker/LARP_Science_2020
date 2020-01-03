using LARP.Science.Economics;
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
        [DataMember] public bool Virtual { get; set; } = false;

        public bool IsAugmented => !(AugmentEquivalent == null);

        public Augment EjectAugment()
        {
            if (IsAugmented)
            {
                Augment aug = AugmentEquivalent;
                AugmentEquivalent = null;
                if (aug.IsReplacement) Virtual = true;
                return aug;
            }
            else return null;
        }

        public BitmapImage GetImage()
        {
            string path = Virtual ? Character.BodyPartSlot.GetSlotPictureEmpty(Slot) : 
                (IsAugmented ? 
                (string.IsNullOrEmpty(AugmentEquivalent.ImagePath) ? ImagePath : AugmentEquivalent.ImagePath) 
                : ImagePath);
            if (!File.Exists(path))
            {
                if (!IsAugmented)
                    path = Character.BodyPartSlot.GetDefaultSlotPicture(Slot);
                else if (AugmentEquivalent.IsReplacement)
                    path = Character.BodyPartSlot.GetDefaultSlotPicture(Slot, 2);
                else path = Character.BodyPartSlot.GetDefaultSlotPicture(Slot, 1);
            }

            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            fileStream.Seek(0, SeekOrigin.Begin);
            bitmapImage.StreamSource = fileStream;
            bitmapImage.EndInit();
            return bitmapImage;
        }

        internal EjectedOrgan ConvertToEjectedOrgan() => new EjectedOrgan(this);

        public Organ(string name, Character.BodyPartSlot.SlotType slot, string image, string description = "") : base(name, slot, image, description) { }
    }
}