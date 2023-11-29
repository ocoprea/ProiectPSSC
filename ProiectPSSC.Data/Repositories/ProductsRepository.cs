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
        public TryAsync<Tuple<string, int, double>> TryGetProduct(ProductId productId) => async () =>
        {
            Tuple<string, int, double> tuple = new("0", 0, 0.0);
            var foundProducts = await orderingContext_.Products
                                                .Where(product => productId.Value == product.CodProdus)
                                                .AsNoTracking()
                                                .ToListAsync();
            if (foundProducts.Count == 0)
                throw new ProductNotFoundException();
            else
                return new Tuple<string, int, double>(foundProducts[0].CodProdus,
                    foundProducts[0].Cantitate, foundProducts[0].Pret);
        };
        public class ProductNotFoundException : ApplicationException
        { }
    }
}
