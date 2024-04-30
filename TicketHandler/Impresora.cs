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

        public static Dictionary<string, object> printTicketTecMovil(Document doc, InfoEmpresa infoEmpresa, List<ViaPago> viasPago, Cliente cliente, bool proforma, string empleado, bool printFooter = false, bool test = false)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var tfnEmpresa = infoEmpresa.Telefono;
                var footerLines = infoEmpresa.Footer;

                MemoryStream printer = new MemoryStream();

                printer.InicializarImpresora();
                printer.CambiarCodePageTecMovil();
                printer.SeleccionarIdioma();
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
                    printer.WriteLn($"Dirección: {cliente.Address}", 1);
                }

                printer.PrintProductsAndPaymentsTecMovil(doc, viasPago);

                if (printFooter)
                {
                    printer.PrintFooter(footerLines);

                    printer.SkipLines(3);

                    printer.PrintBarcode(doc);
                }

                // Rellanamos 5 filas para que no se corte información de la factura
                printer.SkipLines(5);
                if (test)
                    printer.CortarFacturaTecPV();
                else
                    printer.CortarFacturaTecMovil();

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

        public static Dictionary<string, object> printTicketTecPV(Document doc, InfoEmpresa infoEmpresa, List<ViaPago> viasPago, Cliente cliente, bool proforma, string empleado, bool mostrarCodigoArticulo = false, bool printFooter = true)
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var tfnEmpresa = infoEmpresa.Telefono;
                var footerLines = infoEmpresa.Footer;

                MemoryStream printer = new MemoryStream();

                printer.InicializarImpresora();
                printer.CambiarCodePageTecPV();
                printer.SeleccionarIdioma();
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
                    printer.WriteLn($"Dirección: {cliente.Address}", 1);
                }

                printer.PrintProductsAndPaymentsTecPV(doc, viasPago, mostrarCodigoArticulo);

                if (printFooter)
                {
                    printer.PrintFooter(footerLines);

                    printer.SkipLines(3);

                    printer.PrintBarcode(doc);
                }

                // Rellanamos 5 filas para que no se corte información de la factura
                printer.SkipLines(5);
                printer.CortarFacturaTecPV();

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
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('@'));
            stream.Write(TecPVCommands.SKIP_LINE);
        }

        /// <summary>
        /// Escribe el contenido y luego salta <paramref name="linesToSkip"/> lineas.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="content"></param>
        /// <param name="linesToSkip"></param>
        private static void WriteLn(this MemoryStream stream, string content, int linesToSkip)
        {
            stream.Write(Encoding.GetEncoding(858).GetBytes(content));
            SkipLines(stream, linesToSkip);
        }
        /// <summary>
        /// Salta <paramref name="numLinea"/> lineas
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="numLinea"></param>
        private static void SkipLines(this MemoryStream stream, int numLinea)
        {
            stream.Write(TecPVCommands.SKIP_LINE);
            for (int i = 0; i < numLinea; i++)
            {
                stream.Write(TecPVCommands.LF);
            }
        }

        /// <summary>
        /// Cambia el CodePage para poder imprimir diéresis, tildes y símbolo € en la impresora TecPV
        /// </summary>
        /// <param name="stream"></param>
        private static void CambiarCodePageTecPV(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('t'));
            stream.Write(GetBytes(19));
        }

        /// <summary>
        /// Cambia el CodePage para poder imprimir diéresis, tildes y símbolo € en la impresora TecPV
        /// </summary>
        /// <param name="stream"></param>
        private static void CambiarCodePageTecMovil(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('t'));
            stream.Write(GetBytes(4));
        }


        /// <summary>
        /// Se encarga de cortar el papel de la factura
        /// </summary>
        /// <param name="stream"></param>
        private static void CortarFacturaTecPV(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.GS);
            stream.Write(GetBytes('V'));
            stream.Write(GetBytes('0'));
        }

        private static void CortarFacturaTecMovil(this MemoryStream stream)
        {
            stream.Write(TecMovilCommands.ESC);
            stream.Write(GetBytes('d'));
        }

        /// <summary>
        /// Cambia al tamaño grande de la letra
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoGrande(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(16));
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(32));
        }

        /// <summary>
        /// Cambia al modo de texto en negrita
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoNegrita(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(8));
        }

        /// <summary>
        /// Fuente para el título de la factura
        /// </summary>
        /// <param name="stream"></param>
        private static void TituloFactura(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(8));
        }

        /// <summary>
        /// Fuente con texto centrado
        /// </summary>
        /// <param name="stream"></param>
        private static void CentrarTexto(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('a'));
            stream.Write(GetBytes(1));
        }

        private static void SeleccionarIdioma(this MemoryStream stream)
        {
            stream.Write(TecMovilCommands.ESC);
            stream.Write(GetBytes('R'));
            stream.Write(GetBytes(11));
        }

        /// <summary>
        /// Configura el alto del código de barras
        /// </summary>
        /// <param name="stream"></param>
        private static void ConfigurarCodigoBarras(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.GS);
            stream.Write(GetBytes('h'));
            stream.Write(GetBytes(65));
            stream.Write(TecPVCommands.GS);
            stream.Write(GetBytes('w'));
            stream.Write(GetBytes(2));
            stream.Write(TecPVCommands.GS);
            stream.Write(GetBytes('k'));
            stream.Write(GetBytes(4));
        }
        /// <summary>
        /// Fuente por defecto
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoDefecto(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
            stream.Write(GetBytes('!'));
            stream.Write(GetBytes(2));
        }
        /// <summary>
        /// Fuente con texto centrado verticalmente y a la izquierda
        /// </summary>
        /// <param name="stream"></param>
        private static void TextoCentradoIzquierda(this MemoryStream stream)
        {
            stream.Write(TecPVCommands.ESC);
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
        /// Función para imprimir separador del ticket. de TecPV
        /// </summary>
        /// <param name="stream"></param>
        private static void SeparatorTecPV(this MemoryStream stream)
        {
            stream.WriteLn($"{"".PadLeft(47, '=')}", 1);
        }
        /// <summary>
        /// Función para imprimir separador del ticket. de TecMovil
        /// </summary>
        /// <param name="stream"></param>
        private static void SeparatorTecMovil(this MemoryStream stream)
        {
            stream.WriteLn($"{"".PadLeft(72, '=')}", 1);
        }


        /// <summary>
        /// Función para imprimir los productos, vías de pago e importes de TECPV
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ticket"></param>
        /// <param name="viasPago"></param>
        private static void PrintProductsAndPaymentsTecPV(this MemoryStream stream, Document ticket, List<ViaPago> viasPago, bool mostrarCodigoArticulo)
        {

            stream.TextoNegrita();
            stream.WriteLn($"{"UDS DESCRIPCION".PadRight(33)}PRECIO IMPORTE", 1);
            stream.TextoDefecto();
            stream.WriteLn($"{"".PadLeft(47, '=')}", 1);
            if (ticket.DocumentHeader.ImportePromocion > 0.0)
            {
                ticket.DocumentLines.ForEach(line => {
                    string itemName = new string(line.ItemName.Take(16).ToArray());
                    stream.WriteLn($" {line.Quantity}  ${itemName.PadRight(20)}", 1);

                    if (mostrarCodigoArticulo)
                    {
                        stream.WriteLn($"{line.ItemCode.PadLeft(15)}", 1);
                    }
                });

            }
            else
            {
                ticket.DocumentLines.ForEach(line =>
                {
                    string itemName = new string(line.ItemName.Take(16).ToArray());
                    stream.WriteLn($" {line.Quantity}  " +
                        $"{itemName.PadRight(20)} " +
                        $"{TicketHandlerUtils.FormatAsMoney(line.PrecioUnitario).ToString().PadLeft(12)} " +
                        TicketHandlerUtils.FormatAsMoney(line.Importe).ToString().PadLeft(7),
                        1);

                    if (mostrarCodigoArticulo)
                    {
                        stream.WriteLn(line.ItemCode.PadLeft(15), 1);
                    }
                });
            }
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
                if (payment.Cambio != 0.0)
                {
                    var cambio = "CAMBIO: ".PadLeft(30) + payment.Cambio.ToString().PadLeft(15);
                    stream.Write(GetBytes(cambio));
                    stream.Write(Euro());
                    stream.SkipLines(1);
                }
            });

            stream.SkipLines(3);
        }

        /// <summary>
        /// Función para imprimir los productos, vías de pago e importes de TECMOVIL
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="ticket"></param>
        /// <param name="viasPago"></param>
        private static void PrintProductsAndPaymentsTecMovil(this MemoryStream stream, Document ticket, List<ViaPago> viasPago)
        {

            stream.TextoNegrita();
            stream.WriteLn($"Codigo   Descrip.   Cant.   Precio   Dto%   Dto E   IGIC   Importe", 1);
            stream.TextoDefecto();
            stream.SeparatorTecMovil();
            ticket.DocumentLines.ForEach(line =>
            {
                string UoM = new string(line.UoM.Take(17).ToArray());
                stream.WriteLn($" {line.ItemCode}" +
                    $"   {line.ItemName}\n" +
                    $"   {line.UoM.PadRight(17)}" +
                    $"   {TicketHandlerUtils.FormatAsMoney(line.Quantity)}" +
                    $"   {line.PrecioUnitario}€ " +
                    $"   {line.PorcentajeDescuento} " +
                    $"   {line.ImporteDescuento} " +
                    $"   {line.ImporteIGIC}  " +
                    $"   {TicketHandlerUtils.FormatAsMoney(line.Importe)}€",
                    1);
            });
            stream.TextoDefecto();
            stream.SeparatorTecMovil();
            stream.WriteLn("", 4);
            var importSubTotal = "SUBTOTAL:".PadLeft(60) + (ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.Importe.ToString().PadLeft(1);

            stream.TextoNegrita();
            stream.WriteLn(importSubTotal + "€",1);
            

            stream.TextoDefecto();
            stream.SeparatorTecMovil();

            stream.TextoNegrita();
            stream.WriteLn("Base Imp.        %IGIC           Imp. IGIC" + "Total".PadLeft(25),1);
            
            stream.TextoDefecto();

            var impuestosOrdenados = ticket.DocumentLines.GroupBy(x => x.Impuesto);
            var importeAcumulado = ticket.DocumentHeader.Importe;
            impuestosOrdenados.ToList().ForEach(lineaImpuesto =>
            {
                var importe = lineaImpuesto.Sum(l => l.Importe);
                var porcentajeImpuesto = TicketHandlerUtils.FormatAsMoney(Double.Parse(lineaImpuesto.First().Impuesto)); 
                var igic = lineaImpuesto.Sum(i => i.ImporteIGIC);
                importeAcumulado += igic;
                stream.WriteLn($" {importe}   " +
                    $"        {porcentajeImpuesto}%               " + 
                    $"           {igic}€" +
                    $"{(TicketHandlerUtils.FormatAsMoney(importeAcumulado.GetValueOrDefault(0.0)) + "€").PadLeft(25)}",
                    1);
            });
            
            stream.WriteLn($"{ticket.DocumentHeader.ImporteTotal}€".PadLeft(60), 1);

            ticket.DocumentPayments.ForEach(payment =>
            {
                ViaPago viaPago = viasPago.FirstOrDefault(paymentType => payment.EntryViaPago == paymentType.CreditCard);
                if (viaPago != null)
                {
                    var pago = "".PadLeft(60) + viaPago.Nombre.ToUpper() + (((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + payment.Importe.ToString())).PadLeft(15);
                    stream.Write(GetBytes(pago));
                    stream.Write(Euro());
                }
                stream.SkipLines(1);
                    var entregado = "".PadLeft(45)+ "ENTREGADO: " + payment.Entregado.ToString().PadLeft(14) + "€";
                    stream.WriteLn(entregado,1);
                if (payment.Cambio != 0.0)
                {
                    var cambio = "CAMBIO: ".PadLeft(45) + payment.Cambio.ToString().PadLeft(14) + "€";
                    stream.WriteLn(cambio,1);
                }
            });

            stream.SkipLines(1);
        }

        


        #region FUNCIONES MAPPER BYTES

        private static byte[] GetBytes(char letra)
        {
            return Encoding.GetEncoding(858).GetBytes(letra.ToString());
        }

        private static byte[] GetBytes(int numero)
        {
            return Encoding.GetEncoding(858).GetBytes(((char)numero).ToString());
        }

        private static byte[] GetBytes(string content)
        {
            return Encoding.GetEncoding(858).GetBytes(content);
        }

        private static byte[] Euro()
        {
            char euro = '€';
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
