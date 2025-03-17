Public Class Usuario
    Public Property usuarioId As Integer
    Public Property nombre As String
    Public Property carreraId As Integer?
    Public Property facultadId As Integer?
    Public Property email As String
    Public Property password As String
End Class

Public Module UserSession
    Public Property UserId As Integer
    Public Property CarreraId As Integer
    Public Property FacultadId As Integer
    Public Property NombreUsuario As String
    Public Property EmailUsuario As String
End Module
