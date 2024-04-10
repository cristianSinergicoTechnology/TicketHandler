using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class Document
    {
        public DocumentHeader DocumentHeader {  get; set; }
        public List<DocumentLine> DocumentLines { get; set; }
        public List<DocumentRead> DocumentReads { get; set; }
        public List<DocumentPayment> DocumentPayments { get; set; }
    }
}
