using Dominio.Entities;
using Dominio.Entities.Usuarios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Infrastructure;
public class ParqueDbContext(DbContextOptions<ParqueDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Visitante> Visitantes => Set<Visitante>();
    public DbSet<Operador> Operadores => Set<Operador>();
    public DbSet<Administrador> Administradores => Set<Administrador>();
    public DbSet<Atraccion> Atracciones => Set<Atraccion>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Incidencia> Incidencias => Set<Incidencia>();
    public DbSet<AccesoAtraccion> AccesosAtraccion => Set<AccesoAtraccion>();
    public DbSet<Reloj> Relojes => Set<Reloj>();
    public DbSet<HistorialPuntuacion> HistorialesPuntuacion => Set<HistorialPuntuacion>();
    public DbSet<Recompensa> Recompensas => Set<Recompensa>();
    public DbSet<Canje> Canjes => Set<Canje>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigurarUsuarios(modelBuilder);
        ConfigurarReloj(modelBuilder);
        ConfigurarAtracciones(modelBuilder);
        ConfigurarEventos(modelBuilder);
        ConfigurarTickets(modelBuilder);
        ConfigurarIncidencias(modelBuilder);
        ConfigurarAccesosAtraccion(modelBuilder);
        ConfigurarHistorialPuntuacion(modelBuilder);
        ConfigurarRecompensas(modelBuilder);
        ConfigurarCanjes(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }

    private void ConfigurarUsuarios(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasDiscriminator<string>("TipoUsuario")
            .HasValue<Visitante>("Visitante")
            .HasValue<Operador>("Operador")
            .HasValue<Administrador>("Administrador");

        var usuario = modelBuilder.Entity<Usuario>();
        usuario.HasIndex(u => u.Email).IsUnique();
        usuario.Property(u => u.Nombre).HasMaxLength(50).IsRequired();
        usuario.Property(u => u.Apellido).HasMaxLength(50).IsRequired();
        usuario.Property(u => u.Email).HasMaxLength(200).IsRequired();

        modelBuilder.Entity<Usuario>().OwnsOne(u => u.Contrasenia, b =>
        {
            b.Property(c => c.Hash)
                .HasColumnName("ContraseniaHash")
                .IsRequired();
        });

        modelBuilder.Entity<Visitante>()
            .Property(v => v.NfcId)
            .IsRequired();
    }

    private void ConfigurarReloj(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reloj>(b =>
        {
            b.HasKey(r => r.Id);

            b.Property(r => r.FechaHora)
                .IsRequired();
        });
    }

    private void ConfigurarAtracciones(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atraccion>(b =>
        {
            b.HasKey(a => a.Id);
            b.HasIndex(a => a.Nombre).IsUnique();
            b.Property(a => a.Nombre).HasMaxLength(100).IsRequired();
            b.Property(a => a.Tipo)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();
            b.Property(a => a.EdadMinima).IsRequired();
            b.Property(a => a.CapacidadMaxima).IsRequired();
            b.Property(a => a.Descripcion).HasMaxLength(1000).IsRequired();
            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Atraccion_EdadMinima", "EdadMinima >= 0");
                tb.HasCheckConstraint("CK_Atraccion_CapacidadMaxima", "CapacidadMaxima >= 1");
            });
        });
    }

    private void ConfigurarEventos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evento>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            b.Property(e => e.Fecha).IsRequired();
            var timeSpanToTicks = new TimeSpanToTicksConverter();
            b.Property(e => e.Hora)
                .HasConversion(timeSpanToTicks)
                .IsRequired();
            b.Property(e => e.Aforo).IsRequired();
            b.Property(e => e.CostoAdicional).HasPrecision(18, 2).IsRequired();
            b.HasIndex(e => new { e.Nombre, e.Fecha, e.Hora }).IsUnique();
            b.ToTable(tb =>
            {
                tb.HasCheckConstraint("CK_Evento_Aforo", "Aforo >= 1");
                tb.HasCheckConstraint("CK_Evento_Costo", "CostoAdicional >= 0");
            });
            b.HasMany(e => e.Atracciones)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "EventoAtracciones",
                    j => j.HasOne<Atraccion>()
                        .WithMany()
                        .HasForeignKey("AtraccionId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Evento>()
                        .WithMany()
                        .HasForeignKey("EventoId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("EventoId", "AtraccionId");
                    });
        });
    }

    private void ConfigurarTickets(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ticket>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.FechaVisita).IsRequired();
            b.Property(t => t.TipoEntrada)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            b.HasOne(t => t.Visitante)
                .WithMany()
                .HasForeignKey("VisitanteId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasOne(t => t.EventoAsociado)
                .WithMany()
                .HasForeignKey("EventoAsociadoId")
                .OnDelete(DeleteBehavior.Restrict);

            b.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Ticket_EventoEspecial_Requiere_Evento",
                    "(TipoEntrada <> 'EventoEspecial') OR (EventoAsociadoId IS NOT NULL)");
            });
            b.HasIndex("VisitanteId", nameof(Ticket.FechaVisita));
        });
    }

    private void ConfigurarIncidencias(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Incidencia>(b =>
        {
            b.HasKey(i => i.Id);

            b.Property(i => i.TipoIncidencia)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            b.Property(i => i.Descripcion)
                .HasMaxLength(500)
                .IsRequired();

            b.Property(i => i.EstaActiva)
                .IsRequired();

            b.HasOne(i => i.Atraccion)
                .WithMany(a => a.Incidencias)
                .HasForeignKey(i => i.AtraccionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasIndex("AtraccionId", nameof(Incidencia.EstaActiva));

            b.HasDiscriminator<string>("Discriminator")
                .HasValue<Incidencia>("Incidencia")
                .HasValue<MantenimientoPreventivo>("MantenimientoPreventivo");
        });

        modelBuilder.Entity<MantenimientoPreventivo>(b =>
        {
            b.Property(m => m.FechaInicio).IsRequired();
            b.Property(m => m.FechaFin).IsRequired();
        });
    }

    private void ConfigurarAccesosAtraccion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccesoAtraccion>(b =>
        {
            b.HasKey(a => a.Id);

            b.Property(a => a.FechaHoraIngreso)
                .IsRequired();

            b.Property(a => a.FechaHoraEgreso);

            b.Property(a => a.PuntosObtenidos)
                .IsRequired();

            b.HasOne(a => a.Visitante)
                .WithMany()
                .HasForeignKey("VisitanteId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasOne(a => a.Atraccion)
                .WithMany()
                .HasForeignKey(i => i.AtraccionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasOne(a => a.Ticket)
                .WithMany()
                .HasForeignKey("TicketId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.Ignore(a => a.EstaActivo);
            b.Ignore(a => a.TiempoPermanencia);

            b.HasIndex("AtraccionId", "FechaHoraIngreso");
            b.HasIndex("VisitanteId", "FechaHoraIngreso");
        });
    }

    private void ConfigurarHistorialPuntuacion(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HistorialPuntuacion>(b =>
        {
            b.HasKey(h => h.Id);

            b.Property(h => h.Puntos)
                .IsRequired();

            b.Property(h => h.Origen)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(h => h.EstrategiaActiva)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(h => h.FechaHora)
                .IsRequired();

            b.HasOne(h => h.Visitante)
                .WithMany()
                .HasForeignKey("VisitanteId")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            b.HasIndex("VisitanteId", nameof(HistorialPuntuacion.FechaHora));
        });
    }

    private void ConfigurarRecompensas(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Recompensa>(b =>
        {
            b.HasKey(r => r.Id);

            b.Property(r => r.Nombre)
                .IsRequired()
                .HasMaxLength(20);

            b.Property(r => r.Descripcion)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(r => r.Costo)
                .IsRequired();

            b.Property(r => r.CantidadDisponible)
                .IsRequired();

            b.Property(r => r.NivelRequerido)
                .HasConversion<int?>()
                .IsRequired(false);
        });
    }

    private void ConfigurarCanjes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Canje>(b =>
        {
            b.HasKey(c => c.Id);

            b.Property(c => c.FechaCanje)
                .IsRequired();

            b.HasOne(c => c.Usuario)
                .WithMany()
                .HasForeignKey("UsuarioId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasOne(c => c.Recompensa)
                .WithMany()
                .HasForeignKey("RecompensaId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasIndex("UsuarioId", "RecompensaId", "FechaCanje");
        });
    }
}
