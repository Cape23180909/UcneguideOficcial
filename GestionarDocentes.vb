Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks

Public Class GestionarDocentes
    Inherits Form

    Private ReadOnly apiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private WithEvents dgvDocentes As DataGridView
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

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

        ' Icono en el panel superior
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(80, 80),
            .Location = New Point(20, 10),
            .Anchor = AnchorStyles.Left
        }
        topPanel.Controls.Add(iconoPictureBox)

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
    End Sub

    Private Async Sub GestionarDocentes_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await CargarDocentes()
    End Sub

    Private Async Function CargarDocentes() As Task
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrlDocentes)
                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim docentes As List(Of Docente) = JsonConvert.DeserializeObject(Of List(Of Docente))(jsonResponse)

                    dgvDocentes.Invoke(Sub()
                                           dgvDocentes.DataSource = docentes
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
