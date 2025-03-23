
Imports Newtonsoft.Json

Public Class Comentarios
    <JsonProperty("comentarioId")>
    Public Property ComentarioId As Integer

    <JsonProperty("contenido")>
    Public Property Comentario As String

    <JsonProperty("docenteId")>
    Public Property DocenteId As Integer

    <JsonProperty("asignaturaId")> ' Usar "asignaturaId" en lugar de "codigoAsignatura"
    Public Property AsignaturaId As Integer
    <JsonProperty("carreraId")> ' Añadir si la API usa este campo
    Public Property CarreraId As Integer


    <JsonProperty("usuarioId")>
    Public Property UsuarioId As Integer

    <JsonProperty("fechaComentario")>
    Public Property FechaComentario As DateTime


    ' Constructor para enviar nuevos comentarios
    Public Sub New(comentario As String, docenteId As Integer, asignaturaId As Integer, usuarioId As Integer)
        Me.Comentario = comentario
        Me.DocenteId = docenteId
        Me.AsignaturaId = asignaturaId
        Me.UsuarioId = usuarioId
        Me.FechaComentario = DateTime.Now ' Establecer fecha actual
    End Sub

End Class
