using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadeviVentasApi
{
    public class Configuration
    {
        public long AntesCentralID { get; set; }
        public long DespuesCentralID { get; set; }
        public long RotativaID { get; set; }
        public string UrlFrontend { get; set; }
    }
}
