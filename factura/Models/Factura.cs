using System;
using System.Collections.Generic;

namespace factura.Models;

public partial class Factura
{
    public int IdFactura { get; set; }

    public int? NumeroFactura { get; set; }

    public DateTime Fecha { get; set; }

    public string TipodePago { get; set; } = null!;

    public string DocumentoCliente { get; set; } = null!;

    public string NombreCliente { get; set; } = null!;

    public decimal Subtotal { get; set; }

    public decimal Descuento { get; set; }

    public decimal Iva { get; set; }

    public decimal TotalDescuento { get; set; }

    public decimal TotalImpuesto { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<Detalle> Detalles { get; set; } = new List<Detalle>();
}
