using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class DocumentRead
    {
        string IDTicket { get; set; }
        int RowNum { get; set; }
        string IDLectura { get; set; }
        string ItemCode { get; set; }
        int Cantidad { get; set; }
        int UoM { get; set; }
        string Codebar { get; set; }
        string TipoLectura { get; set; }
        string EmpID { get; set; }
        string CreateDate { get; set; }
        string CreateTime { get; set; }
        string UpdateDate { get; set; }
        string UpdateTime { get; set; }
    }
}
