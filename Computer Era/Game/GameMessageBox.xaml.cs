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
using System.Windows.Shapes;

namespace Computer_Era.Game
{
    /// <summary>
    /// Логика взаимодействия для GameMessageBox.xaml
    /// </summary>
    public partial class GameMessageBox : Window
    {
        public enum MessageBoxType
        {
            ConfirmationWithYesNo = 0,
            Information,
            Error,
            Warning
        }

        public enum MessageBoxImage
        {
            Warning = 0,
            Question,
            Information,
            Error,
            None
        }
        public GameMessageBox()
        {
            InitializeComponent();
        }
        static GameMessageBox _messageBox;
        static MessageBoxResult _result = MessageBoxResult.No;
        public static MessageBoxResult Show
       (string caption, string msg, MessageBoxType type)
        {
            switch (type)
            {
                case MessageBoxType.ConfirmationWithYesNo:
                    return Show(caption, msg, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                case MessageBoxType.Information:
                    return Show(caption, msg, MessageBoxButton.OK,
                    MessageBoxImage.Information);
                case MessageBoxType.Error:
                    return Show(caption, msg, MessageBoxButton.OK,
                    MessageBoxImage.Error);
                case MessageBoxType.Warning:
                    return Show(caption, msg, MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                default:
                    return MessageBoxResult.No;
            }
        }

        public static MessageBoxResult Show(string msg, MessageBoxType type)
        {
            return Show(string.Empty, msg, type);
        }
        public static MessageBoxResult Show(string msg)
        {
            return Show(string.Empty, msg,
            MessageBoxButton.OK, MessageBoxImage.None);
        }
        public static MessageBoxResult Show
        (string caption, string text)
        {
            return Show(caption, text,
            MessageBoxButton.OK, MessageBoxImage.None);
        }
        public static MessageBoxResult Show
        (string caption, string text, MessageBoxButton button)
        {
            return Show(caption, text, button,
            MessageBoxImage.None);
        }
        public static MessageBoxResult Show
        (string caption, string text,
        MessageBoxButton button, MessageBoxImage image)
        {
            _messageBox = new GameMessageBox
            { Text = { Text = text }, Title = { Content = caption } };
            SetVisibilityOfButtons(button);
            //SetImageOfMessageBox(image);
            _messageBox.ShowDialog();
            return _result;
        }

        private static void SetVisibilityOfButtons(MessageBoxButton button)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    _messageBox.ButtonNo.Visibility = Visibility.Collapsed;
                    _messageBox.ButtonYes.Visibility = Visibility.Collapsed;
                    _messageBox.ButtonOk.Focus();
                    break;
                case MessageBoxButton.YesNo:
                    _messageBox.ButtonOk.Visibility = Visibility.Collapsed;
                    _messageBox.ButtonNo.Focus();
                    break;
                default:
                    break;
            }
        }
    }
}
