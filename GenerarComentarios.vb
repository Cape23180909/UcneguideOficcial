﻿Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Linq

Public Class GenerarComentarios
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private ReadOnly HttpClient As New HttpClient()

    ' Controles del formulario
    Private WithEvents CbAsignaturas As ComboBox
    Private CbDocentes As ComboBox
    Private TxtComentario As TextBox
    Private BtnRegistrar As Button
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

    ' Variables para almacenar datos
    Private asignaturas As List(Of Asignaturas)
    Private docentes As List(Of Docente)

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
        Me.Size = New Size(800, 600)
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Segoe UI", 10)
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
            .Image = Image.FromFile("C:\Ucneguide\Resources\logo.jpg"),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(80, 80),
            .Location = New Point(20, 10),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        ' Título centrado
        Dim lblTitle As New Label With {
            .Text = "CREAR COMENTARIO",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = True
        }

        AddHandler topPanel.Resize, Sub()
                                        lblTitle.Location = New Point(
                                        (topPanel.Width - lblTitle.Width) \ 2,
                                        (topPanel.Height - lblTitle.Height) \ 2)
                                    End Sub

        topPanel.Controls.Add(lblTitle)
        lblTitle.BringToFront()

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
            .Padding = New Padding(40)
        }

        Dim contentPanel As New Panel With {
            .Size = New Size(700, 500),
            .Dock = DockStyle.None,
            .Anchor = AnchorStyles.None,
            .Left = (Me.ClientSize.Width - 700) \ 2,
            .Top = (Me.ClientSize.Height - 500) \ 2
        }

        ' Configurar estilos base
        Dim labelStyle As New Label With {
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#074788"),
            .AutoSize = True
        }

        ' Configurar controles
        CbAsignaturas = New ComboBox With {
            .Font = New Font("Segoe UI", 11),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .Width = 500,
            .Height = 40,
            .Margin = New Padding(0, 0, 0, 25)
        }

        CbDocentes = New ComboBox With {
            .Font = New Font("Segoe UI", 11),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .Width = 500,
            .Height = 40,
            .Margin = New Padding(0, 0, 0, 25)
        }

        TxtComentario = New TextBox With {
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical,
            .Font = New Font("Segoe UI", 11),
            .BorderStyle = BorderStyle.FixedSingle,
            .Size = New Size(500, 150),
            .Margin = New Padding(0, 0, 0, 25)
        }

        BtnRegistrar = New Button With {
            .Text = "REGISTRAR COMENTARIO",
            .Size = New Size(300, 45),
            .BackColor = ColorTranslator.FromHtml("#28A745"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        AddHandler BtnRegistrar.Click, AddressOf RegistrarComentario

        ' Organizar controles
        Dim yPosition As Integer = 20

        ' Asignatura
        Dim lblAsignatura = CreateLabel("Asignatura:", labelStyle, yPosition)
        lblAsignatura.Left = (contentPanel.Width - lblAsignatura.Width) \ 2
        contentPanel.Controls.Add(lblAsignatura)
        yPosition += 35

        CbAsignaturas.Location = New Point((contentPanel.Width - CbAsignaturas.Width) \ 2, yPosition)
        contentPanel.Controls.Add(CbAsignaturas)
        yPosition += 70

        ' Docente
        Dim lblDocente = CreateLabel("Docente:", labelStyle, yPosition)
        lblDocente.Left = (contentPanel.Width - lblDocente.Width) \ 2
        contentPanel.Controls.Add(lblDocente)
        yPosition += 35

        CbDocentes.Location = New Point((contentPanel.Width - CbDocentes.Width) \ 2, yPosition)
        contentPanel.Controls.Add(CbDocentes)
        yPosition += 70

        ' Comentario
        Dim lblComentario = CreateLabel("Comentario:", labelStyle, yPosition)
        lblComentario.Left = (contentPanel.Width - lblComentario.Width) \ 2
        contentPanel.Controls.Add(lblComentario)
        yPosition += 35

        TxtComentario.Location = New Point((contentPanel.Width - TxtComentario.Width) \ 2, yPosition)
        contentPanel.Controls.Add(TxtComentario)
        yPosition += 180

        ' Botón
        BtnRegistrar.Location = New Point((contentPanel.Width - BtnRegistrar.Width) \ 2, yPosition)
        contentPanel.Controls.Add(BtnRegistrar)

        ' Manejar redimensionamiento
        AddHandler Me.Resize, Sub(sender, e)
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
            Dim carreraIdUsuario As Integer = UserSession.carreraId

            ' Cargar asignaturas de la carrera del usuario
            Dim responseAsignaturas = Await HttpClient.GetAsync($"{ApiUrlAsignaturas}?carreraId={carreraIdUsuario}")
            If responseAsignaturas.IsSuccessStatusCode Then
                Dim jsonAsignaturas = Await responseAsignaturas.Content.ReadAsStringAsync()
                asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(jsonAsignaturas)

                ' Configurar ComboBox de asignaturas
                CbAsignaturas.DataSource = asignaturas
                CbAsignaturas.DisplayMember = "NombreAsignatura"
                CbAsignaturas.ValueMember = "AsignaturaId"
            End If

            ' Cargar todos los docentes de la carrera
            Dim responseDocentes = Await HttpClient.GetAsync($"{ApiUrlDocentes}?carreraId={carreraIdUsuario}")
            If responseDocentes.IsSuccessStatusCode Then
                Dim jsonDocentes = Await responseDocentes.Content.ReadAsStringAsync()
                docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(jsonDocentes)
            End If

            ' Configurar evento para cuando cambie la selección de asignatura
            AddHandler CbAsignaturas.SelectedIndexChanged, AddressOf CbAsignaturas_SelectedIndexChanged

            ' Filtrar docentes inicialmente según la primera asignatura seleccionada
            If asignaturas IsNot Nothing AndAlso asignaturas.Any() Then
                FiltrarDocentesPorAsignatura(asignaturas.First().AsignaturaId)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CbAsignaturas_SelectedIndexChanged(sender As Object, e As EventArgs)
        If CbAsignaturas.SelectedItem IsNot Nothing Then
            Dim asignaturaSeleccionada = DirectCast(CbAsignaturas.SelectedItem, Asignaturas)
            FiltrarDocentesPorAsignatura(asignaturaSeleccionada.AsignaturaId)
        End If
    End Sub

    Private Sub FiltrarDocentesPorAsignatura(asignaturaId As Integer)
        If asignaturas Is Nothing OrElse docentes Is Nothing Then Return

        ' Obtener la asignatura seleccionada
        Dim asignaturaSeleccionada = asignaturas.FirstOrDefault(Function(a) a.AsignaturaId = asignaturaId)
        If asignaturaSeleccionada Is Nothing Then Return

        ' Obtener el docenteId de la asignatura
        Dim docenteIdDeLaAsignatura = asignaturaSeleccionada.DocenteId

        ' Filtrar docentes para mostrar solo el que imparte esta materia
        Dim docenteFiltrado = docentes.Where(Function(d) d.docenteId = docenteIdDeLaAsignatura).ToList()

        ' Configurar ComboBox de docentes
        CbDocentes.DataSource = docenteFiltrado
        CbDocentes.DisplayMember = "NombreCompleto"
        CbDocentes.ValueMember = "docenteId"

        ' Seleccionar automáticamente el docente si hay uno
        If docenteFiltrado.Any() Then
            CbDocentes.SelectedValue = docenteIdDeLaAsignatura
        End If
    End Sub

    Private Async Sub RegistrarComentario(sender As Object, e As EventArgs)
        If Not ValidarCampos() Then Return

        Try
            Dim nuevoComentario As New ComentarioRequest With {
                .Comentario = TxtComentario.Text.Trim(),
                .DocenteId = CInt(CbDocentes.SelectedValue),
                .AsignaturaId = CInt(CbAsignaturas.SelectedValue),
                .UsuarioId = UserSession.usuarioId, ' Usar el ID del usuario logueado
                .FechaComentario = DateTime.Now
            }

            Dim json = JsonConvert.SerializeObject(nuevoComentario)
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")

            HttpClient.DefaultRequestHeaders.Accept.Clear()
            HttpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))

            Dim response = Await HttpClient.PostAsync(ApiUrlComentarios, content)

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Comentario registrado exitosamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LimpiarCampos()
            Else
                Dim errorContent = Await response.Content.ReadAsStringAsync()
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
        CbAsignaturas.SelectedIndex = 0
        CbDocentes.SelectedIndex = 0
        TxtComentario.Focus()
    End Sub
End Class