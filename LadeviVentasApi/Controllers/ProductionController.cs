// namespace LadeviVentasApi.Controllers;

// using Microsoft.AspNetCore.Mvc;
// using LadeviVentasApi.Data;
// using LadeviVentasApi.Models.Domain;
// using Microsoft.EntityFrameworkCore;
// using LadeviVentasApi.Services.Xubio;
// using LadeviVentasApi.DTOs;
// using LadeviVentasApi.Services.Xubio.DTOs;
// using LadeviVentasApi.Services.Xubio.FixedValues;
// using Microsoft.Extensions.Configuration;
// using LadeviVentasApi.Data.Migrations;

// [Route("api/[controller]")]
// [ApiController]
// public class ProductionController : ControllerBase
// {
//     private readonly ApplicationDbContext context;
//     private readonly Lazy<ApplicationUser> currentAppUser;
//     private readonly IConfiguration configuration;

//     public ProductionController(ApplicationDbContext context, IConfiguration configuration)
//     {
//         this.context = context;
//         this.currentAppUser = new Lazy<ApplicationUser>(GetAppUser, LazyThreadSafetyMode.ExecutionAndPublication);
//         this.configuration = configuration;
//     }

//     private ApplicationUser GetAppUser()
//     {
//         if (User?.Identity?.IsAuthenticated != true)
//             throw new InvalidOperationException("Usuario no autenticado");

//         var userMail = User.Claims
//             .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
//             ?? throw new InvalidOperationException("Email de usuario no encontrado");

//         return this.context.ApplicationUsers
//             .Include(u => u.ApplicationRole)
//             .Include(u => u.Country)
//             .Single(u => u.CredentialsUser.Email.ToLower() == userMail.ToLower());
//     }

//     [HttpGet("ProductionInventory")]
//     public async Task<IActionResult> GetProductionInventory(long productEditionId)
//     {
//         var productEdition = await context.ProductEditions
//                                         .Include(pe => pe.InventoryProductAdvertisingSpaces)
//                                         .AsNoTracking()
//                                         .Select(pe => new
//                                         {
//                                             pe.Id,
//                                             pe.PageCount,
//                                             pe.InventoryProductAdvertisingSpaces,
//                                             pe.Deleted
//                                         })
//                                         .SingleOrDefaultAsync(pe => (!pe.Deleted.HasValue || !pe.Deleted.Value) && pe.Id == productEditionId);

//         if (productEdition == null)
//         {
//             return NotFound("No se encontró la edición del producto.");
//         }

//         List<dynamic> inventoryView = new List<dynamic>();
//         for (int i = 1; i < productEdition.PageCount; i++)
//         {
//             inventoryView.Add(new
//             {
//                 PageNumber = i,

//             });
//         }

//         var inventory = await context.InventoryProductAdvertisingSpaces
//             .AsNoTracking()
//             .Include(ipas => ipas.ProductAdvertisingSpace)
//             .Where(ipas => (!ipas.Deleted.HasValue || !ipas.Deleted.Value)
//                            && ipas.ProductEditionId == productEditionId)
//             .Select(ipas => new
//             {
//                 ipas.Id,
//                 ipas.ProductAdvertisingSpaceId,
//                 ProductAdvertisingSpaceName = ipas.ProductAdvertisingSpace.Name,
//                 ipas.Quantity
//             })
//             .ToListAsync();

//         return Ok(inventory);
//     }

//     //   [HttpPost("Orders")]
//     //   public async Task<IActionResult> OrdersBilling([FromBody] List<InvoiceWritingDto> invoicesData)
//     //   {
//     //     if (!invoicesData?.Any() == true)
//     //     {
//     //       return BadRequest("Se requiere al menos una factura para procesar");
//     //     }

//     //     var results = new List<object>();
//     //     var errors = new List<string>();

//     //     foreach (var invoiceWriting in invoicesData)
//     //     {
//     //       try
//     //       {
//     //         ControllerContext.HttpContext.Items["current-user"] = this.currentAppUser;

//     //         var validationResult = await ValidateInvoiceRequest(invoiceWriting);
//     //         if (validationResult != null)
//     //         {
//     //           errors.Add($"Cliente {invoiceWriting.ClientId}: Error de validación");
//     //           continue;
//     //         }

//     //         var clientDb = await GetClientById(invoiceWriting.ClientId);
//     //         var xubioPointOfSale = await GetXubioPointOfSale(clientDb);
//     //         var billingNumber = await GetBillingNumberIfNeeded(xubioPointOfSale, clientDb, invoiceWriting);

