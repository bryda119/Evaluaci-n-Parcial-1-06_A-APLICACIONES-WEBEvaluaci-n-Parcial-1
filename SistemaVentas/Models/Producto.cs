using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Models;

[Table("Productos")]
public class Producto
{
    [Key]
    [Column("producto_id")]
    public int ProductoId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ0-9\s\.\-]+$", ErrorMessage = "El nombre solo permite letras, números, espacios, punto y guion")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede superar 500 caracteres")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser un número entre 0.01 y 999999.99")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Precio")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "El stock es obligatorio")]
    [Range(0, 1000000, ErrorMessage = "El stock debe ser un número entero entre 0 y 1.000.000")]
    [Display(Name = "Stock")]
    public int Stock { get; set; }

    public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
}
