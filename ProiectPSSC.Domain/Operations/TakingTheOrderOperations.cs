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
            List<ValidatedProduct> validatedProducts_ = new();
            foreach (var product in products)
            {
                string? reason_ = null;
                var validProductId = await TryParseProductId(product.productId)
                    .ToEither(u => "Invalid product id [" + product.productId + "]");
                validProductId.Match(Right: async productId => 
                {
                    var validQuantity = await TryParseProductQuantity(product.quantity)
                        .ToEither(u => "Invalid quantity [" + product.quantity + "]");
                    validQuantity.Match(Right: validQuantity => { validatedProducts_.Add(new ValidatedProduct(productId, validQuantity)); },
                                        Left: reason => { reason_ = reason; });
                },
                                     Left: reason => { reason_ = reason; });
                if (!string.IsNullOrEmpty(reason_))
                {
                    reason_ += " for Product: " + product.productId;
                    return new InvalidatedCart(products, reason_);
                }
            }
            return new PreValidatedCart(validatedProducts_.ToArray());
        }
        public static async Task<ICart> ValidateProducts(ICart preValidatedCart, IProductsRepository productsRepository) => preValidatedCart.Match(
            whenUnvalidatedCart: unvalidatedCart => unvalidatedCart,
            whenPreValidatedCart: preValidatedCart_ =>
            {
                string? reason = null;
                var products = preValidatedCart_.ValidatedProducts;
                foreach (var item in products)
                {
                    var result = productsRepository.TryGetExistingProduct(item.productId.Value);
                    result.Match(Succ: _ => { },
                                 Fail: _ => { reason = "Product not found"; });

                    if (!string.IsNullOrEmpty(reason))
                    {
                        return new InvalidatedCart(null, reason + ", ProductId: " + item.productId.Value);
                    }
                }
                return new ValidatedCart(preValidatedCart_.ValidatedProducts); 
            },
            whenValidatedCart: validatedCart => validatedCart,
            whenInvalidatedCart: invalidatedCart => invalidatedCart,
            whenCalculatedCart: calculatedCart => calculatedCart
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
    }
}
