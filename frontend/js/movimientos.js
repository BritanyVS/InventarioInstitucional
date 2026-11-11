/* ============================================================
   movimientos.js - Entradas, salidas y asignaciones
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('tablaMovimientos')) {
        cargarMovimientos();
        cargarActivosParaSelect();

        document.getElementById('formMovimiento').addEventListener('submit', function (e) {
            e.preventDefault();
            var dto = {
                activoId: parseInt(document.getElementById('movActivo').value, 10),
                tipo: document.getElementById('movTipo').value,
                cantidad: parseInt(document.getElementById('movCantidad').value, 10) || 1,
                usuario: document.getElementById('movUsuario').value,
                observacion: document.getElementById('movObservacion').value
            };
            Api.crearMovimiento(dto)
                .then(function () {
                    Util.mostrarMensaje('#mensajeMovimiento', 'Movimiento registrado.', 'ok');
                    document.getElementById('formMovimiento').reset();
                    cargarMovimientos();
                })
                .catch(function () {
                    Util.mostrarMensaje('#mensajeMovimiento', 'Error al registrar el movimiento.', 'error');
                });
        });
    }

    if (document.getElementById('tablaAsignaciones')) {
        cargarAsignaciones();
        cargarActivosParaSelect('asigActivo');

        document.getElementById('formAsignacion').addEventListener('submit', function (e) {
            e.preventDefault();
            var dto = {
                activoId: parseInt(document.getElementById('asigActivo').value, 10),
                usuarioAsignado: document.getElementById('asigUsuario').value,
                area: document.getElementById('asigArea').value
            };
            Api.crearAsignacion(dto)
                .then(function () {
                    Util.mostrarMensaje('#mensajeAsignacion', 'Asignación registrada.', 'ok');
                    document.getElementById('formAsignacion').reset();
                    cargarAsignaciones();
                })
                .catch(function () {
                    Util.mostrarMensaje('#mensajeAsignacion', 'Error al registrar la asignación.', 'error');
                });
        });
    }
});

function cargarActivosParaSelect(idSelect) {
    idSelect = idSelect || 'movActivo';
    Api.listarActivos().then(function (datos) {
        var select = document.getElementById(idSelect);
        if (!select) return;
        var opciones = '<option value="">Seleccione un activo...</option>';
        (datos || []).forEach(function (a) {
            opciones += '<option value="' + a.id + '">' + a.codigo + ' - ' + a.nombre + '</option>';
        });
        select.innerHTML = opciones;
    });
}

function cargarMovimientos() {
    Api.listarMovimientos()
        .then(function (datos) {
            var filas = '';
            (datos || []).forEach(function (m) {
                filas += '<tr>' +
                    '<td>' + m.id + '</td>' +
                    '<td>' + (m.activoNombre || m.activoId) + '</td>' +
                    '<td><span class="badge ' + m.tipo + '">' + m.tipo + '</span></td>' +
                    '<td>' + m.cantidad + '</td>' +
                    '<td>' + (m.usuario || '-') + '</td>' +
                    '<td>' + (m.observacion || '-') + '</td>' +
                    '<td>' + Util.formatearFecha(m.fecha) + '</td>' +
                    '</tr>';
            });
            if (!filas) filas = '<tr><td colspan="7">Sin movimientos registrados.</td></tr>';
            document.querySelector('#tablaMovimientos tbody').innerHTML = filas;
        })
        .catch(function () {
            Util.mostrarMensaje('#mensajeMovimiento', 'No se pudieron cargar los movimientos.', 'error');
        });
}

function cargarAsignaciones() {
    Api.listarAsignaciones()
        .then(function (datos) {
            var filas = '';
            (datos || []).forEach(function (a) {
                filas += '<tr>' +
                    '<td>' + a.id + '</td>' +
                    '<td>' + (a.activoNombre || a.activoId) + '</td>' +
                    '<td>' + a.usuarioAsignado + '</td>' +
                    '<td>' + (a.area || '-') + '</td>' +
                    '<td>' + Util.formatearFecha(a.fechaAsignacion) + '</td>' +
                    '</tr>';
            });
            if (!filas) filas = '<tr><td colspan="5">Sin asignaciones registradas.</td></tr>';
            document.querySelector('#tablaAsignaciones tbody').innerHTML = filas;
        })
        .catch(function () {
            Util.mostrarMensaje('#mensajeAsignacion', 'No se pudieron cargar las asignaciones.', 'error');
        });
}
