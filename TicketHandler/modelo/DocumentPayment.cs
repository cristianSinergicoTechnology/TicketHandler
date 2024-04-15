using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    public class DocumentPayment
    {
        public string IDTienda { get; set; }
        public string IDCaja { get; set; }
        public string IDTicket { get; set; }
        public string IDCobro { get; set; }
        public string IDAnulacion { get; set; }
        public string? IDCierre { get; set; } = null;
        public string TipoCobro { get; set; }
        public Decimal Importe { get; set; }
        public Decimal? Entregado { get; set; } = null;
        public Decimal? Cambio { get; set; } = null;
        public string? ViaPago { get; set; } = null;
        public int EntryViaPago { get; set; }
        public string EmpID { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string? TipoDocumento { get; set; } = null;
        public string PromoID { get; set; } = "";
        public int PromoVecesAplicada { get; set; } = 0;

    }
}
