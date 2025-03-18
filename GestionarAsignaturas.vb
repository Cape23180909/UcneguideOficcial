Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks

Public Class GestionarAsignaturas
    Inherits Form


    Private ReadOnly apiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly apiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private ReadOnly apiUrlCarreras As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Carreras"

    Private WithEvents dgvAsignaturas As DataGridView
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    Private ReadOnly _carreraId As Integer

    Public Sub New(carreraId As Integer)
        _carreraId = carreraId
        InitializeComponents()
        Me.Text = $"Asignaturas - CarreraId: {_carreraId}"
    End Sub

    Private Async Sub InitializeComponents()
        ' Configuración principal del formulario
        Me.WindowState = FormWindowState.Maximized
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.None

        ' Cargar datos iniciales
        Await CargarSesionUsuario()

        ' Panel superior
        CrearPanelSuperior()

        ' Borde amarillo
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Configurar DataGridView
        ConfigurarDataGridView()

        ' Agregar controles al formulario
        Me.Controls.Add(dgvAsignaturas)
        Me.Controls.Add(bottomBorder)
        Me.Controls.Add(topPanel)
    End Sub

    Private Sub CrearPanelSuperior()
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Icono
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3, ' Asegurar tener este recurso
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(80, 80),
            .Location = New Point(20, 10),
            .Anchor = AnchorStyles.Left
        }
        topPanel.Controls.Add(iconoPictureBox)

        ' Texto informativo
        Dim lblHeader = New Label With {
            .Text = ObtenerTextoEncabezado(),
            .Font = New Font("Arial", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .Size = New Size(600, 60),
            .Location = New Point(120, 20),
            .TextAlign = ContentAlignment.MiddleLeft
        }
        topPanel.Controls.Add(lblHeader)
    End Sub

    Private Function ObtenerTextoEncabezado() As String
        Return If(UserSession.Nombre IsNot Nothing,
            $"Usuario: {UserSession.Nombre}" & vbCrLf &
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
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None
        }

        ' Columnas
        dgvAsignaturas.Columns.AddRange({
            New DataGridViewTextBoxColumn With {.DataPropertyName = "AsignaturaId", .HeaderText = "AsignaturaId", .Width = 50},
            New DataGridViewTextBoxColumn With {.DataPropertyName = "CodigoAsignatura", .HeaderText = "CodigoAsignatura", .Width = 120},
            New DataGridViewTextBoxColumn With {.DataPropertyName = "NombreAsignatura", .HeaderText = "NombreAsignatura", .Width = 250},
            New DataGridViewTextBoxColumn With {.DataPropertyName = "DescripcionAsignatura", .HeaderText = "DescripcionAsignatura", .Width = 250},
            New DataGridViewTextBoxColumn With {.DataPropertyName = "NombreDocenteCompleto", .HeaderText = "NombreDocenteCompleto", .Width = 200},
            New DataGridViewTextBoxColumn With {.DataPropertyName = "NombreCarrera", .HeaderText = "NombreCarrera", .Width = 200}
        })
    End Sub

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

    Private Sub ActualizarGrid(asignaturas As List(Of Asignaturas))
        dgvAsignaturas.DataSource = asignaturas
    End Sub
End Class
