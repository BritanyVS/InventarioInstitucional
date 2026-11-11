/* ============================================================
   config.js - Configuración del frontend
   Permite alternar entre los dos backends de la demostración.
   ============================================================ */
(function (global) {
    'use strict';

    var BACKENDS = {
        csharp: { nombre: 'C# / .NET 8', url: 'http://localhost:5080/api' },
        vbnet: { nombre: 'VB.NET / .NET Framework 4.8', url: 'http://localhost:5081/api' }
    };

    var clave = localStorage.getItem('backendActivo') || 'csharp';
    if (!BACKENDS[clave]) clave = 'csharp';

    global.CONFIG = {
        backends: BACKENDS,
        backendActivo: clave,
        apiBase: BACKENDS[clave].url,
        backendNombre: BACKENDS[clave].nombre,
        cambiarBackend: function (nuevaClave) {
            localStorage.setItem('backendActivo', nuevaClave);
            global.location.reload();
        }
    };
})(window);
