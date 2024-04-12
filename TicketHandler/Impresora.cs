using System.Reflection;
using System.Text;
using System.Text.Json;
using TicketHandler.modelo;
using TicketHandler.utils;

namespace TicketHandler
{
    internal static class Impresora
    {
        public static Dictionary<string, object> printTicket(Document? doc, List<ViaPago> viasPago, Cliente? cliente, bool proforma, string empleado, bool mostrarCodigoArticulo = false, string ruta = "./pruebaFactura.txt")
        {
            try
            {
                
                var tfnEmpresa = Utils.TELEFONO_EMPRESA;
                var footerLines = Utils.FOOTER;

                MemoryStream printer = new MemoryStream();
                printer.InicializarImpresora();
                printer.CambiarCodePage();
                printer.TituloFactura();
                printer.CentrarTexto();
                #region TICKET HEADER
                if (proforma)
                {
                    printer.TextoNegrita();
                    printer.WriteLn("FACTURA PROFORMA",1);
                }

                printer.TextoDefecto();
                printer.TextoGrande();
                printer.WriteLn(Utils.NOMBRE_EMPRESA, 1);

                printer.TextoDefecto();
                printer.WriteLn(Utils.DIRECCION_EMPRESA, 1);
                
                printer.TextoNegrita();
                printer.Write(GetBytes("TELF. "));
                printer.TextoDefecto();
                printer.WriteLn(tfnEmpresa, 1);

                printer.TextoNegrita();
                printer.Write(GetBytes("NIF: "));
                printer.TextoDefecto();
                printer.WriteLn(Utils.NIF, 2);
                #endregion

                printer.TextoCentradoIzquierda();
                printer.Write(GetBytes("Ticket: "));
                printer.TextoDefecto();
                printer.WriteLn($"{doc.DocumentHeader.IDTicket} {Utils.FECHA_CREACION} {Utils.HORA_CREACION}",2);

                printer.TextoNegrita();
                printer.Write(GetBytes("Le ha atendido: "));
                printer.TextoDefecto();
                printer.WriteLn(empleado, 2);
                
                var linesToSkip = proforma ? 1 : 2;
                printer.WriteLn($"Cliente: {cliente.CardName}",linesToSkip);
                if(proforma)
                {
                    printer.WriteLn($"NIF: {cliente.FederalTaxId}", 1);
                    printer.WriteLn($"Direccion: {cliente.Address}", 1);
                }

                printer.PrintProductsAndPayments(doc, viasPago, mostrarCodigoArticulo);

                printer.PrintFooter(footerLines);

                printer.SkipLines(3);

                printer.PrintBarcode(doc);

                // Rellanamos 5 filas para que no se corte información de la factura
                printer.SkipLines(5);
                printer.CortarFactura();

                printer.Close();

                return loadContent(printer.ToArray());

            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
                return new Dictionary<string, object>()
                {
                    { "data","ERROR" }
                };
            }
        }

        private static Dictionary<string,object> loadContent(byte[] bytes)
        {
            //JsonSerializerOptions options = new JsonSerializerOptions()
            //{
            //    WriteIndented = true,
            //};
            var json = new Dictionary<string, object>()
            {
                { "type","ticket" },
                { "data", bytes.ToList() },
            };

            var finalJson = JsonSerializer.Serialize(json);

            File.WriteAllText("./respuesta.json",finalJson);
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
            stream.Write(Commands.ESC);
            stream.Write(GetBytes('d'));
            stream.Write(GetBytes(numLinea));
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
        private static void PrintBarcode(this MemoryStream stream,Document ticket)
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
        private static void PrintProductsAndPayments(this MemoryStream stream, Document ticket, List<ViaPago> viasPago, bool mostrarCodigoArticulo)
        {

            stream.TextoNegrita();
            stream.WriteLn($"{"UDS DESCRIPCION".PadRight(33)}PRECIO IMPORTE",1);
            stream.TextoDefecto();
            stream.WriteLn($"{"".PadLeft(47,'=')}",1);
            if(ticket.DocumentHeader.ImportePromocion > Decimal.Zero)
            {
                ticket.DocumentLines.ForEach(line => {
                    stream.WriteLn($" {line.Quantity}  ${line.ItemName.Substring(0, 16).PadRight(20)}", 1);

                if (mostrarCodigoArticulo)
                {
                    stream.WriteLn($"{line.ItemCode.PadLeft(15)}", 1);
                }
            });

            } else
            {
                ticket.DocumentLines.ForEach(line =>
                {
                    stream.WriteLn($" {line.Quantity}  " +
                        $"{line.ItemName.Substring(0,16).PadRight(20)} " +
                        $"{Utils.FormatAsMoney(line.PrecioUnitario).ToString().PadLeft(12)} " + 
                        Utils.FormatAsMoney(line.ImporteTotal).ToString().PadLeft(7),
                        1);

                    if (mostrarCodigoArticulo)
                    {
                        stream.WriteLn(line.ItemCode.PadLeft(15), 1);
                    }
                });
            }
            stream.TextoDefecto();
            stream.WriteLn($"{"".PadLeft(47, '=')}", 1);

            var tipoDocumento = "SUBTOTAL:".PadLeft(30) +(ticket.DocumentHeader.TipoDocumento == Utils.DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.ImporteTotal.ToString().PadLeft(15); 
            
            stream.TextoNegrita();
            stream.Write(GetBytes(tipoDocumento));
            stream.Write(Euro());

            stream.WriteLn("", 2);

            var importeTotal = "TOTAL:".PadLeft(30) + (ticket.DocumentHeader.TipoDocumento == Utils.DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.ImporteTotal.ToString().PadLeft(15);

            stream.TextoNegrita();
            stream.Write(GetBytes(importeTotal));
            stream.Write(Euro());
            stream.TextoDefecto();

            stream.SkipLines(2);
            ticket.DocumentPayments.ForEach(payment =>
            {
                ViaPago? viaPago = viasPago.FirstOrDefault(paymentType => payment.EntryViaPago == paymentType.CreditCard);
                if(viaPago != null)
                {
                    var pago = (viaPago.Nombre.ToUpper().PadLeft(30) + (((ticket.DocumentHeader.TipoDocumento == Utils.DOCTYPE_ABONO ? "-" : "") + payment.Importe.ToString())).PadLeft(15));
                    stream.Write(GetBytes(pago));
                    stream.Write(Euro());
                }
                stream.SkipLines(1);
                if(payment.Entregado != null)
                {
                    var entregado = "ENTREGADO: ".PadLeft(30) + payment.Entregado.ToString().PadLeft(15);
                    stream.Write(GetBytes(entregado));
                    stream.Write(Euro());
                    stream.SkipLines(1);
                }
                if(payment.Cambio != null)
                {
                    var entregado = "Cambio: ".PadLeft(30) + payment.Cambio.ToString().PadLeft(15);
                    stream.Write(GetBytes(entregado));
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
            return euroBytes;
        }

        private static string EuroAsString()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var letra = (char)128;
            //return Encoding.GetEncoding(858).GetString();
            return letra.ToString();
        }


        #endregion

        #endregion

        
    }
}
