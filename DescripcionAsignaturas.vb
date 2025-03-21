Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Windows.Forms

Public Class DescripcionAsignaturas
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private CodigoAsignatura As String
    Private DocenteId As String

    ' Controles del formulario
    Private LblNombreAsignatura As Label
    Private LblCodigoAsignatura As Label
    Private LblDescripcionAsignatura As Label
    Private LblNombreDocenteCompleto As Label
    Private LstComentarios As ListBox
    Private TxtComentario As TextBox
    Private BtnEnviarComentario As Button

    ' Constructor
    Public Sub New(codigo As String)
        MyBase.New()
        Me.CodigoAsignatura = codigo
        Me.Text = "Descripción de la Asignatura"
        Me.Size = New System.Drawing.Size(600, 500)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog

        ' Inicialización de controles
        LblNombreAsignatura = New Label With {.Text = "Nombre: ", .Location = New Drawing.Point(20, 20), .AutoSize = True}
        LblCodigoAsignatura = New Label With {.Text = "Código: ", .Location = New Drawing.Point(20, 50), .AutoSize = True}
        LblDescripcionAsignatura = New Label With {.Text = "Descripción: ", .Location = New Drawing.Point(20, 80), .AutoSize = True, .Width = 500}
        LblNombreDocenteCompleto = New Label With {.Text = "Docente: ", .Location = New Drawing.Point(20, 110), .AutoSize = True}
        LstComentarios = New ListBox With {.Location = New Drawing.Point(20, 140), .Size = New Drawing.Size(550, 150)}
        TxtComentario = New TextBox With {.Location = New Drawing.Point(20, 300), .Size = New Drawing.Size(400, 30)}
        BtnEnviarComentario = New Button With {.Text = "Enviar Comentario", .Location = New Drawing.Point(430, 300), .Size = New Drawing.Size(140, 30)}

        ' Agregar controles al formulario
        Me.Controls.Add(LblNombreAsignatura)
        Me.Controls.Add(LblCodigoAsignatura)
        Me.Controls.Add(LblDescripcionAsignatura)
        Me.Controls.Add(LblNombreDocenteCompleto)
        Me.Controls.Add(LstComentarios)
        Me.Controls.Add(TxtComentario)
        Me.Controls.Add(BtnEnviarComentario)

        ' Eventos
        AddHandler Me.Load, AddressOf DescripcionAsignaturas_Load
        AddHandler BtnEnviarComentario.Click, AddressOf EnviarComentario
    End Sub

    ' Cargar datos de la asignatura y comentarios
    Private Async Sub DescripcionAsignaturas_Load(sender As Object, e As EventArgs)
        Dim detalleAsignatura = Await ObtenerDatosAPI(Of List(Of Dictionary(Of String, Object)))(ApiUrlAsignaturas)

        If detalleAsignatura Is Nothing OrElse Not detalleAsignatura.Any() Then
            MessageBox.Show("No se pudieron obtener datos de la API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Buscar la asignatura
        Dim asignaturaSeleccionada = detalleAsignatura.FirstOrDefault(Function(a) a.ContainsKey("codigoAsignatura") AndAlso a("codigoAsignatura").ToString() = CodigoAsignatura)

        If asignaturaSeleccionada Is Nothing Then
            MessageBox.Show("Asignatura no encontrada.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Asignar valores a los controles
        LblNombreAsignatura.Text &= If(asignaturaSeleccionada.ContainsKey("nombreAsignatura"), asignaturaSeleccionada("nombreAsignatura").ToString(), "Desconocido")
        LblCodigoAsignatura.Text &= CodigoAsignatura
        LblDescripcionAsignatura.Text &= If(asignaturaSeleccionada.ContainsKey("descripcionAsignatura"), asignaturaSeleccionada("descripcionAsignatura").ToString(), "Sin descripción")

        ' Obtener docenteId y validar
        If asignaturaSeleccionada.ContainsKey("docenteId") Then
            DocenteId = asignaturaSeleccionada("docenteId").ToString()
            LblNombreDocenteCompleto.Text &= Await ObtenerNombreDocente(DocenteId)
        Else
            LblNombreDocenteCompleto.Text &= "No hay docente registrado."
        End If

        ' Cargar comentarios
        Await CargarComentarios()
    End Sub

    ' Obtener nombre del docente
    Private Async Function ObtenerNombreDocente(docenteId As String) As Task(Of String)
        If String.IsNullOrEmpty(docenteId) Then Return "Docente desconocido"

        Dim docenteData = Await ObtenerDatosAPI(Of Dictionary(Of String, String))(ApiUrlAsignaturas & "/Docente?DocenteId=" & docenteId)
        Return If(docenteData IsNot Nothing AndAlso docenteData.ContainsKey("nombre"), docenteData("nombre"), "Docente desconocido")
    End Function

    ' Obtener y mostrar comentarios
    Private Async Function CargarComentarios() As Task
        Dim comentarios = Await ObtenerDatosAPI(Of List(Of Dictionary(Of String, String)))(ApiUrlAsignaturas & "/Comentarios?CodigoAsignatura=" & CodigoAsignatura)
        LstComentarios.Items.Clear()

        If comentarios IsNot Nothing AndAlso comentarios.Any() Then
            For Each comentario In comentarios
                If comentario.ContainsKey("usuario") AndAlso comentario.ContainsKey("contenido") Then
                    LstComentarios.Items.Add($"@{comentario("usuario")}: {comentario("contenido")}")
                End If
            Next
        Else
            LstComentarios.Items.Add("No hay comentarios disponibles.")
        End If
    End Function

    ' Enviar un nuevo comentario
    Private Async Sub EnviarComentario(sender As Object, e As EventArgs)
        Dim nuevoComentario As New Comentarios(0, TxtComentario.Text, If(IsNumeric(DocenteId), CInt(DocenteId), 0), CInt(CodigoAsignatura), 1, DateTime.Now)

        Dim json As String = JsonConvert.SerializeObject(nuevoComentario)
        Using client As New HttpClient()
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage = Await client.PostAsync(ApiUrlAsignaturas & "/Comentarios", content)

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Comentario enviado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                TxtComentario.Clear()
                Await CargarComentarios()
            Else
                MessageBox.Show("Error al enviar comentario: " & Await response.Content.ReadAsStringAsync(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using
    End Sub

    ' Método genérico para obtener datos de la API
    Private Async Function ObtenerDatosAPI(Of T)(url As String) As Task(Of T)
        Using client As New HttpClient()
            Try
                Dim response As HttpResponseMessage = Await client.GetAsync(url)
                If response.IsSuccessStatusCode Then
                    Return JsonConvert.DeserializeObject(Of T)(Await response.Content.ReadAsStringAsync())
                Else
                    Return Nothing
                End If
            Catch
                Return Nothing
            End Try
        End Using
    End Function
End Class


