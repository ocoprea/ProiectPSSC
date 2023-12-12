using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    public record OrderId
    {
        public string Value { get; }
        private static Regex model = new Regex(@"^\d+$");
        private OrderId(string value)
        {
            Value = value;
        }
        public static bool TryParse(string value, out OrderId orderId)
        {
            orderId = null;
            if (model.IsMatch(value))
            {
                orderId = new OrderId(value);
                return true;
            }
            return false;
        }
    }
}
