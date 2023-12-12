using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    [AsChoice]
    public static partial class Invoice
    {
        public interface IInvoice { }
        public record UnvalidatedInfo : IInvoice { 
        public UnvalidatedInfo(IReadOnlyCollection<UnvalidateInvoice> unvalidatedInvoices) 
            {
                UnvalidatedInfos = unvalidatedInvoices;
            }
            public IReadOnlyCollection<UnvalidateInvoice> UnvalidatedInfos { get; }
        }
        public record PreValidatedInfo : IInvoice
        {
            public PreValidatedInfo(IReadOnlyCollection<PreValidateInvoice> preValidatedInvoices)
            {
                PreValidateInvoices = preValidatedInvoices;
            }
            public IReadOnlyCollection<PreValidateInvoice > PreValidateInvoices { get; }
        }

        public record UnValidatedInfo : IInvoice
        {
            public UnValidatedInfo(IReadOnlyCollection<UnvalidateInvoice> unvalidatedInvoices, string reason)
            {
                UnvalidatedInvoices = unvalidatedInvoices;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidateInvoice>? UnvalidatedInvoices { get; }
            public string Reason { get; }
        }
    }
}
