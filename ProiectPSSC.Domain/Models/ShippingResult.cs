using System.Collections.Generic;

namespace ProiectPSSC.Domain.Models
{
    public class ShippingResult
    {
        public List<ShippingEvent.ShippingOrderSucceededEvent> SuccessEvents { get; } = new List<ShippingEvent.ShippingOrderSucceededEvent>();
        public List<ShippingEvent.ShippingOrderFailedEvent> FailureEvents { get; } = new List<ShippingEvent.ShippingOrderFailedEvent>();

        public void AddSuccess(string orderId, string awb)
        {
            SuccessEvents.Add(new ShippingEvent.ShippingOrderSucceededEvent(orderId, awb));
        }

        public void AddFailure(string orderId, string reason)
        {
            FailureEvents.Add(new ShippingEvent.ShippingOrderFailedEvent(orderId, reason));
        }
    }
}
