using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Computer_Era.Game
{
    class Inventory
    {
        int MaxSize;
        private Grid Program;

        public Inventory(Grid program)
        {
            MaxSize = 16;
            Program = program;
            Draw();
            Program.Visibility = Visibility.Visible;
        }

        public Inventory(Grid program, int max_size)
        {
            MaxSize = max_size;
            Draw();
            Program.Visibility = Visibility.Visible;
        }

        public void Draw()
        {
            Color color = (Color)ColorConverter.ConvertFromString("#E5747474");

            DockPanel header = new DockPanel
            {
                Background = new SolidColorBrush(color), //#E5747474
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top
            };

            TextBlock textBlock = new TextBlock
            {
                Text = "Кладовка (" + MaxSize + " ячеек)",
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.White),
                Margin = new Thickness(10, 0, 10, 0)
            };

            Button buttonClose = new Button
            {
                Background = new SolidColorBrush(Colors.Red),
                Foreground = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(0, 0, 0, 0),
                Content = "X",
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 24
            };

            buttonClose.Click += new RoutedEventHandler(CloseForm);

            header.Children.Add(textBlock);
            header.Children.Add(buttonClose);

            StackPanel content = new StackPanel
            {
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(0, 24, 0, 0)
            };

            ListBox itemsBox = new ListBox
            {

            };

            content.Children.Add(itemsBox);

            Program.Children.Add(header);
            Program.Children.Add(content);
        }

        void CloseForm(object sender, RoutedEventArgs e)
        {
            Program.Visibility = Visibility.Hidden;
        }
    }
}
