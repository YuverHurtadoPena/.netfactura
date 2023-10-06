using System;
using System.Collections.Generic;

namespace factura.Models;

public partial class Detalle
{
    public int IdDetalle { get; set; }

    public int IdFactura { get; set; }

    public int IdProducto { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public virtual Factura IdFacturaNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
