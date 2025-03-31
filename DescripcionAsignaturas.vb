Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.Net.Http

Imports System.Net.Http.Headers
Imports System.Text



Public Class DescripcionAsignaturas
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private CodigoAsignatura As String
    Private DocenteId As String
    Private AsignaturaId As Integer  ' ?? Eliminado valor hardcodeadoa



    ' Controles del formulario
    Private topPanel As Panel
    Private mainPanel As Panel
    Private iconoPictureBox As PictureBox
    Private LblNombreAsignatura, LblCodigoAsignatura, LblDescripcionAsignatura, LblNombreDocenteCompleto As Label
    Private LstComentarios As ListBox
    Private TxtComentario As TextBox
    Private BtnEnviarComentario As Button
    Private docenteIdInt As Integer

    ' Constructor
    Public Sub New(codigo As String)
        Me.CodigoAsignatura = codigo
        InitializeForm()
    End Sub

    Private Sub InitializeForm()
        Me.Text = "Descripción de Asignatura"
        Me.WindowState = FormWindowState.Maximized
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.None

        ' Panel superior
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

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

        Dim lblTitle As New Label With {
            .Text = "DETALLE ASIGNATURA",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = True
        }

        AddHandler topPanel.Resize, Sub()
                                        lblTitle.Location = New Point(
                (topPanel.Width - lblTitle.Width) \ 2,
                (topPanel.Height - lblTitle.Height) \ 2)
                                    End Sub

        topPanel.Controls.AddRange({iconoPictureBox, lblTitle})
        lblTitle.BringToFront()

        ' Panel principal
        mainPanel = New Panel With {
            .Size = New Size(600, 500),
            .BackColor = Color.White,
            .Anchor = AnchorStyles.None
        }
        mainPanel.Location = New Point((Me.ClientSize.Width - mainPanel.Width) \ 2, (Me.ClientSize.Height - mainPanel.Height) \ 2)
        AddHandler Me.Resize, AddressOf AjustarPanelCentrado

        ' Configuración de estilos
        Dim estiloTitulo As New Font("Arial", 10, FontStyle.Bold)
        Dim estiloValor As New Font("Arial", 10)
        Dim colorTitulo As Color = ColorTranslator.FromHtml("#074788")
        Dim colorTexto As Color = Color.Black

        ' Función para crear títulos
        Dim CrearTitulo = Function(texto As String, y As Integer) As Label
                              Return New Label With {
                .Text = texto,
                .Location = New Point(20, y),
                .AutoSize = True,
                .Font = estiloTitulo,
                .ForeColor = colorTitulo
            }
                          End Function

        ' Función para crear valores
        Dim CrearValor = Function(y As Integer) As Label
                             Return New Label With {
                .Location = New Point(120, y),
                .AutoSize = True,
                .Font = estiloValor,
                .ForeColor = colorTexto,
                .Width = 450
            }
                         End Function

        ' Crear controles
        Dim posY As Integer = 20
        Dim lblNombreTitulo = CrearTitulo("Nombre:", posY)
        LblNombreAsignatura = CrearValor(posY)
        posY += 25

        Dim lblCodigoTitulo = CrearTitulo("Código:", posY)
        LblCodigoAsignatura = CrearValor(posY)
        posY += 25

        Dim lblDescTitulo = CrearTitulo("Descripción:", posY)
        LblDescripcionAsignatura = New Label With {
            .Location = New Point(20, posY + 25),
            .Size = New Size(550, 40),
            .Font = estiloValor,
            .ForeColor = colorTexto
        }
        posY += 65

        Dim lblDocenteTitulo = CrearTitulo("Docente:", posY)
        LblNombreDocenteCompleto = CrearValor(posY)
        posY += 35

        ' ListBox y controles de comentarios
        LstComentarios = New ListBox With {
            .Location = New Point(20, posY),
            .Size = New Size(550, 150),
            .Font = estiloValor,
            .ForeColor = colorTexto
        }
        posY += 160

        TxtComentario = New TextBox With {
            .Location = New Point(20, posY),
            .Size = New Size(400, 30),
            .Font = estiloValor
        }

        BtnEnviarComentario = New Button With {
            .Text = "Enviar Comentario",
            .Location = New Point(430, posY),
            .Size = New Size(140, 30),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Arial", 9, FontStyle.Bold)
        }

        ' Agregar controles
        mainPanel.Controls.AddRange({
            lblNombreTitulo, LblNombreAsignatura,
            lblCodigoTitulo, LblCodigoAsignatura,
            lblDescTitulo, LblDescripcionAsignatura,
            lblDocenteTitulo, LblNombreDocenteCompleto,
            LstComentarios, TxtComentario, BtnEnviarComentario
        })

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
            .Font = New Font("Arial", 10, FontStyle.Bold),
            .ForeColor = Color.White
        }
    End Function

    ' Evento de carga
    Private Async Sub DescripcionAsignaturas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await CargarDatosAsignatura()
        Await CargarComentarios()
    End Sub


    Private Async Function CargarDatosAsignatura() As Task
        Dim detalleAsignatura = Await ObtenerDatosAPI(Of List(Of Dictionary(Of String, Object)))(ApiUrlAsignaturas)
        If detalleAsignatura Is Nothing OrElse Not detalleAsignatura.Any() Then Return

        ' Buscar la asignatura por código
        Dim asignatura = detalleAsignatura.FirstOrDefault(Function(a) a.ContainsKey("codigoAsignatura") AndAlso a("codigoAsignatura").ToString() = CodigoAsignatura)
        If asignatura Is Nothing Then Return

        ' Asignar valores de la asignatura
        LblNombreAsignatura.Text &= asignatura("nombreAsignatura").ToString()
        LblCodigoAsignatura.Text &= CodigoAsignatura
        LblDescripcionAsignatura.Text &= asignatura("descripcionAsignatura").ToString()

        ' Asignar el AsignaturaId
        If asignatura.ContainsKey("asignaturaId") Then
            AsignaturaId = Convert.ToInt32(asignatura("asignaturaId"))
        End If

        ' Obtener solo el docente de la asignatura
        If asignatura.ContainsKey("docenteId") Then
            DocenteId = asignatura("docenteId").ToString()
            LblNombreDocenteCompleto.Text &= Await ObtenerNombreDocente(DocenteId)
        End If
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



    Private Async Sub EnviarComentario(sender As Object, e As EventArgs)
        Try
            If String.IsNullOrWhiteSpace(TxtComentario.Text) Then
                MessageBox.Show("Escribe un comentario antes de enviar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            ' Obtener ID del usuario desde la sesión
            Dim usuarioIdActual As Integer = UserSession.usuarioId

            ' Convertir DocenteId a entero
            Dim docenteIdInt As Integer
            If Not Integer.TryParse(DocenteId, docenteIdInt) Then
                MessageBox.Show("ID de docente inválido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Crear comentario con el usuario real
            Dim nuevoComentario As New Comentarios(
                comentario:=TxtComentario.Text.Trim(),
                docenteId:=docenteIdInt,
                asignaturaId:=AsignaturaId,
                usuarioId:=usuarioIdActual ' Usar ID de sesión
            )
            ' Serializar
            Dim json As String = JsonConvert.SerializeObject(nuevoComentario)

            Using client As New HttpClient()
                client.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
                Dim response As HttpResponseMessage = Await client.PostAsync(
                ApiUrlComentarios,
                New StringContent(json, Encoding.UTF8, "application/json")
            )

                If response.IsSuccessStatusCode Then
                    TxtComentario.Clear()
                    Await CargarComentarios()
                    MessageBox.Show("¡Comentario enviado!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Dim errorContent As String = Await response.Content.ReadAsStringAsync()
                    MessageBox.Show($"Error del API: {errorContent}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
            ' ... (resto del código de envío sin cambios)
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Function CargarComentarios() As Task
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetAsync(ApiUrlComentarios)
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim todosComentarios = JsonConvert.DeserializeObject(Of List(Of Comentarios))(json)

                    ' Filtrar por usuario logeado y asignatura actual
                    Dim comentariosFiltrados = todosComentarios.
                    Where(Function(c) c.UsuarioId = UserSession.usuarioId AndAlso c.AsignaturaId = AsignaturaId).
                    ToList()

                    ' Limpiar y cargar comentarios
                    LstComentarios.Items.Clear()
                    For Each comentario In comentariosFiltrados
                        LstComentarios.Items.Add($"[{comentario.FechaComentario:dd/MM/yy HH:mm}] {comentario.Comentario}")
                    Next
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error cargando comentarios: {ex.Message}")
        End Try
    End Function


    ' Método para obtener el ID del usuario (implementa según tu lógica de autenticación)
    Private Function ObtenerUsuarioIdActual() As Integer
        Return UserSession.usuarioId
    End Function

    ' Función para obtener el nombre del usuario
    Private Async Function ObtenerNombreUsuario(usuarioId As Integer) As Task(Of String)
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetStringAsync($"https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Usuarios/{usuarioId}")
                Dim usuario As JObject = JsonConvert.DeserializeObject(Of JObject)(response)
                Return usuario("nombre").ToString()
            End Using
        Catch
            Return "Usuario Anónimo"
        End Try
    End Function

    ' Método genérico para obtener datos de la API
    Private Async Function ObtenerDatosAPI(Of T)(url As String) As Task(Of T)
        Using client As New HttpClient()
            Dim response = Await client.GetAsync(url)
            Return If(response.IsSuccessStatusCode, JsonConvert.DeserializeObject(Of T)(Await response.Content.ReadAsStringAsync()), Nothing)
        End Using
    End Function


End Class