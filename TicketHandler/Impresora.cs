using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using TicketHandler.modelo;
using TicketHandler.utils;
using System.Runtime.CompilerServices;

namespace TicketHandler
{
    public static class Impresora
    {
        const string DOCTYPE_ABONO = "TA";

        public static Dictionary<string, object> printTicket(Document doc, InfoEmpresa infoEmpresa
            , List<ViaPago> viasPago, Cliente cliente, bool proforma, string empleado, bool printFooter = false, string ruta = "./pruebaFactura.txt")
        {
            try
            {
                var tfnEmpresa = infoEmpresa.Telefono;
                var footerLines = infoEmpresa.Footer;

                MemoryStream printer = new MemoryStream();
                printer.InicializarImpresora();
                printer.CambiarCodePage();
                printer.TituloFactura();
                printer.CentrarTexto();
                #region TICKET HEADER
                if (proforma)
                {
                    printer.TextoNegrita();
                    printer.WriteLn("FACTURA PROFORMA", 1);
                }

                printer.TextoDefecto();
                printer.TextoGrande();
                printer.WriteLn(infoEmpresa.Nombre, 1);

                printer.TextoDefecto();
                printer.WriteLn(infoEmpresa.Direccion, 1);

                printer.TextoNegrita();
                printer.Write(GetBytes("TELF. "));
                printer.TextoDefecto();
                printer.WriteLn(tfnEmpresa, 1);

                printer.TextoNegrita();
                printer.Write(GetBytes("NIF: "));
                printer.TextoDefecto();
                printer.WriteLn(infoEmpresa.NIF, 2);
                #endregion

                printer.TextoCentradoIzquierda();
                printer.Write(GetBytes("Ticket: "));
                printer.TextoDefecto();
                printer.WriteLn($"{doc.DocumentHeader.IDTicket} {infoEmpresa.FechaCreacion} {infoEmpresa.HoraCreacion}", 2);

                printer.TextoNegrita();
                printer.Write(GetBytes("Le ha atendido: "));
                printer.TextoDefecto();
                printer.WriteLn(empleado, 2);

                var linesToSkip = proforma ? 1 : 2;
                printer.WriteLn($"Cliente: {cliente.CardName}", linesToSkip);
                if (proforma)
                {
                    printer.WriteLn($"NIF: {cliente.FederalTaxId}", 1);
                    printer.WriteLn($"Direccion: {cliente.Address}", 1);
                }

                printer.PrintProductsAndPayments(doc, viasPago);

                if (printFooter)
                {
                    printer.PrintFooter(footerLines);

                    printer.SkipLines(3);

                    printer.PrintBarcode(doc);
                }

                // Rellanamos 5 filas para que no se corte información de la factura
                printer.SkipLines(5);
                printer.CortarFactura();

                printer.Close();

                return loadContent(printer.ToArray());

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return new Dictionary<string, object>()
                {
                    { "errorMessage", $"Error al generar ticket: {ex}" },
                    { "data","" }
                };
            }
        }

        private static Dictionary<string, object> loadContent(byte[] bytes)
        {
            //JsonSerializerOptions options = new JsonSerializerOptions()
            //{
            //    WriteIndented = true,
            //};
            var json = new Dictionary<string, object>()
            {
                { "errorMessage","" },
                { "data", bytes.ToList() },
            };

            return json;
        }



        #region FUNCIONES HELPER IMPRESORA

        /// <summary>
        /// Función que se debe ejecutar SIEMPRE ANTES DE IMPRIMIR
        /// </summary>
        /// <param name="stream"></param>
        private static void InicializarImpresora(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('@'));
            stream.Write(Commands.SKIP_LINE);
        }

        /// <summary>
        /// Escribe el contenido y luego salta <paramref name="linesToSkip"/> lineas.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="content"></param>
        /// <param name="linesToSkip"></param>
        private static void WriteLn(this MemoryStream stream, string content, int linesToSkip)
        {
            stream.Write(Encoding.Default.GetBytes(content));
            SkipLines(stream, linesToSkip);
        }
        /// <summary>
        /// Salta <paramref name="numLinea"/> lineas
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="numLinea"></param>
        private static void SkipLines(this MemoryStream stream, int numLinea)
        {
            stream.Write(Commands.SKIP_LINE);
            for (int i = 0; i < numLinea; i++)
            {
                stream.Write(Commands.LF);
            }
            //stream.Write(Commands.ESC);
            //stream.Write(GetBytes('d'));
            //stream.Write(GetBytes(numLinea));
        }

