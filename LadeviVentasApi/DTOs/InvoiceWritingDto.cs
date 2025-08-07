namespace LadeviVentasApi.DTOs;

using System.Collections.Generic;

/// <summary>
/// Modelo para representar la solicitud de creación de factura que se envía a Xubio
/// </summary>
public class InvoiceWritingDto
{
    /// <summary>
    /// ID del cliente al que se le emitirá la factura
    /// </summary>
    public long ClientId { get; set; }

    /// <summary>
    /// Observaciones globales que se aplicarán a toda la factura
    /// </summary>
    public string GlobalObservations { get; set; }

    /// <summary>
    /// Tipo de entidad a facturar (CONTRACT u ORDER)
    /// </summary>
    public string EntityType { get; set; }

    /// <summary>
    /// Indica si la factura es consolidada (true) o separada (false)
    /// </summary>
    public bool IsConsolidated { get; set; }

    /// <summary>
    /// Lista de ítems que se incluirán en la factura
    /// </summary>
    public List<InvoiceItemDto> Items { get; set; }
}

/// <summary>
/// Modelo para representar un ítem de factura
/// </summary>
public class InvoiceItemDto
{
    /// <summary>
    /// ID de la entidad (ID del espacio de contrato o ID de la orden)
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Campo opcional para IDs consolidados
    /// </summary>
    public List<long>? ConsolidatedIds { get; set; }

    /// <summary>
    /// ID del producto de Xubio a utilizar para este ítem
    /// </summary>
    public string XubioProductCode { get; set; }

    /// <summary>
    /// Cantidad de unidades de este detalle
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Precio unitario de este detalle
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Monto total del ítem
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Impuesto del ítem
    /// </summary>
    public decimal TotalTaxes { get; set; }

    /// <summary>
    /// Observaciones específicas para este ítem
    /// </summary>
    public string Observations { get; set; }
}

