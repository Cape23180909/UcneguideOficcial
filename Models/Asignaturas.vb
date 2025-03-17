Public Class Asignaturas
    Public Property asignaturaId As Integer
    Public Property CodigoAsignatura As String
    Public Property NombreAsignatura As String
    Public Property DocenteId As Integer
    Public Property CarreraId As Integer
    Public Property AsignaturaDetalle As List(Of AsignaturaDetalle)

    Public Sub New()
        AsignaturaDetalle = New List(Of AsignaturaDetalle)()
    End Sub
End Class

Public Class AsignaturaDetalle
    ' Define las propiedades de AsignaturaDetalle según sea necesario
End Class
