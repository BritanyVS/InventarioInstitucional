/* ============================================================
   reportes.js - Reportes y exportación
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    if (!document.getElementById('tablaReporte')) return;

    cargarReporte();

    // ⚠️ El "filtro de valor" se evalúa con eval() (Code Injection - Snyk Code).
    var formFiltro = document.getElementById('formFiltro');
    if (formFiltro) {
        formFiltro.addEventListener('submit', function (e) {
            e.preventDefault();
            var expresion = document.getElementById('expresion').value;
            var resultado = Util.evaluarFiltroValor(expresion);
            Util.mostrarMensaje('#mensajeFiltro', 'Resultado: ' + resultado, 'ok');
        });
    }

    // Lectura de un reporte exportado en el servidor (VB.NET => Path Traversal).
    var formArchivo = document.getElementById('formArchivo');
    if (formArchivo) {
        formArchivo.addEventListener('submit', function (e) {
            e.preventDefault();
            var nombre = document.getElementById('nombreArchivo').value;
            Api.leerArchivoReporte(nombre)
                .then(function (r) {
                    document.getElementById('contenidoArchivo').textContent = r.contenido || '(vacío)';
                })
                .catch(function () {
                    Util.mostrarMensaje('#mensajeArchivo', 'No se pudo leer el archivo (endpoint disponible solo en el backend VB.NET).', 'error');
                });
        });
    }
});

function cargarReporte() {
    Api.listarActivos().then(function (datos) {
        var total = 0;
        var filas = '';
        (datos || []).forEach(function (a) {
            total += Number(a.valor) || 0;
            filas += '<tr>' +
                '<td>' + a.codigo + '</td>' +
                '<td>' + a.nombre + '</td>' +
                '<td>' + a.categoria + '</td>' +
                '<td><span class="badge ' + a.estado + '">' + a.estado + '</span></td>' +
                '<td>' + Util.formatearMoneda(a.valor) + '</td>' +
                '<td>' + Util.formatearFecha(a.fechaRegistro) + '</td>' +
                '</tr>';
        });
        if (!filas) filas = '<tr><td colspan="6">Sin datos.</td></tr>';
        document.querySelector('#tablaReporte tbody').innerHTML = filas;
        document.getElementById('totalReporte').textContent = Util.formatearMoneda(total);
    });
}
