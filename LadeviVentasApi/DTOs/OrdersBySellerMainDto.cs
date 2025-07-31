namespace LadeviVentasApi.DTOs;

public class OrdersBySellerMainDto
{
    public string Edicion { get; set; }
    public string ProductCountry { get; set; }
    public string Marca { get; set; }
    public string RazonSocial { get; set; }
    public DateTime FechaSalida { get; set; }
    public string Pagina { get; set; }
    public string Cliente { get; set; }
    public string Vendedor { get; set; }
    public string Numero { get; set; }
    public string Contrato { get; set; }
    public string Descuentos { get; set; }
    public ComisionDataDto ComisionData { get; set; }
    public double Importe { get; set; }
    public string BillingCondition { get; set; }
    public string Invoice { get; set; }
    public string TipoEspacio { get; set; }
    public double CantidadEspacios { get; set; }
    public string Moneda { get; set; }
    public double Quantity { get; set; }
}
