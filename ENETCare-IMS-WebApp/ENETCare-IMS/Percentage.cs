using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace ENETCare.IMS
{
    [ComplexType()]
    public class Percentage
    {
        public decimal Value { get; private set; }

        private Percentage() { }

        private Percentage (decimal value)
        {
            // Constrains the percentage value to 0...100
            this.Value = value.Clamp(0m, 100.0m);
        }

        public static implicit operator Percentage (decimal value)
        {
            return new Percentage(value);
        }

        public static implicit operator decimal (Percentage inValue)
        {
            return inValue.Value;
        }

        public override string ToString()
        {
            return String.Format("{0:00.0}%", Value);
        }
    }
}
