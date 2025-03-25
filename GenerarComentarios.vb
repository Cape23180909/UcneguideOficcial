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
    ' Añadir clase modelo para el comentario
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
        Me.Size = New Size(500, 400)
        Me.BackColor = ColorTranslator.FromHtml("#1E1E1E")
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Segoe UI", 10)

        ' Crear y configurar controles
        CbAsignaturas = New ComboBox With {
            .Location = New Point(120, 50),
            .Width = 300,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        CbDocentes = New ComboBox With {
            .Location = New Point(120, 100),
            .Width = 300,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        TxtComentario = New TextBox With {
            .Location = New Point(120, 150),
            .Size = New Size(300, 100),
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical
        }

        BtnRegistrar = New Button With {
            .Text = "Registrar Comentario",
            .Location = New Point(120, 270),
            .Size = New Size(300, 35),
            .BackColor = ColorTranslator.FromHtml("#28A745"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }

        ' Agregar controles y etiquetas
        AgregarEtiqueta("Asignatura:", 20, 50)
        Me.Controls.Add(CbAsignaturas)
        AgregarEtiqueta("Docente:", 20, 100)
        Me.Controls.Add(CbDocentes)
        AgregarEtiqueta("Comentario:", 20, 150)
        Me.Controls.Add(TxtComentario)
        Me.Controls.Add(BtnRegistrar)

        ' Eventos
        AddHandler BtnRegistrar.Click, AddressOf RegistrarComentario
    End Sub

    Private Sub AgregarEtiqueta(texto As String, x As Integer, y As Integer)
        Me.Controls.Add(New Label With {
            .Text = texto,
            .Location = New Point(x, y),
            .ForeColor = Color.White,
            .AutoSize = True
        })
    End Sub

    Private Async Sub CargarAsignaturasYDocentes()
        Try
            ' Cargar asignaturas
            Dim responseAsignaturas = Await HttpClient.GetAsync(ApiUrlAsignaturas)
            If responseAsignaturas.IsSuccessStatusCode Then
                Dim jsonAsignaturas = Await responseAsignaturas.Content.ReadAsStringAsync()
                Dim asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(jsonAsignaturas)
                CbAsignaturas.DataSource = asignaturas
                CbAsignaturas.DisplayMember = "NombreAsignatura"
                CbAsignaturas.ValueMember = "AsignaturaId"
            End If

            ' Cargar docentes
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