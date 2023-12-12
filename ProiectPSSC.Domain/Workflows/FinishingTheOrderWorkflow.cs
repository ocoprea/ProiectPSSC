using ProiectPSSC.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Workflows
{
    public class FinishingTheOrderWorkflow
    {
        private readonly IOrdersRepository ordersRepository_;
        private readonly IInvoiceRepository invoiceRepository_;

        public FinishingTheOrderWorkflow(IOrdersRepository ordersRepository_, IInvoiceRepository invoiceRepository_)
        {
            this.ordersRepository_ = ordersRepository_;
            this.invoiceRepository_ = invoiceRepository_;
        }   
    }
}
