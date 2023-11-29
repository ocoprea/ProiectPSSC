using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    [AsChoice]
    public static partial class Cart
    {
        public interface ICart { }
        public record UnvalidatedCart: ICart
        {
            public UnvalidatedCart(IReadOnlyCollection<UnvalidatedProduct> unvalidatedProducts)
            {
                UnvalidatedProducts = unvalidatedProducts;
            }
            public IReadOnlyCollection<UnvalidatedProduct> UnvalidatedProducts { get; }
        }
        public record PreValidatedCart : ICart
        {
            public PreValidatedCart(IReadOnlyCollection<PreValidatedProduct> preValidatedProducts)
            {
                PreValidatedProducts = preValidatedProducts;
            }
            public IReadOnlyCollection<PreValidatedProduct> PreValidatedProducts { get; }
        }

        public record ValidatedCart : ICart
        {
            public ValidatedCart(IReadOnlyCollection<ValidatedProduct> validatedProducts)
            {
                ValidatedProducts = validatedProducts;
            }
            public IReadOnlyCollection<ValidatedProduct> ValidatedProducts { get; }
        }
        public record InvalidatedCart : ICart
        {
            public InvalidatedCart (IReadOnlyCollection<UnvalidatedProduct> unvalidatedProducts, string reason)
            {
                UnvalidatedProducts = unvalidatedProducts;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidatedProduct> ? UnvalidatedProducts { get; }
            public string Reason { get; }
        }
        public record CalculatedCart : ICart
        {
            public CalculatedCart (IReadOnlyCollection<CalculatedProduct> calculatedProducts)
            {
                CalculatedProducts = calculatedProducts;
            }
            public IReadOnlyCollection<CalculatedProduct> CalculatedProducts { get; }
        }
    }
}
