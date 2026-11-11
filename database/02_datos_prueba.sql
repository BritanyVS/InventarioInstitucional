/* ============================================================
   Script 02 - Datos de prueba
   ============================================================ */

USE InventarioInstitucional;
GO

/* Usuarios. Las claves se almacenan como MD5 (vulnerabilidad intencional).
   admin / admin123  -> 0192023a7bbd73250516f069df18b500
   consulta / 123456 -> e10adc3949ba59abbe56e057f20f883e  */
INSERT INTO dbo.Usuarios (NombreUsuario, NombreCompleto, ClaveHash, Rol)
VALUES
    ('admin',    'Administrador del Sistema', '0192023a7bbd73250516f069df18b500', 'Administrador'),
    ('consulta', 'Usuario de Consulta',       'e10adc3949ba59abbe56e057f20f883e', 'Consulta');
GO

INSERT INTO dbo.Activos (Codigo, Nombre, Categoria, Marca, Modelo, NumeroSerie, Estado, Ubicacion, Valor)
VALUES
    ('MAG-0001', 'Laptop HP ProBook 450', 'Equipo de Cómputo', 'HP',     'ProBook 450 G9', 'SN-HP-0001', 'Disponible',   'Bodega Central',      8500.00),
    ('MAG-0002', 'Impresora Epson L3250',  'Impresora',         'Epson',  'L3250',          'SN-EP-0002', 'Asignado',     'Dirección Técnica',    1850.00),
    ('MAG-0003', 'Escritorio de Oficina',  'Mobiliario',        'Genérico','E-120',         'SN-MB-0003', 'Disponible',   'Bodega Central',       950.00),
    ('MAG-0004', 'Servidor Dell PowerEdge','Servidor',          'Dell',   'PowerEdge R650', 'SN-DL-0004', 'Mantenimiento','Sala de Servidores', 45000.00),
    ('MAG-0005', 'Proyector ViewSonic',    'Equipo Audiovisual','ViewSonic','PA503S',      'SN-VS-0005', 'Disponible',   'Bodega Central',      3200.00),
    ('MAG-0006', 'Switch Cisco 24 puertos','Redes',             'Cisco',  'SG250-24',       'SN-CS-0006', 'Asignado',     'Sala de Servidores',  6200.00);
GO

INSERT INTO dbo.Movimientos (ActivoId, Tipo, Cantidad, Usuario, Observacion)
VALUES
    (1, 'Entrada', 1, 'admin', 'Ingreso inicial de equipo'),
    (2, 'Salida',  1, 'admin', 'Asignado a Dirección Técnica'),
    (5, 'Entrada', 1, 'admin', 'Ingreso por compra 2026');
GO

INSERT INTO dbo.Asignaciones (ActivoId, UsuarioAsignado, Area)
VALUES
    (2, 'Juan Pérez', 'Dirección Técnica'),
    (6, 'María López', 'Informática');
GO

PRINT 'Datos de prueba insertados correctamente.';
GO
