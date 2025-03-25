Imports Newtonsoft.Json

Public Class Docente
    <JsonProperty("docenteId")>
    Public Property docenteId As Integer

    <JsonProperty("nombre")>
    Public Property nombre As String
    <JsonProperty("apellido")>
    Public Property apellido As String
    <JsonProperty("rol")>
    Public Property rol As String
    <JsonProperty("asignaturaId")>
    Public Property asignaturaId As Integer
    <JsonProperty("carreraId")>
    Public Property carreraId As Integer

End Class



