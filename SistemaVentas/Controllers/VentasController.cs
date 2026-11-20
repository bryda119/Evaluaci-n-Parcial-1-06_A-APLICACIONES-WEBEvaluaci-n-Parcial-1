using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Data;
using SistemaVentas.Models;

namespace SistemaVentas.Controllers;

public class VentasController : Controller
{
    private readonly AppDbContext _context;

    public VentasController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var ventas = await _context.Ventas
            .Include(v => v.Cliente)
            .OrderByDescending(v => v.Fecha)
            .ToListAsync();
        return View(ventas);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var venta = await _context.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Detalles)
                .ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(v => v.VentaId == id);

        if (venta == null) return NotFound();
        return View(venta);
    }

    public async Task<IActionResult> Create()
    {
        await CargarCombos();
        return View(new VentaCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VentaCreateViewModel model)
    {
        var lineasValidas = model.Lineas
            .Where(l => l.ProductoId.HasValue && l.ProductoId > 0 && l.Cantidad > 0)
            .ToList();

        if (lineasValidas.Count == 0)
            ModelState.AddModelError(string.Empty, "Agregue al menos un producto a la venta.");

        var clienteExiste = await _context.Clientes.AnyAsync(c => c.ClienteId == model.ClienteId);
        if (!clienteExiste)
            ModelState.AddModelError(nameof(model.ClienteId), "Cliente no válido.");

        var agrupadas = lineasValidas
            .GroupBy(l => l.ProductoId!.Value)
            .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(x => x.Cantidad) })
            .ToList();

        var productos = await _context.Productos
            .Where(p => agrupadas.Select(a => a.ProductoId).Contains(p.ProductoId))
            .ToDictionaryAsync(p => p.ProductoId);

        foreach (var linea in agrupadas)
        {
            if (!productos.TryGetValue(linea.ProductoId, out var producto))
            {
                ModelState.AddModelError(string.Empty, $"Producto {linea.ProductoId} no existe.");
                continue;
            }

            if (producto.Stock < linea.Cantidad)
            {
                ModelState.AddModelError(string.Empty,
                    $"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}, solicitado: {linea.Cantidad}.");
            }
        }

        if (!ModelState.IsValid)
        {
            await CargarCombos();
            return View(model);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var venta = new Venta
            {
                ClienteId = model.ClienteId,
                Fecha = DateTime.Now,
                Total = 0
            };

            foreach (var linea in agrupadas)
            {
                var producto = productos[linea.ProductoId];
                var subtotal = producto.Precio * linea.Cantidad;

                venta.Detalles.Add(new DetalleVenta
                {
                    ProductoId = producto.ProductoId,
                    Cantidad = linea.Cantidad,
                    PrecioUnitario = producto.Precio,
                    Subtotal = subtotal
                });

                producto.Stock -= linea.Cantidad;
                venta.Total += subtotal;
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            TempData["Mensaje"] = $"Venta #{venta.VentaId} registrada correctamente.";
            return RedirectToAction(nameof(Details), new { id = venta.VentaId });
        }
        catch
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "Error al registrar la venta. Intente de nuevo.");
            await CargarCombos();
            return View(model);
        }
    }

    private async Task CargarCombos()
    {
        ViewBag.Clientes = new SelectList(
            await _context.Clientes.OrderBy(c => c.Apellido).ToListAsync(),
            "ClienteId",
            "NombreCompleto");

        ViewBag.Productos = new SelectList(
            await _context.Productos.Where(p => p.Stock > 0).OrderBy(p => p.Nombre).ToListAsync(),
            "ProductoId",
            "Nombre");
    }
}
