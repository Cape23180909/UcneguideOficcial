
Imports Newtonsoft.Json



Public Class Comentarios
    <JsonProperty("comentarioId")>
    Public Property ComentarioId As Integer

    <JsonProperty("comentario")> ' ✅ Nombre exacto que usa la API
    Public Property Comentario As String

    <JsonProperty("docenteId")>
    Public Property DocenteId As Integer

    <JsonProperty("asignaturaId")>
    Public Property AsignaturaId As Integer

    <JsonProperty("usuarioId")>
    Public Property UsuarioId As Integer

    <JsonProperty("fechaComentario")>
    Public Property FechaComentario As DateTime

    ' Constructor
    Public Sub New(comentario As String, docenteId As Integer, asignaturaId As Integer, usuarioId As Integer)
        Me.Comentario = comentario
        Me.DocenteId = docenteId
        Me.AsignaturaId = asignaturaId
        Me.UsuarioId = usuarioId
        Me.FechaComentario = DateTime.Now
    End Sub
End Class