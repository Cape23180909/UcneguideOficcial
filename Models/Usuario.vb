Public Class Usuario
    Friend token As String
    Public Property usuarioId As Integer
    Public Property nombre As String
    Public Property carreraId As Integer?
    Public Property facultadId As Integer?
    Public Property email As String
    Public Property password As String
End Class

Public Class UserSession
    Friend Shared nombreCarrera As String
    Public Shared Property UserId As Integer = 1 ' Simulación de sesión, reemplázalo con el ID real del usuario autenticado
    Public Shared Property Nombre As String
    Public Shared Property Email As String
    Public Shared Property FacultadId As Integer?
    Public Shared Property CarreraId As Integer?
    Public Shared Property Token As String
End Class


