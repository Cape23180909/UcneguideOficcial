Imports Newtonsoft.Json
Imports System.Drawing
Imports System.Net.Http
Imports System.Text
Imports System.Windows.Forms

Public Class LoginForm
    Inherits Form

    Private ReadOnly apiUrl As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Usuarios"
    Private lblTitulo As Label
    Private txtEmail As TextBox
    Private txtPassword As TextBox
    Private btnLogin As Button
    Private lblCrearCuenta As LinkLabel
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

    Public Sub New()
        Me.WindowState = FormWindowState.Maximized
        Me.BackColor = Color.White
        ConfigurarInterfaz()
    End Sub

    Private Sub ConfigurarInterfaz()
        Me.Text = "Login - Sistema UCNE"
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Panel superior con color #074788
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 180,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Primera línea amarilla justo debajo del panel azul
        Dim bottomBorder1 As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Segunda línea amarilla debajo de la primera
        Dim bottomBorder2 As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Icono en el centro del panel azul
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(100, 100),
            .Location = New Point((topPanel.Width - 100) \ 2, (topPanel.Height - 100) \ 2 + 20), ' Ajuste de posición
             .Anchor = AnchorStyles.None
        }
        topPanel.Controls.Add(iconoPictureBox)

        lblTitulo = New Label With {
            .Text = "Login",
            .Font = New Font("Arial", 16, FontStyle.Bold),
            .AutoSize = True,
            .ForeColor = Color.Black
        }

        txtEmail = New TextBox With {
            .Text = "Email",
            .ForeColor = Color.Gray,
            .Size = New Size(250, 30),
            .Font = New Font("Arial", 12)
        }
        AddHandler txtEmail.Enter, AddressOf Placeholder_Enter
        AddHandler txtEmail.Leave, AddressOf TextBox_Leave

        txtPassword = New TextBox With {
            .Text = "Contraseña",
            .ForeColor = Color.Gray,
            .Size = New Size(250, 30),
            .Font = New Font("Arial", 12),
            .UseSystemPasswordChar = False
        }
        AddHandler txtPassword.Enter, AddressOf Placeholder_Enter
        AddHandler txtPassword.Leave, AddressOf TextBox_Leave

        btnLogin = New Button With {
            .Text = "Login",
            .Size = New Size(250, 40),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click

        lblCrearCuenta = New LinkLabel With {
            .Text = "Crear cuenta",
            .AutoSize = True,
            .Font = New Font("Arial", 10, FontStyle.Underline),
            .ForeColor = Color.Blue
        }
        AddHandler lblCrearCuenta.Click, AddressOf CrearCuenta_Click

        Me.Controls.AddRange({topPanel, bottomBorder1, bottomBorder2, lblTitulo, txtEmail, txtPassword, btnLogin, lblCrearCuenta})
        CenterControls()
    End Sub

    Private Sub CenterControls()
        Dim centerX As Integer = (Me.ClientSize.Width - txtEmail.Width) \ 2
        Dim centerY As Integer = (Me.ClientSize.Height - 200) \ 2

        lblTitulo.Location = New Point(centerX + (txtEmail.Width - lblTitulo.Width) \ 2, centerY - 80)
        txtEmail.Location = New Point(centerX, centerY)
        txtPassword.Location = New Point(centerX, centerY + 50)
        btnLogin.Location = New Point(centerX, centerY + 100)
        lblCrearCuenta.Location = New Point(centerX + (btnLogin.Width - lblCrearCuenta.Width) \ 2, centerY + 150)
    End Sub

    Private Async Sub BtnLogin_Click(sender As Object, e As EventArgs)
        If txtEmail.ForeColor = Color.Gray OrElse txtPassword.ForeColor = Color.Gray Then
            MessageBox.Show("Complete todos los campos", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrl)
                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim usuarios As List(Of Usuario) = JsonConvert.DeserializeObject(Of List(Of Usuario))(jsonResponse)
                    ' Después de validar el login, obtener asignaturaId para docentes

                    If usuarios IsNot Nothing Then
                        Dim usuarioValido = usuarios.FirstOrDefault(Function(u) u.email = txtEmail.Text.Trim() AndAlso u.password = txtPassword.Text.Trim())
                        If usuarioValido IsNot Nothing Then
                            ' Guardar TODOS los datos en sesión
                            UserSession.usuarioId = usuarioValido.usuarioId
                            UserSession.nombre = usuarioValido.nombre
                            UserSession.email = usuarioValido.email
                            UserSession.facultadId = usuarioValido.facultadId
                            UserSession.carreraId = usuarioValido.carreraId
                            UserSession.Token = usuarioValido.token


                            ' Abrir BienvenidaForm
                            Dim bienvenidaForm As New BienvenidaForm()
                            bienvenidaForm.Show()
                            Me.Hide()
                        Else
                            MessageBox.Show("Credenciales incorrectas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error de conexión: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Placeholder_Enter(sender As Object, e As EventArgs)
        Dim txt = CType(sender, TextBox)
        If txt.ForeColor = Color.Gray Then
            txt.Text = ""
            txt.ForeColor = Color.Black
            If txt Is txtPassword Then txt.UseSystemPasswordChar = True
        End If
    End Sub

    Private Sub TextBox_Leave(sender As Object, e As EventArgs)
        Dim txt = CType(sender, TextBox)
        If String.IsNullOrWhiteSpace(txt.Text) Then
            txt.ForeColor = Color.Gray
            If txt Is txtEmail Then
                txt.Text = "Email"
            Else
                txt.Text = "Contraseña"
                txt.UseSystemPasswordChar = False
            End If
        End If
    End Sub

    Private Sub CrearCuenta_Click(sender As Object, e As EventArgs)
        Dim registroForm As New RegistroForm()
        registroForm.Show()
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        CenterControls()
    End Sub

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class



Public Class RegistroForm
    Inherits Form

    Private ReadOnly apiUrlUsuarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Usuarios"
    Private ReadOnly apiUrlFacultades As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Facultades"
    Private ReadOnly apiUrlCarreras As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Carreras"

    Private txtNombre As TextBox
    Private txtEmail As TextBox
    Private txtPassword As TextBox
    Private cmbFacultad As ComboBox
    Private cmbCarrera As ComboBox
    Private btnRegistrar As Button
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

    Public Sub New()
        Me.Text = "Registro - Sistema UCNE"
        Me.WindowState = FormWindowState.Maximized
        Me.BackColor = Color.White

        ' Panel superior azul
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 180,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }
        Me.Controls.Add(topPanel)

        ' Líneas amarillas debajo del panel azul
        Dim bottomBorder1 As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        Me.Controls.Add(bottomBorder1)

        Dim bottomBorder2 As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        Me.Controls.Add(bottomBorder2)

        ' Icono centrado en el panel azul
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(100, 100),
            .Location = New Point((topPanel.Width - 100) \ 2, (topPanel.Height - 100) \ 2 + 20),
            .Anchor = AnchorStyles.None
        }
        topPanel.Controls.Add(iconoPictureBox)

        ' Panel contenedor para centrar el formulario
        Dim mainPanel As New Panel With {
            .Size = New Size(400, 500),
            .Location = New Point((Me.ClientSize.Width - 400) \ 2, (Me.ClientSize.Height - 500) \ 2),
            .Anchor = AnchorStyles.None
        }
        Me.Controls.Add(mainPanel)

        ' Campos de entrada
        Dim lblNombre As New Label With {.Text = "Nombre:", .Location = New Point(50, 50)}
        txtNombre = New TextBox With {.Size = New Size(250, 30), .Location = New Point(50, 70)}

        Dim lblEmail As New Label With {.Text = "Email:", .Location = New Point(50, 110)}
        txtEmail = New TextBox With {.Size = New Size(250, 30), .Location = New Point(50, 130)}

        Dim lblPassword As New Label With {.Text = "Contraseña:", .Location = New Point(50, 170)}
        txtPassword = New TextBox With {.Size = New Size(250, 30), .Location = New Point(50, 190), .UseSystemPasswordChar = True}

        Dim lblFacultad As New Label With {.Text = "Facultad:", .Location = New Point(50, 230)}
        cmbFacultad = New ComboBox With {.Size = New Size(250, 30), .Location = New Point(50, 250), .DropDownStyle = ComboBoxStyle.DropDownList}

        Dim lblCarrera As New Label With {.Text = "Carrera:", .Location = New Point(50, 290)}
        cmbCarrera = New ComboBox With {.Size = New Size(250, 30), .Location = New Point(50, 310), .DropDownStyle = ComboBoxStyle.DropDownList}

        btnRegistrar = New Button With {
            .Text = "Registrar",
            .Location = New Point(50, 350),
            .Size = New Size(250, 40),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnRegistrar.Click, AddressOf BtnRegistrar_Click

        mainPanel.Controls.AddRange({lblNombre, txtNombre, lblEmail, txtEmail, lblPassword, txtPassword, lblFacultad, cmbFacultad, lblCarrera, cmbCarrera, btnRegistrar})

        ' Cargar datos desde la API
        CargarFacultades()
    End Sub

    Private Async Sub CargarFacultades()
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrlFacultades)
                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim facultades As List(Of Facultades) = JsonConvert.DeserializeObject(Of List(Of Facultades))(jsonResponse)

                    cmbFacultad.Items.Clear()
                    For Each facultad In facultades
                        cmbFacultad.Items.Add(New KeyValuePair(Of Integer, String)(facultad.facultadId, facultad.nombreFacultad))
                    Next
                    cmbFacultad.DisplayMember = "Value"
                    cmbFacultad.ValueMember = "Key"

                    ' Verificar que hay elementos antes de seleccionar
                    If cmbFacultad.Items.Count > 0 Then
                        cmbFacultad.SelectedIndex = 0
                        CargarCarreras(CType(cmbFacultad.SelectedItem, KeyValuePair(Of Integer, String)).Key)
                    End If
                Else
                    MessageBox.Show("Error al obtener facultades.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error de conexión al cargar facultades: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' Manejar cambio de selección en facultad para actualizar carreras
        AddHandler cmbFacultad.SelectedIndexChanged, Sub()
                                                         If cmbFacultad.SelectedItem IsNot Nothing Then
                                                             CargarCarreras(CType(cmbFacultad.SelectedItem, KeyValuePair(Of Integer, String)).Key)
                                                         End If
                                                     End Sub
    End Sub

    Private Async Sub CargarCarreras(facultadId As Integer)
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync($"{apiUrlCarreras}?facultadId={facultadId}")
                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim carreras As List(Of Carreras) = JsonConvert.DeserializeObject(Of List(Of Carreras))(jsonResponse)

                    cmbCarrera.Items.Clear()
                    For Each carrera In carreras
                        cmbCarrera.Items.Add(New KeyValuePair(Of Integer, String)(carrera.carreraId, carrera.nombreCarrera))
                    Next
                    cmbCarrera.DisplayMember = "Value"
                    cmbCarrera.ValueMember = "Key"

                    If cmbCarrera.Items.Count > 0 Then
                        cmbCarrera.SelectedIndex = 0
                    End If
                Else
                    MessageBox.Show("Error al obtener carreras.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error de conexión al cargar carreras: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Sub BtnRegistrar_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtNombre.Text) OrElse
           String.IsNullOrWhiteSpace(txtEmail.Text) OrElse
           String.IsNullOrWhiteSpace(txtPassword.Text) OrElse
           cmbFacultad.SelectedItem Is Nothing OrElse
           cmbCarrera.SelectedItem Is Nothing Then
            MessageBox.Show("Complete todos los campos", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim nuevoUsuario = New With {
            .nombre = txtNombre.Text.Trim(),
            .email = txtEmail.Text.Trim(),
            .password = txtPassword.Text.Trim(),
            .facultadId = CType(cmbFacultad.SelectedItem, KeyValuePair(Of Integer, String)).Key,
            .carreraId = CType(cmbCarrera.SelectedItem, KeyValuePair(Of Integer, String)).Key
        }

        Try
            Using client As New HttpClient()
                Dim json As String = JsonConvert.SerializeObject(nuevoUsuario)
                Dim content As New StringContent(json, Encoding.UTF8, "application/json")
                Dim response = Await client.PostAsync(apiUrlUsuarios, content)

                If response.IsSuccessStatusCode Then
                    MessageBox.Show("Usuario registrado con éxito", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.Close()
                Else
                    MessageBox.Show("Error al registrar usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error de conexión al registrar usuario: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class



