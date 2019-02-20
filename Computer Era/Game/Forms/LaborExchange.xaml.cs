﻿using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Computer_Era.Game.Forms
{
    public class JobCard
    {
        public int Id { get; }
        public string Name { get; }
        public string CompanyName { get; }
        public double Salary { get; }
        public double Complexity { get; } //Огрничить от 0 до 1
        public DateTime FromTime { get; }
        public DateTime ToTime { get; }
        public DateTime DateEmployment { get; set; }
        public SolidColorBrush StickerColor { get; }

        public JobCard(Profession profession, List<Company> company, int id, Random rnd) {
            Id = id;
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
                    int case_id = rnd.Next(1, 4);
                    if (case_id == 1) { goto case 1; }
                    else if (case_id == 2) { goto case 2; }
                    else if (case_id == 3) { goto case 3; }
                    else { goto case 4; }
            }

            //Цвет стикера
            switch (rnd.Next(1, 5))
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
        readonly Collection<JobCard> JobCards = new Collection<JobCard>();
        readonly PlayerProfile Player;
        readonly GameEvents GameEvents;
        readonly GameMessages Messages;

        GameEvent CurrentGameEvent;
        DateTime BeginningWork;
        readonly Collection<Currency> PlayerCurency;
        readonly Random rnd;

        public LaborExchange(PlayerProfile player, Collection<Profession> profession, Collection<Company> companies, Collection<Currency> curency, GameEvents events, Random random, GameMessages messages)
        {
            InitializeComponent();

            rnd = random;
            Player = player;
            if (player.Job != null) { Dismissal.IsEnabled = true; }

            GameEvents = events;
            Messages = messages;
            PlayerCurency = curency;

            CreateJobCards(profession, companies, events.GameTimer.DateAndTime);
        }

        private void CreateJobCards(Collection<Profession> profession, Collection<Company> companies, DateTime game_date)
        {
            List<Company> current_companies = new List<Company>();
            current_companies = companies.ToList();
            current_companies.RemoveAll(e => DateTime.Compare(e.OpeningYear, game_date) > 0);

            for (int i = 0; i < profession.Count; i++)
            {
                JobCards.Add(new JobCard(profession[i], current_companies, i, rnd));
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
                    //double sc = (i + 1) * size;
                    //double stCount = count;

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

            //string text = JobCards[index].Name;
            //text = char.ToUpper(text[0]) + text.Substring(1);

            TextBlock professionName = new TextBlock
            {
                Text = "Требуется " + JobCards[index].Name + " в компанию " + '\u0022' + JobCards[index].CompanyName + '\u0022',
                TextWrapping = TextWrapping.Wrap,
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.Blue),
                Margin = new Thickness(10, 20, 10, 5)
            };

            TextBlock professionSalary = new TextBlock
            {
                Text = "Оклад: " + JobCards[index].Salary * PlayerCurency[0].Course + " " + PlayerCurency[0].Abbreviation,
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
                Tag = JobCards[index].Id.ToString(),
                Margin = new Thickness(10, 10, 10, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 250,
                Width = 250,
                Background = JobCards[index].StickerColor,
                Cursor = Cursors.Hand,
            };

            stackPanel.MouseDown += new MouseButtonEventHandler(StackPanel_MouseDown);

            stackPanel.Children.Add(professionName);
            stackPanel.Children.Add(professionSalary);
            stackPanel.Children.Add(professionTime);

            return stackPanel;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (Player.Job == null)
                {
                    if (sender is StackPanel)
                    {
                        Player.Job = JobCards[Convert.ToInt32((sender as StackPanel).Tag)];
                        Player.Job.DateEmployment = GameEvents.GameTimer.DateAndTime.AddDays(1);

                        BeginningWork = GameEvents.GameTimer.DateAndTime.AddDays(1);
                        CurrentGameEvent = new GameEvent("job", BeginningWork, Periodicity.Month, 1, Payroll, true);
                        GameEvents.Events.Add(CurrentGameEvent);
                        MessageBox.Show("Поздравляем вы устроильсь на вакансию: " + Player.Job.Name + " с окладом " + Player.Job.Salary * PlayerCurency[0].Course + " " + PlayerCurency[0].Abbreviation);
                    }
                } else {
                    MessageBox.Show("У вас уже есть работа, сначала увольтесь!");
                }
            }
        }

        public void Payroll()
        {
            double amount;
            if (BeginningWork.Year != 1)
            {
                amount = DateTime.DaysInMonth(CurrentGameEvent.ResponseTime.Year, CurrentGameEvent.ResponseTime.Month) - BeginningWork.Day;
                BeginningWork = new DateTime(1, 1, 1);
            } else {
                amount = DateTime.DaysInMonth(CurrentGameEvent.ResponseTime.Year, CurrentGameEvent.ResponseTime.Month);
            }
            amount *= (Player.Job.Salary * PlayerCurency[0].Course);

            PlayerCurency[0].TopUp("Выплата зарплаты", Player.Job.CompanyName, GameEvents.GameTimer.DateAndTime, amount);
            Messages.NewMessage("Поступление средств", "Танцуйте! Вам пришла зарплата, если вы еще не забыли вы работаете на должности \"" + Player.Job.Name + "\". Выплаты составили " + amount + " " + PlayerCurency[0].Abbreviation, GameMessages.Icon.Money);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                DrawGrid();
            }          
        } 

        private void Dismissal_Click(object sender, RoutedEventArgs e) //Дописать зависимость от времени работы
        { 
            Dismissal.IsEnabled = false;

            foreach (GameEvent value in GameEvents.Events.Where(v => v.Name == "job"))
            {
                CurrentGameEvent = value;
                break;
            }

            GameEvents.Events.Remove(CurrentGameEvent);

            double amount = 0;
            if (Player.Job.DateEmployment < GameEvents.GameTimer.DateAndTime) { 
                if (Player.Job.DateEmployment.Month == GameEvents.GameTimer.DateAndTime.Month & Player.Job.DateEmployment.Year == GameEvents.GameTimer.DateAndTime.Year)
                {
                    amount = GameEvents.GameTimer.DateAndTime.Day - Player.Job.DateEmployment.Day;
                } else {
                    amount = GameEvents.GameTimer.DateAndTime.Day;
                }   
                amount *= (Player.Job.Salary * PlayerCurency[0].Course);
            }

            string Company = Player.Job.CompanyName;
            Player.Job = null;
            CurrentGameEvent = null;

            PlayerCurency[0].TopUp("Увольнение", Company, GameEvents.GameTimer.DateAndTime, amount);
            MessageBox.Show("Поздравляем вы уволились, вам выплачено " + amount + " " + PlayerCurency[0].Abbreviation);
        }
    }
}
