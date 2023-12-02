using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Models
{
    public class OrderedProductDto
    {
        public int IdProdusComandat { get; set; }
        public int IdProdus { get; set; }
        public int IdComanda { get; set; }
        public int Cantitate { get; set; }
    }
}
