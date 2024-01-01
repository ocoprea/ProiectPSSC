using ProiectPSSC.Domain.Models;
using System.Collections.Generic;
using System;

namespace ProiectPSSC.Domain.Workflows
{
    public class ShippingWorkflow
    {
        public ShippingResult Execute(List<Order> orders, Func<Order, bool> validateOrder)
        {
            var shippingResult = new ShippingResult();

            foreach (var order in orders)
            {
                if (validateOrder(order))
                {
                    var awb = GenerateRandomAwb();
                    shippingResult.AddSuccess(order.OrderId.ToString(), awb);
                }
                else
                {
                    shippingResult.AddFailure(order.OrderId.ToString(), "Invalid order ID");
                }
            }

            return shippingResult;
        }

        private string GenerateRandomAwb()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }
    }
}
