Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Net.Http.Headers

Public Class ActualizarPerfil
    Inherits Form

    ' Campos readonly con nombres PascalCase
    Private ReadOnly apiUrl As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Usuarios"
    Private ReadOnly apiUrlFacultades As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Facultades"
    Private ReadOnly apiUrlCarreras As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Carreras"

    ' Controles con WithEvents y PascalCase
    Private WithEvents TxtNombre As TextBox
    Private WithEvents TxtEmail As TextBox
    Private WithEvents TxtPassword As TextBox
    Private WithEvents TxtConfirmPassword As TextBox
    Private WithEvents CmbFacultad As ComboBox
    Private WithEvents CmbCarrera As ComboBox
    Private WithEvents BtnGuardar As Button
    Private WithEvents BtnVolver As Button

    ' Constructor corregido
    Public Sub New()
        InitializeComponent()
        InitializeCustomComponents()
        CargarFacultades()
        CargarDatosUsuario()
    End Sub

    ' Inicialización de componentes
    Private Sub InitializeCustomComponents()
        Me.Text = "Actualizar Perfil"
        Me.Size = New Size(500, 600)
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Configuración de controles
        Dim lblTitulo As New Label With {
            .Text = "Actualizar Perfil",
            .Font = New Font("Arial", 18, FontStyle.Bold),
            .Location = New Point(20, 20),
            .AutoSize = True
        }

        ' Inicialización de controles
        TxtNombre = New TextBox With {
            .Size = New Size(400, 30),
            .Location = New Point(40, 80),
            .ForeColor = Color.Gray,
            .Text = "Nombre completo"
        }

        TxtEmail = New TextBox With {
            .Size = New Size(400, 30),
            .Location = New Point(40, 140),
            .ForeColor = Color.Gray,
            .Text = "Correo electrónico"
        }

        TxtPassword = New TextBox With {
            .Size = New Size(400, 30),
            .Location = New Point(40, 200),
            .UseSystemPasswordChar = False,
            .ForeColor = Color.Gray,
            .Text = "Nueva contraseña"
        }

        TxtConfirmPassword = New TextBox With {
            .Size = New Size(400, 30),
            .Location = New Point(40, 260),
            .UseSystemPasswordChar = False,
            .ForeColor = Color.Gray,
            .Text = "Confirmar contraseña"
        }

        CmbFacultad = New ComboBox With {
            .Size = New Size(400, 30),
            .Location = New Point(40, 320),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        CmbCarrera = New ComboBox With {
            .Size = New Size(400, 30),
            .Location = New Point(40, 380),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        BtnGuardar = New Button With {
            .Text = "Actualizar",
            .Size = New Size(180, 40),
            .Location = New Point(40, 440),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }

        BtnVolver = New Button With {
            .Text = "Volver",
            .Size = New Size(180, 40),
            .Location = New Point(260, 440),
            .BackColor = ColorTranslator.FromHtml("#F7D917"),
            .ForeColor = Color.Black,
            .FlatStyle = FlatStyle.Flat
        }

        ' Manejo de eventos
        AddHandler TxtNombre.Enter, AddressOf TextBox_Enter
        AddHandler TxtNombre.Leave, AddressOf TextBox_Leave
        AddHandler TxtEmail.Enter, AddressOf TextBox_Enter
        AddHandler TxtEmail.Leave, AddressOf TextBox_Leave
        AddHandler TxtPassword.Enter, AddressOf TextBox_Enter
        AddHandler TxtPassword.Leave, AddressOf TextBox_Leave
        AddHandler TxtConfirmPassword.Enter, AddressOf TextBox_Enter
        AddHandler TxtConfirmPassword.Leave, AddressOf TextBox_Leave
        AddHandler CmbFacultad.SelectedIndexChanged, AddressOf CmbFacultad_SelectedIndexChanged
        AddHandler BtnGuardar.Click, AddressOf BtnGuardar_Click
        AddHandler BtnVolver.Click, Sub(s, e) Me.Close()

        Me.Controls.AddRange({
            lblTitulo, TxtNombre, TxtEmail, TxtPassword, TxtConfirmPassword,
            CmbFacultad, CmbCarrera, BtnGuardar, BtnVolver
        })
    End Sub

    Private Async Sub CargarFacultades()
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync(apiUrlFacultades)
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim facultades = JsonConvert.DeserializeObject(Of List(Of Facultades))(json)

                    CmbFacultad.BeginUpdate()
                    CmbFacultad.Items.Clear()
                    For Each f In facultades
                        CmbFacultad.Items.Add(New KeyValuePair(Of Integer, String)(f.facultadId, f.nombreFacultad))
                    Next
                    CmbFacultad.DisplayMember = "Value"
                    CmbFacultad.ValueMember = "Key"
                    CmbFacultad.EndUpdate()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error cargando facultades: " & ex.Message)
        End Try
    End Sub

    Private Async Sub CargarDatosUsuario()
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync($"{apiUrl}/{UserSession.UserId}")
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim usuario = JsonConvert.DeserializeObject(Of Usuario)(json)

                    TxtNombre.Text = usuario.nombre
                    TxtEmail.Text = usuario.email

                    If usuario.facultadId.HasValue Then
                        Await SeleccionarFacultad(usuario.facultadId.Value)
                        If usuario.carreraId.HasValue Then
                            Await SeleccionarCarrera(usuario.carreraId.Value)
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error cargando datos del usuario: " & ex.Message)
        End Try
    End Sub

    Private Async Function SeleccionarFacultad(facultadId As Integer) As Task
        For Each item In CmbFacultad.Items
            Dim kvp = CType(item, KeyValuePair(Of Integer, String))
            If kvp.Key = facultadId Then
                CmbFacultad.SelectedItem = item
                Await CargarCarreras(facultadId)
                Exit For
            End If
        Next
    End Function

    Private Async Function SeleccionarCarrera(carreraId As Integer) As Task
        For Each item In CmbCarrera.Items
            Dim kvp = CType(item, KeyValuePair(Of Integer, String))
            If kvp.Key = carreraId Then
                CmbCarrera.SelectedItem = item
                Exit For
            End If
        Next
    End Function

    Private Async Sub CmbFacultad_SelectedIndexChanged(sender As Object, e As EventArgs)
        If CmbFacultad.SelectedItem Is Nothing Then Return

        Dim facultadId = CType(CmbFacultad.SelectedItem, KeyValuePair(Of Integer, String)).Key
        Await CargarCarreras(facultadId)
    End Sub

    Private Async Function CargarCarreras(facultadId As Integer) As Task
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync($"{apiUrlCarreras}?facultadId={facultadId}")
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim carreras = JsonConvert.DeserializeObject(Of List(Of Carreras))(json)

                    CmbCarrera.BeginUpdate()
                    CmbCarrera.Items.Clear()
                    For Each c In carreras
                        CmbCarrera.Items.Add(New KeyValuePair(Of Integer, String)(c.carreraId, c.nombreCarrera))
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

    Private Sub TextBox_Enter(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox)
        If txt.ForeColor = Color.Gray Then
            txt.Text = ""
            txt.ForeColor = Color.Black
            If txt Is TxtPassword OrElse txt Is TxtConfirmPassword Then
                txt.UseSystemPasswordChar = True
            End If
        End If
    End Sub

    Private Sub TextBox_Leave(sender As Object, e As EventArgs)
        Dim txt = DirectCast(sender, TextBox)
        If String.IsNullOrWhiteSpace(txt.Text) Then
            txt.ForeColor = Color.Gray
            If txt Is TxtPassword OrElse txt Is TxtConfirmPassword Then
                txt.UseSystemPasswordChar = False
                txt.Text = If(txt Is TxtPassword, "Nueva contraseña", "Confirmar contraseña")
            Else
                txt.Text = If(txt Is TxtNombre, "Nombre completo", "Correo electrónico")
            End If
        End If
    End Sub

    Private Async Sub BtnGuardar_Click(sender As Object, e As EventArgs)
        If Not ValidarCampos() Then Return

        Try
            Dim usuarioActualizado = New With {
                .UsuarioId = UserSession.UserId,
                .Nombre = TxtNombre.Text.Trim(),
                .Email = TxtEmail.Text.Trim(),
                .Password = If(String.IsNullOrEmpty(TxtPassword.Text), Nothing, TxtPassword.Text.Trim()),
                .FacultadId = CType(CmbFacultad.SelectedItem, KeyValuePair(Of Integer, String)).Key,
                .CarreraId = CType(CmbCarrera.SelectedItem, KeyValuePair(Of Integer, String)).Key
            }

            Using client As New HttpClient()
                Dim json = JsonConvert.SerializeObject(usuarioActualizado)
                Dim content = New StringContent(json, Encoding.UTF8, "application/json")
                Dim response = Await client.PutAsync($"{apiUrl}/{UserSession.UserId}", content)

                If response.IsSuccessStatusCode Then
                    ' Actualizar datos de sesión
                    UserSession.Nombre = usuarioActualizado.Nombre
                    UserSession.Email = usuarioActualizado.Email
                    UserSession.FacultadId = usuarioActualizado.FacultadId
                    UserSession.CarreraId = usuarioActualizado.CarreraId

                    MessageBox.Show("Perfil actualizado correctamente")
                    Me.Close()
                Else
                    Dim errorContent = Await response.Content.ReadAsStringAsync()
                    MessageBox.Show($"Error al actualizar: {errorContent}")
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error de conexión: {ex.Message}")
        End Try
    End Sub




    ' Método de validación corregido
    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(TxtNombre.Text) OrElse TxtNombre.Text = "Nombre completo" Then
            MessageBox.Show("El nombre es requerido")
            Return False
        End If

        If String.IsNullOrWhiteSpace(TxtEmail.Text) OrElse TxtEmail.Text = "Correo electrónico" Then
            MessageBox.Show("Ingrese un email válido")
            Return False
        End If

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

    ' Método InitializeComponent requerido
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Name = "ActualizarPerfil"
        Me.ResumeLayout(False)
    End Sub

    Private Sub ActualizarPerfil_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class