using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Commands
{
    public record TakingTheOrderCommand
    {
        public TakingTheOrderCommand(IReadOnlyCollection<UnvalidatedProduct> unvalidatedProducts)
        {
            UnvalidatedProducts = unvalidatedProducts;
        }
        public IReadOnlyCollection<UnvalidatedProduct> UnvalidatedProducts { get; }
    }
}
