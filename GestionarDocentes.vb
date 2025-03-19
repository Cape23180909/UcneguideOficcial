Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks

Public Class GestionarDocentes
    Inherits Form

    Private ReadOnly apiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private WithEvents dgvDocentes As DataGridView
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox


    Private WithEvents txtFiltro As TextBox
    Private WithEvents btnFiltrar As Button
    Private _docentes As List(Of Docente)

    Public Sub New()
        InitializeComponents()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Gestión de Docentes"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Panel superior con color azul
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Línea amarilla debajo del panel azul
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Top,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Icono PictureBox
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(90, 90),
            .Location = New Point(25, 15),
            .Anchor = AnchorStyles.Left,
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        ' Configuración del TextBox de filtro (corregido)
        txtFiltro = New TextBox With {
            .Size = New Size(200, 30),
            .Location = New Point(iconoPictureBox.Right + 20, 40),
            .Anchor = AnchorStyles.Left,
            .Text = "Filtrar por nombre..." ' Simulamos placeholder
        }

        ' Agregamos manejo de eventos para el placeholder
        AddHandler txtFiltro.GotFocus, AddressOf RemovePlaceholder
        AddHandler txtFiltro.LostFocus, AddressOf SetPlaceholder

        btnFiltrar = New Button With {
            .Text = "Filtrar",
            .Size = New Size(80, 30),
            .Location = New Point(txtFiltro.Right + 10, 40),
            .Anchor = AnchorStyles.Left
        }

        topPanel.Controls.Add(txtFiltro)
        topPanel.Controls.Add(btnFiltrar)

        ' DataGridView para docentes
        dgvDocentes = New DataGridView With {
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        ' Agregar controles al formulario
        Me.Controls.Add(dgvDocentes)
        Me.Controls.Add(bottomBorder)
        Me.Controls.Add(topPanel)

        AddHandler Me.Load, AddressOf GestionarDocentes_Load
        AddHandler btnFiltrar.Click, AddressOf FiltrarDocentes
    End Sub

    Private Sub RemovePlaceholder(sender As Object, e As EventArgs)
        If txtFiltro.Text = "Filtrar por nombre..." Then
            txtFiltro.Text = ""
            txtFiltro.ForeColor = SystemColors.WindowText
        End If
    End Sub

    Private Sub SetPlaceholder(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtFiltro.Text) Then
            txtFiltro.Text = "Filtrar por nombre..."
            txtFiltro.ForeColor = SystemColors.GrayText
        End If
    End Sub

    Private Async Sub GestionarDocentes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await CargarDocentes()
    End Sub
    Private Sub FiltrarDocentes(sender As Object, e As EventArgs)
        If _docentes Is Nothing Then Exit Sub

        Dim filtro As String = If(txtFiltro.Text = "Filtrar por nombre...", "", txtFiltro.Text.Trim().ToLower())

        If String.IsNullOrEmpty(filtro) Then
            dgvDocentes.DataSource = _docentes
        Else
            Dim filtered = _docentes.Where(Function(d)
                                               Dim nombre = If(d.nombre IsNot Nothing, d.nombre.ToLower(), "")
                                               Dim apellido = If(d.apellido IsNot Nothing, d.apellido.ToLower(), "")
                                               Return nombre.Contains(filtro) OrElse apellido.Contains(filtro)
                                           End Function).ToList()
            dgvDocentes.DataSource = filtered
        End If
    End Sub

    Private Async Function CargarDocentes() As Task
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrlDocentes)
                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    _docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(jsonResponse)

                    dgvDocentes.Invoke(Sub()
                                           dgvDocentes.DataSource = _docentes
                                           dgvDocentes.Columns("docenteId").HeaderText = "DocenteId"
                                           dgvDocentes.Columns("nombre").HeaderText = "Nombre"
                                           dgvDocentes.Columns("apellido").HeaderText = "Apellido"
                                           dgvDocentes.Columns("rol").HeaderText = "Rol"
                                           dgvDocentes.Columns("asignaturaId").HeaderText = "AsignaturaId"
                                           dgvDocentes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                                       End Sub)
                Else
                    MessageBox.Show("Error al obtener docentes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error de conexión: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function



End Class
