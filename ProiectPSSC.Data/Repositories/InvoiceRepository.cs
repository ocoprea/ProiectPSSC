using LanguageExt;
using LanguageExt.ClassInstances;
using Microsoft.EntityFrameworkCore;
using ProiectPSSC.Data.Models;
using ProiectPSSC.Domain.Models;
using ProiectPSSC.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        
        private readonly InvoiceContext invoiceContext_;

        public InvoiceRepository(InvoiceContext invoiceContext)
        {
            invoiceContext_ = invoiceContext;
        }

        

        public TryAsync<Tuple<double, string>> TryFindCommand(OrderId id) => async () =>
        {
           
            Tuple<double, string> Tuple = new(0.0, "0");
            var foundOrder = await invoiceContext_.Orders
                                                .Where(foundCommand => id.Value == foundCommand.IdComanda.ToString())
                                                .AsNoTracking()
                                                .ToListAsync();
           
            if (foundOrder.Count == 0)
           
                throw new OrderNotFoundException();
   
            else return new Tuple<double, string>(foundOrder[0].PretTotal, foundOrder[0].StatusComanda);

        };

        public TryAsync<Tuple<int, float, float, float>> TrySaveNewInvoice(int orderId, float orderPrice, string adresa, string metodaplata) => async () =>
        {
            InvoiceDto newInvoice = new();
            OrderDto newOrder = new OrderDto();
            float discount;
            newInvoice.IdComanda = orderId;
            newInvoice.PretComanda = orderPrice;
            if (orderPrice > 100)
                discount = orderPrice * 0.1f;
            else
               discount = 0;
            newInvoice.Discount = discount;
            newInvoice.TotalPlata = orderPrice - discount;

            await invoiceContext_.AddAsync(newInvoice);
            await invoiceContext_.SaveChangesAsync();
            newOrder.IdComanda = orderId;
            newOrder.Adresa = adresa;
            newOrder.PretTotal = orderPrice;
            newOrder.MetodaPlata = metodaplata;
            newOrder.StatusComanda = "Finalizata";
            invoiceContext_.Update(newOrder);
            await invoiceContext_.SaveChangesAsync();
            return new Tuple<int, float, float, float>(newInvoice.IdComanda, newInvoice.PretComanda, newInvoice.Discount, newInvoice.TotalPlata);
        };

        public class OrderNotFoundException : ApplicationException
        { }
    }
}
