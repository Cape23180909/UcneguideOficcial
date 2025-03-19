Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks
Imports System.ComponentModel
Public Class GestionarAsignaturas
    Inherits Form


    Private ReadOnly apiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly apiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private ReadOnly apiUrlCarreras As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Carreras"

    Private WithEvents dgvAsignaturas As DataGridView
    Private WithEvents txtFiltro As TextBox
    Private WithEvents btnBuscar As Button
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    Private ReadOnly _carreraId As Integer
    Private bindingSource As New BindingSource()
    Public Sub New(carreraId As Integer)
        _carreraId = carreraId
        InitializeComponents()
        Me.Text = $"Asignaturas - CarreraId: {_carreraId}"
    End Sub

    Private Async Sub InitializeComponents()
        Me.WindowState = FormWindowState.Maximized
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.None

        Await CargarSesionUsuario()
        CrearPanelSuperior()

        ' Borde amarillo
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Panel de filtro
        Dim filterPanel As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 40,
            .BackColor = Color.White
        }

        ' Configurar controles de filtro
        txtFiltro = New TextBox With {
            .Dock = DockStyle.Left,
            .Width = 300,
            .Margin = New Padding(10, 5, 5, 5),
            .Font = New Font("Arial", 10)
        }

        btnBuscar = New Button With {
            .Text = "Buscar",
            .Dock = DockStyle.Right,
            .Width = 100,
            .Margin = New Padding(5, 5, 10, 5),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat
        }
        btnBuscar.FlatAppearance.BorderSize = 0

        Dim lblFiltro As New Label With {
            .Text = "Filtrar:",
            .Dock = DockStyle.Left,
            .Width = 60,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Arial", 10, FontStyle.Bold),
            .Margin = New Padding(10, 0, 0, 0)
        }

        filterPanel.Controls.Add(btnBuscar)
        filterPanel.Controls.Add(txtFiltro)
        filterPanel.Controls.Add(lblFiltro)

        ConfigurarDataGridView()

        ' Orden de controles
        Me.Controls.Add(dgvAsignaturas)
        Me.Controls.Add(filterPanel)
        Me.Controls.Add(bottomBorder)
        Me.Controls.Add(topPanel)
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
            .Anchor = AnchorStyles.Left
        }
        topPanel.Controls.Add(iconoPictureBox)


    End Sub




    ' Modificaciones en GestionarAsignaturas
    Private Function ObtenerTextoEncabezado() As String
        Return If(Not String.IsNullOrEmpty(UserSession.nombre),
        $"Usuario: {UserSession.nombre}" & vbCrLf &
        $"Carrera: {UserSession.nombreCarrera}",
        "Carrera no especificada")
    End Function

    Private Sub ConfigurarDataGridView()
        dgvAsignaturas = New DataGridView With {
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .AutoGenerateColumns = False,
            .DataSource = bindingSource,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None
        }

        dgvAsignaturas.Columns.AddRange({
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CodigoAsignatura",
                .HeaderText = "CodigoAsignatura",
                .Width = 120
            },
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "NombreAsignatura",
                .HeaderText = "NombreAsignatura",
                .Width = 250
            },
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "descripcionAsignatura",
                .HeaderText = "descripcionAsignatura",
                .Width = 250
            },
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "NombreDocenteCompleto",
                .HeaderText = "NombreDocenteCompleto",
                .Width = 200
            },
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "nombreCarrera",
                .HeaderText = "nombreCarrera",
                .Width = 200
            }
        })
    End Sub

    ' Método AplicarFiltro optimizado
    Private Sub AplicarFiltro()
        Try
            If Not String.IsNullOrWhiteSpace(txtFiltro.Text) Then
                Dim searchText = txtFiltro.Text.Trim()
                Dim dt = CType(bindingSource.DataSource, DataTable)

                dt.DefaultView.RowFilter = String.Format(
                "CodigoAsignatura LIKE '%{0}%' OR " &
                "NombreAsignatura LIKE '%{0}%' OR " &
                "descripcionAsignatura LIKE '%{0}%'",
                searchText.Replace("'", "''"))
            Else
                bindingSource.Filter = Nothing
            End If
        Catch ex As Exception
            bindingSource.Filter = Nothing
        End Try
    End Sub

    Private Sub txtFiltro_TextChanged(sender As Object, e As EventArgs) Handles txtFiltro.TextChanged
        AplicarFiltro()
    End Sub

    Private Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        AplicarFiltro()
    End Sub

    Private Function ConvertirListaADataTable(asignaturas As List(Of Asignaturas)) As DataTable
        Dim dt As New DataTable()

        dt.Columns.Add("CodigoAsignatura", GetType(String))
        dt.Columns.Add("NombreAsignatura", GetType(String))
        dt.Columns.Add("descripcionAsignatura", GetType(String))
        dt.Columns.Add("NombreDocenteCompleto", GetType(String))
        dt.Columns.Add("nombreCarrera", GetType(String))

        For Each a In asignaturas
            dt.Rows.Add(
            a.codigoAsignatura,
            a.nombreAsignatura,
            a.descripcionAsignatura,
            a.NombreDocenteCompleto,
            a.nombreCarrera
        )
        Next

        Return dt
    End Function
    Private Async Sub GestionarAsignaturas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _carreraId = -1 Then
            MessageBox.Show("No se especificó una carrera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
            Return
        End If
        Await CargarAsignaturas()
    End Sub

    ' Cargar nombre de carrera en la sesión
    Private Async Function CargarSesionUsuario() As Task
        If _carreraId > 0 Then
            UserSession.nombreCarrera = Await ObtenerNombreCarrera(_carreraId)
        End If
    End Function

    ' Obtener el nombre de la carrera desde la API
    Private Async Function ObtenerNombreCarrera(carreraId As Integer) As Task(Of String)
        Try
            Using client As New HttpClient()
                Dim response = Await client.GetStringAsync($"{apiUrlCarreras}/{carreraId}")
                Dim carrera As Carreras = JsonConvert.DeserializeObject(Of Carreras)(response)
                Return If(carrera IsNot Nothing, carrera.nombreCarrera, "Desconocido")
            End Using
        Catch
            Return "Desconocido"
        End Try
    End Function

    ' Cargar asignaturas con docentes y nombres de carrera
    Private Async Function CargarAsignaturas() As Task
        Try
            Using client As New HttpClient()
                Dim urlFiltrada = $"{apiUrlAsignaturas}?carreraId={_carreraId}"
                Dim response As HttpResponseMessage = Await client.GetAsync(urlFiltrada)

                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim asignaturas As List(Of Asignaturas) = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(jsonResponse)

                    ' Asignar nombres de docentes y carreras
                    Await AsignarNombres(asignaturas)

                    ' Actualizar el DataGridView
                    If InvokeRequired Then
                        Invoke(Sub() ActualizarGrid(asignaturas))
                    Else
                        ActualizarGrid(asignaturas)
                    End If
                Else
                    MessageBox.Show($"Error al obtener datos: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error crítico: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    ' Asignar nombres de docentes y carreras a las asignaturas
    Private Async Function AsignarNombres(asignaturas As List(Of Asignaturas)) As Task
        Try
            Using client As New HttpClient()
                ' Obtener lista de docentes
                Dim docentesResponse = Await client.GetStringAsync(apiUrlDocentes)
                Dim docentes As List(Of Docente) = JsonConvert.DeserializeObject(Of List(Of Docente))(docentesResponse)

                ' Obtener lista de carreras
                Dim carrerasResponse = Await client.GetStringAsync(apiUrlCarreras)
                Dim carreras As List(Of Carreras) = JsonConvert.DeserializeObject(Of List(Of Carreras))(carrerasResponse)

                ' Asignar nombres completos a las asignaturas
                For Each asignatura In asignaturas
                    Dim docente = docentes.FirstOrDefault(Function(d) d.docenteId = asignatura.DocenteId)
                    asignatura.NombreDocenteCompleto = If(docente IsNot Nothing, $"{docente.nombre} {docente.apellido}", "Desconocido")

                    ' Asignar nombre de carrera basado en la lista obtenida
                    Dim carrera = carreras.FirstOrDefault(Function(c) c.carreraId = asignatura.CarreraId)
                    asignatura.nombreCarrera = If(carrera IsNot Nothing, carrera.nombreCarrera, "Desconocido")
                Next
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error obteniendo nombres: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function




    ' Modificar el método ActualizarGrid
    Private Sub ActualizarGrid(asignaturas As List(Of Asignaturas))
        Dim dt As DataTable = ConvertirListaADataTable(asignaturas)
        bindingSource.DataSource = dt
    End Sub

End Class
