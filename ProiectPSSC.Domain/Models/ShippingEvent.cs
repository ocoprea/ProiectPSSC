using CSharp.Choices;

namespace ProiectPSSC.Domain.Models
{
    [AsChoice]
    public static partial class ShippingEvent
    {
        public interface IShippingEvent { }

        public record ShippingOrderSucceededEvent : IShippingEvent
        {
            public string OrderId { get; }
            public string Awb { get; }

            public ShippingOrderSucceededEvent(string orderId, string awb)
            {
                OrderId = orderId;
                Awb = awb;
            }
        }

        public record ShippingOrderFailedEvent : IShippingEvent
        {
            public string OrderId { get; }
            public string Reason { get; }

            public ShippingOrderFailedEvent(string orderId, string reason)
            {
                OrderId = orderId;
                Reason = reason;
            }
        }
    }
}
