using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Models;

[Table("Ventas")]
public class Venta
{
    [Key]
    [Column("venta_id")]
    public int VentaId { get; set; }

    [Required]
    [Column("cliente_id")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    [Required]
    [Display(Name = "Fecha")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Total")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Total { get; set; }

    [ForeignKey(nameof(ClienteId))]
    public Cliente? Cliente { get; set; }

    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
