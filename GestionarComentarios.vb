Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text

Public Class GestionarComentarios
    Private ReadOnly ApiUrl As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private httpClient As New HttpClient()
    Private currentComentario As Comentarios

    ' Controles del formulario
    Private dgvComentarios As DataGridView
    Private txtBusqueda As TextBox
    Private btnNuevo As Button
    Private btnGuardar As Button
    Private btnEliminar As Button
    Private txtContenido As TextBox
    Private txtUsuarioId As TextBox
    Private txtAsignaturaId As TextBox

    Private Sub GestionarComentarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeComponents()
        CargarComentarios()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Gestión de Comentarios"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Configurar DataGridView
        dgvComentarios = New DataGridView With {
            .Dock = DockStyle.Top,
            .Height = 300,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        ' Configurar controles de entrada
        Dim panelControles As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 150
        }

        txtContenido = New TextBox With {
            .Location = New Point(10, 10),
            .Size = New Size(300, 20)
        }

        txtUsuarioId = New TextBox With {
            .Location = New Point(10, 40),
            .Size = New Size(100, 20)
        }

        txtAsignaturaId = New TextBox With {
            .Location = New Point(120, 40),
            .Size = New Size(100, 20)
        }

        btnNuevo = New Button With {
            .Text = "Nuevo",
            .Location = New Point(10, 70),
            .Size = New Size(75, 30)
        }

        btnGuardar = New Button With {
            .Text = "Guardar",
            .Location = New Point(95, 70),
            .Size = New Size(75, 30),
            .Enabled = False
        }

        btnEliminar = New Button With {
            .Text = "Eliminar",
            .Location = New Point(180, 70),
            .Size = New Size(75, 30),
            .Enabled = False
        }

        ' Agregar controles al panel
        panelControles.Controls.AddRange({
            txtContenido, txtUsuarioId, txtAsignaturaId,
            btnNuevo, btnGuardar, btnEliminar
        })

        ' Configurar barra de búsqueda
        txtBusqueda = New TextBox With {
            .Dock = DockStyle.Top,
            .Height = 30
        }

        ' Agregar controles al formulario
        Me.Controls.AddRange({dgvComentarios, panelControles, txtBusqueda})

        ' Configurar eventos
        AddHandler btnNuevo.Click, AddressOf NuevoComentario
        AddHandler btnGuardar.Click, AddressOf GuardarComentario
        AddHandler btnEliminar.Click, AddressOf EliminarComentario
        AddHandler dgvComentarios.SelectionChanged, AddressOf SeleccionComentario
        AddHandler txtBusqueda.TextChanged, AddressOf BuscarComentarios
    End Sub

    Private Async Sub CargarComentarios()
        Try
            Dim response = Await httpClient.GetAsync(ApiUrl)
            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim comentarios = JsonConvert.DeserializeObject(Of List(Of Comentarios))(json)
                dgvComentarios.DataSource = comentarios
                ConfigurarColumnas()
            End If
        Catch ex As Exception
            MessageBox.Show($"Error cargando comentarios: {ex.Message}")
        End Try
    End Sub


    Private Sub ConfigurarColumnas()
        dgvComentarios.AutoGenerateColumns = False
        dgvComentarios.Columns.Clear()

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ComentarioId",
            .HeaderText = "ComentarioId",
            .Width = 50
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Comentario",
            .HeaderText = "Comentario",
            .Width = 200
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "FechaComentario",
            .HeaderText = "Fecha",
            .Width = 100
        })
    End Sub

    Private Sub NuevoComentario(sender As Object, e As EventArgs)

        LimpiarCampos()
        HabilitarEdicion(True)
    End Sub
    Private Async Sub GuardarComentario(sender As Object, e As EventArgs)
        Try
            If Not ValidarCampos() Then Return

            ' Corrección 3: Usar propiedad Contenido
            currentComentario.Comentario = txtContenido.Text
            currentComentario.UsuarioId = Integer.Parse(txtUsuarioId.Text)
            currentComentario.AsignaturaId = Integer.Parse(txtAsignaturaId.Text)

            Dim json = JsonConvert.SerializeObject(currentComentario)
            Dim content = New StringContent(json, Encoding.UTF8, "application/json")

            Dim response As HttpResponseMessage

            If currentComentario.ComentarioId = 0 Then
                response = Await httpClient.PostAsync(ApiUrl, content)
            Else
                response = Await httpClient.PutAsync($"{ApiUrl}/{currentComentario.ComentarioId}", content)
            End If

            If response.IsSuccessStatusCode Then
                CargarComentarios()
                LimpiarCampos()
                HabilitarEdicion(False)
            End If

        Catch ex As Exception
            MessageBox.Show($"Error guardando comentario: {ex.Message}")
        End Try
    End Sub

    Private Async Sub EliminarComentario(sender As Object, e As EventArgs)
        If currentComentario Is Nothing OrElse currentComentario.ComentarioId = 0 Then Return

        Dim result = MessageBox.Show("¿Está seguro de eliminar este comentario?", "Confirmar", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            Try
                Dim response = Await httpClient.DeleteAsync($"{ApiUrl}/{currentComentario.ComentarioId}")
                If response.IsSuccessStatusCode Then
                    CargarComentarios()
                    LimpiarCampos()
                End If
            Catch ex As Exception
                MessageBox.Show($"Error eliminando comentario: {ex.Message}")
            End Try
        End If
    End Sub

    Private Sub SeleccionComentario(sender As Object, e As EventArgs)
        If dgvComentarios.SelectedRows.Count > 0 Then
            currentComentario = TryCast(dgvComentarios.SelectedRows(0).DataBoundItem, Comentarios)
            If currentComentario IsNot Nothing Then
                txtContenido.Text = currentComentario.Comentario
                txtUsuarioId.Text = currentComentario.UsuarioId.ToString()
                txtAsignaturaId.Text = currentComentario.AsignaturaId.ToString()
                HabilitarEdicion(True)
            End If
        End If
    End Sub

    Private Sub BuscarComentarios(sender As Object, e As EventArgs)
        If dgvComentarios.DataSource IsNot Nothing Then
            Dim vista As DataView = CType(dgvComentarios.DataSource, DataView)
            vista.RowFilter = $"Contenido LIKE '%{txtBusqueda.Text}%'"
        End If
    End Sub

    Private Sub HabilitarEdicion(habilitar As Boolean)
        btnGuardar.Enabled = habilitar
        btnEliminar.Enabled = habilitar
        txtContenido.Enabled = habilitar
        txtUsuarioId.Enabled = habilitar
        txtAsignaturaId.Enabled = habilitar
    End Sub

    Private Sub LimpiarCampos()
        txtContenido.Clear()
        txtUsuarioId.Clear()
        txtAsignaturaId.Clear()
        currentComentario = Nothing
    End Sub

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(txtContenido.Text) Then
            MessageBox.Show("El contenido es requerido")
            Return False
        End If

        If Not Integer.TryParse(txtUsuarioId.Text, Nothing) Then
            MessageBox.Show("Usuario ID debe ser numérico")
            Return False
        End If

        If Not Integer.TryParse(txtAsignaturaId.Text, Nothing) Then
            MessageBox.Show("Asignatura ID debe ser numérico")
            Return False
        End If

        Return True
    End Function
End Class

