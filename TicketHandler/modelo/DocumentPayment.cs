using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class DocumentPayment
    {
        string IDTienda { get; set; }
        string IDCaja { get; set; }
        string IDTicket { get; set; }
        string IDCobro { get; set; }
        string IDAnulacion { get; set; }
        string? IDCierre { get; set; } = null;
        string TipoCobro { get; set; }
        Decimal Importe { get; set; }
        Decimal? Entregado { get; set; } = null;
        Decimal? Cambio { get; set; } = null;
        string? ViaPago { get; set; } = null;
        int EntryViaPago { get; set; }
        string EmpID { get; set; }
        string CreateDate { get; set; }
        string CreateTime { get; set; }
        string UpdateDate { get; set; }
        string UpdateTime { get; set; }
        string? TipoDocumento { get; set; } = null;
        string PromoID { get; set; } = "";
        int PromoVecesAplicada { get; set; } = 0;

    }
}
