Imports Newtonsoft.Json
Imports System.Drawing
Imports System.Net.Http
Imports System.Text
Imports System.Windows.Forms

Public Class LoginForm
    Inherits Form

    Private ReadOnly apiUrl As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Usuarios"
    Private txtEmail As TextBox
    Private txtPassword As TextBox

    Public Sub New()
        ConfigurarInterfaz()
    End Sub

    Private Sub ConfigurarInterfaz()
        Me.Text = "Login - Sistema UCNE"
        Me.Size = New Size(400, 450)
        Me.BackColor = Color.White
        Me.FormBorderStyle = FormBorderStyle.FixedSingle
        Me.StartPosition = FormStartPosition.CenterScreen

        Dim topPanel As New Panel With {
            .Size = New Size(Me.Width, 100),
            .BackColor = Color.FromArgb(0, 51, 102),
            .Location = New Point(0, 0)
        }

        txtEmail = New TextBox With {
            .Text = "Email",
            .ForeColor = Color.Gray,
            .Size = New Size(250, 30),
            .Location = New Point(75, 150),
            .Font = New Font("Arial", 12)
        }
        AddHandler txtEmail.Enter, AddressOf Placeholder_Enter
        AddHandler txtEmail.Leave, AddressOf TextBox_Leave

        txtPassword = New TextBox With {
            .Text = "Contraseña",
            .ForeColor = Color.Gray,
            .Size = New Size(250, 30),
            .Location = New Point(75, 200),
            .Font = New Font("Arial", 12),
            .UseSystemPasswordChar = False
        }
        AddHandler txtPassword.Enter, AddressOf Placeholder_Enter
        AddHandler txtPassword.Leave, AddressOf TextBox_Leave

        Dim btnLogin As New Button With {
            .Text = "Login",
            .Size = New Size(250, 40),
            .Location = New Point(75, 250),
            .BackColor = Color.FromArgb(0, 102, 204),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click

        ' Link para crear cuenta
        Dim lblCrearCuenta As New LinkLabel With {
            .Text = "Crear cuenta",
            .Location = New Point(150, 300),
            .AutoSize = True,
            .Font = New Font("Arial", 10, FontStyle.Underline),
            .ForeColor = Color.Blue
        }
        AddHandler lblCrearCuenta.Click, AddressOf CrearCuenta_Click

        Me.Controls.AddRange({topPanel, txtEmail, txtPassword, btnLogin, lblCrearCuenta})
    End Sub

    Private Async Sub BtnLogin_Click(sender As Object, e As EventArgs)
        If txtEmail.ForeColor = Color.Gray OrElse txtPassword.ForeColor = Color.Gray Then
            MessageBox.Show("Complete todos los campos", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Using client As New HttpClient()
                client.DefaultRequestHeaders.Accept.Clear()
                client.DefaultRequestHeaders.Accept.Add(New Headers.MediaTypeWithQualityHeaderValue("application/json"))

                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrl)

                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim usuarios As List(Of Usuario) = JsonConvert.DeserializeObject(Of List(Of Usuario))(jsonResponse)

                    If usuarios IsNot Nothing Then
                        Dim usuarioValido = usuarios.FirstOrDefault(Function(u) u.email = txtEmail.Text.Trim() AndAlso u.password = txtPassword.Text.Trim())
                        If usuarioValido IsNot Nothing Then
                            MessageBox.Show("Inicio de sesión exitoso", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                            Dim menu As New Menu()
                            menu.Show()
                            Me.Hide()
                        Else
                            MessageBox.Show("Credenciales incorrectas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End If
                    Else
                        MessageBox.Show("Respuesta de la API vacía o inválida", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Else
                    MessageBox.Show($"Error en la solicitud: {response.StatusCode} - {response.ReasonPhrase}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error de conexión: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    ' Evento para abrir el formulario de registro
    Private Sub CrearCuenta_Click(sender As Object, e As EventArgs)
        Dim registroForm As New RegistroForm()
        registroForm.Show()
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

    Public Sub New()
        Me.Text = "Registro - Sistema UCNE"
        Me.Size = New Size(400, 500)
        Me.StartPosition = FormStartPosition.CenterScreen

        Dim lblNombre As New Label With {.Text = "Nombre:", .Location = New Point(50, 50)}
        txtNombre = New TextBox With {.Size = New Size(250, 30), .Location = New Point(50, 70)}

        Dim lblEmail As New Label With {.Text = "Email:", .Location = New Point(50, 110)}
        txtEmail = New TextBox With {.Size = New Size(250, 30), .Location = New Point(50, 130)}

        Dim lblPassword As New Label With {.Text = "Contraseña:", .Location = New Point(50, 170)}
        txtPassword = New TextBox With {.Size = New Size(250, 30), .Location = New Point(50, 190)}

        Dim lblFacultad As New Label With {.Text = "Facultad:", .Location = New Point(50, 230)}
        cmbFacultad = New ComboBox With {.Size = New Size(250, 30), .Location = New Point(50, 250), .DropDownStyle = ComboBoxStyle.DropDownList}

        Dim lblCarrera As New Label With {.Text = "Carrera:", .Location = New Point(50, 290)}
        cmbCarrera = New ComboBox With {.Size = New Size(250, 30), .Location = New Point(50, 310), .DropDownStyle = ComboBoxStyle.DropDownList}

        btnRegistrar = New Button With {.Text = "Registrar", .Location = New Point(50, 350)}
        AddHandler btnRegistrar.Click, AddressOf BtnRegistrar_Click

        Me.Controls.AddRange({lblNombre, txtNombre, lblEmail, txtEmail, lblPassword, txtPassword, lblFacultad, cmbFacultad, lblCarrera, cmbCarrera, btnRegistrar})

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



