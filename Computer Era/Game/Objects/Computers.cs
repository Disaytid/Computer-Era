using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    public class Computers
    {
        public Collection<Computer> PlayerComputers = new Collection<Computer>();
        public Computer CurrentPlayerComputer;

    }

    public class Computer //Для создания объекта "Computer" должны обязательно указываться корпус или материнская плата, без них объект бесполезен и равноценен пустому
    {
        public string Name { get; set; }
        public Case Case { get; set; }
        public Motherboard Motherboard { get; set; }
        public PowerSupplyUnit PSU { get; set; }
        public CPU CPU { get; set; }
        public Collection<RAM> RAMs { get; set; }

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
