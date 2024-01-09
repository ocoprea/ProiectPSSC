using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Domain.Models
{
    public record ValidateInfo(OrderId OrderId, OrderAdress OrderAdress, PayMetod PayMetod, double Price);
}
