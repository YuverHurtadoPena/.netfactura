using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace factura.Models;

public partial class TFacurasContext : DbContext
{
    public TFacurasContext()
    {
    }

    public TFacurasContext(DbContextOptions<TFacurasContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Detalle> Detalles { get; set; }

    public virtual DbSet<Factura> Facturas { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

  //  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     //   => optionsBuilder.UseSqlServer("Server=localhost;Database=t_facuras;Integrated Security=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Detalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__Detalle__49CAE2FBB23E3BD5");

            entity.ToTable("Detalle");

            entity.Property(e => e.IdDetalle).HasColumnName("idDetalle");
            entity.Property(e => e.IdFactura).HasColumnName("idFactura");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdFacturaNavigation).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdFactura)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Factura_Detalle");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdProducto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Producto_Detalle");
        });

        modelBuilder.Entity<Factura>(entity =>
        {
            entity.HasKey(e => e.IdFactura).HasName("PK__Factura__3CD5687E163CAD88");

            entity.ToTable("Factura");

            entity.HasIndex(e => e.NumeroFactura, "UQ_NumeroFactura").IsUnique();

            entity.Property(e => e.IdFactura).HasColumnName("idFactura");
            entity.Property(e => e.Descuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.DocumentoCliente)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("IVA");
            entity.Property(e => e.NombreCliente)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TipodePago)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalDescuento).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TotalImpuesto).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__07F4A1324CD18E82");

            entity.ToTable("Producto");

            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.Producto1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("producto");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
