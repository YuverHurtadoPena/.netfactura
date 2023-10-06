using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using factura.Models;
using System.Collections;
using System.Text.Json;

namespace factura.Controllers
{
    public class FacturasController : Controller
    {
        private readonly TFacurasContext _context;
        List<FacturaDto> facturaInfoTemp = new List<FacturaDto>();


        public FacturasController(TFacurasContext context)
        {
            _context = context;
            
        }

        // GET: Facturas
        public async Task<IActionResult> Index()
        {
              return _context.Facturas != null ? 
                          View(await _context.Facturas.ToListAsync()) :
                          Problem("Entity set 'TFacurasContext.Facturas'  is null.");
        }

        // GET: Facturas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // GET: Facturas/Create
        public IActionResult Create()
        {
            var productos = _context.Productos.ToList();
            ViewBag.Productos = productos ?? new List<Producto>();
            List<FacturaDto> facturaInfoTemp = ObtenerFacturaInfoTemp();

            ViewBag.FacturaInfo = facturaInfoTemp;
            return View();
        }
        private List<FacturaDto> ObtenerFacturaInfoTemp()
        {
            if (Request.Cookies.TryGetValue("FacturaInfoTempCookie", out string facturaInfoTempJson))
            {
                return JsonSerializer.Deserialize<List<FacturaDto>>(facturaInfoTempJson);
            }
            return null;
        }

        
        public async Task<bool> ExisteFacturaPorNumero(int numeroFactura)
        {
            return await _context.Facturas.AnyAsync(f => f.NumeroFactura == numeroFactura);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacturaDto facturaDto, int enviar)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();


            List<FacturaDto> facturaInfoTemp = new List<FacturaDto>();
            if (!await ExisteFacturaPorNumero(facturaDto.NumeroFactura).ConfigureAwait(false)) { 
           
            if (enviar == 0){
                if (facturaInfoTemp == null && (string.IsNullOrEmpty(facturaDto.NombreCliente) ||
                    string.IsNullOrEmpty(facturaDto.DocumentoCliente) ||
                    facturaDto.FechaCompra == default(DateTime) ||
                    string.IsNullOrEmpty(facturaDto.FormaPago) ||
                    facturaDto.ProductoId == 0 ||
                    facturaDto.Precio < 0 ||
                    facturaDto.Cantidad < 0 ||
                    facturaDto.NumeroFactura <= 0 ||
                    facturaDto.PorcentajeDescuento <= 0))
                {
                    var producto = _context.Productos.ToList();
                    ViewBag.Productos = producto ?? new List<Producto>();
                    ViewBag.msjError = "Todos los compos son obligatorios y campos como precio, cantidad deben ser mayor a cero (0)";

                    return View(facturaDto);
                }
                else if (facturaInfoTemp != null && (string.IsNullOrEmpty(facturaDto.NombreCliente) ||

                    facturaDto.ProductoId == 0 ||
                    facturaDto.Precio <= 0 ||
                    facturaDto.Cantidad <= 0 ||
                    facturaDto.PorcentajeDescuento <= 0))
                {
                    var producto = _context.Productos.ToList();
                    ViewBag.Productos = producto ?? new List<Producto>();
                     

                }
                var nuevaFacturaDto = new FacturaDto
                {
                    NombreCliente = facturaDto.NombreCliente,
                    DocumentoCliente = facturaDto.DocumentoCliente,
                    FechaCompra = facturaDto.FechaCompra,
                    FormaPago = facturaDto.FormaPago,
                    ProductoId = facturaDto.ProductoId,
                    Precio = facturaDto.Precio,
                    Cantidad = facturaDto.Cantidad,
                    NumeroFactura = facturaDto.NumeroFactura,
                    PorcentajeDescuento = facturaDto.PorcentajeDescuento
                };

                    
                    if (Request.Cookies.TryGetValue("FacturaInfoTempCookie", out string facturaInfoTempJson))
                    {
                        facturaInfoTemp = JsonSerializer.Deserialize<List<FacturaDto>>(facturaInfoTempJson);
                    }

                    facturaInfoTemp.Add(nuevaFacturaDto);
                GuardarFacturaInfoTempCookie(facturaInfoTemp);

                ViewBag.FacturaInfo = facturaInfoTemp ?? new List<FacturaDto>();

                ModelState.Clear();

                var productos = _context.Productos.ToList();
                ViewBag.Productos = productos ?? new List<Producto>();
                ViewBag.msjExito = "Producto agregado correctamente";
            }
            else
            {
                try {
                    List<FacturaDto> DataFactura = ObtenerFacturaInfoTemp();
                    decimal TotalImpuesto = 0;
                    decimal SubTotal = 0;
                    decimal Total = 0;
                     decimal TotalDescuento = 0;
                    foreach (FacturaDto Item in DataFactura)
                    {
                        SubTotal = SubTotal + (Item.Precio * Item.Cantidad);
                        TotalImpuesto = TotalImpuesto + (Item.Precio * (19.0M / 100.0M));
                        TotalDescuento = TotalDescuento + (Item.Precio * (Item.PorcentajeDescuento / 100));
                    }
                    Total = SubTotal + TotalImpuesto - TotalDescuento;
                    Factura Ftra = new Factura();
                    Ftra.Total = (decimal)Total;
                    Ftra.Subtotal = (decimal)SubTotal;
                    Ftra.TotalImpuesto = (decimal)TotalImpuesto;
                    Ftra.TotalDescuento = (decimal)TotalDescuento;
                    FacturaDto PrimerElemento = DataFactura[0];
                    Ftra.NumeroFactura = PrimerElemento.NumeroFactura;
                    Ftra.DocumentoCliente = PrimerElemento.DocumentoCliente;
                    Ftra.NombreCliente = PrimerElemento.NombreCliente;
                    Ftra.TipodePago = PrimerElemento.FormaPago;
                    Ftra.Iva = 19;
                    _context.Add(Ftra);
                    await _context.SaveChangesAsync();
                    int facturaId = Ftra.IdFactura;
                    Detalle Dlle = null;
                    foreach (FacturaDto Item in DataFactura)
                    {
                        Dlle = new Detalle();
                        Dlle.Cantidad = Item.Cantidad;
                        Dlle.IdProducto = Item.ProductoId;
                        Dlle.PrecioUnitario = (decimal)Item.Precio;
                        Dlle.IdFactura = facturaId;
                        _context.Detalles.Add(Dlle);
                        await _context.SaveChangesAsync();
                    }
                        transaction.Commit();  

                        EliminarFacturaInfoTempCookie();

                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex) {
                    transaction.Rollback();
                     EliminarFacturaInfoTempCookie();
                     ViewBag.msjError = "Ocurrió un error al guardar los datos: ";
                    return View(facturaDto);
                }
                


            }
            }
            else { ViewBag.msjError = "El nùmero de factura ya se encuentra en la base de datos, por favor cambiarlo";
                EliminarFacturaInfoTempCookie();
            }
            return View();
        }

