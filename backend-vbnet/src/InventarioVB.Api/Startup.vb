Imports System.Web.Http
Imports System.Web.Http.Cors
Imports Owin

''' <summary>Configuración de OWIN para hospedar ASP.NET Web API 2.</summary>
Public Class Startup

    Public Sub Configuration(app As IAppBuilder)
        Dim config = New HttpConfiguration()

        ' Rutas por atributos (coinciden con los controladores).
        config.MapHttpAttributeRoutes()

        ' Ruta por convención de respaldo.
        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="api/{controller}/{id}",
            defaults:=New With {.id = RouteParameter.Optional})

        ' CORS abierto para el frontend de demostración.
        config.EnableCors(New EnableCorsAttribute("*", "*", "*"))

        ' Respuestas JSON legibles.
        config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented

        ' Igualar el contrato JSON del backend C#: propiedades en camelCase.
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
            New Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()

        ' Serializar los enums como texto ("Disponible", "Entrada") en lugar de número.
        config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(
            New Newtonsoft.Json.Converters.StringEnumConverter())

        ' Detalle de errores (útil durante la demostración).
        config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always

        ' Registro de excepciones no controladas (ayuda al diagnóstico).
        app.Use(Async Function(context, [next])
                    Try
                        Await [next]()
                    Catch ex As Exception
                        Console.WriteLine("EXCEPCION: " & ex.ToString())
                        Throw
                    End Try
                End Function)

        app.UseWebApi(config)
    End Sub

End Class
