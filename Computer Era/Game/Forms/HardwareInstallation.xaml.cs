using Computer_Era.Game.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Computer_Era.Game.Forms
{
    /// <summary>
    /// Логика взаимодействия для HardwareInstallation.xaml
    /// </summary>
    public partial class HardwareInstallation : UserControl
    {
        Computers Computers;
        Items Items;

        public HardwareInstallation(Items items, Computers computers)
        {
            InitializeComponent();
            Computers = computers;
            Items = items;
            LoadItems(items);
            LoadComputers(computers);
        }

        public void LoadItems(Items items)
        {
            List<ListBoxObject> items_source = new List<ListBoxObject>();

            for (int i = 0; i <= items.Cases.Count - 1; i++) //CASES
            { items_source.Add(new ListBoxObject(items.Cases[i], new BitmapImage(new Uri("pack://application:,,,/Resources/coffin.png")), items.Cases[i], items.Cases[i].ToString())); }

            for (int i = 0; i <= items.Motherboards.Count - 1; i++) //MOTHERBOARDS
            { items_source.Add(new ListBoxObject(items.Motherboards[i], new BitmapImage(new Uri("pack://application:,,,/Resources/circuitry.png")), items.Motherboards[i], items.Motherboards[i].ToString())); }

            for (int i = 0; i <= items.PowerSupplyUnits.Count - 1; i++) //PowerSupplyUnits
            { items_source.Add(new ListBoxObject(items.PowerSupplyUnits[i], new BitmapImage(new Uri("pack://application:,,,/Resources/plug.png")), items.PowerSupplyUnits[i], items.PowerSupplyUnits[i].ToString())); }

            for (int i = 0; i <= items.CPUs.Count - 1; i++) //CPUs
            { items_source.Add(new ListBoxObject(items.CPUs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/processor.png")), items.CPUs[i], items.CPUs[i].ToString())); }

            for (int i = 0; i <= items.RAMs.Count - 1; i++) //RAMs
            { items_source.Add(new ListBoxObject(items.RAMs[i], new BitmapImage(new Uri("pack://application:,,,/Resources/brain.png")), items.RAMs[i], items.RAMs[i].ToString())); }

            СomponentsList.ItemsSource = items_source;
        }

        public void LoadComputers(Computers computers)
        {
            foreach (Computer computer in computers.PlayerComputers)
            {
                AssemblyList.Items.Add(computer.Name);
            }
        }

        private void AddAssembly_Click(object sender, RoutedEventArgs e)
        {
            string name = AssemblyList.Text;

            foreach (String item in AssemblyList.Items)
            {
                if (item == name)
                {
                    name = null;
                    MessageBox.Show("Данное имя уже существует!");
                    break;
                }
            }

            if (name != null)
            {
                AssemblyList.Items.Add(name);
                ComputerСomponents.ItemsSource = new Collection<ListBoxObject>();
            }
        }

        private int GetCount(Collection<ListBoxObject> collection, string type)
        {
            if (collection.Count > 0)
            {
                return collection.Where(i => i.Item.GetTypeValue() == type).Count();
            } else {
                return 0;
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button & !String.IsNullOrEmpty(AssemblyList.Text))
            {
                if (!(ComputerСomponents.ItemsSource is Collection<ListBoxObject>)) { ComputerСomponents.ItemsSource = new Collection<ListBoxObject>(); }

                Button button = sender as Button;
                Collection<ListBoxObject> items = ComputerСomponents.ItemsSource as Collection<ListBoxObject>;

                if (button.Tag is Case)
                {
                    Case @case = (button.Tag as Case);
                    if (GetCount(items, @case.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "motherboard") == 0 || (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).CheckCompatibility(@case.Properties))
                        {
                            if (GetCount(items, "psu") == 0 || (items.Single(i => i.Item.GetTypeValue() == "psu").IObject as PowerSupplyUnit).CheckCompatibility(@case.Properties))
                            {
                                items.Add(new ListBoxObject(@case, @case, @case.ToString()));

                                ComputerСomponents.Items.Refresh();

                                button.Content = "Установлено";
                                button.IsEnabled = false;
                            } else  {
                                MessageBox.Show("Блок питания не станет в этот корпус!");
                            }
                        } else {
                            MessageBox.Show("Материнская плата не станет в этот корпус!");
                        }
                    } else {
                        MessageBox.Show("У вас уже есть корпус в этой конфигурации!");
                    }
                } else if (button.Tag is Motherboard) {
                    Motherboard motherboard = (button.Tag as Motherboard);
                    if  (GetCount(items, motherboard.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "case") == 0 || motherboard.CheckCompatibility((items.Single(i => i.Item.GetTypeValue() == "case").IObject as Case).Properties))
                        {
                            items.Add(new ListBoxObject(motherboard, motherboard, motherboard.ToString()));

                            ComputerСomponents.Items.Refresh();

                            button.Content = "Установлено";
                            button.IsEnabled = false;
                        } else {
                            MessageBox.Show("Материнская плата не станет в этот корпус!");
                        }
                    } else {
                        MessageBox.Show("У вас уже есть материнская плата в этой конфигурации!");
                    }
                } else if (button.Tag is PowerSupplyUnit) {
                    PowerSupplyUnit psu = (button.Tag as PowerSupplyUnit);
                    if (GetCount(items, psu.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "motherboard") == 1)
                        {
                            if (GetCount(items, "case") == 0 || psu.CheckCompatibility((items.Single(i => i.Item.GetTypeValue() == "case").IObject as Case).Properties))
                            {
                                items.Add(new ListBoxObject(psu, psu, psu.ToString()));

                                ComputerСomponents.Items.Refresh();

                                button.Content = "Установлено";
                                button.IsEnabled = false;
                            }
                            else
                            {
                                MessageBox.Show("Блок питания не станет в этот корпус!");
                            }
                        } else {
                            MessageBox.Show("У вас нет материнской платы, к чему подключать собрались?");
                        }
                    } else {
                        MessageBox.Show("У вас уже есть блок питания в этой конфигурации!");
                    }
                } else if (button.Tag is CPU) {
                    CPU cpu = (button.Tag as CPU);
                    if (GetCount(items, cpu.GetTypeValue()) == 0)
                    {
                        if (GetCount(items, "motherboard") == 1)
                        {
                            if (cpu.Properties.Socket == (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.Socket)
                            {
                                items.Add(new ListBoxObject(cpu, cpu, cpu.ToString()));

                                ComputerСomponents.Items.Refresh();

                                button.Content = "Установлено";
                                button.IsEnabled = false;
                            } else {
                                MessageBox.Show("Впихнуть невпихуемое? На сокет посмотри!");
                            }
                        } else {
                            MessageBox.Show("У вас нет материнской платы, и куда вы собрались ставить процессор?");
                        }
                    } else {
                        MessageBox.Show("У вас уже есть процессор в этой конфигурации!");
                    }
                } else if (button.Tag is RAM) {
                    RAM ram = (button.Tag as RAM);
                    if (GetCount(items, "motherboard") == 1)
                    {
                        if (GetCount(items, ram.GetTypeValue()) < (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.RAMSlots)
                        {
                            //Дописать проверку на частоту
                            int volume = 0;

                            foreach (object obj in (items.Where(i => i.Item.GetTypeValue() == "ram")))
                            {
                                volume += (obj as RAM).Properties.Volume;
                            }

                            if (ram.Properties.Volume + volume <= (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.RAMVolume)
                            {
                                if (ram.Properties.RAMTypes == (items.Single(i => i.Item.GetTypeValue() == "motherboard").IObject as Motherboard).Properties.RamType)
                                {
                                    items.Add(new ListBoxObject(ram, ram, ram.ToString()));

                                    ComputerСomponents.Items.Refresh();

                                    button.Content = "Установлено";
                                    button.IsEnabled = false;
                                }  else {
                                    MessageBox.Show("Ну не лезет же, тип памяти другой!");
                                }
                            } else {
                                MessageBox.Show("По объему не подходит!");
                            }
                        } else {
                            MessageBox.Show("Все слоты заняты!");
                        }
                    } else {
                        MessageBox.Show("У вас нет материнской платы, и куда вы собрались вставлять память?");
                    }
                }
            }  
        }

        private void AssemblyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Computer> computer = Computers.PlayerComputers.Where<Computer>(c => c.Name == AssemblyList.Name).ToList();

            if (computer.Count == 1)
            {
                Collection<ListBoxObject> items = new Collection<ListBoxObject>();
                if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].Case, computer[0].Case, computer[0].Case.ToString())); }
                if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].Motherboard, computer[0].Motherboard, computer[0].Motherboard.ToString())); }
                if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].CPU, computer[0].CPU, computer[0].CPU.ToString())); }
                if (computer[0].Case != null) { items.Add(new ListBoxObject(computer[0].PSU, computer[0].PSU, computer[0].PSU.ToString())); }
                if (computer[0].Case != null) { foreach (RAM ram in computer[0].RAMs) { items.Add(new ListBoxObject(ram, ram, ram.ToString())); } }

                ComputerСomponents.ItemsSource = items;
            }
            ComputerСomponents.Items.Refresh();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
