using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    public class DocumentLine
    {
        public string IDTicket { get; set; }
        public int RowNum { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public int UoMEntry { get; set; }
        public string UoM { get; set; }
        public string? PromotionID { get; set; } = null;
        public Decimal PrecioUnitario { get; set; }
        public Decimal PorcentajeDescuento { get; set; }
        public Decimal ImporteDescuento { get; set; }
        public Decimal Importe { get; set; }
        public string TipoImpuesto { get; set; }
        public Decimal Impuesto { get; set; }
        public Decimal ImporteIGIC { get; set; }
        public Decimal ImporteTotal { get; set; }
        public string EmpID {  get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }

    }
}
