using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    public record PayMetod
    {
        public string Value { get; }
        private PayMetod(string value)
        {
            Value = value;
        }
        public static bool TryParse(string value, out PayMetod result)
        {
            result = null;
            if (value == "Cash" || value == "Card")
            {
                result = new PayMetod(value);
                return true;
            }
            return false;
        }
    }
}
