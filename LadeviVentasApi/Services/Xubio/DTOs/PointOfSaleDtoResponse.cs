namespace LadeviVentasApi.Services.Xubio.DTOs;

public enum ModoNumeracion
{
    Automatico = 1,
    Manual = 2
}

public class PointOfSaleDtoResponse
{
    public string Codigo { get; set; }

    public string PuntoVenta { get; set; }

    public ModoNumeracion ModoNumeracion { get; set; }

}