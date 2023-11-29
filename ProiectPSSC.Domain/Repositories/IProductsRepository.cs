using LanguageExt;
using ProiectPSSC.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Repositories
{
    public interface IProductsRepository
    {
        TryAsync<string> TryGetExistingProduct(string product);
        TryAsync<Tuple<string, int, double>> TryGetProduct(ProductId productId);
    }
}
