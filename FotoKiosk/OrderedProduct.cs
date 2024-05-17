using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FotoKiosk
{
    internal class OrderedProduct
    {
        public int fotoNum { get; set; }
        public string productName { get; set; }
        public int amt { get; set; }
        public double totalPrice { get; set; }
        public string totalPriceStr { get; set; }
    }
}
