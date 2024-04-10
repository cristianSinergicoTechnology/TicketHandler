using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class DocumentLine
    {
        string IDTicket { get; set; }
        int RowNum { get; set; }
        string ItemCode { get; set; }
        string ItemName { get; set; }
        int Quantity { get; set; }
        int UoMEntry { get; set; }
        string UoM { get; set; }
        string? PromotionID { get; set; } = null;
        Decimal PrecioUnitario { get; set; }
        Decimal PorcentajeDescuento { get; set; }
        Decimal ImporteDescuento { get; set; }
        Decimal Importe { get; set; }
        Decimal ImporteIGIC { get; set; }
        Decimal ImporteTotal { get; set; }
        string EmpID {  get; set; }
        string CreateDate { get; set; }
        string CreateTime { get; set; }
        string UpdateDate { get; set; }
        string UpdateTime { get; set; }

    }
}
