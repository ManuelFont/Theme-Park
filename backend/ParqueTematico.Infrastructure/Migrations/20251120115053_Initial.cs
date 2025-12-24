using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atracciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EdadMinima = table.Column<int>(type: "int", nullable: false),
                    CapacidadMaxima = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atracciones", x => x.Id);
                    table.CheckConstraint("CK_Atraccion_CapacidadMaxima", "CapacidadMaxima >= 1");
                    table.CheckConstraint("CK_Atraccion_EdadMinima", "EdadMinima >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Hora = table.Column<long>(type: "bigint", nullable: false),
                    Aforo = table.Column<int>(type: "int", nullable: false),
                    CostoAdicional = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                    table.CheckConstraint("CK_Evento_Aforo", "Aforo >= 1");
                    table.CheckConstraint("CK_Evento_Costo", "CostoAdicional >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Recompensas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Costo = table.Column<int>(type: "int", nullable: false),
                    CantidadDisponible = table.Column<int>(type: "int", nullable: false),
                    NivelRequerido = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recompensas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Relojes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relojes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContraseniaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoUsuario = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    NfcId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NivelMembresia = table.Column<int>(type: "int", nullable: true),
                    PuntosActuales = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Incidencias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtraccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoIncidencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstaActiva = table.Column<bool>(type: "bit", nullable: false),
                    AtraccionId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incidencias_Atracciones_AtraccionId",
                        column: x => x.AtraccionId,
                        principalTable: "Atracciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Incidencias_Atracciones_AtraccionId1",
                        column: x => x.AtraccionId1,
                        principalTable: "Atracciones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventoAtracciones",
                columns: table => new
                {
                    EventoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtraccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoAtracciones", x => new { x.EventoId, x.AtraccionId });
                    table.ForeignKey(
                        name: "FK_EventoAtracciones_Atracciones_AtraccionId",
                        column: x => x.AtraccionId,
                        principalTable: "Atracciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoAtracciones_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Canjes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecompensaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCanje = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canjes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Canjes_Recompensas_RecompensaId",
                        column: x => x.RecompensaId,
                        principalTable: "Recompensas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Canjes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesPuntuacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Puntos = table.Column<int>(type: "int", nullable: false),
                    Origen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EstrategiaActiva = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesPuntuacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesPuntuacion_Usuarios_VisitanteId",
                        column: x => x.VisitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaVisita = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoEntrada = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EventoAsociadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.CheckConstraint("CK_Ticket_EventoEspecial_Requiere_Evento", "(TipoEntrada <> 'EventoEspecial') OR (EventoAsociadoId IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_Tickets_Eventos_EventoAsociadoId",
                        column: x => x.EventoAsociadoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_Usuarios_VisitanteId",
                        column: x => x.VisitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccesosAtraccion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AtraccionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaHoraIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaHoraEgreso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PuntosObtenidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesosAtraccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccesosAtraccion_Atracciones_AtraccionId",
                        column: x => x.AtraccionId,
                        principalTable: "Atracciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccesosAtraccion_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccesosAtraccion_Usuarios_VisitanteId",
                        column: x => x.VisitanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccesosAtraccion_AtraccionId_FechaHoraIngreso",
                table: "AccesosAtraccion",
                columns: new[] { "AtraccionId", "FechaHoraIngreso" });

            migrationBuilder.CreateIndex(
                name: "IX_AccesosAtraccion_TicketId",
                table: "AccesosAtraccion",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_AccesosAtraccion_VisitanteId_FechaHoraIngreso",
                table: "AccesosAtraccion",
                columns: new[] { "VisitanteId", "FechaHoraIngreso" });

            migrationBuilder.CreateIndex(
                name: "IX_Atracciones_Nombre",
                table: "Atracciones",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_RecompensaId",
                table: "Canjes",
                column: "RecompensaId");

            migrationBuilder.CreateIndex(
                name: "IX_Canjes_UsuarioId_RecompensaId_FechaCanje",
                table: "Canjes",
                columns: new[] { "UsuarioId", "RecompensaId", "FechaCanje" });

            migrationBuilder.CreateIndex(
                name: "IX_EventoAtracciones_AtraccionId",
                table: "EventoAtracciones",
                column: "AtraccionId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_Nombre_Fecha_Hora",
                table: "Eventos",
                columns: new[] { "Nombre", "Fecha", "Hora" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesPuntuacion_VisitanteId_FechaHora",
                table: "HistorialesPuntuacion",
                columns: new[] { "VisitanteId", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_AtraccionId_EstaActiva",
                table: "Incidencias",
                columns: new[] { "AtraccionId", "EstaActiva" });

            migrationBuilder.CreateIndex(
                name: "IX_Incidencias_AtraccionId1",
                table: "Incidencias",
                column: "AtraccionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EventoAsociadoId",
                table: "Tickets",
                column: "EventoAsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_VisitanteId_FechaVisita",
                table: "Tickets",
                columns: new[] { "VisitanteId", "FechaVisita" });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccesosAtraccion");

            migrationBuilder.DropTable(
                name: "Canjes");

            migrationBuilder.DropTable(
                name: "EventoAtracciones");

            migrationBuilder.DropTable(
                name: "HistorialesPuntuacion");

            migrationBuilder.DropTable(
                name: "Incidencias");

            migrationBuilder.DropTable(
                name: "Relojes");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Recompensas");

            migrationBuilder.DropTable(
                name: "Atracciones");

            migrationBuilder.DropTable(
                name: "Eventos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
