Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text

Public Class GenerarComentarios
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private ReadOnly HttpClient As New HttpClient()

    ' Controles del formulario
    Private CbAsignaturas As ComboBox
    Private CbDocentes As ComboBox
    Private TxtComentario As TextBox
    Private BtnRegistrar As Button
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

    Public Class ComentarioRequest
        Public Property Comentario As String
        Public Property DocenteId As Integer
        Public Property AsignaturaId As Integer
        Public Property UsuarioId As Integer
        Public Property FechaComentario As DateTime
    End Class

    Private Sub GenerarComentarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ConfigurarInterfaz()
        CargarAsignaturasYDocentes()
    End Sub

    Private Sub ConfigurarInterfaz()
        Me.Text = "Generar Comentarios"
        Me.Size = New Size(600, 500)
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Segoe UI", 10)

        ' Cambiar el FormBorderStyle y habilitar los botones de control
        Me.FormBorderStyle = FormBorderStyle.Sizable
        Me.ControlBox = True
        Me.MaximizeBox = True
        Me.MinimizeBox = True

        CrearPanelSuperior()
        CrearControlesFormulario()
    End Sub

    Private Sub CrearPanelSuperior()
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(80, 80),
            .Location = New Point(20, 10),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        topPanel.Controls.Add(bottomBorder)

        Me.Controls.Add(topPanel)
    End Sub

    Private Sub CrearControlesFormulario()
        Dim mainPanel As New Panel With {
        .Dock = DockStyle.Fill,
        .BackColor = Color.White,
        .Padding = New Padding(20)
    }

        Dim contentPanel As New Panel With {
        .Size = New Size(500, 400),
        .Dock = DockStyle.None,
        .Anchor = AnchorStyles.None,
        .Left = (Me.ClientSize.Width - 500) \ 2,
        .Top = (Me.ClientSize.Height - 400) \ 2
    }

        ' Configurar estilos base
        Dim labelStyle As New Label With {
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .ForeColor = ColorTranslator.FromHtml("#074788"),
        .AutoSize = True
    }

        ' Configurar controles con alineación central
        CbAsignaturas = New ComboBox With {
        .Font = New Font("Segoe UI", 10),
        .DropDownStyle = ComboBoxStyle.DropDownList,
        .FlatStyle = FlatStyle.Flat,
        .Width = 350,
        .Height = 35,
        .Margin = New Padding(0, 0, 0, 20)
    }

        CbDocentes = New ComboBox With {
        .Font = New Font("Segoe UI", 10),
        .DropDownStyle = ComboBoxStyle.DropDownList,
        .FlatStyle = FlatStyle.Flat,
        .Width = 350,
        .Height = 35,
        .Margin = New Padding(0, 0, 0, 20)
    }

        TxtComentario = New TextBox With {
        .Multiline = True,
        .ScrollBars = ScrollBars.Vertical,
        .Font = New Font("Segoe UI", 10),
        .BorderStyle = BorderStyle.FixedSingle,
        .Size = New Size(350, 120),
        .Margin = New Padding(0, 0, 0, 20)
    }

        ' Configurar botón centrado
        BtnRegistrar = New Button With {
        .Text = "REGISTRAR COMENTARIO",
        .Size = New Size(200, 40),
        .BackColor = ColorTranslator.FromHtml("#28A745"),
        .ForeColor = Color.White,
        .FlatStyle = FlatStyle.Flat,
        .Font = New Font("Segoe UI", 10, FontStyle.Bold),
        .Cursor = Cursors.Hand
    }

        ' Organizar controles verticalmente centrados
        Dim yPosition As Integer = 0

        ' Asignatura
        Dim lblAsignatura = CreateLabel("Asignatura:", labelStyle, yPosition)
        lblAsignatura.Left = (contentPanel.Width - lblAsignatura.Width) \ 2
        contentPanel.Controls.Add(lblAsignatura)
        yPosition += 25

        CbAsignaturas.Location = New Point((contentPanel.Width - CbAsignaturas.Width) \ 2, yPosition)
        contentPanel.Controls.Add(CbAsignaturas)
        yPosition += 55

        ' Docente
        Dim lblDocente = CreateLabel("Docente:", labelStyle, yPosition)
        lblDocente.Left = (contentPanel.Width - lblDocente.Width) \ 2
        contentPanel.Controls.Add(lblDocente)
        yPosition += 25

        CbDocentes.Location = New Point((contentPanel.Width - CbDocentes.Width) \ 2, yPosition)
        contentPanel.Controls.Add(CbDocentes)
        yPosition += 55

        ' Comentario
        Dim lblComentario = CreateLabel("Comentario:", labelStyle, yPosition)
        lblComentario.Left = (contentPanel.Width - lblComentario.Width) \ 2
        contentPanel.Controls.Add(lblComentario)
        yPosition += 25

        TxtComentario.Location = New Point((contentPanel.Width - TxtComentario.Width) \ 2, yPosition)
        contentPanel.Controls.Add(TxtComentario)
        yPosition += 140

        ' Botón centrado
        BtnRegistrar.Location = New Point((contentPanel.Width - BtnRegistrar.Width) \ 2, yPosition)
        contentPanel.Controls.Add(BtnRegistrar)

        ' Eventos
        AddHandler BtnRegistrar.Click, AddressOf RegistrarComentario

        ' Manejar redimensionamiento
        AddHandler contentPanel.Layout, Sub(sender, e)
                                            contentPanel.Left = (mainPanel.Width - contentPanel.Width) \ 2
                                            contentPanel.Top = (mainPanel.Height - contentPanel.Height) \ 2
                                        End Sub

        mainPanel.Controls.Add(contentPanel)
        Me.Controls.Add(mainPanel)
    End Sub

    Private Function CreateLabel(text As String, style As Label, y As Integer) As Label
        Return New Label With {
            .Text = text,
            .Font = style.Font,
            .ForeColor = style.ForeColor,
            .Location = New Point(0, y),
            .AutoSize = True
        }
    End Function

    Private Async Sub CargarAsignaturasYDocentes()
        Try
            Dim responseAsignaturas = Await HttpClient.GetAsync(ApiUrlAsignaturas)
            If responseAsignaturas.IsSuccessStatusCode Then
                Dim jsonAsignaturas = Await responseAsignaturas.Content.ReadAsStringAsync()
                Dim asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(jsonAsignaturas)
                CbAsignaturas.DataSource = asignaturas
                CbAsignaturas.DisplayMember = "NombreAsignatura"
                CbAsignaturas.ValueMember = "AsignaturaId"
            End If

            Dim responseDocentes = Await HttpClient.GetAsync(ApiUrlDocentes)
            If responseDocentes.IsSuccessStatusCode Then
                Dim jsonDocentes = Await responseDocentes.Content.ReadAsStringAsync()
                Dim docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(jsonDocentes)
                CbDocentes.DataSource = docentes
                CbDocentes.DisplayMember = "NombreCompleto"
                CbDocentes.ValueMember = "DocenteId"
            End If

        Catch ex As Exception
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Sub RegistrarComentario(sender As Object, e As EventArgs)
        If Not ValidarCampos() Then Return

        Try
            ' Usar la clase modelo en lugar de objeto anónimo
            Dim nuevoComentario As New ComentarioRequest With {
                .Comentario = TxtComentario.Text.Trim(),
                .DocenteId = CInt(CbDocentes.SelectedValue),
                .AsignaturaId = CInt(CbAsignaturas.SelectedValue),
                .UsuarioId = 1, ' Reemplazar con ID real
                .FechaComentario = DateTime.Now
            }

            Dim json = JsonConvert.SerializeObject(nuevoComentario)
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")

            ' Agregar headers si son necesarios
            HttpClient.DefaultRequestHeaders.Accept.Clear()
            HttpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

            Dim response = Await HttpClient.PostAsync(ApiUrlComentarios, content)

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Comentario registrado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LimpiarCampos()
            Else
                Dim errorContent = Await response.Content.ReadAsStringAsync()
                ' Mejorar manejo de errores
                Dim errorMessage = $"Error {CInt(response.StatusCode)}: {response.ReasonPhrase}"
                If Not String.IsNullOrEmpty(errorContent) Then
                    errorMessage &= $"{vbCrLf}Detalles: {errorContent}"
                End If
                MessageBox.Show(errorMessage, "Error del servidor", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error de conexión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(TxtComentario.Text) Then
            MessageBox.Show("El comentario no puede estar vacío", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If CbAsignaturas.SelectedIndex = -1 OrElse CbDocentes.SelectedIndex = -1 Then
            MessageBox.Show("Debe seleccionar una asignatura y un docente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Sub LimpiarCampos()
        TxtComentario.Clear()
        CbAsignaturas.SelectedIndex = -1
        CbDocentes.SelectedIndex = -1
        TxtComentario.Focus()
    End Sub

End Class