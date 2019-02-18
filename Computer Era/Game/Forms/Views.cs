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
        public BaseItem Item { get; set; }
        public string Tag { get; set; }

        public ListBoxComponent(BaseItem item, BitmapImage image, string tag)
        {
            Item = item;
            Item.Image = image;
            Tag = tag;
        }
    }

    public class ListBoxObject
    {
        public BaseItem Item { get; set; }
        public object IObject { get; set; }
        public string Tag { get; set; }
        public bool IsEnabled { get; set; }

        public ListBoxObject(object obj, BitmapImage image)
        {
            Item = obj as BaseItem;
            Item.Image = image;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = false;
        }
        public ListBoxObject(object obj, BitmapImage image, bool isEnabled)
        {
            Item = obj as BaseItem;
            Item.Image = image;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = isEnabled;
        }

        public ListBoxObject(object obj)
        {
            Item = obj as BaseItem;
            IObject = obj;
            Tag = Item.ToString();
            IsEnabled = false;
        }
    }
}
