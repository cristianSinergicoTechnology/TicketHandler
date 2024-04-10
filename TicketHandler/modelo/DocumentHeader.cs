using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class DocumentHeader
    {
        string IDTienda { get; set; }
        string IDCaja { get; set; }
        string TipoDocumento { get; set; }
        int ObjType { get; set; }
        string EmpID { get; set; }
        string Terminal { get; set; }
        string IDTicket { get; set; }
        string FromWarehouseCode { get; set; }
        string ToWarehouseCode { get; set; }
        string CardCodeFactura { get; set; }
        string? ShipTo { get; set; }
        string CardCode { get; set; }
        string bReplicaFactura { get; set; }
        Decimal? Importe { get; set; }
        Decimal? ImporteImpuesto { get; set; }
        Decimal? ImporteTotal { get; set; } = default;
        Decimal? ImporteCobrado { get; set; } = default;
        string CreateDate { get; set; }
        string CreateTime { get; set; }
        string UpdateDate { get; set; }
        string UpdateTime { get; set; }
        string DocDueDate { get; set; } = "";
        string B_Cobrado { get; set; }
        string WhsA { get; set; } = "";
        string WhsC { get; set; } = "";
    }
}