//     //         var xubioReceipt = CreateBaseXubioReceipt(
//     //             clientDb,
//     //             xubioPointOfSale,
//     //             invoiceWriting.GlobalObservations,
//     //             billingNumber);

//     //         // Procesar según el tipo de entidad
//     //         var result = invoiceWriting.EntityType.ToUpper() switch
//     //         {
//     //           "ORDER" => await ProcessOrderInvoiceForMultiple(invoiceWriting, xubioReceipt, clientDb, xubioPointOfSale),
//     //           _ => throw new InvalidOperationException("Tipo de entidad no válido")
//     //         };

//     //         results.Add(new
//     //         {
//     //           ClientId = invoiceWriting.ClientId,
//     //           ClientName = clientDb.BrandName,
//     //           Success = true,
//     //           NumeroDocumento = result.NumeroDocumento,
//     //           TransaccionId = result.TransaccionId
//     //         });
//     //       }
//     //       catch (Exception ex)
//     //       {
//     //         errors.Add($"Cliente {invoiceWriting.ClientId}: {ex.Message}");
//     //         results.Add(new
//     //         {
//     //           ClientId = invoiceWriting.ClientId,
//     //           Success = false,
//     //           Error = ex.Message
//     //         });
//     //       }
//     //     }

//     //     // Retornar resultados
//     //     return Ok(new
//     //     {
//     //       TotalProcessed = invoicesData.Count,
//     //       SuccessCount = results.Count(r => ((dynamic)r).Success),
//     //       ErrorCount = errors.Count,
//     //       Results = results,
//     //       Errors = errors
//     //     });
//     //   }

//     //   #region Validation Methods

//     //   private async Task<IActionResult?> ValidateInvoiceRequest(InvoiceWritingDto invoiceWriting)
//     //   {
//     //     if (invoiceWriting.ClientId <= 0)
//     //     {
//     //       ModelState.AddModelError("", "ID de cliente inválido");
//     //       return Utils.ActionResultForModelStateValidation(ModelState, HttpContext.Response);
//     //     }

//     //     if (string.IsNullOrWhiteSpace(invoiceWriting.EntityType))
//     //     {
//     //       ModelState.AddModelError("", "Tipo de entidad requerido");
//     //       return Utils.ActionResultForModelStateValidation(ModelState, HttpContext.Response);
//     //     }

//     //     if (!invoiceWriting.Items?.Any() == true)
//     //     {
//     //       ModelState.AddModelError("", "Items requeridos para la facturación");
//     //       return Utils.ActionResultForModelStateValidation(ModelState, HttpContext.Response);
//     //     }

//     //     return null;
//     //   }

//     //   private async Task<Client> GetClientById(long clientId)
//     //   {
//     //     var client = await this.context.Clients.AsNoTracking()
//     //         .SingleOrDefaultAsync(c => c.Id == clientId);

//     //     if (client == null || client.DeletedDate != null)
//     //       throw new InvalidOperationException("Cliente no encontrado");

//     //     if (client.XubioId == null)
//     //       throw new InvalidOperationException("El cliente no tiene un ID de Xubio asociado");

//     //     return client;
//     //   }

//     //   private async Task<PointOfSaleDtoResponse> GetXubioPointOfSale(Client client)
//     //   {
//     //     var pointsOfSale = await this.xubioService.GetPointOfSaleAsync(client.IsComtur);
//     //     var pointOfSale = pointsOfSale.SingleOrDefault(ps =>
//     //         ps.PuntoVenta == client.BillingPointOfSale.PadLeft(5, '0'));

//     //     if (pointOfSale == null)
//     //       throw new InvalidOperationException("El punto de venta no existe en Xubio");

//     //     return pointOfSale;
//     //   }

//     //   private async Task<BookDocumentDtoResponse?> GetBillingNumberIfNeeded(
//     //       PointOfSaleDtoResponse pointOfSale, Client client, InvoiceWritingDto invoiceWriting)
//     //   {
//     //     if (pointOfSale.ModoNumeracion != ModoNumeracion.Manual)
//     //       return null;

//     //     var xubioClient = (await this.xubioService.GetClientsAsync(client.IsComtur, client.IdentificationValue))
//     //         .FirstOrDefault();

//     //     if (xubioClient == null)
//     //       throw new InvalidOperationException("El cliente no existe en Xubio");

