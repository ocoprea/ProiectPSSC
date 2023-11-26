using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.Cart;
using LanguageExt;

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
        private static TryAsync<ValidatedProduct> TryValidateProduct(string id, string quantity) => async () =>
        {
            ProductId id_;
            ProductQuantity quantity_;
            if (ProductId.TryParse(id, out id_) && ProductQuantity.TryParse(quantity, out quantity_))
                return new ValidatedProduct(id_, quantity_);
            else
                throw new Exception();
        };
    }
}
