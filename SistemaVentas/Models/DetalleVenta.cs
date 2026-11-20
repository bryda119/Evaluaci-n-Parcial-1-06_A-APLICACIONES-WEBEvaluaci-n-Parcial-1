using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Models;

[Table("DetalleVentas")]
public class DetalleVenta
{
    [Key]
    [Column("detalle_id")]
    public int DetalleId { get; set; }

    [Required]
    [Column("venta_id")]
    public int VentaId { get; set; }

    [Required]
    [Column("producto_id")]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
    [Display(Name = "Cantidad")]
    public int Cantidad { get; set; }

    [Required]
    [Column("precio_unitario", TypeName = "decimal(18,2)")]
    [Display(Name = "Precio unitario")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal PrecioUnitario { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Subtotal")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Subtotal { get; set; }

    [ForeignKey(nameof(VentaId))]
    public Venta? Venta { get; set; }

    [ForeignKey(nameof(ProductoId))]
    public Producto? Producto { get; set; }
}
