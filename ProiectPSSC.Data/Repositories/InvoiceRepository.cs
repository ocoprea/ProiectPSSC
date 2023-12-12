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
        private readonly OrderingContext orderingContext_;

        public InvoiceRepository(OrderingContext orderingContext)
        {
            orderingContext_ = orderingContext;
        }

        public TryAsync<Tuple<double, string>> TryFindCommand(int id) => async () =>
        {
            Tuple<double, string> Tuple = new(0.0, "0");
            var foundOrder = await orderingContext_.Orders
                                                .Where(foundCommand => id == foundCommand.IdComanda)
                                                .AsNoTracking()
                                                .ToListAsync();
            if (foundOrder.Count == 0)

                throw new OrderNotFoundException();
            else return new Tuple<double, string>(foundOrder[0].PretTotal, foundOrder[0].StatusComanda);

        };

        public class OrderNotFoundException : ApplicationException
        { }
    }
}
