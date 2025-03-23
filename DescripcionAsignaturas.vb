Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Net.Http.Headers

Imports System.Text

Public Class DescripcionAsignaturas
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
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

    ' Cargar datos de la asignatura y obtener el docente específico
    Private Async Function CargarDatosAsignatura() As Task
        Dim detalleAsignatura = Await ObtenerDatosAPI(Of List(Of Dictionary(Of String, Object)))(ApiUrlAsignaturas)
        If detalleAsignatura Is Nothing OrElse Not detalleAsignatura.Any() Then Return

        Dim asignatura = detalleAsignatura.FirstOrDefault(Function(a) a.ContainsKey("codigoAsignatura") AndAlso a("codigoAsignatura").ToString() = CodigoAsignatura)
        If asignatura Is Nothing Then Return

        LblNombreAsignatura.Text &= asignatura("nombreAsignatura").ToString()
        LblCodigoAsignatura.Text &= CodigoAsignatura
        LblDescripcionAsignatura.Text &= asignatura("descripcionAsignatura").ToString()

        ' Obtener solo el docente de la asignatura
        If asignatura.ContainsKey("docenteId") Then
            DocenteId = asignatura("docenteId").ToString()
            LblNombreDocenteCompleto.Text &= Await ObtenerNombreDocente(DocenteId)
        End If

        Await CargarComentarios()
    End Function





    ' Obtener nombre del docente que imparte la asignatura
    Private Async Function ObtenerNombreDocente(docenteId As String) As Task(Of String)
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetStringAsync($"{ApiUrlDocentes}/{docenteId}")
                Dim docente As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(response)

                If docente IsNot Nothing AndAlso docente.ContainsKey("nombre") AndAlso docente.ContainsKey("apellido") Then
                    Return $"{docente("nombre")} {docente("apellido")}"
                Else
                    Return "Docente no encontrado"
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error obteniendo nombre del docente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return "Error al obtener el docente"
        End Try
    End Function

    Private Async Function CargarComentarios() As Task
        Try
            ' Obtener comentarios filtrados por docente y asignatura
            Dim url = $"https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios?docenteId={DocenteId}&asignaturaId=1" ' Cambia "1" por el ID real
            Dim comentarios = Await ObtenerDatosAPI(Of List(Of Comentarios))(url)

            LstComentarios.Items.Clear()

            If comentarios IsNot Nothing Then
                For Each c As Comentarios In comentarios
                    LstComentarios.Items.Add($"@{c.UsuarioId}: {c.Comentario} ({c.FechaComentario.ToString("dd/MM/yyyy")})")
                Next
            Else
                LstComentarios.Items.Add("No hay comentarios aún.")
            End If
        Catch ex As Exception
            MessageBox.Show($"Error al cargar comentarios: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Private Async Sub EnviarComentario(sender As Object, e As EventArgs)
        Try
            ' Validar campo vacío
            If String.IsNullOrWhiteSpace(TxtComentario.Text) Then
                MessageBox.Show("Escribe un comentario antes de enviar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Convertir IDs a enteros (asegúrate de que DocenteId y AsignaturaId sean numéricos)
            Dim docenteIdInt As Integer = Integer.Parse(DocenteId)
            Dim asignaturaIdInt As Integer = 1 ' Reemplaza con el ID real de la asignatura

            ' Crear objeto con la estructura correcta
            Dim nuevoComentario As New Comentarios(
            comentario:=TxtComentario.Text.Trim(),
            docenteId:=docenteIdInt,
            asignaturaId:=asignaturaIdInt,
            usuarioId:=1 ' Reemplaza con el ID del usuario autenticado
        )

            ' Serializar a JSON
            Dim json As String = JsonConvert.SerializeObject(nuevoComentario)

            Using client As New HttpClient()
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

                ' Endpoint correcto para POST
                Dim response As HttpResponseMessage = Await client.PostAsync(
                "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios",
                New StringContent(json, Encoding.UTF8, "application/json")
            )

                If response.IsSuccessStatusCode Then
                    TxtComentario.Clear()
                    Await CargarComentarios() ' Recargar la lista
                    MessageBox.Show("¡Comentario enviado!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                    MessageBox.Show($"Error del API: {errorContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error crítico: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    ' Método genérico para obtener datos de la API
    Private Async Function ObtenerDatosAPI(Of T)(url As String) As Task(Of T)
        Using client As New HttpClient()
            Dim response = Await client.GetAsync(url)
            Return If(response.IsSuccessStatusCode, JsonConvert.DeserializeObject(Of T)(Await response.Content.ReadAsStringAsync()), Nothing)
        End Using
    End Function
End Class