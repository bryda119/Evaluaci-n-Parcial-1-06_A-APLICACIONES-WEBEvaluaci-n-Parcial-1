using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaVentas.Data;
using SistemaVentas.Models;

namespace SistemaVentas.Controllers;

public class ClientesController : Controller
{
    private readonly AppDbContext _context;

    public ClientesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Clientes.OrderBy(c => c.Apellido).ThenBy(c => c.Nombre).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == id);
        if (cliente == null) return NotFound();

        return View(cliente);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Cliente cliente)
    {
        if (await EmailExiste(cliente.Email))
            ModelState.AddModelError(nameof(cliente.Email), "Ya existe un cliente con ese email.");

        if (!ModelState.IsValid) return View(cliente);

        _context.Add(cliente);
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Cliente creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound();

        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Cliente cliente)
    {
        if (id != cliente.ClienteId) return NotFound();

        if (await EmailExiste(cliente.Email, id))
            ModelState.AddModelError(nameof(cliente.Email), "Ya existe un cliente con ese email.");

        if (!ModelState.IsValid) return View(cliente);

        try
        {
            _context.Update(cliente);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Cliente actualizado correctamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Clientes.AnyAsync(c => c.ClienteId == id))
                return NotFound();
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteId == id);
        if (cliente == null) return NotFound();

        return View(cliente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            var tieneVentas = await _context.Ventas.AnyAsync(v => v.ClienteId == id);
            if (tieneVentas)
            {
                TempData["Error"] = "No se puede eliminar: el cliente tiene ventas registradas.";
                return RedirectToAction(nameof(Index));
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Cliente eliminado correctamente.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> EmailExiste(string email, int? excluirId = null)
    {
        return await _context.Clientes
            .AnyAsync(c => c.Email == email && (!excluirId.HasValue || c.ClienteId != excluirId.Value));
    }
}