//     //     var documentType = GetDocumentTypeByFiscalCategory(xubioClient.CategoriaFiscal.Codigo, invoiceWriting, client, this.configuration);

//     //     var bookDocuments = await this.xubioService.GetBookDocumentsAsync(client.IsComtur, pointOfSale.PuntoVenta);
//     //     return bookDocuments.SingleOrDefault(b => b.TipoComprobante == documentType);
//     //   }

//     //   private static string GetDocumentTypeByFiscalCategory(string fiscalCategory, InvoiceWritingDto invoiceWriting, Client client, IConfiguration configuration)
//     //   {
//     //     if (fiscalCategory == CategoriaFiscalFixedValues.ResponsableInscripto)
//     //     {
//     //       if (client.IsBigCompany.HasValue && client.IsBigCompany.Value)
//     //       {
//     //         var totalAmountWithTaxes = CalculateTotalAmountWithTaxes(invoiceWriting);
//     //         var minimumAmountForBigCompany = configuration.GetValue<decimal>("MinimumAmountForBigCompany", 746290.00m);
//     //         if (totalAmountWithTaxes > minimumAmountForBigCompany)
//     //           return "Facturas de Venta A - Crédito";
//     //       }
//     //       return "Facturas de Venta A";
//     //     }

//     //     if (fiscalCategory == CategoriaFiscalFixedValues.Monotributista ||
//     //         fiscalCategory == CategoriaFiscalFixedValues.ConsumidorFinal ||
//     //         fiscalCategory == CategoriaFiscalFixedValues.Exento ||
//     //         fiscalCategory == CategoriaFiscalFixedValues.IvaNoAlcanzado)
//     //     {
//     //       return "Facturas de Venta B";
//     //     }

//     //     if (fiscalCategory == CategoriaFiscalFixedValues.Exterior)
//     //       return "Facturas de Venta E";

//     //     throw new InvalidOperationException("Categoría fiscal del cliente no soportada");
//     //   }

//     //   private static decimal CalculateTotalAmountWithTaxes(InvoiceWritingDto invoiceWriting)
//     //   {
//     //     if (invoiceWriting.IsConsolidated)
//     //     {
//     //       return invoiceWriting.Items.Sum(i => i.Amount + i.TotalTaxes);
//     //     }
//     //     else
//     //     {
//     //       return invoiceWriting.Items.Sum(i => (i.Price * i.Quantity) + i.TotalTaxes);
//     //     }
//     //   }

//     //   #endregion

//     //   #region Processing Methods

//     //   private async Task<IActionResult> ProcessContractInvoice(
//     //       InvoiceWritingDto invoiceWriting,
//     //       ReceiptDtoRequest xubioReceipt,
//     //       Client clientDb,
//     //       PointOfSaleDtoResponse xubioPointOfSale)
//     //   {
//     //     var soldSpaceIds = GetEntityIds(invoiceWriting);
//     //     var soldSpacesDb = await this.context.SoldSpaces
//     //                                         .Include(sp => sp.Contract)
//     //                                         .ThenInclude(c => c.BillingCondition)
//     //                                         .Where(s => soldSpaceIds.Contains(s.Id))
//     //                                         .ToListAsync();

//     //     if (!soldSpacesDb.Any())
//     //       return BadRequest("No se encontraron espacios vendidos");

//     //     // Solo contratos anticipados deben procesarse aquí
//     //     var nonAnticipatedContracts = soldSpacesDb
//     //         .Where(ss => ss.Contract.BillingCondition.Name != BillingCondition.Anticipated)
//     //         .ToList();

//     //     if (nonAnticipatedContracts.Any())
//     //     {
//     //       return BadRequest("Hay contratos que no cumplen con la condición de facturación indicada.");
//     //     }

//     //     ConfigureReceiptCurrency(xubioReceipt, soldSpacesDb.First().Contract.CurrencyId);
//     //     ConfigureReceiptItems(xubioReceipt, invoiceWriting);

//     //     var xubioResponse = await SendReceiptToXubio(xubioReceipt, clientDb, xubioPointOfSale);
//     //     if (xubioResponse.Error != null)
//     //     {
//     //       ModelState.AddModelError("", $"Error en Xubio: {xubioResponse.Error.Description}");
//     //       return Utils.ActionResultForModelStateValidation(ModelState, HttpContext.Response);
//     //     }

//     //     await UpdateContractsStatus(soldSpacesDb, xubioResponse.NumeroDocumento, xubioResponse.Transaccionid);

