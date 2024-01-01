using System.Runtime.Serialization;

namespace ProiectPSSC.Domain.Exceptions
{
    [Serializable]
    internal class InvalidOrderIDException : Exception
    {
        public InvalidOrderIDException()
        {
        }

        public InvalidOrderIDException(string message) : base(message)
        {
        }

        public InvalidOrderIDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidOrderIDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
