using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ENETCare.IMS
{
    public class Percentage
    {
        private decimal value;

        private Percentage (decimal value)
        {
            // Constrains the percentage value to 0...100
            this.value = value.Clamp(0m, 100.0m);
        }

        public static implicit operator Percentage (decimal value)
        {
            return new Percentage(value);
        }

        public static implicit operator decimal (Percentage inValue)
        {
            return inValue.value;
        }

        public override string ToString()
        {
            return String.Format("{0:00.0}%", value);
        }
    }
}
