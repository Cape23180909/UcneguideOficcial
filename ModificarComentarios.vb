Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports System.Linq

Public Class ModificarComentarios
    Inherits Form

    ' URLs de la API
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"

    ' Componentes del formulario
    Private WithEvents cmbAsignaturas As ComboBox
    Private WithEvents cmbDocentes As ComboBox
    Private WithEvents txtComentario As TextBox
    Private lblFecha As Label
    Private WithEvents btnActualizar As Button
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

    ' Variables de estado
    Private httpClient As New HttpClient()
    Private asignaturas As List(Of Asignaturas)
    Private docentes As List(Of Docente)
    Private comentarioActual As Comentarios
    Private comentarioId As Integer

    Public Sub New(comentarioId As Integer, asignaturas As List(Of Asignaturas), docentes As List(Of Docente))
        Me.comentarioId = comentarioId
        Me.asignaturas = asignaturas
        Me.docentes = docentes
        InitializeFormComponents()
        AddHandler Me.Load, AddressOf ModificarComentarios_Load
    End Sub

    Private Sub InitializeFormComponents()
        Me.SuspendLayout()

        ' Configuración básica del formulario
        ConfigurarInterfaz()
        CrearPanelSuperior()
        CrearControlesFormulario()

        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Private Sub ConfigurarInterfaz()
        Me.Text = "Editar Comentario"
        Me.Size = New Size(800, 600)
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.Font = New Font("Segoe UI", 10)
        Me.FormBorderStyle = FormBorderStyle.Sizable
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

        ' Título centrado
        Dim lblTitle As New Label With {
            .Text = "MODIFICAR COMENTARIO",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = True
        }

        ' Posicionamiento dinámico del título
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
            .Size = New Size(700, 600),
            .Dock = DockStyle.None,
            .Anchor = AnchorStyles.None,
            .Left = (Me.ClientSize.Width - 700) \ 2,
            .Top = (Me.ClientSize.Height - 600) \ 2,
            .AutoScroll = True
        }

        ' Configurar estilos base
        Dim labelStyle As New Label With {
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .ForeColor = ColorTranslator.FromHtml("#074788"),
            .AutoSize = True
        }

        ' Configurar controles
        lblFecha = CreateLabel("Fecha:", labelStyle, 20)
        cmbAsignaturas = New ComboBox With {
            .Font = New Font("Segoe UI", 11),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .Size = New Size(500, 40),
            .Margin = New Padding(0, 0, 0, 25)
        }

        cmbDocentes = New ComboBox With {
            .Font = New Font("Segoe UI", 11),
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .FlatStyle = FlatStyle.Flat,
            .Size = New Size(500, 40),
            .Margin = New Padding(0, 0, 0, 25)
        }

        txtComentario = New TextBox With {
            .Multiline = True,
            .ScrollBars = ScrollBars.Vertical,
            .Font = New Font("Segoe UI", 11),
            .BorderStyle = BorderStyle.FixedSingle,
            .Size = New Size(500, 150),
            .Margin = New Padding(0, 0, 0, 25)
        }

        btnActualizar = New Button With {
            .Text = "ACTUALIZAR COMENTARIO",
            .Size = New Size(300, 45),
            .BackColor = ColorTranslator.FromHtml("#28A745"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }

        ' Organizar controles
        Dim yPosition As Integer = 60

        contentPanel.Controls.Add(lblFecha)
        yPosition += 30

        contentPanel.Controls.Add(CreateLabel("Asignatura:", labelStyle, yPosition))
        yPosition += 35
        cmbAsignaturas.Location = New Point((contentPanel.Width - cmbAsignaturas.Width) \ 2, yPosition)
        contentPanel.Controls.Add(cmbAsignaturas)
        yPosition += 70

        contentPanel.Controls.Add(CreateLabel("Docente:", labelStyle, yPosition))
        yPosition += 35
        cmbDocentes.Location = New Point((contentPanel.Width - cmbDocentes.Width) \ 2, yPosition)
        contentPanel.Controls.Add(cmbDocentes)
        yPosition += 70

        contentPanel.Controls.Add(CreateLabel("Comentario:", labelStyle, yPosition))
        yPosition += 35
        txtComentario.Location = New Point((contentPanel.Width - txtComentario.Width) \ 2, yPosition)
        contentPanel.Controls.Add(txtComentario)
        yPosition += 180

        ' Posición del botón
        btnActualizar.Location = New Point(
            (contentPanel.Width - btnActualizar.Width) \ 2,
            txtComentario.Bottom + 20
        )

        contentPanel.Controls.Add(btnActualizar)

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
            .Location = New Point((Me.ClientSize.Width - 500) \ 2, y),
            .AutoSize = True
        }
    End Function

    Private Async Sub ModificarComentarios_Load(sender As Object, e As EventArgs)
        Await LoadInitialData()
        ConfigureControls()
    End Sub

    Private Async Function LoadInitialData() As Task
        Try
            ' Cargar comentario
            Dim response = Await httpClient.GetAsync($"{ApiUrlComentarios}/{comentarioId}")
            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                comentarioActual = JsonConvert.DeserializeObject(Of Comentarios)(json)

                If comentarioActual Is Nothing Then
                    MessageBox.Show("El comentario no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Me.Close()
                    Return
                End If
            End If

            ' Obtener carrera ID del comentario a través de la asignatura
            Dim responseAsignatura = Await httpClient.GetAsync($"{ApiUrlAsignaturas}/{comentarioActual.AsignaturaId}")
            If responseAsignatura.IsSuccessStatusCode Then
                Dim jsonAsignatura = Await responseAsignatura.Content.ReadAsStringAsync()
                Dim asignaturaComentario = JsonConvert.DeserializeObject(Of Asignaturas)(jsonAsignatura)

                ' Cargar asignaturas de LA MISMA CARRERA que el comentario
                Dim responseAsignaturas = Await httpClient.GetAsync($"{ApiUrlAsignaturas}?carreraId={asignaturaComentario.CarreraId}")
                If responseAsignaturas.IsSuccessStatusCode Then
                    asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(Await responseAsignaturas.Content.ReadAsStringAsync())
                End If

                ' Cargar docentes de LA MISMA CARRERA que el comentario
                Dim responseDocentes = Await httpClient.GetAsync($"{ApiUrlDocentes}?carreraId={asignaturaComentario.CarreraId}")
                If responseDocentes.IsSuccessStatusCode Then
                    docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(Await responseDocentes.Content.ReadAsStringAsync())
                End If
            End If

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End Try
    End Function

    Private Sub ConfigureControls()
        Try
            ' Configurar ComboBox de asignaturas primero
            cmbAsignaturas.DataSource = asignaturas
            cmbAsignaturas.DisplayMember = "NombreAsignatura"
            cmbAsignaturas.ValueMember = "AsignaturaId"
            cmbAsignaturas.SelectedValue = comentarioActual.AsignaturaId

            ' Filtrar docentes basado en la asignatura seleccionada inicialmente
            FilterTeachersBySelectedSubject()

            ' Configurar otros controles
            lblFecha.Text = $"Fecha: {comentarioActual.FechaComentario:dd/MM/yyyy HH:mm}"
            txtComentario.Text = comentarioActual.Comentario

            ' Agregar manejador de eventos para cuando cambie la selección de asignatura
            AddHandler cmbAsignaturas.SelectedIndexChanged, AddressOf cmbAsignaturas_SelectedIndexChanged

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub cmbAsignaturas_SelectedIndexChanged(sender As Object, e As EventArgs)
        FilterTeachersBySelectedSubject()
    End Sub

    Private Sub FilterTeachersBySelectedSubject()
        If cmbAsignaturas.SelectedItem IsNot Nothing Then
            Dim selectedAsignatura = DirectCast(cmbAsignaturas.SelectedItem, Asignaturas)
            Dim docenteIdDeLaAsignatura = selectedAsignatura.DocenteId

            ' Filtrar la lista de docentes para mostrar solo el que imparte esta materia
            Dim docenteFiltrado = docentes.Where(Function(d) d.docenteId = docenteIdDeLaAsignatura).ToList()

            ' Configurar ComboBox de docentes con el docente filtrado
            cmbDocentes.DataSource = docenteFiltrado
            cmbDocentes.DisplayMember = "NombreCompleto"
            cmbDocentes.ValueMember = "docenteId"

            ' Seleccionar el docente correspondiente
            If docenteFiltrado.Any() Then
                cmbDocentes.SelectedValue = docenteIdDeLaAsignatura
            End If
        End If
    End Sub

    Private Async Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        If Not ValidateInputs() Then Return

        Try
            Dim updatedComment = CreateUpdatedComment()
            Dim response = Await UpdateComment(updatedComment)

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Comentario actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                ShowApiError(response)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ValidateInputs() As Boolean
        If String.IsNullOrWhiteSpace(txtComentario.Text) Then
            MessageBox.Show("El comentario no puede estar vacío", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If cmbAsignaturas.SelectedValue Is Nothing OrElse cmbDocentes.SelectedValue Is Nothing Then
            MessageBox.Show("Debe seleccionar una asignatura y un docente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Function CreateUpdatedComment() As Comentarios
        Return New Comentarios With {
            .ComentarioId = comentarioId,
            .AsignaturaId = CInt(cmbAsignaturas.SelectedValue),
            .DocenteId = CInt(cmbDocentes.SelectedValue),
            .Comentario = txtComentario.Text.Trim(),
            .FechaComentario = comentarioActual.FechaComentario,
            .UsuarioId = comentarioActual.UsuarioId
        }
    End Function

    Private Async Function UpdateComment(comment As Comentarios) As Task(Of HttpResponseMessage)
        Dim json = JsonConvert.SerializeObject(comment)
        Dim content = New StringContent(json, Encoding.UTF8, "application/json")
        Return Await httpClient.PutAsync($"{ApiUrlComentarios}/{comentarioId}", content)
    End Function

    Private Async Sub ShowApiError(response As HttpResponseMessage)
        Dim errorContent = Await response.Content.ReadAsStringAsync()
        MessageBox.Show($"Error del servidor: {response.StatusCode}{Environment.NewLine}{errorContent}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
    End Sub
End Class