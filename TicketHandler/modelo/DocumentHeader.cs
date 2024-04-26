using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    public class DocumentHeader
    {
        public string IDTienda { get; set; }
        public string IDCaja { get; set; }
        public string TipoDocumento { get; set; }
        public int ObjType { get; set; }
        public string EmpID { get; set; }
        public string Terminal { get; set; }
        public string IDTicket { get; set; }
        public string FromWarehouseCode { get; set; }
        public string ToWarehouseCode { get; set; }
        public string CardCodeFactura { get; set; }
        public string ShipTo { get; set; } = "";
        public string CardCode { get; set; }
        public string bReplicaFactura { get; set; }
        public double? Importe { get; set; }
        public double? ImporteImpuesto { get; set; }
        public double? ImporteTotal { get; set; } = default;
        public double? ImporteCobrado { get; set; } = default;
        public double? ImportePromocion { get; set; } = default;
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string DocDueDate { get; set; } = "";
        public string B_Cobrado { get; set; }
        public string WhsA { get; set; } = "";
        public string WhsC { get; set; } = "";
    }
}