        private void EliminarFacturaInfoTempCookie()
        { 
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(-1),  
                HttpOnly = true,  
            };
            Response.Cookies.Delete("FacturaInfoTempCookie", cookieOptions);
        }


        
        private void GuardarFacturaInfoTempCookie(List<FacturaDto> facturaInfoTemp)
        {
       
            string facturaInfoTempJson = JsonSerializer.Serialize(facturaInfoTemp);
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1), 
                HttpOnly = true,  
            };

            Response.Cookies.Append("FacturaInfoTempCookie", facturaInfoTempJson, cookieOptions);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas.FindAsync(id);
            if (factura == null)
            {
                return NotFound();
            }
            return View(factura);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdFactura,NumeroFactura,Fecha,TipodePago,DocumentoCliente,NombreCliente,Subtotal,Descuento,Iva,TotalDescuento,TotalImpuesto,Total")] Factura factura)
        {
            if (id != factura.IdFactura)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(factura);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacturaExists(factura.IdFactura))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(factura);
        }

        // GET: Facturas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Facturas == null)
            {
                return NotFound();
            }

            var factura = await _context.Facturas
                .FirstOrDefaultAsync(m => m.IdFactura == id);
            if (factura == null)
            {
                return NotFound();
            }

            return View(factura);
        }

        // POST: Facturas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Facturas == null)
            {
                return Problem("Entity set 'TFacurasContext.Facturas'  is null.");
            }
            var factura = await _context.Facturas.FindAsync(id);
            if (factura != null)
            {
                var detallesRelacionados = _context.Detalles.Where(d => d.IdFactura == id);
                _context.Detalles.RemoveRange(detallesRelacionados);
                _context.Facturas.Remove(factura);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacturaExists(int id)
        {
          return (_context.Facturas?.Any(e => e.IdFactura == id)).GetValueOrDefault();
        }
    }
}
