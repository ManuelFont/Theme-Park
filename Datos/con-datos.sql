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

INSERT INTO ParqueTematicoDB.dbo.Usuarios (Id,Nombre,Apellido,Email,ContraseniaHash,TipoUsuario,NfcId,FechaNacimiento,NivelMembresia) VALUES
	 (N'45FB56EE-8AC3-4F0D-997D-724143AD3BBD',N'Valentina',N'Perez',N'v.p@mail.com',N'RVoKwm46AZPk+TxiDR/0vEw+O1rKTcoRud63G20krwY=',N'Operador',NULL,NULL,NULL),
	 (N'47B6F6C6-FC2C-4BBD-8DB1-95972D3E0D2B',N'admin',N'admin',N'admin@admin.com',N'3iyXMZQtO+A3qKXX4e5vweiaGe2WxSPAnf/UbQB/AkA=',N'Administrador',NULL,NULL,NULL),
	 (N'84857109-B3C2-4B41-8DEB-A4D494E7980F',N'Mariana',N'Gómez',N'mariana.gomez@mail.com',N'WP5WyHXe+39PclLSaOUmLMbMry/xBKOYSRy8hLWhLWI=',N'Visitante',N'59A15862-86DE-44AF-8E17-4564A25EBCFE','1998-07-22 00:00:00.0000000',0);

INSERT INTO ParqueTematicoDB.dbo.Atracciones (Id,Nombre,Tipo,EdadMinima,CapacidadMaxima,Descripcion) VALUES
	 (N'01E3D564-34D5-457E-816E-34979B12BDE9',N'Montaña Rusa del Dragon',N'MontañaRusa',12,24,N'Una montaña rusa con giros de 360 grados y caída libre.'),
	 (N'15BB8204-1648-4F5B-8EC9-BBC03C97127B',N'Trampolin',N'Espectáculo',10,1,N'Extremo'),
	 (N'34B65E12-A332-464E-B32F-D73C10BEA694',N'Tobogan',N'Simulador',0,2,N'Tranquila');

INSERT INTO ParqueTematicoDB.dbo.Eventos (Id,Nombre,Fecha,Hora,Aforo,CostoAdicional) VALUES
	 (N'FF0EE953-673F-4AC5-AE69-2A0999E4ED08',N'Paquete Aventura Extrema','2025-11-15 00:00:00.0000000',378000000000,50,25.50);

INSERT INTO ParqueTematicoDB.dbo.Tickets (Id,VisitanteId,FechaVisita,TipoEntrada,EventoAsociadoId) VALUES
	 (N'6E53C4DE-3D39-4510-9F42-1F353A761F33',N'84857109-B3C2-4B41-8DEB-A4D494E7980F','2026-10-09 10:00:00.0000000',N'General',NULL);


INSERT INTO ParqueTematicoDB.dbo.EventoAtracciones (EventoId,AtraccionId) VALUES
	 (N'FF0EE953-673F-4AC5-AE69-2A0999E4ED08',N'15BB8204-1648-4F5B-8EC9-BBC03C97127B');

INSERT INTO ParqueTematicoDB.dbo.AccesosAtraccion (Id,VisitanteId,AtraccionId,TicketId,FechaHoraIngreso,FechaHoraEgreso,PuntosObtenidos) VALUES
	 (N'15954610-D854-438D-BB51-BB328EC928F4',N'84857109-B3C2-4B41-8DEB-A4D494E7980F',N'01E3D564-34D5-457E-816E-34979B12BDE9',N'6E53C4DE-3D39-4510-9F42-1F353A761F33','2026-10-09 09:21:00.0000000',NULL,0);

INSERT INTO ParqueTematicoDB.dbo.Incidencias (Id,AtraccionId,TipoIncidencia,Descripcion,EstaActiva) VALUES
	 (N'941D4D51-D4BE-4CF2-9B87-ABA84F6C0900',N'01E3D564-34D5-457E-816E-34979B12BDE9',N'FueraDeServicio',N'falla en el motor',1);

INSERT INTO ParqueTematicoDB.dbo.Relojes (FechaHora) VALUES
	 ('2026-10-09 09:21:00.0000000');

