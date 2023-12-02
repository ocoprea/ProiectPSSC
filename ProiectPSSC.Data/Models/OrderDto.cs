using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Models
{
    public class OrderDto
    {
        public int IdComanda { get; set; }
        public string Adresa { get; set; }
        public string MetodaPlata { get; set; }
        public double PretTotal { get; set; }
        public string StatusComanda { get; set; }
    }
}
