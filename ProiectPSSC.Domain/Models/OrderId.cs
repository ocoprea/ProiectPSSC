using ProiectPSSC.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace ProiectPSSC.Domain.Models
{
    public record OrderId
    {
        private static Regex ValidPattern = new Regex(@"^\d+$");

        public string Value { get; }

        private OrderId(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidOrderIDException("");
            }
        }

        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);


        public static bool TryParse(string stringValue, out OrderId orderID)
        {
            bool isValid = false;
            orderID = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                orderID = new OrderId(stringValue);
            }

            return isValid;
        }
    }
}
