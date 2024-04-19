using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    public class InfoEmpresa
    {
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string FechaCreacion { get; set; }
        public string HoraCreacion { get; set; }
        public string NIF { get; set; }
        public string Telefono { get; set; }
        public Dictionary<string, bool> Footer { get; set; }

    }
}
