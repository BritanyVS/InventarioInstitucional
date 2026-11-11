/* ============================================================
   app.js - Comportamiento común: navegación, sesión, dashboard y login
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {

    // --- Enlace activo en la navegación ---
    var pagina = location.pathname.split('/').pop() || 'index.html';
    document.querySelectorAll('nav.app-nav a').forEach(function (a) {
        if (a.getAttribute('href') === pagina) a.classList.add('activo');
    });

    // --- Selector de backend ---
    var selector = document.getElementById('selectorBackend');
    if (selector) {
        selector.value = CONFIG.backendActivo;
        selector.addEventListener('change', function () { CONFIG.cambiarBackend(this.value); });
    }

    var etiquetaBackend = document.getElementById('etiquetaBackend');
    if (etiquetaBackend) etiquetaBackend.textContent = CONFIG.backendNombre;

    // --- Sesión ---
    var sesion = Util.sesion();
    var contSesion = document.getElementById('usuarioSesion');
    if (contSesion) {
        contSesion.textContent = sesion
            ? sesion.nombreCompleto + ' (' + sesion.rol + ')'
            : 'Sesión no iniciada';
    }

    // --- Login (solo en index.html) ---
    var formLogin = document.getElementById('formLogin');
    if (formLogin) {
        formLogin.addEventListener('submit', function (e) {
            e.preventDefault();
            var dto = {
                usuario: document.getElementById('loginUsuario').value,
                clave: document.getElementById('loginClave').value
            };
            Api.login(dto)
                .then(function (res) {
                    if (res && res.exitoso) {
                        localStorage.setItem('sesion', JSON.stringify(res));
                        Util.mostrarMensaje('#mensajeLogin', 'Bienvenido/a ' + res.nombreCompleto, 'ok');
                        setTimeout(function () { location.reload(); }, 800);
                    } else {
                        Util.mostrarMensaje('#mensajeLogin', (res && res.mensaje) || 'Credenciales inválidas.', 'error');
                    }
                })
                .catch(function () {
                    Util.mostrarMensaje('#mensajeLogin', 'No se pudo conectar con el backend.', 'error');
                });
        });
    }

    // --- Dashboard (solo en index.html) ---
    if (document.getElementById('metricas')) {
        cargarDashboard();
    }
});

function cargarDashboard() {
    Api.reporteInventario()
        .then(function (r) {
            document.getElementById('metTotal').textContent = r.totalActivos;
            document.getElementById('metValor').textContent = Util.formatearMoneda(r.valorTotal);

            var categorias = Object.keys(r.porCategoria || {}).length;
            document.getElementById('metCategorias').textContent = categorias;

            var tbodyCat = document.querySelector('#tablaCategorias tbody');
            if (tbodyCat) {
                var filas = '';
                Object.keys(r.porCategoria || {}).forEach(function (k) {
                    filas += '<tr><td>' + k + '</td><td>' + r.porCategoria[k] + '</td></tr>';
                });
                tbodyCat.innerHTML = filas;
            }

            var tbodyEst = document.querySelector('#tablaEstados tbody');
            if (tbodyEst) {
                var filas2 = '';
                Object.keys(r.porEstado || {}).forEach(function (k) {
                    filas2 += '<tr><td><span class="badge ' + k + '">' + k + '</span></td><td>' + r.porEstado[k] + '</td></tr>';
                });
                tbodyEst.innerHTML = filas2;
            }
        })
        .catch(function () {
            Util.mostrarMensaje('#mensajeDashboard', 'No se pudo cargar el dashboard. Verifique que el backend esté en ejecución.', 'error');
        });
}
