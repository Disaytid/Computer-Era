using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Computer_Era.Game.Objects
{
    public class Widgets
    {
        public Collection<Widget> PlayerWidgets = new Collection<Widget>(); 

        public void Draw(StackPanel widgetsPanel)
        {
            foreach (Widget widget in PlayerWidgets)
            {
                widgetsPanel.Children.Add(widget.Control);
            }
        }
    }

    public class Widget
    {
        public UserControl Control;

        public Widget(UserControl control)
        {
            Control = control;
        }
    }
}
