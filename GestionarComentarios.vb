Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text

Public Class GestionarComentarios
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private httpClient As New HttpClient()
    Private currentComentario As Comentarios
    Private asignaturas As List(Of Asignaturas)
    Private docentes As List(Of Docente)


    ' Controles del formulario
    Private dgvComentarios As DataGridView
    Private txtBusqueda As TextBox
    Private btnNuevo As Button
    Private btnGuardar As Button
    Private btnEliminar As Button
    Private txtContenido As TextBox
    Private cmbAsignaturas As ComboBox
    Private cmbDocentes As ComboBox

    Private Sub GestionarComentarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        InitializeComponents()
        CargarDatosIniciales()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Gestión de Comentarios"
        Me.Size = New Size(1000, 600)
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
            .Size = New Size(300, 20),
            .Multiline = True
        }

        cmbAsignaturas = New ComboBox With {
            .Location = New Point(10, 40),
            .Size = New Size(200, 20),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        cmbDocentes = New ComboBox With {
            .Location = New Point(220, 40),
            .Size = New Size(200, 20),
            .DropDownStyle = ComboBoxStyle.DropDownList
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
            txtContenido, cmbAsignaturas, cmbDocentes,
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
    Private Sub NuevoComentario(sender As Object, e As EventArgs)

        LimpiarCampos()
        HabilitarEdicion(True)
    End Sub
    Private Async Sub CargarDatosIniciales()
        Try
            ' Cargar asignaturas
            Dim responseAsignaturas = Await httpClient.GetAsync(ApiUrlAsignaturas)
            If responseAsignaturas.IsSuccessStatusCode Then
                Dim jsonAsignaturas = Await responseAsignaturas.Content.ReadAsStringAsync()
                asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(jsonAsignaturas)
                cmbAsignaturas.DataSource = asignaturas
                cmbAsignaturas.DisplayMember = "NombreAsignatura"
                cmbAsignaturas.ValueMember = "AsignaturaId"
            End If

            ' Cargar docentes
            Dim responseDocentes = Await httpClient.GetAsync(ApiUrlDocentes)
            If responseDocentes.IsSuccessStatusCode Then
                Dim jsonDocentes = Await responseDocentes.Content.ReadAsStringAsync()
                docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(jsonDocentes)
                cmbDocentes.DataSource = docentes
                cmbDocentes.DisplayMember = "NombreDocente"
                cmbDocentes.ValueMember = "DocenteId"
            End If

            ' Cargar comentarios
            Await CargarComentarios()
        Catch ex As Exception
            MessageBox.Show($"Error cargando datos iniciales: {ex.Message}")
        End Try
    End Sub


    Private Async Function CargarComentarios() As Task
        Try
            Dim response = Await httpClient.GetAsync(ApiUrlComentarios)
            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                Dim comentarios = JsonConvert.DeserializeObject(Of List(Of Comentarios))(json)

                ' Obtener nombres de asignaturas y docentes
                For Each comentario In comentarios
                    Dim asignatura = asignaturas.FirstOrDefault(Function(a) a.AsignaturaId = comentario.AsignaturaId)
                    Dim docente = docentes.FirstOrDefault(Function(d) d.docenteId = comentario.DocenteId)
                    comentario.NombreAsignatura = If(asignatura IsNot Nothing, asignatura.NombreAsignatura, "N/A")
                    comentario.NombreDocente = If(docente IsNot Nothing, docente.nombre, "N/A")
                Next

                dgvComentarios.DataSource = Nothing
                dgvComentarios.DataSource = comentarios
                ConfigurarColumnas()
            End If
        Catch ex As Exception
            MessageBox.Show($"Error cargando comentarios: {ex.Message}")
        End Try
    End Function
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
            .DataPropertyName = "NombreAsignatura",
            .HeaderText = "NombreAsignatura",
            .Width = 150
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "NombreDocente",
            .HeaderText = "NombreDocente",
            .Width = 150
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "FechaComentario",
            .HeaderText = "FechaComentario",
            .Width = 100
        })
    End Sub





    Private Async Sub GuardarComentario(sender As Object, e As EventArgs)
        Try
            If Not ValidarCampos() Then Return

            ' Asignar valores desde los controles
            currentComentario.Comentario = txtContenido.Text
            currentComentario.AsignaturaId = CInt(cmbAsignaturas.SelectedValue)
            currentComentario.DocenteId = CInt(cmbDocentes.SelectedValue)

            ' Configurar el JSON correctamente
            Dim json = JsonConvert.SerializeObject(currentComentario, Formatting.None, New JsonSerializerSettings With {
            .NullValueHandling = NullValueHandling.Ignore
        })

            Dim content = New StringContent(json, Encoding.UTF8, "application/json")
            Dim response As HttpResponseMessage

            If currentComentario.ComentarioId = 0 Then
                response = Await httpClient.PostAsync(ApiUrlComentarios, content)
            Else
                response = Await httpClient.PutAsync($"{ ApiUrlComentarios}/{currentComentario.ComentarioId}", content)
            End If

            If response.IsSuccessStatusCode Then
                Await CargarComentarios()
                LimpiarCampos()
                HabilitarEdicion(False)
            Else
                Dim errorContent = Await response.Content.ReadAsStringAsync()
                MessageBox.Show($"Error del API: {errorContent}")
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
                Dim response = Await httpClient.DeleteAsync($"{ ApiUrlComentarios}/{currentComentario.ComentarioId}")
                If response.IsSuccessStatusCode Then
                    Await CargarComentarios()
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
                cmbAsignaturas.SelectedValue = currentComentario.AsignaturaId
                cmbDocentes.SelectedValue = currentComentario.DocenteId
                HabilitarEdicion(True)
            End If
        End If
    End Sub

    Private Sub BuscarComentarios(sender As Object, e As EventArgs)
        If dgvComentarios.DataSource IsNot Nothing Then
            Dim vista As DataView = CType(dgvComentarios.DataSource, DataView)
            vista.RowFilter = $"Comentario LIKE '%{txtBusqueda.Text}%'"
        End If
    End Sub

    Private Sub HabilitarEdicion(habilitar As Boolean)
        btnGuardar.Enabled = habilitar
        btnEliminar.Enabled = habilitar
        txtContenido.Enabled = habilitar
        cmbAsignaturas.Enabled = habilitar
        cmbDocentes.Enabled = habilitar
    End Sub

    Private Sub LimpiarCampos()
        txtContenido.Clear()
        cmbAsignaturas.SelectedIndex = -1
        cmbDocentes.SelectedIndex = -1
        currentComentario = Nothing
    End Sub

    Private Function ValidarCampos() As Boolean
        If String.IsNullOrWhiteSpace(txtContenido.Text) Then
            MessageBox.Show("El contenido es requerido")
            Return False
        End If

        If cmbAsignaturas.SelectedIndex = -1 Then
            MessageBox.Show("Debe seleccionar una asignatura")
            Return False
        End If

        If cmbDocentes.SelectedIndex = -1 Then
            MessageBox.Show("Debe seleccionar un docente")
            Return False
        End If

        Return True
    End Function
End Class



