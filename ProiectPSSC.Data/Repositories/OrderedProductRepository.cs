using LanguageExt;
using ProiectPSSC.Data.Models;
using ProiectPSSC.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Repositories
{
    public class OrderedProductRepository : IOrderedProductRepository
    {
        private readonly OrderingContext orderingContext_;
        public OrderedProductRepository(OrderingContext orderingContext)
        {
            orderingContext_ = orderingContext;
        }
        public TryAsync<Tuple<int, int, int>> TrySaveNewOrderedProducts(int productId, int orderId, int quantity) => async () =>
        {
            OrderedProductDto newOrderedProduct = new();
            newOrderedProduct.IdProdus = productId;
            newOrderedProduct.IdComanda = orderId;
            newOrderedProduct.Cantitate = quantity;

            await orderingContext_.AddAsync(newOrderedProduct);
            await orderingContext_.SaveChangesAsync();

            return new Tuple<int, int, int>
                (newOrderedProduct.IdProdus, newOrderedProduct.IdComanda, newOrderedProduct.Cantitate);
        };
    }
}
