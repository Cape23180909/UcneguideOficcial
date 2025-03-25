Imports Newtonsoft.Json



' Clase UserSession corregida (debe estar en archivo aparte)
Public Class UserSession
    Public Shared Property usuarioId As Integer? = Nothing
    Public Shared Property nombre As String
    Public Shared Property carreraId As Integer?
    Public Shared Property facultadId As Integer?
    Public Shared Property email As String
    Public Property password As String
    Public Shared Property Token As String
    Public Shared Property nombreCarrera As String
    Public Shared Property asignaturaId As Integer
    Public Shared Property rol As String ' Nueva propiedad

    ' Método compartido
    Public Shared Sub LimpiarSesion()
        Token = Nothing
        usuarioId = 0
        nombre = ""
        email = ""
        facultadId = 0
        carreraId = 0
    End Sub
End Class

' Clase Usuario con atributos JsonProperty
Public Class Usuario
    <JsonProperty("usuarioId")>
    Public Property usuarioId As Integer
    <JsonProperty("nombre")>
    Public Property nombre As String
    <JsonProperty("email")>
    Public Property email As String
    <JsonProperty("password")>
    Public Property password As String
    <JsonProperty("facultadId")>
    Public Property facultadId As Integer?
    <JsonProperty("carreraId")>
    Public Property carreraId As Integer?
    Public Property token As String
End Class

