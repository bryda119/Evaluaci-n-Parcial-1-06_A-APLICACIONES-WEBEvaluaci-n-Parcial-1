using System.ComponentModel.DataAnnotations;

namespace SistemaVentas.Models;

public class VentaCreateViewModel
{
    [Required(ErrorMessage = "Seleccione un cliente")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente válido")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    public List<DetalleVentaLinea> Lineas { get; set; } = new()
    {
        new DetalleVentaLinea()
    };
}

public class DetalleVentaLinea
{
    [Display(Name = "Producto")]
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto")]
    public int? ProductoId { get; set; }

    [Required(ErrorMessage = "La cantidad es obligatoria")]
    [Range(1, 100000, ErrorMessage = "La cantidad debe ser un número entero entre 1 y 100000")]
    [Display(Name = "Cantidad")]
    public int Cantidad { get; set; } = 1;
}