        /// <summary>
        /// Se encarga de generar el símbolo del euro €
        /// </summary>
        /// <param name="stream"></param>
        private static void CambiarCodePage(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('t'));
            stream.Write(GetBytes(19));
        }

        /// <summary>
        /// Se encarga de cortar el papel de la factura
        /// </summary>
        /// <param name="stream"></param>
        private static void CortarFactura(this MemoryStream stream)
        {
            stream.Write(Commands.GS);
            stream.Write(GetBytes('V'));
            stream.Write(GetBytes('0'));
        }

        /// <summary>
        /// Cambia al tamaño grande de la letra
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoGrande(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(16));
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(32));
        }

        /// <summary>
        /// Cambia al modo de texto en negrita
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoNegrita(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(8));
        }

        /// <summary>
        /// Fuente para el título de la factura
        /// </summary>
        /// <param name="stream"></param>
        private static void TituloFactura(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(8));
        }

        /// <summary>
        /// Fuente con texto centrado
        /// </summary>
        /// <param name="stream"></param>
        private static void CentrarTexto(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('a'));
            stream.Write(GetBytes(1));
        }

        /// <summary>
        /// Configura el alto del código de barras
        /// </summary>
        /// <param name="stream"></param>
        private static void ConfigurarCodigoBarras(this MemoryStream stream)
        {
            stream.Write(Commands.GS);
            stream.Write(GetBytes('h'));
            stream.Write(GetBytes(65));
            stream.Write(Commands.GS);
            stream.Write(GetBytes('w'));
            stream.Write(GetBytes(2));
            stream.Write(Commands.GS);
            stream.Write(GetBytes('k'));
            stream.Write(GetBytes(4));
        }
        /// <summary>
        /// Fuente por defecto
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoDefecto(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(2));
        }
        /// <summary>
        /// Fuente con texto centrado verticalmente y a la izquierda
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoCentradoIzquierda(this MemoryStream stream)
        {
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('a'));
            stream.Write(GetBytes(0));
        }
        /// <summary>
        /// Función para imprimir mensaje personalizaddo de la empresa
        /// </summary>
        /// <param name="footerLines"></param>
        /// <param name="stream"></param>
        private static void PrintFooter(this MemoryStream stream, Dictionary<string, bool> footerLines)
        {
            stream.CentrarTexto();
            footerLines.ToList().ForEach(line =>
            {
                if (line.Value)
                    stream.TextoNegrita();
                else
                    stream.TextoDefecto();
                stream.WriteLn(line.Key, 1);

            });
            SkipLines(stream, 2);
        }
        /// <summary>
        /// Función para imprimir el código de barras del ticket.
        /// </summary>
        /// <param name="ticket"></param>
        /// <param name="stream"></param>
        private static void PrintBarcode(this MemoryStream stream, Document ticket)
        {
            stream.ConfigurarCodigoBarras();
            stream.Write(GetBytes(ticket.DocumentHeader.IDTicket));
            stream.Write(GetBytes(0));
            SkipLines(stream, 2);
        }
        /// <summary>
        /// Función para imprimir los productos, vías de pago e importes
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ticket"></param>
        /// <param name="viasPago"></param>
        private static void PrintProductsAndPayments(this MemoryStream stream, Document ticket, List<ViaPago> viasPago)
        {

            stream.TextoNegrita();
            stream.WriteLn($"Codigo  Descrip.  Precio  Dto%  Dto E  IGIC  Importe", 1);
            stream.TextoDefecto();
            stream.WriteLn($"{"".PadLeft(47, '=')}", 1);
                ticket.DocumentLines.ForEach(line =>
                {
                    string itemName = new string(line.ItemName.Take(16).ToArray());
                    stream.WriteLn($" {line.ItemCode} " +
                        $"{itemName} " +
                        $"{TicketHandlerUtils.FormatAsMoney(line.PrecioUnitario).ToString()} " +
                        $"{line.Quantity} " +
                        $"{line.PorcentajeDescuento} " +
                        $"{line.ImporteDescuento} " +
                        $"{line.ImporteIGIC} " +
                        TicketHandlerUtils.FormatAsMoney(line.ImporteTotal).ToString().PadLeft(7),
                        1);
                });
            stream.TextoDefecto();
            stream.WriteLn($"{"".PadLeft(47, '=')}", 1);

            var tipoDocumento = "SUBTOTAL:".PadLeft(30) + (ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.ImporteTotal.ToString().PadLeft(15);

            stream.TextoNegrita();
            stream.Write(GetBytes(tipoDocumento));
            stream.Write(Euro());

            stream.WriteLn("", 2);

            var importeTotal = "TOTAL:".PadLeft(30) + (ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.ImporteTotal.ToString().PadLeft(15);

            stream.TextoNegrita();
            stream.Write(GetBytes(importeTotal));
            stream.Write(Euro());
            stream.TextoDefecto();

            stream.SkipLines(2);
            ticket.DocumentPayments.ForEach(payment =>
            {
                ViaPago viaPago = viasPago.FirstOrDefault(paymentType => payment.EntryViaPago == paymentType.CreditCard);
                if (viaPago != null)
                {
                    var pago = (viaPago.Nombre.ToUpper().PadLeft(30) + (((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + payment.Importe.ToString())).PadLeft(15));
                    stream.Write(GetBytes(pago));
                    stream.Write(Euro());
                }
                stream.SkipLines(1);
                    var entregado = "ENTREGADO: ".PadLeft(30) + payment.Entregado.ToString().PadLeft(15);
                    stream.Write(GetBytes(entregado));
                    stream.Write(Euro());
                    stream.SkipLines(1);
                if (payment.Cambio != Decimal.Zero)
                {
                    var cambio = "CAMBIO: ".PadLeft(30) + payment.Cambio.ToString().PadLeft(15);
                    stream.Write(GetBytes(cambio));
                    stream.Write(Euro());
                    stream.SkipLines(1);
                }
            });

            stream.SkipLines(3);
        }


        #region FUNCIONES MAPPER BYTES

        private static byte[] GetBytes(char letra)
        {
            return Encoding.Default.GetBytes(letra.ToString());
        }

        private static byte[] GetBytes(int numero)
        {
            return Encoding.Default.GetBytes(((char)numero).ToString());
        }

        private static byte[] GetBytes(string content)
        {
            return Encoding.Default.GetBytes(content);
        }

        private static byte[] Euro()
        {
            char euro = '€';
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var euroBytes = Encoding.GetEncoding(858).GetBytes(euro.ToString());
            //byte[] euroBytes = { 0xA4 };
            return euroBytes;
        }

        private static void Write(this MemoryStream stream, byte[] content)
        {
            stream.Write(content,0,content.Length);
        }



        #endregion

        #endregion


    }
}