//     //     return Ok(new
//     //     {
//     //       xubioResponse.NumeroDocumento,
//     //       xubioResponse.Transaccionid
//     //     });
//     //   }

//     //   private async Task<dynamic> ProcessOrderInvoiceForMultiple(
//     //       InvoiceWritingDto invoiceWriting,
//     //       ReceiptDtoRequest xubioReceipt,
//     //       Client clientDb,
//     //       PointOfSaleDtoResponse xubioPointOfSale)
//     //   {
//     //     var publishingOrderIds = GetEntityIds(invoiceWriting);
//     //     var publishingOrdersDb = await this.context.PublishingOrders
//     //         .Include(sp => sp.Contract)
//     //         .ThenInclude(c => c.BillingCondition)
//     //         .Where(s => publishingOrderIds.Contains(s.Id))
//     //         .ToListAsync();

//     //     if (!publishingOrdersDb.Any())
//     //       throw new InvalidOperationException("No se encontraron órdenes de publicación");

//     //     // Solo órdenes de contratos "Contra Publicación"
//     //     var invalidOrders = publishingOrdersDb
//     //         .Where(po => po.Contract.BillingCondition.Name != BillingCondition.AgainstPublication)
//     //         .ToList();

//     //     if (invalidOrders.Any())
//     //     {
//     //       throw new InvalidOperationException("Hay ordenes cuyo contratos no cumplen con la condición de facturación indicada.");
//     //     }

//     //     ConfigureReceiptCurrency(xubioReceipt, publishingOrdersDb.First().Contract.CurrencyId);
//     //     ConfigureReceiptItems(xubioReceipt, invoiceWriting);

//     //     var xubioResponse = await SendReceiptToXubio(xubioReceipt, clientDb, xubioPointOfSale);
//     //     if (xubioResponse.Error != null)
//     //     {
//     //       throw new InvalidOperationException($"Error en Xubio: {xubioResponse.Error.Description}");
//     //     }

//     //     await UpdatePublishingOrdersStatus(publishingOrdersDb, xubioResponse.NumeroDocumento, xubioResponse.Transaccionid);

//     //     return new
//     //     {
//     //       NumeroDocumento = xubioResponse.NumeroDocumento,
//     //       TransaccionId = xubioResponse.Transaccionid
//     //     };
//     //   }

//     //   private static List<long> GetEntityIds(InvoiceWritingDto invoiceWriting)
//     //   {
//     //     return invoiceWriting.IsConsolidated
//     //         ? invoiceWriting.Items.SelectMany(i => i.ConsolidatedIds).ToList()
//     //         : invoiceWriting.Items.Select(i => i.Id).ToList();
//     //   }

//     //   #endregion

//     //   #region Receipt Configuration Methods

//     //   private static ReceiptDtoRequest CreateBaseXubioReceipt(
//     //       Client clientDb,
//     //       PointOfSaleDtoResponse pointOfSale,
//     //       string observations,
//     //       BookDocumentDtoResponse? bookDocument = null)
//     //   {
//     //     var currentDate = Utils.GetArgentinaDateTime();
//     //     var receiptDtoRequest = new ReceiptDtoRequest
//     //     {
//     //       Tipo = 1, // Tipo de comprobante (1 para factura)
//     //       Cliente = new ClienteRequest { Id = clientDb.XubioId.Value },
//     //       PuntoVenta = new PuntoVentaRequest { Codigo = pointOfSale.Codigo },
//     //       Fecha = currentDate.ToString("yyyy-MM-dd"),
//     //       FechaVto = currentDate.AddDays(30).ToString("yyyy-MM-dd"),
//     //       CondicionDePago = 1, // Condición de pago (1 para Cuenta Corriente)
//     //       Descripcion = observations
//     //     };

//     //     if (bookDocument != null)
//     //     {
//     //       var newDocumentNumber = int.Parse(bookDocument.UltimoUtilizado) + 1;
//     //       receiptDtoRequest.NumeroDocumento = $"{bookDocument.LetraComprobante}-{pointOfSale.PuntoVenta}-{newDocumentNumber.ToString().PadLeft(8, '0')}";
//     //     }

//     //     return receiptDtoRequest;
//     //   }

//     //   private static void ConfigureReceiptCurrency(ReceiptDtoRequest receipt, long? currencyId)
//     //   {
//     //     var currencyCode = currencyId switch
//     //     {
//     //       null => "EUROS",
//     //       2 => "DOLARES",
//     //       _ => "PESOS_ARGENTINOS"
//     //     };

