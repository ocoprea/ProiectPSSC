using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    [AsChoice]
    public static partial class GenerateTheInvoiceEvent
    {
        public interface IGenerateTheInvoiceEvent { }
        public record GenerateTheInvoiceSuccededEvent : IGenerateTheInvoiceEvent { 
            public GenerateTheInvoiceSuccededEvent(int invoiceId)
            {
                InvoiceId = invoiceId;
            }
            public int InvoiceId { get;}
        }

        public record GenerateTheInvoiceFailedEvent : IGenerateTheInvoiceEvent
        {
            public GenerateTheInvoiceFailedEvent(string reason)
            {
                Reason = reason;
            }
            public string Reason { get;}
        }
    }
}
