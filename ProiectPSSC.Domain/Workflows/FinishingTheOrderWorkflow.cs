using ProiectPSSC.Domain.Commands;
using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProiectPSSC.Domain.Models.TakingTheOrderEvent;
using static ProiectPSSC.Domain.Models.Invoice;
using ProiectPSSC.Domain.Operations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProiectPSSC.Domain.Repositories;
using static ProiectPSSC.Domain.Models.GenerateTheInvoiceEvent;

namespace ProiectPSSC.Domain.Workflows
{
    public class FinishingTheOrderWorkflow
    {
     
        private readonly IInvoiceRepository invoiceRepository_;

        public FinishingTheOrderWorkflow(IInvoiceRepository invoiceRepository_)
        {
            
            this.invoiceRepository_ = invoiceRepository_;
        }   
        public async Task<IGenerateTheInvoiceEvent> ExecuteAsync (GenerateTheInvoiceCommand command)
        {
            IInvoice invoice = await FinishingTheOrderOperations.PreValidateInvoice(command.UnvalidateInvoices);
            invoice = await FinishingTheOrderOperations.ValidateInfos(invoice, invoiceRepository_);
            invoice = await FinishingTheOrderOperations.CreateNewInvoice(invoice, invoiceRepository_);

            return invoice.Match(whenUnvalidatedInfo: invoice => new GenerateTheInvoiceFailedEvent("Unexpected unvalidated state") as IGenerateTheInvoiceEvent,
                                 whenPreValidatedInfo: invoice => new GenerateTheInvoiceFailedEvent("Unexpected prevalidated state"),
                                 whenValidatedInfo: invoice => new GenerateTheInvoiceFailedEvent("Unexpected validated state"),
                                 whenInValidatedInfo: invoice => new GenerateTheInvoiceFailedEvent("Unexpected validated state"),
                                 whenGeneratedInvoice: invoice => new GenerateTheInvoiceSuccededEvent(invoice.InvoiceId));         
                                 
        }
    }
}
