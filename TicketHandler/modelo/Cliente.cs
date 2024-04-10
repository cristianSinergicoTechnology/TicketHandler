using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.modelo
{
    internal class Cliente
    {
        string CardCode { get; set; } = "";
        string CardName { get; set; } = "";
        string Address { get; set; } = "";
        string ZipCode { get; set; } = "";
        string Phone1 { get; set; } = "";
        string Phone2 { get; set; } = "";
        string ContactPerson { get; set; } = "";
        string Notes { get; set; } = "";
        Decimal Discount { get; set; } = default;
        string FederalTaxId { get; set; } = "";
        int PricesListNum { get; set; } = 0;
        string Celular { get; set; } = "";
        string City { get; set; } = "";
        string County { get; set; } = "";
        string Country { get; set; } = "";
        string EmailAddress { get; set; } = "";
        string CardForeignName { get; set; } = "";
        string PaymentMethodCode { get; set; } = "";
        string Territory { get; set; } = "";
        bool bTienda { get; set; } = false;
        string UpdateDate { get; set; } = "";
        string UpdateTime { get; set; } = "";
        string U_SEITienda { get; set; } = "";
        string U_SEIUsarTab { get; set; } = "";
        Decimal Puntos { get; set; } = new Decimal(3);
        string? LOPDfirm { get; set; } = null;
        string? LOPDdate { get; set; } = null;
        string? LOPDtime { get; set; } = null;
    }
}
