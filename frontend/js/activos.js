/* ============================================================
   activos.js - Registro y consulta de activos
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    if (!document.getElementById('tablaActivos')) return;

    cargarActivos();

    document.getElementById('formActivo').addEventListener('submit', function (e) {
        e.preventDefault();
        var dto = {
            codigo: document.getElementById('codigo').value,
            nombre: document.getElementById('nombre').value,
            categoria: document.getElementById('categoria').value,
            marca: document.getElementById('marca').value,
            modelo: document.getElementById('modelo').value,
            numeroSerie: document.getElementById('numeroSerie').value,
            ubicacion: document.getElementById('ubicacion').value,
            valor: parseFloat(document.getElementById('valor').value) || 0
        };
        Api.crearActivo(dto)
            .then(function () {
                Util.mostrarMensaje('#mensajeActivo', 'Activo registrado correctamente.', 'ok');
                document.getElementById('formActivo').reset();
                cargarActivos();
            })
            .catch(function () {
                Util.mostrarMensaje('#mensajeActivo', 'Error al registrar el activo.', 'error');
            });
    });

    // Búsqueda (usa el endpoint con concatenación SQL => SQLi en el backend)
    document.getElementById('formBusqueda').addEventListener('submit', function (e) {
        e.preventDefault();
        var termino = document.getElementById('termino').value;

        // ⚠️ Eco del término con innerHTML (DOM XSS - Snyk Code).
        Util.pintarBusqueda('#resultadoBusqueda', termino);

        Api.buscarActivos(termino)
            .then(function (datos) { pintarTablaActivos(datos); })
            .catch(function () {
                Util.mostrarMensaje('#mensajeActivo', 'Error en la búsqueda.', 'error');
            });
    });
});

function cargarActivos() {
    Api.listarActivos()
        .then(function (datos) { pintarTablaActivos(datos); })
        .catch(function () {
            Util.mostrarMensaje('#mensajeActivo', 'No se pudieron cargar los activos. ¿Backend en ejecución?', 'error');
        });
}

function pintarTablaActivos(datos) {
    // lodash: ordenamiento de resultados (dependencia npm).
    var ordenados = _.orderBy(datos || [], ['id'], ['asc']);

    var filas = '';
    ordenados.forEach(function (a) {
        filas += '<tr>' +
            '<td>' + a.codigo + '</td>' +
            '<td>' + a.nombre + '</td>' +
            '<td>' + a.categoria + '</td>' +
            '<td>' + (a.marca || '-') + '</td>' +
            '<td><span class="badge ' + a.estado + '">' + a.estado + '</span></td>' +
            '<td>' + (a.ubicacion || '-') + '</td>' +
            '<td>' + Util.formatearMoneda(a.valor) + '</td>' +
            '<td><button class="peligro" onclick="eliminarActivo(' + a.id + ')">Eliminar</button></td>' +
            '</tr>';
    });

    if (!filas) filas = '<tr><td colspan="8">Sin resultados.</td></tr>';
    document.querySelector('#tablaActivos tbody').innerHTML = filas;
}

function eliminarActivo(id) {
    if (!confirm('¿Eliminar el activo ' + id + '?')) return;
    Api.eliminarActivo(id)
        .then(function () {
            Util.mostrarMensaje('#mensajeActivo', 'Activo eliminado.', 'ok');
            cargarActivos();
        })
        .catch(function () {
            Util.mostrarMensaje('#mensajeActivo', 'No se pudo eliminar.', 'error');
        });
}
