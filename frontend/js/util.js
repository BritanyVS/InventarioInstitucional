/* ============================================================
   util.js - Utilidades del frontend
   ============================================================ */
(function (global) {
    'use strict';

    var Util = {

        /* ------------------------------------------------------------
           ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - DOM XSS / CWE-79)
           El término buscado por el usuario se inserta con innerHTML sin
           escapar. Payload de prueba en el buscador:
               <img src=x onerror=alert('XSS')>
           Corregiría usando textContent o escapando el contenido.
           ------------------------------------------------------------ */
        pintarBusqueda: function (selector, termino) {
            var contenedor = document.querySelector(selector);
            if (contenedor) {
                contenedor.innerHTML = 'Mostrando resultados para: <strong>' + termino + '</strong>';
            }
        },

        /* ------------------------------------------------------------
           ⚠️ VULNERABILIDAD INTENCIONAL (Snyk Code - Code Injection / CWE-95)
           eval() sobre una expresión ingresada por el usuario (filtro de
           valor). Payload de prueba:
               1+1   ó   alert('ejecutado')
           Corregiría evitando eval() por completo.
           ------------------------------------------------------------ */
        evaluarFiltroValor: function (expresion) {
            try {
                return eval(expresion);
            } catch (e) {
                return NaN;
            }
        },

        formatearMoneda: function (valor) {
            var numero = Number(valor) || 0;
            return 'Q ' + numero.toLocaleString('es-GT', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
        },

        formatearFecha: function (fecha) {
            if (!fecha) return '-';
            var d = new Date(fecha);
            if (isNaN(d.getTime())) return '-';
            return d.toLocaleDateString('es-GT') + ' ' + d.toLocaleTimeString('es-GT', { hour: '2-digit', minute: '2-digit' });
        },

        mostrarMensaje: function (selector, texto, tipo) {
            var el = document.querySelector(selector);
            if (!el) return;
            el.textContent = texto;
            el.className = 'mensaje ' + (tipo || 'ok');
            setTimeout(function () { el.className = 'mensaje'; }, 4000);
        },

        sesion: function () {
            try { return JSON.parse(localStorage.getItem('sesion') || 'null'); }
            catch (e) { return null; }
        }
    };

    global.Util = Util;
})(window);
