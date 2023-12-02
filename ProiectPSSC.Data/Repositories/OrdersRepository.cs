using LanguageExt;
using ProiectPSSC.Data.Models;
using ProiectPSSC.Domain.Models;
using ProiectPSSC.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Repositories
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly OrderingContext orderingContext_;
        public OrdersRepository (OrderingContext orderingContext)
        {
            orderingContext_ = orderingContext;
        }
        public TryAsync<int> TryCreateNewOrder(double ? totalPrice) => async () =>
        {
            OrderDto newOrder = new();
            if (totalPrice == null)
                throw new Exception();
            newOrder.PretTotal = (double)totalPrice;
            newOrder.StatusComanda = "Nefinalizata";
            await orderingContext_.AddAsync(newOrder);
            await orderingContext_.SaveChangesAsync();
            if (newOrder.IdComanda != null)
                return newOrder.IdComanda;
            throw new Exception();
        };
    }
}
