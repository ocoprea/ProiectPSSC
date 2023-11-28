using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    public record ProductId
    {
        public string Value { get; }
        private static Regex model = new Regex("^[A-D]{1}[0-9]{3}$");
        private ProductId(string value)
        {
            Value = value;
        }
        public static bool TryParse(string value, out ProductId productId)
        {
            productId = null;
            if (model.IsMatch(value))
            {
                productId = new ProductId(value);
                return true;
            }
            return false;
        }
    }
}
