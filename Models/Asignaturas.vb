Imports Newtonsoft.Json
Public Class Asignaturas
    <JsonProperty("AsignaturaId")>
    Public Property AsignaturaId As Integer
    <JsonProperty("CodigoAsignatura")>
    Public Property CodigoAsignatura As String
    <JsonProperty("NombreAsignatura")>
    Public Property NombreAsignatura As String
    <JsonProperty("DescripcionAsignatura")>
    Public Property DescripcionAsignatura As String
    <JsonProperty("DocenteId")>
    Public Property DocenteId As Integer
    <JsonProperty("CarreraId")>
    Public Property CarreraId As Integer
    Public Property NombreDocenteCompleto As String
    Public Property NombreCarrera As String
    Public Property AsignaturaDetalle As List(Of AsignaturaDetalle)

    Public Sub New()
        AsignaturaDetalle = New List(Of AsignaturaDetalle)()
    End Sub
End Class

Public Class AsignaturaDetalle
    ' Define las propiedades de AsignaturaDetalle según sea necesario
End Class


