using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC.Data.Models
{
    public class InvoiceDto
    {
        public int IdFactura { get; set; }
        public int IdComanda { get; set; }
        public float PretComanda { get; set; }
        public float Discount {  get; set; }
        public float TotalPlate {  get; set; }


    }
}
