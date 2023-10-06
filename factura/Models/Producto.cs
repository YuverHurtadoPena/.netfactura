using System;
using System.Collections.Generic;

namespace factura.Models;

public partial class Producto
{
    public int IdProducto { get; set; }

    public string? Producto1 { get; set; }

    public virtual ICollection<Detalle> Detalles { get; set; } = new List<Detalle>();
}
