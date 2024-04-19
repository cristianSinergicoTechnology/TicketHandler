using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.utils
{
    public class TicketHandlerUtils
    {
        public static Decimal FormatAsMoney(Decimal num)
        {
            return Decimal.Round(num, 2, MidpointRounding.AwayFromZero);
        }
    }
}
