Imports Newtonsoft.Json
Public Class Asignaturas
    <JsonProperty("AsignaturaId")>
    Public Property asignaturaId As Integer
    <JsonProperty("CodigoAsignatura")>
    Public Property codigoAsignatura As String
    <JsonProperty("NombreAsignatura")>
    Public Property nombreAsignatura As String
    <JsonProperty("DescripcionAsignatura")>
    Public Property descripcionAsignatura As String
    <JsonProperty("DocenteId")>
    Public Property docenteId As Integer
    <JsonProperty("CarreraId")>
    Public Property carreraId As Integer
    Public Property NombreDocenteCompleto As String
    Public Property nombreCarrera As String
    Public Property AsignaturaDetalle As List(Of AsignaturaDetalle)

    Public Sub New()
        AsignaturaDetalle = New List(Of AsignaturaDetalle)()
    End Sub
End Class

Public Class AsignaturaDetalle
    ' Define las propiedades de AsignaturaDetalle según sea necesario
End Class
