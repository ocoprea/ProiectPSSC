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

namespace ProiectPSSC.Domain.Workflows
{
    public class TakingTheOrderWorkflow
    {
        public async Task <ITakingTheOrderEvent> ExecuteAsync (TakingTheOrderCommand command)
        {
            ICart cart = await TakingTheOrderOperations.PreValidateProducts(command.UnvalidatedProducts);

            return cart.Match(whenUnvalidatedCart: cart => new TakingTheOrderFailedEvent("reason") as ITakingTheOrderEvent,
                       whenPreValidatedCart: cart => new TakingTheOrderSuccededEvent(),
                       whenValidatedCart: cart => new TakingTheOrderFailedEvent("asd"),
                       whenInvalidatedCart: cart => new TakingTheOrderFailedEvent(cart.Reason),
                       whenCalculatedCart: cart => new TakingTheOrderFailedEvent("123"));
        }
    }
}
