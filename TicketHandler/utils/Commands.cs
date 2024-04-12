using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketHandler.utils
{
    internal class Commands
    {
        public static readonly byte[] ESC = Encoding.UTF8.GetBytes(((char)27).ToString());
        public static readonly byte[] GS = Encoding.UTF8.GetBytes(((char)29).ToString());
        public static readonly byte[] SKIP_LINE = Encoding.UTF8.GetBytes(((char)13).ToString());
    }
}
