using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.utils
{
    internal class Utils
    {
        public static Decimal FormatAsMoney(Decimal num)
        {
            return Decimal.Round(num,2,MidpointRounding.AwayFromZero);
        }

        #region CONSTANTES
        // constantes empresa
        public const string NOMBRE_EMPRESA = "TABACO BARATO S.L.";
        public const string DIRECCION_EMPRESA = "C/Avenida Castillo 39, 38200";
        public const string FECHA_CREACION = "10/04/2024";
        public const string HORA_CREACION = "12:20";
        public const string NIF = "BAS34284239325C";
        public const string TELEFONO_EMPRESA = "";
        public static Dictionary<string, bool> FOOTER = new Dictionary<string, bool>
                {
                    { "www.TABACOBARATO.com", false },
                    { "TU PRIMERA COMPRA ONLINE CON UN", false },
                    { "-10% DE DESCUENTO CON EL CODIGO", true },
                    { "PROMOCIONAL: SOYFUMADOR", true },
                    { "NO LO DEJES ESCAPAR", false },
                    { "", false },
                    { "", false },
                    { "SIGUENOS EN FACEBOOK", false }
                };


        // constantes cliente
        public const string NOMBRE_CLIENTE = "PEDRO";
        public const string DIRECCION_CLIENTE = "C/La Paz 20, 38152";

        // constantes ticket
        public const string DOCTYPE_ABONO = "TA";
        #endregion
    }
}
