/* ============================================================
   api.js - Cliente HTTP (axios) hacia los backends
   ============================================================ */
(function (global) {
    'use strict';

    var cliente = axios.create({
        baseURL: global.CONFIG.apiBase,
        timeout: 10000,
        headers: { 'Content-Type': 'application/json' }
    });

    var Api = {
        // --- Activos ---
        listarActivos: function () { return cliente.get('/activos').then(r => r.data); },
        buscarActivos: function (termino) { return cliente.get('/activos/buscar', { params: { termino: termino } }).then(r => r.data); },
        crearActivo: function (dto) { return cliente.post('/activos', dto).then(r => r.data); },
        actualizarActivo: function (id, dto) { return cliente.put('/activos/' + id, dto).then(r => r.data); },
        eliminarActivo: function (id) { return cliente.delete('/activos/' + id).then(r => r.data); },

        // --- Movimientos ---
        listarMovimientos: function () { return cliente.get('/movimientos').then(r => r.data); },
        crearMovimiento: function (dto) { return cliente.post('/movimientos', dto).then(r => r.data); },

        // --- Asignaciones ---
        listarAsignaciones: function () { return cliente.get('/asignaciones').then(r => r.data); },
        crearAsignacion: function (dto) { return cliente.post('/asignaciones', dto).then(r => r.data); },

        // --- Autenticación ---
        login: function (dto) { return cliente.post('/auth/login', dto).then(r => r.data); },

        // --- Reportes ---
        reporteInventario: function () { return cliente.get('/reportes/inventario').then(r => r.data); },
        leerArchivoReporte: function (nombre) { return cliente.get('/reportes/archivo', { params: { nombre: nombre } }).then(r => r.data); }
    };

    global.Api = Api;
})(window);
