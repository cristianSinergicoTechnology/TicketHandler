using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class Cliente
    {
        public string CardCode { get; set; } = "";
        public string CardName { get; set; } = "";
        public string Address { get; set; } = "";
        public string ZipCode { get; set; } = "";
        public string Phone1 { get; set; } = "";
        public string Phone2 { get; set; } = "";
        public string ContactPerson { get; set; } = "";
        public string Notes { get; set; } = "";
        public Decimal Discount { get; set; } = default;
        public string FederalTaxId { get; set; } = "";
        public int PricesListNum { get; set; } = 0;
        public string Celular { get; set; } = "";
        public string City { get; set; } = "";
        public string County { get; set; } = "";
        public string Country { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public string CardForeignName { get; set; } = "";
        public string PaymentMethodCode { get; set; } = "";
        public string Territory { get; set; } = "";
        public bool bTienda { get; set; } = false;
        public string UpdateDate { get; set; } = "";
        public string UpdateTime { get; set; } = "";
        public string U_SEITienda { get; set; } = "";
        public string U_SEIUsarTab { get; set; } = "";
        public Decimal Puntos { get; set; } = new Decimal(3);
        public string? LOPDfirm { get; set; } = null;
        public string? LOPDdate { get; set; } = null;
        public string? LOPDtime { get; set; } = null;
    }
}
