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
        public CPUCooler CPUCooler { get; set; }
        public Collection<RAM> RAMs { get; set; } = new Collection<RAM>();
        public Collection<HDD> HDDs { get; set; } = new Collection<HDD>();
        public Collection<VideoСard> VideoСards { get; set; } = new Collection<VideoСard>();
        public Collection<OpticalDrive> OpticalDrives { get; set; } = new Collection<OpticalDrive>();
        public Collection<Monitor> Monitors { get; set; } = new Collection<Monitor>();
        public Collection<Mouse> Mice { get; set; } = new Collection<Mouse>();
        public Collection<Keyboard> Keyboards { get; set; } = new Collection<Keyboard>();
        public OperatingSystem OperatingSystem { get; set; }

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

        public enum ErrorСodes
        {
            Ok,
            NoMotherboard,
            NoPowerSupply,
            NoCPU,
            NoCPUCooler,
            NoRAM,
            NoHDD,
            NoVideoСard,
        }

        private readonly Dictionary<ErrorСodes, string> LocalizedErrorCodes = new Dictionary<ErrorСodes, string>
        {
            { ErrorСodes.Ok, Properties.Resources.ErrorСodeOk },
            { ErrorСodes.NoMotherboard, Properties.Resources.ErrorСodeNoMotherboard },
            { ErrorСodes.NoPowerSupply, Properties.Resources.ErrorСodeNoPowerSupply },
            { ErrorСodes.NoCPU, Properties.Resources.ErrorСodeNoCPU },
            { ErrorСodes.NoCPUCooler, Properties.Resources.ErrorСodeNoCPUCooler },
            { ErrorСodes.NoRAM, Properties.Resources.ErrorСodeNoRAM },
            { ErrorСodes.NoHDD, Properties.Resources.ErrorСodeNoHDD },
            { ErrorСodes.NoVideoСard, Properties.Resources.ErrorСodeNoVideoСard },
        };

        public string GetLocalizedErrorCode(ErrorСodes errorСode)
        {
            if (!LocalizedErrorCodes.ContainsKey(errorСode)) throw new ArgumentException(string.Format("Operation {0} is invalid", errorСode), "op");
            return (string)LocalizedErrorCodes[errorСode];
        }

        public List<ErrorСodes> Diagnostics()
        {
            List<ErrorСodes> errorСodes = new List<ErrorСodes>();
            if (Motherboard  == null) { errorСodes.Add(ErrorСodes.NoMotherboard); }
            if (PSU == null) { errorСodes.Add(ErrorСodes.NoPowerSupply); }
            if (CPU == null) { errorСodes.Add(ErrorСodes.NoCPU); }
            if (CPUCooler == null) { errorСodes.Add(ErrorСodes.NoCPUCooler); }
            if (RAMs.Count == 0) { errorСodes.Add(ErrorСodes.NoRAM); }
            if (HDDs.Count == 0) { errorСodes.Add(ErrorСodes.NoHDD); }
            if (Motherboard != null && !Motherboard.Properties.EmbeddedGraphics & VideoСards.Count == 0) { errorСodes.Add(ErrorСodes.NoVideoСard); }

            if (errorСodes.Count == 0) { errorСodes.Add(ErrorСodes.Ok); }

            return errorСodes;
        }
    }

}
