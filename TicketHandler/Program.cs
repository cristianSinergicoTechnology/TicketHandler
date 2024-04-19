using System.Collections.Generic;
using System;
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

            var fechaCreacion = "19/04/2024";
            var horaCreacion = "11:00:02";
            #region DocumentLines
            var documentLines = new List<DocumentLine>()
            {
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
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion,
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
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion,
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
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion,
                    }
            };
            #endregion
            #region DocumentPayments
            var documentPayments = new List<DocumentPayment>
                {
                    new DocumentPayment()
                    {
                        IDTienda = "104",
                        IDCaja = "04",
                        IDTicket = "T2410404-000170",
                        IDCobro = "CO",
                        Importe = new decimal(7.15),
                        EntryViaPago = 13,
                        ViaPago = "VISAS BBVA",
                        EmpID = "8",
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion
                    },
                    new DocumentPayment()
                    {
                        IDTienda = "104",
                        IDCaja = "04",
                        IDTicket = "T2410404-000170",
                        IDCobro = "CO",
                        Importe = new decimal(7.15),
                        EntryViaPago = 13,
                        ViaPago = "VISAS BBVA",
                        EmpID = "8",
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion
                    },
                    new DocumentPayment()
                    {
                        IDTienda = "104",
                        IDCaja = "04",
                        IDTicket = "T2410404-000170",
                        IDCobro = "CO",
                        Importe = new decimal(7.15),
                        EntryViaPago = 13,
                        ViaPago = "VISAS BBVA",
                        EmpID = "8",
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion
                    },
                };
            #endregion
            #region DocumentReads
            var documentReads = new List<DocumentRead>
                {
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
                        CreateDate = fechaCreacion,
                        CreateTime = horaCreacion,
                        UpdateDate = fechaCreacion,
                        UpdateTime = horaCreacion,
                    }
                };
                #endregion

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
                DocumentLines = documentLines,
                DocumentPayments = documentPayments,
                DocumentReads = documentReads,
            };

        #endregion

            #region CREACION MOCK CLIENT

        Cliente cliente = new Cliente()
        {
            CardName = "paco",
            Address = "Avenida Anaga",
            FederalTaxId = "",

        };

            #endregion

            #region CREACION MOCK VIAPAGO

            var viasPago = new List<ViaPago>()
            {
                 new ViaPago()
            {
                Nombre = "VISAS BBVA",
                IDCaja = "1",
                CreditCard = 13
             },
            };

                #endregion


            #region
            var infoEmpresa = new InfoEmpresa()
            {
                Nombre = "TABACO BARATO S.L.",
                Direccion = "C/Avenida Castillo 39, 38200",
                FechaCreacion = fechaCreacion,
                HoraCreacion = horaCreacion,
                NIF = "BAS34284239325C",
                Telefono = "64712471"
            };
            #endregion


            #endregion
            var json = Impresora.printTicket(doc,infoEmpresa, viasPago, cliente, false, "CRISTIAN");
            var data = (List<byte>)json["data"];
            Console.WriteLine("--------------DATOS DEL TICKET---------------");
                data.ForEach(x => { Console.Write(x + ","); });
                Console.Write("\n");
                Console.WriteLine("--------------DATOS DEL TICKET---------------");
            }
    }
}
