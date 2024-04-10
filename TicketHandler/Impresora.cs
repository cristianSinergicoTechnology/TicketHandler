using System.Text;
using TicketHandler.modelo;
using TicketHandler.utils;

namespace TicketHandler
{
    internal static class Impresora
    {
        public static void printTicketIFT(Document doc, Cliente cliente, bool proforma, string empleado)
        {
            var tfnEmpresa = ""; // no lo tenemos de momento
            var ruta = "./pruebaFactura.txt"; // ruta del archivo
            try
            {
                // DEBUG ONLY CREAMOS DOCUMENT AQUÍ
                #region CREACION MOCK DOCUMENT

                if (doc == null)
                {
                    doc = new Document();
                    var header = new DocumentHeader();


                }

                #endregion

                MemoryStream stream = new MemoryStream();
                stream.InicializarImpresora();

                //TODO TERMINAR PROCESO IMPRESION

                stream.Close();

                File.WriteAllBytes(ruta,stream.ToArray()); // guardamos el contenido en el archivo externo
                //TODO MANDAR INFORMACIÓN A IMPRESORA E IMPRIMIR
            }
            catch (Exception ex) {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        public static void printTicketKoala(Document doc, Cliente cliente, bool proforma, string empleado)
        {
            throw new NotImplementedException();
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
        private static void SkipLines(MemoryStream stream, int numLinea)
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

        #region FUNCIONES MAPPER BYTES

        private static byte[] GetBytes(char letra)
        {
            return Encoding.Default.GetBytes(letra.ToString());
        }

        private static byte[] GetBytes(int numero)
        {
            return Encoding.Default.GetBytes(numero.ToString());
        }

        private static byte[] Euro()
        {
            char euro = '€';
            byte[] euroBytes = Encoding.GetEncoding(858).GetBytes(new char[] { euro });
            return euroBytes;
        }
        #endregion

        #endregion
    }
}
