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
        private readonly IOrdersRepository ordersRepository_;
        private readonly IOrderedProductRepository orderedProductRepository_;
        public TakingTheOrderWorkflow
            (IProductsRepository productsRepository, IOrdersRepository ordersRepository, IOrderedProductRepository orderedProductRepository)
        {
            productsRepository_ = productsRepository;
            ordersRepository_ = ordersRepository;
            orderedProductRepository_ = orderedProductRepository;
        }
        public async Task <ITakingTheOrderEvent> ExecuteAsync (TakingTheOrderCommand command)
        {
            ICart cart = await TakingTheOrderOperations.PreValidateProducts(command.UnvalidatedProducts);
            cart = await TakingTheOrderOperations.ValidateProducts(cart, productsRepository_);
            cart = await TakingTheOrderOperations.CalculateProducts(cart);
            cart = await TakingTheOrderOperations.CreateNewOrder(cart, ordersRepository_, productsRepository_, orderedProductRepository_);

            return cart.Match(whenUnvalidatedCart: cart => new TakingTheOrderFailedEvent("Unexpected unvalidated state") as ITakingTheOrderEvent,
                              whenPreValidatedCart: cart => new TakingTheOrderFailedEvent("Unexpected prevalidated state"),
                              whenValidatedCart: cart => new TakingTheOrderFailedEvent("Total price cannot be calculated !"),
                              whenInvalidatedCart: cart => new TakingTheOrderFailedEvent(cart.Reason),
                              whenCalculatedCart: cart => new TakingTheOrderFailedEvent("Unexpected calculated state"),
                              whenAddedToOrder: cart => new TakingTheOrderSuccededEvent(cart.OrderId));
        }
    }
}
