using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.Cart;
using LanguageExt;
using ProiectPSSC.Domain.Repositories;

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
                            if (item.quantity.Value <= foundProduct.Item3)
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
            whenCalculatedCart: async calculatedCart => calculatedCart
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
                Console.WriteLine(calculatedProducts[0].totalPrice.ToString());
                return new CalculatedCart(calculatedProducts.ToArray());
            },
            whenInvalidatedCart: async invalidatedCart => invalidatedCart,
            whenCalculatedCart: async calculatedCart => calculatedCart
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
    }
}
