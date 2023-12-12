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
        TryAsync<Tuple<double, string>> TryFindCommand(int id);
    }
}
