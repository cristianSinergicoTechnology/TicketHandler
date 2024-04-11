using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class ViaPago
    {
        public string IDTienda { get; set; } = "";
        public string IDCaja { get; set; } = "";
        public int CreditCard { get; set; }
        public string CtaContable { get; set; } = "";
        public string Nombre { get; set; } = "";
        public string bPorDefecto { get; set; } = "";
        public string B0bMgmt { get; set; } = "";
        public Decimal OpenBalnc { get; set; } = Decimal.Zero;

    }
}
