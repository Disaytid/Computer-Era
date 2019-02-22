using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Computer_Era.Game.Widgets
{
    /// <summary>
    /// Логика взаимодействия для MoneyWidget.xaml
    /// </summary>
    public partial class MoneyWidget : UserControl
    {
        Money Money;

        public MoneyWidget(Money money, GameEvents events)
        {
            InitializeComponent();

            Money = money;

            Update();
            events.Events.Add(new GameEvent("money widget", events.GameTimer.DateAndTime.AddHours(1), Periodicity.Hour, 1, Update, true));
        }

        private void Update(GameEvent @event)
        {
            Update();
        }
        private void Update()
        {
            CurrencyList.Children.Clear();

            foreach (Currency currency in Money.PlayerCurrency)
            {
                string path = "Resources/currency/" + currency.SystemName + ".png";
                Uri uri = new Uri("pack://application:,,,/" + path);

                if (System.IO.File.Exists(System.IO.Path.GetFullPath("../../" + path)) == false)
                {
                    uri = new Uri("pack://application:,,,/Resources/currency/coin.png");
                }

                Image image = new Image()
                {
                    Source = new BitmapImage(uri),
                    Width = 32
                };

                Label label = new Label()
                {
                    Content = currency.Count + " " + currency.Abbreviation,
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 14
                };


                StackPanel stackPanel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = VerticalAlignment.Top
                };

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(label);

                CurrencyList.Children.Add(stackPanel);
            }
        }

        private void CheckUncheck()
        {

        }
    }
}
