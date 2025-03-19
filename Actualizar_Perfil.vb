Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Net.Http.Headers

Public Class ActualizarPerfil
    Inherits Form

    Private ReadOnly _apiUrl As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Usuarios"
    Private ReadOnly _apiUrlFacultades As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Facultades"
    Private ReadOnly _apiUrlCarreras As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Carreras"

    Private WithEvents TxtNombre As New TextBox()
    Private WithEvents TxtEmail As New TextBox()
    Private WithEvents TxtPassword As New TextBox()
    Private WithEvents TxtConfirmPassword As New TextBox()
    Private WithEvents CmbFacultad As New ComboBox()
    Private WithEvents CmbCarrera As New ComboBox()
    Private WithEvents BtnGuardar As New Button()
    Private WithEvents BtnVolver As New Button()
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    Public Sub New()
        InitializeComponent()
        InitializeCustomComponents()
        CargarFacultades()
        CargarDatosUsuario()
    End Sub

    Private Sub InitializeCustomComponents()
        Me.Text = "Actualizar Perfil"
        Me.Size = New Size(800, 600) ' Aumentamos ligeramente la altura
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Panel superior con diseño unificado
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Línea decorativa amarilla
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Icono interactivo
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(70, 70),
            .Location = New Point(20, 15),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        ' Título del formulario
        Dim lblTitulo As New Label With {
            .Text = "Actualizar Perfil",
            .Font = New Font("Arial", 16, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = True,
            .Location = New Point(100, 35)
        }
        topPanel.Controls.Add(lblTitulo)

        ' Contenedor principal para controles
        Dim mainContainer As New Panel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True
        }

        ' Configuración de controles con posiciones relativas
        Dim yPosition As Integer = 20
        Dim controlHeight As Integer = 30
        Dim margin As Integer = 40

        Dim AddControl = Sub(ctrl As Control, labelText As String)
                             ctrl.Location = New Point(40, yPosition)
                             ctrl.Size = New Size(400, controlHeight)
                             mainContainer.Controls.Add(New Label With {
                                 .Text = labelText,
                                 .Location = New Point(40, yPosition - 20),
                                 .Font = New Font("Arial", 9)
                             })
                             mainContainer.Controls.Add(ctrl)
                             yPosition += margin + controlHeight
                         End Sub

        AddControl(TxtNombre, "Nombre:")
        AddControl(TxtEmail, "Email:")
        AddControl(TxtPassword, "Contraseña:")
        AddControl(TxtConfirmPassword, "Confirmar Contraseña:")
        AddControl(CmbFacultad, "Facultad:")
        AddControl(CmbCarrera, "Carrera:")

        ' Configuración de botones
        BtnGuardar.Text = "Actualizar"
        BtnVolver.Text = "Volver"
        BtnGuardar.Size = New Size(120, 40)
        BtnVolver.Size = New Size(120, 40)

        BtnGuardar.BackColor = ColorTranslator.FromHtml("#074788")
        BtnGuardar.ForeColor = Color.White
        BtnVolver.BackColor = ColorTranslator.FromHtml("#F7D917")
        BtnVolver.ForeColor = Color.Black

        ' Posicionamiento de botones
        BtnGuardar.Location = New Point(100, yPosition + 20)
        BtnVolver.Location = New Point(260, yPosition + 20)

        ' Efectos hover para botones
        For Each btn As Button In {BtnGuardar, BtnVolver}
            AddHandler btn.MouseEnter, Sub(sender, e)
                                           CType(sender, Button).BackColor = ColorTranslator.FromHtml("#0A5AA8")
                                           CType(sender, Button).FlatStyle = FlatStyle.Flat
                                       End Sub
            AddHandler btn.MouseLeave, Sub(sender, e)
                                           CType(sender, Button).BackColor = If(btn.Text = "Volver", ColorTranslator.FromHtml("#F7D917"), ColorTranslator.FromHtml("#074788"))
                                       End Sub
        Next

        AddHandler BtnGuardar.Click, AddressOf BtnGuardar_Click
        AddHandler BtnVolver.Click, Sub(s, e) Me.Close()

        mainContainer.Controls.Add(BtnGuardar)
        mainContainer.Controls.Add(BtnVolver)

        ' Agregar todos los paneles al formulario
        Me.Controls.Add(mainContainer)
        Me.Controls.Add(bottomBorder)
        Me.Controls.Add(topPanel)
    End Sub


    Private Async Sub CargarFacultades()
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync(_apiUrlFacultades)
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim facultades = JsonConvert.DeserializeObject(Of List(Of Facultades))(json)

                    CmbFacultad.Items.Clear()
                    For Each f In facultades
                        CmbFacultad.Items.Add(New KeyValuePair(Of Integer, String)(f.FacultadId, f.NombreFacultad))
                    Next
                    CmbFacultad.DisplayMember = "Value"
                    CmbFacultad.ValueMember = "Key"
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error cargando facultades: " & ex.Message)
        End Try
    End Sub

    ' En el método CargarDatosUsuario
    Private Async Sub CargarDatosUsuario()
        Try
            Using client As New HttpClient()
                ' Agregar token de autenticación
                client.DefaultRequestHeaders.Authorization =
                New AuthenticationHeaderValue("Bearer", UserSession.Token)

                Dim response = Await client.GetAsync($"{_apiUrl}/{UserSession.usuarioId}")

                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim usuario = JsonConvert.DeserializeObject(Of Usuario)(json)

                    ' Llenar campos con datos del usuario
                    TxtNombre.Text = usuario.nombre
                    TxtEmail.Text = usuario.email
                    Await SeleccionarFacultad(usuario.facultadId.Value)
                    Await SeleccionarCarrera(usuario.carreraId.Value)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error cargando datos: " & ex.Message)
        End Try
    End Sub

    ' En el método BtnGuardar_Click
    Private Async Sub BtnGuardar_Click(sender As Object, e As EventArgs)
        ' ... (validaciones existentes)

        Dim usuarioActualizado As New Usuario With {
        .usuarioId = UserSession.usuarioId,
        .nombre = TxtNombre.Text.Trim(),
        .email = TxtEmail.Text.Trim(),
        .password = If(String.IsNullOrWhiteSpace(TxtPassword.Text), Nothing, TxtPassword.Text.Trim()),
        .facultadId = CType(CmbFacultad.SelectedItem, KeyValuePair(Of Integer, String)).Key,
        .carreraId = CType(CmbCarrera.SelectedItem, KeyValuePair(Of Integer, String)).Key
    }

        Using client As New HttpClient()
            client.DefaultRequestHeaders.Authorization =
            New AuthenticationHeaderValue("Bearer", UserSession.Token)

            Dim json = JsonConvert.SerializeObject(usuarioActualizado)
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")

            Dim response = Await client.PutAsync($"{_apiUrl}/{UserSession.usuarioId}", content)

            If response.IsSuccessStatusCode Then
                ' Actualizar datos de sesión
                UserSession.nombre = usuarioActualizado.nombre
                UserSession.email = usuarioActualizado.email
                UserSession.facultadId = usuarioActualizado.facultadId
                UserSession.carreraId = usuarioActualizado.carreraId

                MessageBox.Show("Perfil actualizado correctamente")
            End If
        End Using
    End Sub
    Private Async Function SeleccionarFacultad(facultadId As Integer) As Task
        Try
            ' Buscar la facultad en la lista de ComboBox
            For Each item As KeyValuePair(Of Integer, String) In CmbFacultad.Items
                If item.Key = facultadId Then
                    CmbFacultad.SelectedItem = item
                    Exit For
                End If
            Next

            ' Cargar carreras asociadas a la facultad seleccionada
            Await CargarCarreras(facultadId)
        Catch ex As Exception
            MessageBox.Show("Error seleccionando facultad: " & ex.Message)
        End Try
    End Function
    Private Async Function CargarCarreras(facultadId As Integer) As Task
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync($"{_apiUrlCarreras}?facultadId={facultadId}")
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim carreras = JsonConvert.DeserializeObject(Of List(Of Carreras))(json)

                    CmbCarrera.BeginUpdate()
                    CmbCarrera.Items.Clear()
                    For Each c In carreras
                        CmbCarrera.Items.Add(New KeyValuePair(Of Integer, String)(c.CarreraId, c.NombreCarrera))
                    Next
                    CmbCarrera.DisplayMember = "Value"
                    CmbCarrera.ValueMember = "Key"
                    CmbCarrera.EndUpdate()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error cargando carreras: " & ex.Message)
        End Try
    End Function

    Private Async Function SeleccionarCarrera(carreraId As Integer) As Task
        Try
            ' Buscar la carrera en la lista de ComboBox
            For Each item As KeyValuePair(Of Integer, String) In CmbCarrera.Items
                If item.Key = carreraId Then
                    CmbCarrera.SelectedItem = item
                    Exit For
                End If
            Next
        Catch ex As Exception
            MessageBox.Show("Error seleccionando carrera: " & ex.Message)
        End Try
    End Function

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(TxtNombre.Text) Then Return False
        If String.IsNullOrWhiteSpace(TxtEmail.Text) Then Return False
        If TxtPassword.Text <> TxtConfirmPassword.Text Then
            MessageBox.Show("Las contraseñas no coinciden")
            Return False
        End If
        If CmbFacultad.SelectedItem Is Nothing OrElse CmbCarrera.SelectedItem Is Nothing Then
            MessageBox.Show("Seleccione facultad y carrera")
            Return False
        End If
        Return True
    End Function

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'ActualizarPerfil
        '
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Name = "ActualizarPerfil"
        Me.ResumeLayout(False)

    End Sub

    Private Sub ActualizarPerfil_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ActualizarCampos(usuario As Usuario)
        TxtNombre.Text = usuario.nombre
        TxtEmail.Text = usuario.email

        If usuario.facultadId.HasValue Then
            SeleccionarFacultad(usuario.facultadId.Value).Wait()
            If usuario.carreraId.HasValue Then
                SeleccionarCarrera(usuario.carreraId.Value).Wait()
            End If
        End If
    End Sub
End Class
