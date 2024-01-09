using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProiectPSSC.Domain.Commands
{
    public record GenerateTheInvoiceCommand
    {
        public GenerateTheInvoiceCommand(IReadOnlyCollection<UnvalidateInvoice> unvalidateInvoices) {
            UnvalidateInvoices = unvalidateInvoices;
        }
        public IReadOnlyCollection<UnvalidateInvoice> UnvalidateInvoices { get; }

    }
}
