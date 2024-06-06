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
using System.Net.Sockets;

namespace TicketHandler
{
    public static class Impresora
    {
        const string DOCTYPE_ABONO = "TA";

        public static Dictionary<string, object> printTicketTecMovil(Document doc, InfoEmpresa infoEmpresa, List<ViaPago> viasPago, Cliente cliente, bool proforma, string empleado, string printer01, bool printFooter = false, bool test = false)
        {
            //proforma = true;      //En TecMóvil sólo se imprimen las facturas/abonos
            bool bFactura = true; //En TecMóvil sólo se imprimen las facturas/abonos
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                //Encoding enc = Encoding.GetEncoding("IBM00858");
                var tfnEmpresa = infoEmpresa.Telefono;
                var footerLines = infoEmpresa.Footer;

                MemoryStream printer = new MemoryStream();

                //printer.InicializarImpresora();
                printer.CambiarCodePageTecMovil();
                //printer.SeleccionarIdioma();
                //printer.WriteLn("€\u00D5\x00D5ií", 1);
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

                printer.TextoNegrita();
                printer.TextoCentradoIzquierda();
                printer.Write(GetBytes(doc.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "Factura rectificativa: " : "Factura: "));
                printer.WriteLn($"{doc.DocumentHeader.NumeroFactura} {infoEmpresa.FechaCreacion}", 1);
                printer.TextoDefecto();
                printer.Write(GetBytes((doc.DocumentHeader.TipoDocumento == "TI" || doc.DocumentHeader.TipoDocumento == "TA") ? "Numero seguimiento interno: " : "Ticket: "));
                printer.TextoDefecto();
                printer.WriteLn($"{doc.DocumentHeader.IDTicket} {infoEmpresa.FechaCreacion} {infoEmpresa.HoraCreacion}", 2);

                printer.TextoNegrita();
                printer.Write(GetBytes("Le ha atendido: "));
                printer.TextoDefecto();
                printer.WriteLn(empleado, 2);

                printer.TextoNegrita();
                printer.WriteLn($"Cliente {cliente.CardCode}", 1);
                printer.TextoDefecto();
                var linesToSkip = (proforma || bFactura) ? 1 : 2;
                printer.WriteLn($"Nombre Fiscal: {cliente.CardName}", linesToSkip);
                if ((cliente.CardForeignName != null) && (cliente.CardName != cliente.CardForeignName) && (cliente.CardForeignName != ""))
                {
                    printer.WriteLn($"Nombre comercial: {cliente.CardForeignName}", 1);
                }
                //printer.WriteLn($"Cliente: {cliente.CardName}", linesToSkip);
                if (proforma || bFactura)
                {
                    //printer.WriteLn($"Comercio: {cliente.}", 1);
                    printer.WriteLn($"NIF: {cliente.FederalTaxId}", 1);
                    printer.WriteLn($"Dirección Fiscal: {cliente.Address}", 1);
                }
                if ((cliente.AddressEntrega != null) && (cliente.AddressEntrega != ""))
                {
                    printer.WriteLn($"Dirección de Entrega: {cliente.AddressEntrega}", 1);
                }

                printer.TextoDefecto();
                printer.WriteLn($"Forma de cobro: {doc.DocumentHeader.sCodePagos}", 2);

                printer.PrintProductsAndPaymentsTecMovil(doc, viasPago, printer01);

                if (printFooter)
                {
                    printer.PrintFooter(footerLines);

                    printer.SkipLines(3);

                    printer.PrintBarcode(doc);
                }

                printer.TextoNegrita();
                printer.CentrarTexto();
                printer.WriteLn("https://cigarrosypuros.com", 1);

                // Rellanamos 5 filas para que no se corte información de la factura
                printer.SkipLines(5);
                //if (test)
                    printer.CortarFacturaTecPV();
                //else
                //    printer.CortarFacturaTecMovil();

                printer.Close();

                return loadContent(printer.ToArray());

            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return new Dictionary<string, object>()
                {
                    { "errorMessage", $"TicketHandler-printTicketTecMovil: Error al generar ticket: {ex}" },
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
            //stream.Write(Encoding.GetEncoding(858).GetBytes(content));
            stream.Write(Encoding.GetEncoding(858).GetBytes(content.Replace("ü","u").Replace("Ñ", "N").Replace("ñ", "n").Replace("€", "eur").
                                                                    Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").
                                                                    Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")));
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
            stream.Write(TecPVCommands.GS);
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
        private static void SeparatorTecMovil(this MemoryStream stream, string printer01 = "001")
        {
            if (printer01 == "001")
            {
                stream.WriteLn($"{"".PadLeft(69, '=')}", 1);
            }
            else 
            {
                stream.WriteLn($"{"".PadLeft(60, '=')}", 1);
            }
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

            var tipoDocumento = "SUBTOTAL:".PadLeft(30) + ((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.ImporteTotal.ToString()).PadLeft(15);

            stream.TextoNegrita();
            stream.Write(GetBytes(tipoDocumento));
            stream.Write(Euro());

            stream.WriteLn("", 2);

            var importeTotal = "TOTAL:".PadLeft(30) + ((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + ticket.DocumentHeader.ImporteTotal.ToString()).PadLeft(15);

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
        private static void PrintProductsAndPaymentsTecMovil(this MemoryStream stream, Document ticket, List<ViaPago> viasPago, string printer01)
        {
            try { 
                stream.TextoNegrita();
                stream.SeparatorTecMovil(printer01);
                if (printer01 == "001")
                {
                    stream.WriteLn($"Codigo   Descrip.    Cant.   Precio    Dto%   Dto E  IGIC     Importe", 1);
                }
                else if (printer01 == "002")
                {
                    stream.WriteLn($"Codigo      Cant.   Precio    Dto%   Dto E  IGIC     Importe", 1);
                }
                stream.SeparatorTecMovil(printer01);
                stream.TextoDefecto();
                if (ticket.DocumentLines != null)
                {
                    ticket.DocumentLines.ForEach(line =>
                    {
                        string UoM = new string(line.UoM.Take(17).ToArray());
                        stream.TextoNegrita();
                        stream.WriteLn($"{line.ItemCode}   {line.ItemName}", 1);
                        stream.TextoDefecto();
                        if (printer01 == "001")
                        {
                            stream.WriteLn($" {line.UoM.PadRight(16)}" +                                         //17
                                $" {TicketHandlerUtils.FormatAsMoney(line.Quantity).ToString().PadLeft(7)}" +    //8
                                $" {String.Format("{0:0.00}", line.PrecioUnitario).PadLeft(9)}" +                //10
                                $" {line.PorcentajeDescuento.ToString().PadLeft(7)}" +                           //8
                                $" {line.ImporteDescuento.ToString().PadLeft(7)}" +                              //8
                                $" {String.Format("{0:0.0}", double.Parse(line.Impuesto) * 100).PadLeft(5)}" +   //6
                                $" {String.Format("{0:0.00}", TicketHandlerUtils.FormatAsMoney(line.Importe)).PadLeft(11)}",   //12
                                1);
                        }
                        else if (printer01 == "002")
                        {
                            stream.WriteLn($" {line.UoM.PadRight(9)}" +                                          //10
                                $" {TicketHandlerUtils.FormatAsMoney(line.Quantity).ToString().PadLeft(5)}" +    //6
                                $" {String.Format("{0:0.00}", line.PrecioUnitario).PadLeft(9)}" +                //10
                                $" {line.PorcentajeDescuento.ToString().PadLeft(7)}" +                           //8
                                $" {line.ImporteDescuento.ToString().PadLeft(7)}" +                              //8
                                $" {String.Format("{0:0.0}", double.Parse(line.Impuesto) * 100).PadLeft(5)}" +   //6
                                $" {String.Format("{0:0.00}", TicketHandlerUtils.FormatAsMoney(line.Importe)).PadLeft(11)}",   //12
                                1);
                        }
                    });
                }
                stream.TextoNegrita();
                stream.SeparatorTecMovil(printer01);
                stream.TextoDefecto();
                //stream.WriteLn("", 4);
                //stream.TextoNegrita();
                //stream.WriteLn($"{importSubTotal}€\u20AC\u000A\u00A0\u0028\u0082\u2082\u2028", 1);
                var importSubTotal = $"SUBTOTAL: {((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + String.Format("{0:0.00}", ticket.DocumentHeader.Importe)).PadLeft(9)} €";
                if (printer01 == "001")
                {
                    stream.WriteLn($"{importSubTotal.PadLeft(67)}", 1);
                }
                else if(printer01 == "002")
                {
                    stream.WriteLn($"{importSubTotal.PadLeft(58)}", 1);
                }

                stream.TextoDefecto();
                //stream.SeparatorTecMovil();

                stream.TextoNegrita();
                stream.WriteLn("Base Imp.  %IGIC     IGIC",1);
            
                stream.TextoDefecto();

                var importeAcumulado = ticket.DocumentHeader.Importe;
                if (ticket.DocumentLines != null)
                {
                    //Intento ordenar el listado tras el GroupBy, pero no lo está ordenando.
                    var impuestosOrdenados = ticket.DocumentLines.GroupBy(x => x.Impuesto).Select(y => y.OrderBy(z => z.Impuesto));
                    impuestosOrdenados.ToList().ForEach(lineaImpuesto =>
                    {
                        var importe = lineaImpuesto.Sum(l => l.Importe);
                        //var porcentajeImpuesto = String.Format("{0:N}", double.Parse(lineaImpuesto.First().Impuesto));
                        var porcentajeImpuesto = double.Parse(lineaImpuesto.First().Impuesto).ToString();
                        //var igic = lineaImpuesto.Sum(i => i.ImporteIGIC);
                        double igic = importe * double.Parse(lineaImpuesto.First().Impuesto);
                        importeAcumulado += igic;
                        stream.WriteLn($"{String.Format("{0:0.00}", importe).PadLeft(9)}" +      //9
                                       $"{String.Format("{0:0.00}", TicketHandlerUtils.FormatAsMoney(double.Parse(porcentajeImpuesto) * 100)).PadLeft(6)}%" +          //7
                                       $"{TicketHandlerUtils.FormatAsMoney(igic).ToString().PadLeft(9)}", 1);                      //9
                    });
                }

                if (printer01 == "001")
                {
                    stream.WriteLn($"    IGIC: {((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + String.Format("{0:0.00}", ticket.DocumentHeader.ImporteImpuesto)).PadLeft(9)} €".PadLeft(67), 1);
                    stream.TextoNegrita();
                    stream.WriteLn($"   TOTAL: {((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + String.Format("{0:0.00}", ticket.DocumentHeader.ImporteTotal)).PadLeft(9)} €".PadLeft(67), 1);
                }
                else if (printer01 == "002")
                {
                    stream.WriteLn($"    IGIC: {((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + String.Format("{0:0.00}", ticket.DocumentHeader.ImporteImpuesto)).PadLeft(9)} €".PadLeft(58), 1);
                    stream.TextoNegrita();
                    stream.WriteLn($"   TOTAL: {((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + String.Format("{0:0.00}", ticket.DocumentHeader.ImporteTotal)).PadLeft(9)} €".PadLeft(58), 1);
                }

                if (ticket.DocumentPayments != null)
                {
                    ticket.DocumentPayments.ForEach(payment =>
                    {
                        ViaPago viaPago = viasPago.FirstOrDefault(paymentType => payment.EntryViaPago == paymentType.CreditCard);
                        if (viaPago != null)
                        {
                            var pago = "".PadLeft(60) + viaPago.Nombre.ToUpper() + ((ticket.DocumentHeader.TipoDocumento == DOCTYPE_ABONO ? "-" : "") + payment.Importe.ToString()).PadLeft(15);
                            stream.Write(GetBytes(pago));
                            stream.Write(Euro());
                        }
                        stream.SkipLines(1);
                        var entregado = "".PadLeft(45) + "ENTREGADO: " + payment.Entregado.ToString().PadLeft(14) + "€";
                        stream.WriteLn(entregado, 1);
                        if (payment.Cambio != 0.0)
                        {
                            var cambio = "CAMBIO: ".PadLeft(45) + payment.Cambio.ToString().PadLeft(14) + "€";
                            stream.WriteLn(cambio, 1);
                        }
                    });
                }

                stream.SkipLines(1);
            }
            catch (Exception ex)
            {
                throw new Exception($"PrintProductsAndPaymentsTecMovil: {ex}");
            }
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
