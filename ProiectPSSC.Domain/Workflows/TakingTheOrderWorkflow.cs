using ProiectPSSC.Domain.Commands;
using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.TakingTheOrderEvent;
using static ProiectPSSC.Domain.Models.Cart;
using ProiectPSSC.Domain.Operations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProiectPSSC.Domain.Repositories;

namespace ProiectPSSC.Domain.Workflows
{
    public class TakingTheOrderWorkflow
    {
        private readonly IProductsRepository productsRepository_;
        public TakingTheOrderWorkflow(IProductsRepository productsRepository)
        {
            productsRepository_ = productsRepository;
        }
        public async Task <ITakingTheOrderEvent> ExecuteAsync (TakingTheOrderCommand command)
        {
            ICart cart = await TakingTheOrderOperations.PreValidateProducts(command.UnvalidatedProducts);
            cart = await TakingTheOrderOperations.ValidateProducts(cart, productsRepository_);

            return cart.Match(whenUnvalidatedCart: cart => new TakingTheOrderFailedEvent("reason") as ITakingTheOrderEvent,
                       whenPreValidatedCart: cart => new TakingTheOrderFailedEvent("123"),
                       whenValidatedCart: cart => new TakingTheOrderSuccededEvent(),
                       whenInvalidatedCart: cart => new TakingTheOrderFailedEvent(cart.Reason),
                       whenCalculatedCart: cart => new TakingTheOrderFailedEvent("123"));
        }
    }
}
