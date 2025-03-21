Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks

Public Class DescripcionAsignaturas
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private CodigoAsignatura As String
    Private DocenteId As String

    ' Controles del formulario
    Private topPanel As Panel
    Private mainPanel As Panel
    Private iconoPictureBox As PictureBox
    Private LblNombreAsignatura, LblCodigoAsignatura, LblDescripcionAsignatura, LblNombreDocenteCompleto As Label
    Private LstComentarios As ListBox
    Private TxtComentario As TextBox
    Private BtnEnviarComentario As Button

    ' Constructor
    Public Sub New(codigo As String)
        Me.CodigoAsignatura = codigo
        InitializeForm()
    End Sub

    ' Método para inicializar componentes visuales
    Private Sub InitializeForm()
        Me.Text = "Descripción de Asignatura"
        Me.WindowState = FormWindowState.Maximized
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.None

        ' Panel superior azul
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Línea amarilla debajo del panel azul
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(90, 90),
            .Location = New Point(25, 5),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        ' Panel principal para centrar contenido
        mainPanel = New Panel With {
            .Size = New Size(600, 500),
            .BackColor = Color.White,
            .Anchor = AnchorStyles.None
        }
        mainPanel.Location = New Point((Me.ClientSize.Width - mainPanel.Width) \ 2, (Me.ClientSize.Height - mainPanel.Height) \ 2)
        AddHandler Me.Resize, AddressOf AjustarPanelCentrado

        ' Labels
        LblNombreAsignatura = CrearLabel("Nombre: ", 20, 20, 500)
        LblCodigoAsignatura = CrearLabel("Código: ", 20, 50, 500)
        LblDescripcionAsignatura = CrearLabel("Descripción: ", 20, 80, 500)
        LblNombreDocenteCompleto = CrearLabel("Docente: ", 20, 110, 500)

        ' ListBox para comentarios
        LstComentarios = New ListBox With {
            .Location = New Point(20, 150),
            .Size = New Size(550, 150),
            .Font = New Font("Arial", 9)
        }

        ' TextBox y botón para comentarios
        TxtComentario = New TextBox With {
            .Location = New Point(20, 320),
            .Size = New Size(400, 30),
            .Font = New Font("Arial", 9)
        }

        BtnEnviarComentario = New Button With {
            .Text = "Enviar Comentario",
            .Location = New Point(430, 320),
            .Size = New Size(140, 30),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler BtnEnviarComentario.Click, AddressOf EnviarComentario

        ' Agregar controles
        mainPanel.Controls.AddRange({LblNombreAsignatura, LblCodigoAsignatura, LblDescripcionAsignatura, LblNombreDocenteCompleto, LstComentarios, TxtComentario, BtnEnviarComentario})
        Me.Controls.AddRange({topPanel, bottomBorder, mainPanel})
    End Sub

    ' Ajustar el panel cuando se redimensiona la ventana
    Private Sub AjustarPanelCentrado(sender As Object, e As EventArgs)
        mainPanel.Location = New Point((Me.ClientSize.Width - mainPanel.Width) \ 2, (Me.ClientSize.Height - mainPanel.Height) \ 2)
    End Sub

    ' Método auxiliar para crear labels
    Private Function CrearLabel(texto As String, x As Integer, y As Integer, Optional width As Integer = 200) As Label
        Return New Label With {
            .Text = texto,
            .Location = New Point(x, y),
            .AutoSize = True,
            .Width = width,
            .Font = New Font("Arial", 10, FontStyle.Bold)
        }
    End Function

    ' Evento de carga
    Private Async Sub DescripcionAsignaturas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await CargarDatosAsignatura()
    End Sub

    ' Cargar datos de la asignatura y comentarios
    Private Async Function CargarDatosAsignatura() As Task
        Dim detalleAsignatura = Await ObtenerDatosAPI(Of List(Of Dictionary(Of String, Object)))(ApiUrlAsignaturas)
        If detalleAsignatura Is Nothing OrElse Not detalleAsignatura.Any() Then Return

        Dim asignatura = detalleAsignatura.FirstOrDefault(Function(a) a.ContainsKey("codigoAsignatura") AndAlso a("codigoAsignatura").ToString() = CodigoAsignatura)
        If asignatura Is Nothing Then Return

        LblNombreAsignatura.Text &= asignatura("nombreAsignatura").ToString()
        LblCodigoAsignatura.Text &= CodigoAsignatura
        LblDescripcionAsignatura.Text &= asignatura("descripcionAsignatura").ToString()

        If asignatura.ContainsKey("docenteId") Then
            DocenteId = asignatura("docenteId").ToString()
            LblNombreDocenteCompleto.Text &= Await ObtenerNombreDocente(DocenteId)
        End If

        Await CargarComentarios()
    End Function

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
                LstComentarios.Items.Add($"@{comentario("usuario")}: {comentario("contenido")}")
            Next
        Else
            LstComentarios.Items.Add("No hay comentarios disponibles.")
        End If
    End Function

    ' Enviar un nuevo comentario
    Private Async Sub EnviarComentario(sender As Object, e As EventArgs)
        Dim nuevoComentario As New With {
        .contenido = TxtComentario.Text,
        .docenteId = DocenteId,
        .codigoAsignatura = CodigoAsignatura
    }

        Dim json As String = JsonConvert.SerializeObject(nuevoComentario)
        Using client As New HttpClient()
            Dim content As New StringContent(json, Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage = Await client.PostAsync(ApiUrlAsignaturas & "/Comentarios", content)
            If response.IsSuccessStatusCode Then
                TxtComentario.Clear()
                Await CargarComentarios()
            Else
                MessageBox.Show("Error al enviar comentario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End Using


    End Sub

    ' Método genérico para obtener datos de la API
    Private Async Function ObtenerDatosAPI(Of T)(url As String) As Task(Of T)
        Using client As New HttpClient()
            Dim response = Await client.GetAsync(url)
            Return If(response.IsSuccessStatusCode, JsonConvert.DeserializeObject(Of T)(Await response.Content.ReadAsStringAsync()), Nothing)
        End Using
    End Function
End Class
