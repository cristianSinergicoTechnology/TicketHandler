using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    public class DocumentRead
    {
        public string IDTicket { get; set; }
        public int RowNum { get; set; }
        public string IDLectura { get; set; }
        public string ItemCode { get; set; }
        public int Cantidad { get; set; }
        public int UoM { get; set; }
        public string Codebar { get; set; }
        public string TipoLectura { get; set; }
        public string EmpID { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
    }
}
