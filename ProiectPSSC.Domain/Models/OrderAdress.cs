using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    public record OrderAdress
    {
        public string Value { get; }
        private OrderAdress(string value)
        {
            Value = value;
        }
        public static bool TryParse(string value, out OrderAdress result)
        {
            result = null;
            if (value.Contains("str.") && value.Contains("oras"))
            {
                result = new OrderAdress(value);
                return true;
            }
            return false;
        }
    }
}
