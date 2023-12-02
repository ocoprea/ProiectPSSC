using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.Cart;
using LanguageExt;
using ProiectPSSC.Domain.Repositories;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Specialized;
using LanguageExt.SomeHelp;

namespace ProiectPSSC.Domain.Operations
{
    public class TakingTheOrderOperations
    {
        public static async Task<ICart> PreValidateProducts(IReadOnlyCollection<UnvalidatedProduct> products)
        {
            List<PreValidatedProduct> preValidatedProducts_ = new();
            foreach (var product in products)
            {
                string? reason_ = null;
                var validProductId = await TryParseProductId(product.productId)
                    .ToEither(u => "Invalid product id [" + product.productId + "]");
                validProductId.Match(Right: async productId =>
                {
                    var validQuantity = await TryParseProductQuantity(product.quantity)
                        .ToEither(u => "Invalid quantity [" + product.quantity + "]");
                    validQuantity.Match(Right: validQuantity => { preValidatedProducts_.Add(new PreValidatedProduct(productId, validQuantity)); },
                                        Left: reason => { reason_ = reason; });
                },
                                     Left: reason => { reason_ = reason; });
                if (!string.IsNullOrEmpty(reason_))
                {
                    reason_ += " for Product: " + product.productId;
                    return new InvalidatedCart(products, reason_);
                }
            }
            return new PreValidatedCart(preValidatedProducts_.ToArray());
        }
        public static async Task<ICart> ValidateProducts(ICart preValidatedCart, IProductsRepository productsRepository) => await preValidatedCart.MatchAsync(
            whenUnvalidatedCart: async unvalidatedCart => unvalidatedCart,
            whenPreValidatedCart: async preValidatedCart_ =>
            {
                string? reason = null;
                var preValidatedProducts = preValidatedCart_.PreValidatedProducts;
                List<ValidatedProduct> validatedProducts = new();

                foreach (var item in preValidatedProducts)
                {
                    var result = await productsRepository.TryGetProduct(item.productId);
                    result.Match(
                        Succ: foundProduct =>
                        {
                            if (item.quantity.Value <= foundProduct.Item2)
                                validatedProducts.Add(new ValidatedProduct(item.productId, item.quantity, foundProduct.Item3));
                            else
                                reason = "Insuficient quantity for Product Id: " + item.productId.Value;
                        },
                        Fail: exception =>
                        {
                            reason = "Product not found, Product Id: " + item.productId.Value;
                        });
                    if (!string.IsNullOrEmpty(reason))
                    {
                        return new InvalidatedCart(null, reason);
                    }
                }
                return new ValidatedCart(validatedProducts.ToArray());
            },
            whenValidatedCart: async validatedCart => validatedCart,
            whenInvalidatedCart: async invalidatedCart => invalidatedCart,
            whenCalculatedCart: async calculatedCart => calculatedCart,
            whenAddedToOrder: async addedToOrderCart => addedToOrderCart
        );
        public static async Task<ICart> CalculateProducts(ICart validatedCart) => await validatedCart.MatchAsync(
            whenUnvalidatedCart: async unvalidatedCart => unvalidatedCart,
            whenPreValidatedCart: async prevalidatedCart => prevalidatedCart,
            whenValidatedCart: async validatedCart =>
            {
                List<CalculatedProduct> calculatedProducts = new();
                bool exception_ = false;
                var validatedProducts = validatedCart.ValidatedProducts;
                foreach (var item in validatedProducts)
                {
                    var result = await TryCalculateTotalPrice(item);
                    result.Match(Succ: (totalPrice) => calculatedProducts.Add(new CalculatedProduct(item.ProductId, item.Quantity, totalPrice)),
                                 Fail: (exception) => exception_ = true);
                    if (exception_)
                        return validatedCart;
                }
                return new CalculatedCart(calculatedProducts.ToArray());
            },
            whenInvalidatedCart: async invalidatedCart => invalidatedCart,
            whenCalculatedCart: async calculatedCart => calculatedCart,
            whenAddedToOrder: async addedToOrderCart => addedToOrderCart
            );
        public static async Task<ICart> CreateNewOrder
            (ICart calculatedCart, IOrdersRepository ordersRepository, IProductsRepository productsRepository, IOrderedProductRepository orderedProductRepository) => await calculatedCart.MatchAsync(
            whenUnvalidatedCart: async unvalidatedCart => unvalidatedCart,
            whenPreValidatedCart: async prevalidatedCart => prevalidatedCart,
            whenValidatedCart: async validatedCart => validatedCart,
            whenInvalidatedCart: async invalidatedCart => invalidatedCart,
            whenCalculatedCart: async calculatedCart =>
            {
                double? totalOrderPrice = null;
                List<Tuple<int, int>> productsPKs = new();
                ICart cart = calculatedCart;

                var query = from item in calculatedCart.CalculatedProducts
                             select (item.totalPrice, item.productId, item.quantity).ToTuple();

                var prices = query.Select(obj => obj.Item1);
                var res = await TryCalculateTotalOrderPrice(prices.ToArray());
                res.Match(Succ: totalPrice => totalOrderPrice = Convert.ToDouble(totalPrice),
                          Fail: _ => { totalOrderPrice = null; });

                foreach (var item in query.Select (_ => { return new Tuple<ProductId, ProductQuantity>(_.Item2, _.Item3); }))
                    await productsRepository.TryGetPrimaryKey(item.Item1).Match(
                        Succ: id => productsPKs.Add(new Tuple<int, int>(id, item.Item2.Value)),
                        Fail: _ => { });
                if (productsPKs.Count() != calculatedCart.CalculatedProducts.Count())
                    return calculatedCart;

                var result = from it in ordersRepository.TryCreateNewOrder(totalOrderPrice).ToEither(ex => -1)
                             from itt in TrySaveOrderedProducts(productsPKs, it, orderedProductRepository).ToEither(ex => -1)
                             from ittt in TryUpdateQuantityForOrderedProducts(productsPKs, productsRepository).ToEither(ex => -1)
                             select it;
                await result.Match(
                    Right: orderId => cart = new AddedToOrder(calculatedCart.CalculatedProducts, orderId),
                    Left: _ => cart = calculatedCart);

                return cart;
            },
            whenAddedToOrder: async addedToOrderCart => addedToOrderCart
            );
        

