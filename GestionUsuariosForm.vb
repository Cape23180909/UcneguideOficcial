Imports Newtonsoft.Json
Imports RestSharp
Imports System.Drawing
Imports System.Net.Http
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
        Me.Size = New Size(400, 400)
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

        Me.Controls.AddRange({topPanel, txtEmail, txtPassword, btnLogin})
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
End Class



