-- DROP SCHEMA dbo;

CREATE SCHEMA dbo;
-- ParqueTematicoDB.dbo.Atracciones definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.Atracciones;

CREATE TABLE ParqueTematicoDB.dbo.Atracciones (
	Id uniqueidentifier NOT NULL,
	Nombre nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Tipo nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EdadMinima int NOT NULL,
	CapacidadMaxima int NOT NULL,
	Descripcion nvarchar(1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK_Atracciones PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Atracciones_Nombre ON ParqueTematicoDB.dbo.Atracciones (  Nombre ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
ALTER TABLE ParqueTematicoDB.dbo.Atracciones WITH NOCHECK ADD CONSTRAINT CK_Atraccion_CapacidadMaxima CHECK (([CapacidadMaxima]>=(1)));
ALTER TABLE ParqueTematicoDB.dbo.Atracciones WITH NOCHECK ADD CONSTRAINT CK_Atraccion_EdadMinima CHECK (([EdadMinima]>=(0)));


-- ParqueTematicoDB.dbo.Eventos definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.Eventos;

CREATE TABLE ParqueTematicoDB.dbo.Eventos (
	Id uniqueidentifier NOT NULL,
	Nombre nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Fecha datetime2 NOT NULL,
	Hora bigint NOT NULL,
	Aforo int NOT NULL,
	CostoAdicional decimal(18,2) NOT NULL,
	CONSTRAINT PK_Eventos PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Eventos_Nombre_Fecha_Hora ON ParqueTematicoDB.dbo.Eventos (  Nombre ASC  , Fecha ASC  , Hora ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
ALTER TABLE ParqueTematicoDB.dbo.Eventos WITH NOCHECK ADD CONSTRAINT CK_Evento_Aforo CHECK (([Aforo]>=(1)));
ALTER TABLE ParqueTematicoDB.dbo.Eventos WITH NOCHECK ADD CONSTRAINT CK_Evento_Costo CHECK (([CostoAdicional]>=(0)));


-- ParqueTematicoDB.dbo.Relojes definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.Relojes;

CREATE TABLE ParqueTematicoDB.dbo.Relojes (
	Id int IDENTITY(1,1) NOT NULL,
	FechaHora datetime2 NOT NULL,
	CONSTRAINT PK_Relojes PRIMARY KEY (Id)
);


-- ParqueTematicoDB.dbo.Usuarios definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.Usuarios;

CREATE TABLE ParqueTematicoDB.dbo.Usuarios (
	Id uniqueidentifier NOT NULL,
	Nombre nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Apellido nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Email nvarchar(200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ContraseniaHash nvarchar(MAX) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	TipoUsuario nvarchar(13) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	NfcId uniqueidentifier NULL,
	FechaNacimiento datetime2 NULL,
	NivelMembresia int NULL,
	CONSTRAINT PK_Usuarios PRIMARY KEY (Id)
);
 CREATE UNIQUE NONCLUSTERED INDEX IX_Usuarios_Email ON ParqueTematicoDB.dbo.Usuarios (  Email ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- ParqueTematicoDB.dbo.[__EFMigrationsHistory] definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.[__EFMigrationsHistory];

CREATE TABLE ParqueTematicoDB.dbo.[__EFMigrationsHistory] (
	MigrationId nvarchar(150) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	ProductVersion nvarchar(32) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
);


-- ParqueTematicoDB.dbo.EventoAtracciones definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.EventoAtracciones;

CREATE TABLE ParqueTematicoDB.dbo.EventoAtracciones (
	EventoId uniqueidentifier NOT NULL,
	AtraccionId uniqueidentifier NOT NULL,
	CONSTRAINT PK_EventoAtracciones PRIMARY KEY (EventoId,AtraccionId),
	CONSTRAINT FK_EventoAtracciones_Atracciones_AtraccionId FOREIGN KEY (AtraccionId) REFERENCES ParqueTematicoDB.dbo.Atracciones(Id) ON DELETE CASCADE,
	CONSTRAINT FK_EventoAtracciones_Eventos_EventoId FOREIGN KEY (EventoId) REFERENCES ParqueTematicoDB.dbo.Eventos(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_EventoAtracciones_AtraccionId ON ParqueTematicoDB.dbo.EventoAtracciones (  AtraccionId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- ParqueTematicoDB.dbo.Incidencias definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.Incidencias;

CREATE TABLE ParqueTematicoDB.dbo.Incidencias (
	Id uniqueidentifier NOT NULL,
	AtraccionId uniqueidentifier NOT NULL,
	TipoIncidencia nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	Descripcion nvarchar(500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EstaActiva bit NOT NULL,
	CONSTRAINT PK_Incidencias PRIMARY KEY (Id),
	CONSTRAINT FK_Incidencias_Atracciones_AtraccionId FOREIGN KEY (AtraccionId) REFERENCES ParqueTematicoDB.dbo.Atracciones(Id)
);
 CREATE NONCLUSTERED INDEX IX_Incidencias_AtraccionId_EstaActiva ON ParqueTematicoDB.dbo.Incidencias (  AtraccionId ASC  , EstaActiva ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;


-- ParqueTematicoDB.dbo.Tickets definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.Tickets;

CREATE TABLE ParqueTematicoDB.dbo.Tickets (
	Id uniqueidentifier NOT NULL,
	VisitanteId uniqueidentifier NOT NULL,
	FechaVisita datetime2 NOT NULL,
	TipoEntrada nvarchar(20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	EventoAsociadoId uniqueidentifier NULL,
	CONSTRAINT PK_Tickets PRIMARY KEY (Id),
	CONSTRAINT FK_Tickets_Eventos_EventoAsociadoId FOREIGN KEY (EventoAsociadoId) REFERENCES ParqueTematicoDB.dbo.Eventos(Id),
	CONSTRAINT FK_Tickets_Usuarios_VisitanteId FOREIGN KEY (VisitanteId) REFERENCES ParqueTematicoDB.dbo.Usuarios(Id) ON DELETE CASCADE
);
 CREATE NONCLUSTERED INDEX IX_Tickets_EventoAsociadoId ON ParqueTematicoDB.dbo.Tickets (  EventoAsociadoId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_Tickets_VisitanteId_FechaVisita ON ParqueTematicoDB.dbo.Tickets (  VisitanteId ASC  , FechaVisita ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
ALTER TABLE ParqueTematicoDB.dbo.Tickets WITH NOCHECK ADD CONSTRAINT CK_Ticket_EventoEspecial_Requiere_Evento CHECK (([TipoEntrada]<>'EventoEspecial' OR [EventoAsociadoId] IS NOT NULL));


-- ParqueTematicoDB.dbo.AccesosAtraccion definition

-- Drop table

-- DROP TABLE ParqueTematicoDB.dbo.AccesosAtraccion;

CREATE TABLE ParqueTematicoDB.dbo.AccesosAtraccion (
	Id uniqueidentifier NOT NULL,
	VisitanteId uniqueidentifier NOT NULL,
	AtraccionId uniqueidentifier NOT NULL,
	TicketId uniqueidentifier NOT NULL,
	FechaHoraIngreso datetime2 NOT NULL,
	FechaHoraEgreso datetime2 NULL,
	PuntosObtenidos int NOT NULL,
	CONSTRAINT PK_AccesosAtraccion PRIMARY KEY (Id),
	CONSTRAINT FK_AccesosAtraccion_Atracciones_AtraccionId FOREIGN KEY (AtraccionId) REFERENCES ParqueTematicoDB.dbo.Atracciones(Id),
	CONSTRAINT FK_AccesosAtraccion_Tickets_TicketId FOREIGN KEY (TicketId) REFERENCES ParqueTematicoDB.dbo.Tickets(Id),
	CONSTRAINT FK_AccesosAtraccion_Usuarios_VisitanteId FOREIGN KEY (VisitanteId) REFERENCES ParqueTematicoDB.dbo.Usuarios(Id)
);
 CREATE NONCLUSTERED INDEX IX_AccesosAtraccion_AtraccionId_FechaHoraIngreso ON ParqueTematicoDB.dbo.AccesosAtraccion (  AtraccionId ASC  , FechaHoraIngreso ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_AccesosAtraccion_TicketId ON ParqueTematicoDB.dbo.AccesosAtraccion (  TicketId ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;
 CREATE NONCLUSTERED INDEX IX_AccesosAtraccion_VisitanteId_FechaHoraIngreso ON ParqueTematicoDB.dbo.AccesosAtraccion (  VisitanteId ASC  , FechaHoraIngreso ASC  )  
	 WITH (  PAD_INDEX = OFF ,FILLFACTOR = 100  ,SORT_IN_TEMPDB = OFF , IGNORE_DUP_KEY = OFF , STATISTICS_NORECOMPUTE = OFF , ONLINE = OFF , ALLOW_ROW_LOCKS = ON , ALLOW_PAGE_LOCKS = ON  )
	 ON [PRIMARY ] ;