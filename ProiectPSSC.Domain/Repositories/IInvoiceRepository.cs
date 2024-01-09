using LanguageExt;
using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Repositories
{
    public interface IInvoiceRepository
    {
        TryAsync<Tuple<double, string>> TryFindCommand(OrderId id);
        TryAsync<Tuple<int, float, float, float>> TrySaveNewInvoice(int orderId, float orderPrice, string orderadress, string paymetod);
    }
}
