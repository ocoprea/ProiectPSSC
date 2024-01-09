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
using ProiectPSSC.Domain.Commands;

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
                    return new InValidatedInfo(invoices, reason_);
                }
            }
            return new PreValidatedInfo(preValidateInvoices_.ToArray());
        }

        public static async Task<IInvoice> ValidateInfos(IInvoice preValidateInfo, IInvoiceRepository invoiceRepository) => await preValidateInfo.MatchAsync(
            whenUnvalidatedInfo: async unvalidateInfo => unvalidateInfo,
            whenPreValidatedInfo: async preValidateInfo_ =>
            {
                string? reason = null;
                var preValidateInfos = preValidateInfo_.PreValidateInvoices;
                List<ValidateInfo> validInfos = new();
                
                foreach (var invoice in preValidateInfos)
                {
                   
                    var result = await invoiceRepository.TryFindCommand(invoice.orderId);
                    
                    result.Match(
                        Succ: foundCommand =>
                        {
                            if (foundCommand.Item2 == "Nefinalizata")
                            {
                                validInfos.Add(new ValidateInfo(invoice.orderId, invoice.orderAdress, invoice.payMetod, foundCommand.Item1));
                              
                            }
                            else
                            { reason = "Comanda deja finalizata";
                                
                            }
                        },
                        Fail: exception =>
                        {
                            reason = "Order not found";
                        });
                    if (!string.IsNullOrEmpty(reason))
                        return new InValidatedInfo(null, reason);
                }
                return new ValidatedInfo(validInfos.ToArray());
            },
            whenValidatedInfo: async validateInfo => validateInfo,
            whenInValidatedInfo: async invalidateInfo => invalidateInfo,
            whenGeneratedInvoice: async generateInvoice => generateInvoice
            );

        public static async Task<IInvoice> CreateNewInvoice(IInvoice validatedInfo, IInvoiceRepository invoiceRepository) => await validatedInfo.MatchAsync(
            whenUnvalidatedInfo: async unvalidateInfo => unvalidateInfo,
            whenPreValidatedInfo: async prevalidateInfo => prevalidateInfo,
            whenInValidatedInfo: async invalidateInfo => invalidateInfo,
            whenValidatedInfo: async validatedInfo =>
            {
                IInvoice invoice = validatedInfo;
                var q = from item in validatedInfo.ValidateInfos
                        select (item.OrderId, item.Price, item.OrderAdress, item.PayMetod).ToTuple();
                int orderId = Convert.ToInt32(q.FirstOrDefault().Item1.Value);
                float price = Convert.ToSingle(q.FirstOrDefault().Item2);
                string orderadress = Convert.ToString(q.FirstOrDefault().Item3.Value);
                string paymetod = Convert.ToString(q.FirstOrDefault().Item4.Value);
                var result = from it in invoiceRepository.TrySaveNewInvoice(orderId, price, orderadress, paymetod).ToEither(ex => -1)
                             select it;
                await result.Match(
                    Right: invoiceId => invoice = new GeneratedInvoice(validatedInfo.ValidateInfos, invoiceId),
                    Left: _ => invoice = validatedInfo);
                return invoice;
            },
            whenGeneratedInvoice : async generateInvoice => generateInvoice
            );

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
