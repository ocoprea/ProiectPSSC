using LanguageExt;
using Microsoft.EntityFrameworkCore;
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
    public class ProductsRepository : IProductsRepository
    {
        private readonly OrderingContext orderingContext_;
        public ProductsRepository(OrderingContext orderingContext)
        {
            orderingContext_ = orderingContext;
        }
        public TryAsync<string> TryGetExistingProduct(string product) => async () =>
        {
            string to_return = "0";
            var products_ = orderingContext_.Products
                                                 .Where(prod => product.Contains(prod.CodProdus))
                                                 .AsNoTracking()
                                                 .ToList();
            to_return =  products_[0].CodProdus;
            return to_return;
        };
    }
}
