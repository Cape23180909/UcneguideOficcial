Public Class Comentarios
    Public Property ComentarioId As Integer
    Public Property Comentario As String
    Public Property DocenteId As Integer
    Public Property AsignaturaId As Integer
    Public Property UsuarioId As Integer
    Public Property FechaComentario As DateTime

    ' Constructor opcional
    Public Sub New(comentarioId As Integer, comentario As String, docenteId As Integer, asignaturaId As Integer, usuarioId As Integer, fechaComentario As DateTime)
        Me.ComentarioId = comentarioId
        Me.Comentario = comentario
        Me.DocenteId = docenteId
        Me.AsignaturaId = asignaturaId
        Me.UsuarioId = usuarioId
        Me.FechaComentario = fechaComentario
    End Sub

    ' Método para mostrar la información del comentario
    Public Overrides Function ToString() As String
        Return $"ID: {ComentarioId}, Comentario: {Comentario}, Docente ID: {DocenteId}, Asignatura ID: {AsignaturaId}, Usuario ID: {UsuarioId}, Fecha: {FechaComentario}"
    End Function

End Class
