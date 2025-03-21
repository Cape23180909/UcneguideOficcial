Public Class Sugerencias
    Public Property SugerenciaId As Integer
    Public Property Sugerencia As String
    Public Property UsuarioId As Integer
    Public Property AsignaturaId As Integer
    Public Property CarreraId As Integer
    Public Property FechaSugerencia As DateTime

    ' Constructor opcional
    Public Sub New(sugerenciaId As Integer, sugerencia As String, usuarioId As Integer, asignaturaId As Integer, carreraId As Integer, fechaSugerencia As DateTime)
        Me.SugerenciaId = sugerenciaId
        Me.Sugerencia = sugerencia
        Me.UsuarioId = usuarioId
        Me.AsignaturaId = asignaturaId
        Me.CarreraId = carreraId
        Me.FechaSugerencia = fechaSugerencia
    End Sub

    ' Método para mostrar la información de la sugerencia
    Public Overrides Function ToString() As String
        Return $"ID: {SugerenciaId}, Sugerencia: {Sugerencia}, Usuario ID: {UsuarioId}, Asignatura ID: {AsignaturaId}, Carrera ID: {CarreraId}, Fecha: {FechaSugerencia}"
    End Function
End Class
