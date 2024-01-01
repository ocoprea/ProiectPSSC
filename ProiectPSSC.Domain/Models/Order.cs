namespace ProiectPSSC.Domain.Models
{
    public class Order
    {
        public OrderId OrderId { get; set; }

        public Order(OrderId orderId)
        {
            OrderId = orderId;
        }
    }
}
