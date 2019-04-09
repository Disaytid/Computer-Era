using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace Computer_Era.Game
{
    public class GameMessages
    {
        readonly GameEvents Events;
        readonly Button ViewMessages;
        readonly StackPanel MessagesPanel;
        readonly StackPanel Bubble;
        int count_messages;

        public GameMessages(GameEvents events, Button viewMessages, StackPanel messagesPanel, StackPanel bubble)
        {
            Events = events;

            ViewMessages = viewMessages;
            MessagesPanel = messagesPanel;
            Bubble = bubble;
        }

        public enum Icon
        {
            Money,
            Info,
        }

        private string GetIcon(Icon icon)
        {
            switch(icon)
            {
                case Icon.Money:
                    return "message-icons/coin.png";
                default:
                    return "message-icons/info.png";
            }
        }

        public void NewMessage(string title, string text, Icon icon)
        {
            for (int i = 0; i <= 1; i++) //Очень похоже на костыль, но другого решения я пока не нашел
            {
                System.Windows.Controls.Image headerIcon = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/" + GetIcon(icon))),
                    Width = 16,
                    Height = 16,
                    Margin = new Thickness(10, 0, 0, 0)
                };

                Label headerTitle = new Label
                {
                    Content = title,
                    FontSize = 16,
                    Margin = new Thickness(5, 0, 10, 0),
                    Foreground = new SolidColorBrush(Colors.White)
                };

                DockPanel header = new DockPanel
                {
                    VerticalAlignment = VerticalAlignment.Top,
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 72, 186, 255))
                };

                header.Children.Add(headerIcon);
                header.Children.Add(headerTitle);

                TextBlock content = new TextBlock
                {
                    Text = text,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Colors.DarkSlateGray),
                    Margin = new Thickness(10)
                };

                StackPanel message = new StackPanel
                {
                    Margin = new Thickness(10, 10, 10, 5),
                    Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, 72, 186, 255))
                };

                message.Children.Add(header);
                message.Children.Add(content);

                if (i == 0)
                {
                    MessagesPanel.Children.Add(message);
                }

                if (i == 1)
                {
                    message.Margin = new Thickness(0);
                    Bubble.Children.Add(message);
                    Bubble.Visibility = Visibility.Visible;
                    Events.Events.Add(new GameEvent("message", Events.GameTimer.DateAndTime.AddHours(2), Periodicity.Hour, 2, HideBubble));
                }
            }

            count_messages += 1;
            if (count_messages > 100) { ViewMessages.Content = "99+"; } else { ViewMessages.Content = count_messages; }
        }

        private void HideBubble(GameEvent @event)
        {
            Bubble.Children.Clear();
            Bubble.Visibility = Visibility.Collapsed;
        }
    }
}
