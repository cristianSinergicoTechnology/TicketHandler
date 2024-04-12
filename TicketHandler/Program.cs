using TicketHandler.modelo;
using TicketHandler.utils;

namespace TicketHandler
{
    internal class Program
    {
        static void Main(string[] args)
        {

            #region CREACIÓN DE MOCKS

            // DEBUG ONLY CREAMOS DOCUMENT AQUÍ
            #region CREACION MOCK DOCUMENT

            Document doc = new Document()
            {
                #region DocumentHeader
                DocumentHeader = new DocumentHeader()
                {
                    IDTicket = "T3424155-524523",
                    IDTienda = "10",
                    CardCode = "352895235",
                    TipoDocumento = "CC",
                    Importe = new decimal(7.15),
                    ImporteTotal = new decimal(7.15)

                },
                #endregion
                #region DocumentLines
                DocumentLines =
                [
                    new DocumentLine()
                    {
                        ItemCode = "000039",
                        ItemName = "AMERICAN SPIRIT NARANJA",
                        RowNum = 0,
                        Quantity = 1,
                        UoMEntry = 1,
                        PrecioUnitario = new decimal(7.15),
                        Importe = new decimal(7.15),
                        TipoImpuesto = "RC0",
                        Impuesto = Decimal.Zero,
                        ImporteIGIC = Decimal.Zero,
                        ImporteTotal = new decimal(7.15),
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION,
                    },
                    new DocumentLine()
                    {
                        ItemCode = "000039",
                        ItemName = "AMERICAN SPIRIT NARANJA",
                        RowNum = 0,
                        Quantity = 1,
                        UoMEntry = 1,
                        PrecioUnitario = new decimal(7.15),
                        Importe = new decimal(7.15),
                        TipoImpuesto = "RC0",
                        Impuesto = Decimal.Zero,
                        ImporteIGIC = Decimal.Zero,
                        ImporteTotal = new decimal(7.15),
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION,
                    },
                    new DocumentLine()
                    {
                        ItemCode = "000039",
                        ItemName = "AMERICAN SPIRIT NARANJA",
                        RowNum = 0,
                        Quantity = 1,
                        UoMEntry = 1,
                        PrecioUnitario = new decimal(7.15),
                        Importe = new decimal(7.15),
                        TipoImpuesto = "RC0",
                        Impuesto = Decimal.Zero,
                        ImporteIGIC = Decimal.Zero,
                        ImporteTotal = new decimal(7.15),
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION,
                    },

                ],
                #endregion
                #region DocumentPayments
                DocumentPayments =
                [
                    new DocumentPayment
                    {
                        IDTienda = "104",
                        IDCaja = "04",
                        IDTicket = "T2410404-000170",
                        IDCobro = "CO",
                        Importe = new decimal(7.15),
                        EntryViaPago = 13,
                        ViaPago = "VISAS BBVA",
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION
                    },
                    new DocumentPayment
                    {
                        IDTienda = "104",
                        IDCaja = "04",
                        IDTicket = "T2410404-000170",
                        IDCobro = "CO",
                        Importe = new decimal(7.15),
                        EntryViaPago = 13,
                        ViaPago = "VISAS BBVA",
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION
                    },
                    new DocumentPayment
                    {
                        IDTienda = "104",
                        IDCaja = "04",
                        IDTicket = "T2410404-000170",
                        IDCobro = "CO",
                        Importe = new decimal(7.15),
                        EntryViaPago = 13,
                        ViaPago = "VISAS BBVA",
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION
                    },

                ],
                #endregion
                #region DocumentReads
                DocumentReads = [
                    new DocumentRead
                    {
                        IDTicket = "T2410404-000170",
                        RowNum = 0,
                        IDLectura = "000",
                        ItemCode = "000039",
                        Cantidad = 1,
                        UoM = 1,
                        Codebar = "8436002681855",
                        TipoLectura = "Manual",
                        EmpID = "8",
                        CreateDate = Utils.FECHA_CREACION,
                        CreateTime = Utils.HORA_CREACION,
                        UpdateDate = Utils.FECHA_CREACION,
                        UpdateTime = Utils.HORA_CREACION,
                    }
                ]
                #endregion
            };

            #endregion

            #region CREACION MOCK CLIENT

            Cliente cliente = new()
            {
                CardName = Utils.NOMBRE_CLIENTE,
                Address = Utils.DIRECCION_CLIENTE,
                FederalTaxId = Utils.NOMBRE_CLIENTE,

            };

            #endregion

            #region CREACION MOCK VIAPAGO

            ViaPago viaPago = new ViaPago()
            {
                Nombre = "VISAS BBVA",
                IDCaja = "1",
                CreditCard = 13
            };

            #endregion

            #endregion
            var json = Impresora.printTicket(doc, [viaPago], cliente, false, "CRISTIAN");
            var data = (List<byte>)json["data"];
            Console.WriteLine("--------------DATOS DEL TICKET---------------");
            data.ForEach(x => { Console.Write(x + ","); });
            Console.Write("\n");
            Console.WriteLine("--------------DATOS DEL TICKET---------------");
        }
    }
}
