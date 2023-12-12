using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.Invoice;
using LanguageExt;
using ProiectPSSC.Domain.Repositories;
using System.Reflection.Metadata.Ecma335;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Specialized;
using LanguageExt.SomeHelp;
using LanguageExt.ClassInstances;

namespace ProiectPSSC.Domain.Operations
{
   public class FinishingTheOrderOperations
    {
        public static async Task<IInvoice> PreValidateInvoice(IReadOnlyCollection<UnvalidateInvoice> invoices)
        {
            List<PreValidateInvoice> preValidateInvoices_ = new();
            foreach (var invoice in invoices)
            {
                string? reason_ = null;
                var validOrderId = await TryParseOrderId(invoice.orderId)
                    .ToEither(u => "Invalid order id [" + invoice.orderId + "]");
                validOrderId.Match(Right: async orderId =>
                {
                    var validAdress = await TryAdress(invoice.adress)
                    .ToEither(u => "Invalid adress [" + invoice.adress + "]");
                    validAdress.Match(Right: async validAdress =>
                    {
                        var validPayMetod = await TryPayMetod(invoice.payMetod)
                        .ToEither(u => "Invalid pay metod [" + invoice.payMetod + "]");
                        validPayMetod.Match(Right: validPayMetod =>{ preValidateInvoices_.Add(new PreValidateInvoice(orderId, validAdress, validPayMetod)); },
                                            Left: reason => { reason_ = reason; });
                
                    },              Left: reason => { reason_ = reason; });
                },                  Left: reason => { reason_ = reason; });
                if (!string.IsNullOrEmpty(reason_))
                {
                    reason_ += " for Order " + invoice.orderId;
                    return new UnValidatedInfo(invoices, reason_);
                }
            }
            return new PreValidatedInfo(preValidateInvoices_.ToArray());
        }

        private static TryAsync<OrderId> TryParseOrderId(string Id) => async () =>
        {
            OrderId orderId;
            if (OrderId.TryParse(Id, out orderId))
                return orderId;
            else
                throw new Exception();
        };
        private static TryAsync<OrderAdress> TryAdress(string adress) => async () =>
        {
            OrderAdress orderAdress;
            if (OrderAdress.TryParse(adress, out orderAdress))
                return orderAdress;
            else
                throw new Exception();
        };

        private static TryAsync<PayMetod> TryPayMetod(string payMetod) => async () =>
        {
            PayMetod pay;
            if (PayMetod.TryParse(payMetod, out pay))
                return pay;
            else throw new Exception();
        };
    }
}
