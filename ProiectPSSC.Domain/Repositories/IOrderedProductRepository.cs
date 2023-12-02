using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Repositories
{
    public interface IOrderedProductRepository
    {
        TryAsync<Tuple<int, int, int>> TrySaveNewOrderedProducts(int productId, int orderId, int quantity);
    }
}
