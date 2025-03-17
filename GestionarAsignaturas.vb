Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks

Public Class GestionarAsignaturas
    Inherits Form

    Private ReadOnly apiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private WithEvents dgvAsignaturas As DataGridView
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    Private ReadOnly _carreraId As Integer

    ' Constructor para el diseñador
    Public Sub New()
        Me.New(-1)
    End Sub

    ' Constructor principal
    Public Sub New(carreraId As Integer)
        _carreraId = carreraId
        InitializeComponents()
        Me.Text = $"Asignaturas - CarreraId: {_carreraId}"
    End Sub

    Private Sub InitializeComponents()
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' 1. Inicializar controles PRIMERO
        dgvAsignaturas = New DataGridView()
        topPanel = New Panel()
        iconoPictureBox = New PictureBox()

        ' 2. Configurar panel superior
        topPanel.Dock = DockStyle.Top
        topPanel.Height = 180
        topPanel.BackColor = ColorTranslator.FromHtml("#074788")

        ' 3. Cabecera informativa (verificar sesión)
        Dim headerText = If(UserSession.NombreUsuario IsNot Nothing,
                          $"Bienvenido: {UserSession.NombreUsuario}" & vbCrLf &
                          $"Carrera: {UserSession.CarreraId}",
                          "Carrera no especificada")

        Dim lblHeader = New Label With {
            .Text = headerText,
            .Font = New Font("Arial", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .Dock = DockStyle.Bottom,
            .Height = 60,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        topPanel.Controls.Add(lblHeader)

        ' 4. Configurar DataGridView
        dgvAsignaturas.Dock = DockStyle.Fill
        dgvAsignaturas.AllowUserToAddRows = False
        dgvAsignaturas.AllowUserToDeleteRows = False
        dgvAsignaturas.ReadOnly = True
        dgvAsignaturas.AutoGenerateColumns = False

        ' 5. Añadir columnas
        dgvAsignaturas.Columns.AddRange({
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "asignaturaId", .HeaderText = "asignaturaId", .Width = 50},
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CodigoAsignatura", .HeaderText = "CodigoAsignatura", .Width = 120},
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "NombreAsignatura", .HeaderText = "NombreAsignatura", .Width = 250},
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "DocenteId", .HeaderText = "DocenteId", .Width = 100},
            New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CarreraId", .HeaderText = "CarreraId", .Width = 100}
        })

        ' 6. Añadir controles al formulario EN ORDEN CORRECTO
        Me.Controls.Add(dgvAsignaturas)
        Me.Controls.Add(topPanel)
    End Sub

    Private Async Sub GestionarAsignaturas_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If _carreraId = -1 Then
            MessageBox.Show("No se especificó una carrera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
            Return
        End If

        Await CargarAsignaturas()
    End Sub

    Private Async Function CargarAsignaturas() As Task
        Try
            Using client As New HttpClient()
                Dim urlFiltrada = $"{apiUrlAsignaturas}?carreraId={_carreraId}"
                Dim response As HttpResponseMessage = Await client.GetAsync(urlFiltrada)

                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim asignaturas As List(Of Asignaturas) = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(jsonResponse)

                    If InvokeRequired Then
                        Invoke(Sub()
                                   ActualizarGrid(asignaturas)
                               End Sub)
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

    Private Sub ActualizarGrid(asignaturas As List(Of Asignaturas))
        If asignaturas?.Any() Then
            dgvAsignaturas.DataSource = asignaturas
        Else
            MessageBox.Show("No hay asignaturas disponibles", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
        End If
    End Sub
End Class