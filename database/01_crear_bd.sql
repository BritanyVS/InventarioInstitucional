/* ============================================================
   Sistema de Gestión de Inventario Institucional
   Script 01 - Creación de base de datos y esquema
   Motor: SQL Server
   ============================================================ */

IF DB_ID('InventarioInstitucional') IS NULL
BEGIN
    CREATE DATABASE InventarioInstitucional;
END
GO

USE InventarioInstitucional;
GO

/* ---------- Usuarios ---------- */
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
CREATE TABLE dbo.Usuarios
(
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario   NVARCHAR(60)  NOT NULL UNIQUE,
    NombreCompleto  NVARCHAR(120) NOT NULL,
    ClaveHash       NVARCHAR(128) NOT NULL,   -- Hash MD5 (intencionalmente débil)
    Rol             NVARCHAR(30)  NOT NULL DEFAULT 'Consulta',
    Activo          BIT           NOT NULL DEFAULT 1,
    FechaCreacion   DATETIME      NOT NULL DEFAULT GETDATE()
);
GO

/* ---------- Activos ---------- */
IF OBJECT_ID('dbo.Activos', 'U') IS NOT NULL DROP TABLE dbo.Activos;
CREATE TABLE dbo.Activos
(
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    Codigo        NVARCHAR(30)   NOT NULL UNIQUE,
    Nombre        NVARCHAR(150)  NOT NULL,
    Categoria     NVARCHAR(60)   NOT NULL,
    Marca         NVARCHAR(60)   NULL,
    Modelo        NVARCHAR(60)   NULL,
    NumeroSerie   NVARCHAR(80)   NULL,
    Estado        NVARCHAR(30)   NOT NULL DEFAULT 'Disponible',
    Ubicacion     NVARCHAR(120)  NULL,
    Valor         DECIMAL(12,2)  NOT NULL DEFAULT 0,
    FechaRegistro DATETIME       NOT NULL DEFAULT GETDATE()
);
GO

/* ---------- Movimientos (entradas / salidas) ---------- */
IF OBJECT_ID('dbo.Movimientos', 'U') IS NOT NULL DROP TABLE dbo.Movimientos;
CREATE TABLE dbo.Movimientos
(
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    ActivoId     INT           NOT NULL,
    Tipo         NVARCHAR(15)  NOT NULL,   -- Entrada | Salida
    Cantidad     INT           NOT NULL DEFAULT 1,
    Usuario      NVARCHAR(60)  NULL,
    Observacion  NVARCHAR(250) NULL,
    Fecha        DATETIME      NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Movimientos_Activos FOREIGN KEY (ActivoId) REFERENCES dbo.Activos(Id)
);
GO

/* ---------- Asignaciones ---------- */
IF OBJECT_ID('dbo.Asignaciones', 'U') IS NOT NULL DROP TABLE dbo.Asignaciones;
CREATE TABLE dbo.Asignaciones
(
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    ActivoId         INT           NOT NULL,
    UsuarioAsignado  NVARCHAR(120) NOT NULL,
    Area             NVARCHAR(80)  NULL,
    FechaAsignacion  DATETIME      NOT NULL DEFAULT GETDATE(),
    FechaDevolucion  DATETIME      NULL,
    CONSTRAINT FK_Asignaciones_Activos FOREIGN KEY (ActivoId) REFERENCES dbo.Activos(Id)
);
GO

PRINT 'Base de datos InventarioInstitucional creada correctamente.';
GO
