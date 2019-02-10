using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Era.Game.Forms
{
    public class ListBoxComponent
    {
        public Item Item { get; set; }
        public string Tag { get; set; }

        public ListBoxComponent(Item item, BitmapImage image, string tag)
        {
            Item = item;
            Item.Image = image;
            Tag = tag;
        }
    }

    public class ListBoxObject
    {
        public Item Item { get; set; }
        public object IObject { get; set; }
        public string Tag { get; set; }


        public ListBoxObject(Item item, BitmapImage image, object obj, string tag)
        {
            Item = item;
            Item.Image = image;
            IObject = obj;
            Tag = tag;
        }

        public ListBoxObject(Item item, object obj, string tag)
        {
            Item = item;
            IObject = obj;
            Tag = tag;
        }
    }
}
