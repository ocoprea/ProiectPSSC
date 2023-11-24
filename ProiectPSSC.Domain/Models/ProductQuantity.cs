using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    public record ProductQuantity
    {
        public Int16 Value { get; }
        private ProductQuantity(Int16 value) 
        {
            Value = value;
        }
        public static bool TryParse(string value, out ProductQuantity result)
        {
            result = null;
            Int16 value_;
            if (Int16.TryParse(value, out value_) && IsValid(value_))
            {
                result = new ProductQuantity(value_);
                return true;
            }
            return false;
        }
        private static bool IsValid(Int16 value)
        {
            if (value > 0 && value <= 1000)
                return true;
            return false;
        }
    }
}
