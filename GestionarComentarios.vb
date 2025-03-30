Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text

Public Class GestionarComentarios
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private httpClient As New HttpClient()
    Private asignaturas As List(Of Asignaturas)
    Private docentes As List(Of Docente)

    ' Controles del formulario
    Private dgvComentarios As DataGridView
    Private btnCrear As Button
    Private contentPanel As Panel
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    Private WithEvents btnEditar As Button
    Private btnEliminar As Button
    Private Sub GestionarComentarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeComponents()
        CargarDatosIniciales()
    End Sub

    Private Sub InitializeComponents()
        Me.WindowState = FormWindowState.Maximized
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.None
        Me.BackColor = Color.White

        ' Crear controles en el orden correcto
        CrearPanelSuperior()
        CrearContenidoPrincipal()
    End Sub

    Private Sub CrearPanelSuperior()
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 120,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Icono/Logo
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(90, 90),
            .Location = New Point(25, 15),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        ' Título centrado
        Dim lblTitle As New Label With {
        .Text = "Historial de comentarios",
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
        lblTitle.BringToFront() ' Asegurar que está sobre el borde

        ' Borde decorativo
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        topPanel.Controls.Add(bottomBorder)

        Me.Controls.Add(topPanel)
    End Sub

    Private Sub CrearContenidoPrincipal()
        contentPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(20, 10, 20, 10),
            .BackColor = Color.White
        }

        ConfigurarDataGridView()
        CrearPanelBotones()
        Me.Controls.Add(contentPanel)
        contentPanel.BringToFront()
    End Sub

    Private Sub ConfigurarDataGridView()
        dgvComentarios = New DataGridView With {
        .Dock = DockStyle.Fill,
        .AllowUserToAddRows = False,
        .AllowUserToDeleteRows = False,
        .ReadOnly = True,
        .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        .BackgroundColor = Color.White,
        .BorderStyle = BorderStyle.None,
        .RowHeadersVisible = False,
        .Margin = New Padding(0, 0, 0, 5),
        .SelectionMode = DataGridViewSelectionMode.FullRowSelect, ' Selección completa de fila
        .MultiSelect = False, ' Deshabilitar selección múltiple
        .EditMode = DataGridViewEditMode.EditProgrammatically, ' Evitar edición con clic
        .DefaultCellStyle = New DataGridViewCellStyle With {
            .Font = New Font("Segoe UI", 10, FontStyle.Regular),
            .ForeColor = Color.FromArgb(64, 64, 64),
            .BackColor = Color.White,
            .Padding = New Padding(5),
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        },
        .ColumnHeadersDefaultCellStyle = New DataGridViewCellStyle With {
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .Font = New Font("Arial", 11, FontStyle.Bold),
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        },
        .AlternatingRowsDefaultCellStyle = New DataGridViewCellStyle With {
            .BackColor = Color.FromArgb(245, 245, 245)
        }
    }

        ' Mejorar la renderización del texto
        dgvComentarios.EnableHeadersVisualStyles = False
        dgvComentarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing
        dgvComentarios.ColumnHeadersHeight = 40
        dgvComentarios.RowTemplate.Height = 35

        contentPanel.Controls.Add(dgvComentarios)
    End Sub

    Private Sub CrearPanelBotones()
        Dim footerPanel As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 60,
            .BackColor = Color.White,
            .Padding = New Padding(20, 5, 20, 5)
        }

        ' Botón Crear
        btnCrear = New Button With {
            .Text = "Crear Nuevo",
            .Size = New Size(150, 40),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Arial", 10, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnCrear.FlatAppearance.BorderSize = 0

        ' Botón Eliminar
        btnEliminar = New Button With {
            .Text = "Eliminar",
            .Size = New Size(150, 40),
            .BackColor = ColorTranslator.FromHtml("#dc3545"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Arial", 10, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnEliminar.FlatAppearance.BorderSize = 0

        ' Botón Editar
        btnEditar = New Button With {
            .Text = "Editar",
            .Size = New Size(150, 40),
            .BackColor = ColorTranslator.FromHtml("#28a745"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Arial", 10, FontStyle.Bold),
            .Cursor = Cursors.Hand
        }
        btnEditar.FlatAppearance.BorderSize = 0

        ' Manejadores de eventos
        AddHandler btnCrear.Click, AddressOf AbrirGenerarComentarios
        AddHandler btnEliminar.Click, AddressOf EliminarComentario
        AddHandler btnEditar.Click, AddressOf EditarComentario

        ' Posicionamiento de botones
        AddHandler footerPanel.Resize, Sub(s, e)
                                           btnCrear.Left = footerPanel.Width - btnCrear.Width - 20
                                           btnEliminar.Left = btnCrear.Left - btnEliminar.Width - 10
                                           btnEditar.Left = btnEliminar.Left - btnEditar.Width - 10
                                       End Sub

        footerPanel.Controls.Add(btnEditar)
        footerPanel.Controls.Add(btnEliminar)
        footerPanel.Controls.Add(btnCrear)
        contentPanel.Controls.Add(footerPanel)
    End Sub

    Private Async Sub CargarDatosIniciales()
        Try
            Dim t1 = httpClient.GetAsync(ApiUrlAsignaturas)
            Dim t2 = httpClient.GetAsync(ApiUrlDocentes)
            Await Task.WhenAll(t1, t2)

            If t1.Result.IsSuccessStatusCode Then
                asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(
                    Await t1.Result.Content.ReadAsStringAsync())
            End If

            If t2.Result.IsSuccessStatusCode Then
                docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(
                    Await t2.Result.Content.ReadAsStringAsync())
            End If

            Await CargarComentarios()
        Catch ex As Exception
            MessageBox.Show($"Error inicial: {ex.Message}")
        End Try
    End Sub



    Private Async Sub EliminarComentario(sender As Object, e As EventArgs)
        If dgvComentarios.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleccione un comentario para eliminar")
            Return
        End If

        Dim comentarioId = CInt(dgvComentarios.SelectedRows(0).Cells("ComentarioId").Value)

        ' Mostrar formulario de confirmación
        Using eliminarForm As New EliminarComentarios(comentarioId)
            If eliminarForm.ShowDialog() = DialogResult.OK Then
                ' Actualizar lista si la eliminación fue exitosa
                Await CargarComentarios()
            End If
        End Using
    End Sub

    Private Async Sub EditarComentario(sender As Object, e As EventArgs) Handles btnEditar.Click
        If dgvComentarios.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleccione un comentario para editar",
                      "Advertencia",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim selectedId = CInt(dgvComentarios.SelectedRows(0).Cells("ComentarioId").Value)
            Using editForm = New ModificarComentarios(selectedId, asignaturas, docentes)
                If editForm.ShowDialog() = DialogResult.OK Then
                    Await CargarComentarios()
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error al editar: {ex.Message}",
                      "Error",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Function CargarComentarios() As Task
        Try
            ' Asegurar que las asignaturas y docentes están cargados
            If asignaturas Is Nothing OrElse docentes Is Nothing Then
                Await CargarAsignaturasYDocentes()
            End If

            Using client As New HttpClient()
                Dim response = Await client.GetAsync(ApiUrlComentarios)
                If response.IsSuccessStatusCode Then
                    Dim json = Await response.Content.ReadAsStringAsync()
                    Dim todosComentarios = JsonConvert.DeserializeObject(Of List(Of Comentarios))(json)

                    ' Obtener ID del usuario actual
                    Dim usuarioIdActual As Integer = ObtenerUsuarioIdActual()

                    ' Filtrar por usuario (si aplica)
                    Dim comentariosFiltrados = todosComentarios.
                    Where(Function(c) c.UsuarioId = usuarioIdActual). ' Filtro por usuario
                    ToList()

                    ' Mapear nombres de asignaturas y docentes
                    For Each c In comentariosFiltrados
                        ' Verificar si se encuentra la asignatura correspondiente
                        Dim asignatura = asignaturas?.FirstOrDefault(Function(a) a.AsignaturaId = c.AsignaturaId)

                        If asignatura IsNot Nothing Then
                            c.nombreAsignatura = asignatura.nombreAsignatura
                        Else
                            ' Si no se encuentra la asignatura, mostrar mensaje de depuración
                            c.nombreAsignatura = "Asignatura no encontrada"
                            Debug.WriteLine($"Asignatura no encontrada para el ComentarioId: {c.ComentarioId}, AsignaturaId: {c.AsignaturaId}")
                        End If

                        ' Opcional: Si necesitas docentes
                        Dim docente = docentes?.FirstOrDefault(Function(d) d.docenteId = c.DocenteId)
                        c.NombreDocenteCompleto = If(docente IsNot Nothing, docente.nombre & " " & docente.apellido, "N/A")

                    Next

                    ' Actualizar DataGridView de forma segura
                    dgvComentarios.Invoke(Sub()
                                              dgvComentarios.DataSource = Nothing
                                              dgvComentarios.DataSource = comentariosFiltrados
                                              ConfigurarColumnas() ' Asegurar formato de columnas
                                          End Sub)
                Else
                    MessageBox.Show("Error al cargar comentarios: " & response.StatusCode)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error crítico: {ex.Message}")
        End Try
    End Function




    Private Async Function ObtenerDatosAPI(Of T)(url As String) As Task(Of T)
        Using client As New HttpClient()
            Dim response = Await client.GetAsync(url)
            Return If(response.IsSuccessStatusCode, JsonConvert.DeserializeObject(Of T)(Await response.Content.ReadAsStringAsync()), Nothing)
        End Using
    End Function

    Private Async Function CargarAsignaturasYDocentes() As Task
        Dim t1 = httpClient.GetAsync(ApiUrlAsignaturas)
        Dim t2 = httpClient.GetAsync(ApiUrlDocentes)
        Await Task.WhenAll(t1, t2)

        asignaturas = If(t1.Result.IsSuccessStatusCode,
            JsonConvert.DeserializeObject(Of List(Of Asignaturas))(Await t1.Result.Content.ReadAsStringAsync()),
            New List(Of Asignaturas))

        docentes = If(t2.Result.IsSuccessStatusCode,
            JsonConvert.DeserializeObject(Of List(Of Docente))(Await t2.Result.Content.ReadAsStringAsync()),
            New List(Of Docente))
    End Function

    ' Método para obtener el ID del usuario (implementa según tu lógica de autenticación)
    Private Function ObtenerUsuarioIdActual() As Integer
        Return UserSession.usuarioId
    End Function


    Private Sub ConfigurarColumnas()
        dgvComentarios.AutoGenerateColumns = False
        dgvComentarios.Columns.Clear()


        ' Asegurar que el nombre de la columna coincida EXACTAMENTE
        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
        .Name = "ComentarioId",
        .DataPropertyName = "ComentarioId",
        .HeaderText = "ComentarioId",
        .Visible = False
    })

        ' Columna Comentario
        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
        .DataPropertyName = "Comentario",
        .HeaderText = "COMENTARIO",
        .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
        .DefaultCellStyle = New DataGridViewCellStyle With {
            .Padding = New Padding(5),
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }
    })

        ' Columna Asignatura
        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
        .DataPropertyName = "NombreAsignatura",
        .HeaderText = "ASIGNATURA",
        .Width = 200,
        .DefaultCellStyle = New DataGridViewCellStyle With {
            .Padding = New Padding(5),
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }
    })

        ' Columna Docente
        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
        .DataPropertyName = "NombreDocenteCompleto",
        .HeaderText = "DOCENTE",
        .Width = 200,
        .DefaultCellStyle = New DataGridViewCellStyle With {
            .Padding = New Padding(5),
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }
    })

        ' Columna Fecha
        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
        .DataPropertyName = "FechaComentario",
        .HeaderText = "FECHA",
        .Width = 150,
        .DefaultCellStyle = New DataGridViewCellStyle With {
            .Format = "dd/MM/yyyy HH:mm",
            .Alignment = DataGridViewContentAlignment.MiddleCenter
        }
    })

        ' Estilos adicionales
        dgvComentarios.RowTemplate.Height = 35
        dgvComentarios.ColumnHeadersHeight = 40
    End Sub


    Private Sub AbrirGenerarComentarios(sender As Object, e As EventArgs)
        Dim formGenerar As New GenerarComentarios()
        formGenerar.ShowDialog()
    End Sub

End Class