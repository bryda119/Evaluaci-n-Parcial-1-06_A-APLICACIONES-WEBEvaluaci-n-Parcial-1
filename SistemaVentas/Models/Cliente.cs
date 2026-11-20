using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentas.Models;

[Table("Clientes")]
public class Cliente
{
    [Key]
    [Column("cliente_id")]
    public int ClienteId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]+$", ErrorMessage = "El nombre solo permite letras y espacios")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
    [RegularExpression(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]+$", ErrorMessage = "El apellido solo permite letras y espacios")]
    [Display(Name = "Apellido")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Ingrese un email válido (ejemplo: usuario@correo.com)")]
    [StringLength(150, ErrorMessage = "El email no puede superar 150 caracteres")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(15, ErrorMessage = "El teléfono no puede superar 15 caracteres")]
    [RegularExpression(@"^$|^(\+?\d{7,15})$", ErrorMessage = "El teléfono solo permite números (7 a 15 dígitos), opcionalmente con +")]
    [Display(Name = "Teléfono")]
    [DataType(DataType.PhoneNumber)]
    public string? Telefono { get; set; }

    [NotMapped]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto => $"{Nombre} {Apellido}";

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}
