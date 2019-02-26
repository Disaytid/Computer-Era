using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game.Objects
{
    public class OpticalDiskProperties
    {
        
    }
    public class OpticalDisk : Item<OpticalDiskProperties>
    {
        public OpticalDisk(int uid, string name, string type, int price, DateTime man_date, OpticalDiskProperties properties) : base(uid, name, type, price, man_date, properties) { }
    }
}
