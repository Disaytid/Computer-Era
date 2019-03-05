using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Era.Game
{
    public static class GameConverter
    {
        public enum MediaCapacityUnits
        {
            Byte = 1,
            Kilobyte,
            Megabyte,
            Gigabyte,
            Terabyte
        }

        public static KeyValuePair<double, MediaCapacityUnits> СonversionToMore(MediaCapacityUnits unit, double value)
        {
            for (int i = (int)unit; i < Enum.GetNames(typeof(MediaCapacityUnits)).Length + 1; i++)
            {
                MediaCapacityUnits local_unit = (MediaCapacityUnits)Enum.GetValues(typeof(MediaCapacityUnits)).GetValue(i - 1);
                if (value >= 1024) { value /= 1024; } else { return new KeyValuePair<double, MediaCapacityUnits>(value, local_unit); }
            }
            return new KeyValuePair<double, MediaCapacityUnits>(value, unit);
        }
    }
}
