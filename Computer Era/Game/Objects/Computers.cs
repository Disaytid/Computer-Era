using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    class Computers
    {
        public Collection<Computer> PlayerComputers = new Collection<Computer>();
        public Computer CurrentPlayerComputer;

    }

    public class Computer //Для создания объекта "Computer" должны обязательно указываться корпус или материнская плата, без них объект бесполезен и равноценен пустому
    {
        string Name { get; set; }
        Case Case { get; set; }
        Motherboard Motherboard { get; set; }
        PowerSupplyUnit PSU { get; set; }
        CPU CPU { get; set; }
        Collection<RAM> RAMs { get; set; }

        public Computer(string name, Case @case)
        {
            Name = name;
            Case = @case;
        }

        public Computer(string name, Motherboard motherboard)
        {
            Name = name;
            Motherboard = motherboard;
        }

        public Computer(string name, Case @case, Motherboard motherboard)
        {
            Name = name;
            Case = @case;
            Motherboard = motherboard;
        }
    }

}