        private static TryAsync<ProductId> TryParseProductId(string id) => async () =>
        {
            ProductId productId;
            if (ProductId.TryParse(id, out productId))
                return productId;
            else
                throw new Exception();
        };
        private static TryAsync<ProductQuantity> TryParseProductQuantity(string quantity) => async () =>
        {
            ProductQuantity productQuantity;
            if (ProductQuantity.TryParse(quantity, out productQuantity))
                return productQuantity;
            else
                throw new Exception();
        };
        private static TryAsync<decimal> TryCalculateTotalPrice(ValidatedProduct validatedProduct) => async () =>
        {
            decimal totalPrice;
            totalPrice = Convert.ToDecimal(validatedProduct.Price);
            totalPrice *= validatedProduct.Quantity.Value;
            return totalPrice;
        };
        private static TryAsync<int> TrySaveOrderedProducts(List<Tuple<int, int>> orderedProducts, int orderId, IOrderedProductRepository orderedProductRepository) => async () =>
        {
            bool exception = false;
            if (orderId <= 0)
                throw new Exception();
            foreach(var item in orderedProducts)
            {
                var res = await orderedProductRepository.TrySaveNewOrderedProducts(item.Item1, orderId, item.Item2);
                res.Match(Succ: _ => { },
                          Fail: _ => exception = true);
            }
            if (exception)
                throw new Exception();
            return 1;
        };
        private static TryAsync<int> TryUpdateQuantityForOrderedProducts(List<Tuple<int, int>> orderedProducts, IProductsRepository productsRepository) => async () =>
        {
            bool exception = false;
            foreach (var item in orderedProducts)
            {
                await productsRepository.TryUpdateQuantity(item.Item1, item.Item2).Match(
                    Succ: _ => { exception = false; },
                    Fail: _ => { Console.WriteLine("Nu merge"); exception = true; });
                if (exception == true)
                    throw new Exception();
            }
            return 1;
        };
        private static TryAsync<decimal> TryCalculateTotalOrderPrice(decimal[] prices) => async () =>
        {
            decimal totalOrderPrice = 0;
            foreach (var item in prices)
                totalOrderPrice += item;
            return totalOrderPrice;
        };
    }
}
