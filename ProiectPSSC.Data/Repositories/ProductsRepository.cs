using LanguageExt;
using LanguageExt.ClassInstances;
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
        public TryAsync<int> TryGetPrimaryKey(ProductId productId) => async () =>
        {
            var foundProducts = await orderingContext_.Products
                                                .Where(product => productId.Value == product.CodProdus)
                                                .AsNoTracking()
                                                .ToListAsync();
            if (foundProducts.Count == 0)
                throw new ProductNotFoundException();
            else
                return foundProducts[0].IdProdus;
        };
        public TryAsync<Unit> TryUpdateQuantity(int ProductPrimaryKey, int orderedQuantity) => async () =>
        {
            int newQuantity = 0;
            var foundProducts = await orderingContext_.Products
                                                .Where(product => ProductPrimaryKey == product.IdProdus)
                                                .AsNoTracking()
                                                .ToListAsync();
            if (foundProducts.Count == 0)
            {
                throw new ProductNotFoundException();
            }

            newQuantity = foundProducts[0].Cantitate - orderedQuantity;
            if (newQuantity < 0)
            {
                throw new Exception();
            }

            if (newQuantity != foundProducts[0].Cantitate)
            {
                foundProducts[0].Cantitate = newQuantity;

                orderingContext_.Update(foundProducts[0]);

                await orderingContext_.SaveChangesAsync();
            }
            return new Unit();
        };
        public class ProductNotFoundException : ApplicationException
        { }
    }
}
