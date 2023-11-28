using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Models
{
    public class ProductDto
    {
        public int IdProdus { get; set; }
        public string CodProdus { get; set; }
        public string Denumire { get; set; }
        public int Cantitate { get; set; }
        public double Pret { get; set; }
    }
}
