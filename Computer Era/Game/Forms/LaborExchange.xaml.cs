using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для LaborExchange.xaml
    /// </summary>
    /// 

    public class JobCard
    {
        public string Name { get; }
        public string CompanyName { get; }
        public double Salary { get; }
        public double Complexity { get; } //Огрничить от 0 до 1
        public DateTime FromTime { get; }
        public DateTime ToTime { get; }
        public SolidColorBrush StickerColor { get; }

        public JobCard(Profession profession, List<Company> company, DateTime game_date, Random rnd) {
            Name = profession.Name;

            //Название компании
            List<Company> current_company = company; //Поправить
            if (current_company.Count > 0)
            {
                int companyId = rnd.Next(0, current_company.Count);
                CompanyName = current_company[companyId].Name;
            } else { CompanyName = "У черта на рогах"; }

            //Оклад
            int floatSalary = Convert.ToInt32(profession.Salary * 10 / 100);

            //Сделать потом нормальный рандом
            rnd.Next(1000);
            rnd.Next(2000);
            rnd.Next(3000);
            rnd.Next(4000);
            Salary = profession.Salary + rnd.Next(- floatSalary, floatSalary);

            Complexity = profession.Complexity;

            //Период работы
            switch (profession.DayPeriod)
            {
                case 1:
                    FromTime = new DateTime(1000, 1, 1, 8, 0, 0);
                    ToTime = FromTime.AddHours(profession.WorkingHours);
                    break;
                case 2:
                    FromTime = new DateTime(1000, 1, 1, 14, 0, 0);
                    ToTime = FromTime.AddHours(profession.WorkingHours);
                    break;
                case 3:
                    FromTime = new DateTime(1000, 1, 1, 20, 0, 0);
                    ToTime = FromTime.AddHours(profession.WorkingHours);
                    break;
                case 4:
                    FromTime = new DateTime(1000, 1, 1, 2, 0, 0);
                    ToTime = FromTime.AddHours(profession.WorkingHours);
                    break;
                default:
                    //Сделать потом нормальный рандом
                    rnd.Next(1000);
                    rnd.Next(2000);
                    rnd.Next(3000);
                    rnd.Next(4000);

                    int case_id = rnd.Next(1, 4);
                    if (case_id == 1) { goto case 1; }
                    else if (case_id == 2) { goto case 2; }
                    else if (case_id == 3) { goto case 3; }
                    else { goto case 4; }
            }

            //Сделать потом нормальный рандом
            rnd.Next(1000);
            rnd.Next(2000);
            rnd.Next(3000);
            rnd.Next(4000);

            //Цвет стикера
            switch (rnd.Next(1, 4))
            {
                case 1:
                    StickerColor = new SolidColorBrush(Color.FromRgb(255, 224, 175));
                    break;
                case 2:
                    StickerColor = new SolidColorBrush(Color.FromRgb(252, 136, 252));
                    break;
                case 3:
                    StickerColor = new SolidColorBrush(Color.FromRgb(184, 233, 134));
                    break;
                case 4:
                    StickerColor = new SolidColorBrush(Color.FromRgb(72, 186, 255));
                    break;
                default:
                    StickerColor = new SolidColorBrush(Colors.White);
                    break;
            }
        }
    }
    
    public partial class LaborExchange : UserControl
    {
        Collection<JobCard> JobCards = new Collection<JobCard>();

        Collection<Currency> PlayerCurency;

        Random rnd = new Random();

        public LaborExchange(Collection<Profession> profession, Collection<Company> companies, Collection<Currency> curency, DateTime game_date)
        {
            InitializeComponent();

            PlayerCurency = curency;

            CreateJobCards(profession, companies, game_date);
        }

        private void CreateJobCards(Collection<Profession> profession, Collection<Company> companies, DateTime game_date)
        {
            List<Company> current_companies = companies.Where(e => e.OpeningYear <= game_date).ToList();

            for (int i = 0; i < profession.Count; i++)
            {
                JobCards.Add(new JobCard(profession[i], current_companies, game_date, rnd));
            }
        }

        private void DrawGrid()
        {
            Jobs.ColumnDefinitions.Clear();
            Jobs.RowDefinitions.Clear();
            Jobs.Children.Clear();

            double cell_size = 250;
            double size = Math.Floor(Jobs.ActualWidth / cell_size);
            double len = Jobs.ActualWidth / size;

            for (int i = 0; i < size; i++)
            {
                ColumnDefinition col = new ColumnDefinition
                {
                    Width = new GridLength(len, GridUnitType.Star)
                };
                Jobs.ColumnDefinitions.Insert(i, col);
            }

            int index = 0;
            for (int i = 0; i < Convert.ToInt32(size); i++)
            {

                StackPanel columnPanel = new StackPanel
                {
                    
                };

                if (JobCards.Count <= size)
                {
                    for (int j = 0; j < JobCards.Count; j++)
                    {
                        StackPanel panel;
                        panel = AddCard(j);
                        panel.SetValue(Grid.ColumnProperty, j);
                        Jobs.Children.Add(panel);
                    }
                    break;
                } else {
                    double count = Math.Ceiling(JobCards.Count / size);
                    double sc = (i + 1) * size;
                    double stCount = count;

                    //if (sc > Profession.Count) { stCount -=1; } //Пересмотреть

                    for (int j = 0; j < count; j++)
                    {
                        if (i + 1 > size - (size * count - JobCards.Count) & j == count - 1) { break; }
                        columnPanel.Children.Add(AddCard(index));
                        index += 1;
                    }

                    columnPanel.SetValue(Grid.ColumnProperty, i);
                    Jobs.Children.Add(columnPanel);
                }

            }
        }

        private StackPanel AddCard(int index)
        {
            //MessageBox.Show(index.ToString());

            string text = JobCards[index].Name;
            text = char.ToUpper(text[0]) + text.Substring(1);

            TextBlock professionName = new TextBlock
            {
                Text = "Требуеться " + JobCards[index].Name + " в компанию " + '\u0022' + JobCards[index].CompanyName + '\u0022',
                TextWrapping = TextWrapping.Wrap,
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Blue),
                Margin = new Thickness(10, 20, 10, 5)
            };

            TextBlock professionSalary = new TextBlock
            {
                Text = "Оклад: " + JobCards[index].Salary * PlayerCurency[0].Course  + " " + PlayerCurency[0].Abbreviation,
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Blue),
                Margin = new Thickness(10, 5, 10, 5)
            };

            TextBlock professionTime = new TextBlock
            {
                Text = "Время работы: " + JobCards[index].FromTime.ToString("HH:mm") + " - " + JobCards[index].ToTime.ToString("HH:mm"),
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Blue),
                Margin = new Thickness(10, 5, 10, 5)
            };

            StackPanel stackPanel = new StackPanel
            {
                Margin = new Thickness(10, 10, 10, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 250,
                Width = 250,
                Background = JobCards[index].StickerColor
            };

            stackPanel.Children.Add(professionName);
            stackPanel.Children.Add(professionSalary);
            stackPanel.Children.Add(professionTime);

            return stackPanel;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                DrawGrid();
            }          
        }
    }
}
