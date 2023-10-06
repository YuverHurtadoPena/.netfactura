using System.ComponentModel.DataAnnotations;

namespace factura.Models
{
    public class FacturaDto
    {

        [Required(ErrorMessage = "El campo Documento del Cliente es obligatorio.")] 
        public string NombreCliente { get; set; }
        public string DocumentoCliente { get; set; }
        public DateTime FechaCompra { get; set; }
        public string FormaPago { get; set; }
        public int ProductoId { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public int NumeroFactura { get; set; }
        public decimal PorcentajeDescuento { get; set; }

    }
}