//     //     receipt.Moneda = new MonedaRequest { Codigo = currencyCode };
//     //   }

//     //   private static void ConfigureReceiptItems(ReceiptDtoRequest receipt, InvoiceWritingDto invoiceWriting)
//     //   {
//     //     if (invoiceWriting.IsConsolidated)
//     //     {
//     //       ConfigureConsolidatedItems(receipt, invoiceWriting);
//     //     }
//     //     else
//     //     {
//     //       ConfigureSeparateItems(receipt, invoiceWriting);
//     //     }
//     //   }

//     //   private static void ConfigureConsolidatedItems(ReceiptDtoRequest receipt, InvoiceWritingDto invoiceWriting)
//     //   {
//     //     var totalQuantity = invoiceWriting.Items.Sum(i => i.Quantity);
//     //     var totalAmount = invoiceWriting.Items.Sum(i => i.Amount);
//     //     var consolidatedObservations = string.Join("\n", invoiceWriting.Items.Select(i => i.Observations));

//     //     receipt.TransaccionProductoItems = new List<TransaccionProductoItemRequest>
//     //         {
//     //             new()
//     //             {
//     //                 Producto = new ProductoRequest { Codigo = invoiceWriting.Items.First().XubioProductCode },
//     //                 Descripcion = consolidatedObservations,
//     //                 Cantidad = 1,
//     //                 Precio =  totalAmount ,
//     //             }
//     //         };
//     //   }

//     //   private static void ConfigureSeparateItems(ReceiptDtoRequest receipt, InvoiceWritingDto invoiceWriting)
//     //   {
//     //     receipt.TransaccionProductoItems = invoiceWriting.Items.Select(item => new TransaccionProductoItemRequest
//     //     {
//     //       Producto = new ProductoRequest { Codigo = item.XubioProductCode },
//     //       Descripcion = item.Observations,
//     //       Cantidad = item.Quantity,
//     //       Precio = item.Price
//     //     }).ToList();
//     //   }

//     //   #endregion

//     //   #region Xubio Integration Methods

//     //   private async Task<ReceiptDtoResponse> SendReceiptToXubio(
//     //       ReceiptDtoRequest receipt,
//     //       Client clientDb,
//     //       PointOfSaleDtoResponse pointOfSale)
//     //   {
//     //     return await this.xubioService.CreateReceiptAsync(
//     //         receipt,
//     //         clientDb.IsComtur,
//     //         pointOfSale.ModoNumeracion == ModoNumeracion.Manual);
//     //   }

//     //   #endregion

//     //   #region Update Methods

//     //   private async Task UpdateContractsStatus(List<SoldSpace> soldSpaces, string documentNumber, long? transactionId)
//     //   {
//     //     var contractIds = soldSpaces.Select(s => s.ContractId).Distinct().ToList();
//     //     var contractsDb = await this.context.Contracts
//     //         .Include(c => c.SoldSpaces)
//     //         .Where(c => contractIds.Contains(c.Id))
//     //         .ToListAsync();

//     //     foreach (var contract in contractsDb)
//     //     {
//     //       UpdateContractInvoiceNumbers(contract, documentNumber, transactionId);
//     //     }

//     //     await this.context.SaveChangesAsync();
//     //   }

//     //   private static void UpdateContractInvoiceNumbers(Contract contract, string documentNumber, long? transactionId)
//     //   {
//     //     var invoiceNumbers = contract.InvoiceNumber
//     //         .Split(',', StringSplitOptions.RemoveEmptyEntries)
//     //         .Select(i => i.ToUpper().Trim())
//     //         .ToHashSet();

//     //     invoiceNumbers.Add(documentNumber);
//     //     contract.InvoiceNumber = string.Join(",", invoiceNumbers);
//     //     contract.XubioTransactionId = transactionId;
//     //     foreach (var soldSpace in contract.SoldSpaces)
//     //     {
//     //       soldSpace.XubioDocumentNumber = documentNumber;
//     //     }
//     //   }

//     //   private async Task UpdatePublishingOrdersStatus(List<PublishingOrder> publishingOrders, string documentNumber, long? transactionId)
//     //   {
//     //     publishingOrders.ForEach(po =>
//     //     {
//     //       po.XubioDocumentNumber = documentNumber;
//     //       po.InvoiceNumber = documentNumber;
//     //       po.XubioTransactionId = transactionId;
//     //     });

//     //     await this.context.SaveChangesAsync();
//     //   }

//     //   #endregion  
// }