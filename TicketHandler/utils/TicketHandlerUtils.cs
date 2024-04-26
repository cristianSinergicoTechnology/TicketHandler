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
        public static double FormatAsMoney(double num)
        {
            return Math.Round(num, 2);
        }
    }
}
