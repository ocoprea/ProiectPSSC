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
        public record UnvalidatedInfo : IInvoice
        {
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
            public IReadOnlyCollection<PreValidateInvoice> PreValidateInvoices { get; }
        }

        public record InValidatedInfo : IInvoice
        {
            public InValidatedInfo(IReadOnlyCollection<UnvalidateInvoice> unvalidatedInvoices, string reason)
            {
                UnvalidatedInvoices = unvalidatedInvoices;
                Reason = reason;
            }
            public IReadOnlyCollection<UnvalidateInvoice>? UnvalidatedInvoices { get; }
            public string Reason { get; }
        }

        public record ValidatedInfo : IInvoice
        {
            public ValidatedInfo(IReadOnlyCollection<ValidateInfo> validateInfos)
            {
                ValidateInfos = validateInfos;
            }
            public IReadOnlyCollection<ValidateInfo> ValidateInfos { get; }
        }

        public record GeneratedInvoice : IInvoice
        {
            private IReadOnlyCollection<ValidateInfo> validateInfos;
            private Tuple<int, float, float, float> invoiceId;

            public GeneratedInvoice(IReadOnlyCollection<ValidatedInfo> validatedInfos, int invoiceId)
            {
                ValidatedInfos = validatedInfos;
                InvoiceId = invoiceId;
            }

            public GeneratedInvoice(IReadOnlyCollection<ValidateInfo> validateInfos, Tuple<int, float, float, float> invoiceId)
            {
                this.validateInfos = validateInfos;
                this.invoiceId = invoiceId;
            }

            public int InvoiceId { get; }
            public IReadOnlyCollection<ValidatedInfo> ValidatedInfos { get; }
        }
    }
}
