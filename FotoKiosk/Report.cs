using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Audio;

namespace FotoKiosk
{
    internal class Report
    {
        public int PhotoId { get; set; }
        public string Product { get; set; }
        public int Amount { get; set; }
        public double TotalPrice { get; set; }
    }
}
