Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks

Public Class GestionarFacultades
    Inherits Form

    Private ReadOnly apiUrlFacultades As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Facultades"
    Private WithEvents dgvFacultades As DataGridView
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox

    Public Sub New()
        InitializeComponents()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Gestión de Facultades"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Panel superior con color #074788
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 180,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Fondo inferior cambiado al mismo azul del panel superior
        Dim bottomPanel As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 50,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Icono alineado a la izquierda en el panel azul
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(100, 100),
            .Location = New Point(20, 40), ' Alineado a la izquierda
            .Anchor = AnchorStyles.Left Or AnchorStyles.Top
        }
        topPanel.Controls.Add(iconoPictureBox)

        ' DataGridView
        dgvFacultades = New DataGridView With {
            .Dock = DockStyle.Fill,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ReadOnly = True,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
        }

        ' Agregar controles al formulario
        Me.Controls.Add(dgvFacultades)
        Me.Controls.Add(bottomPanel)
        Me.Controls.Add(topPanel)

        AddHandler Me.Load, AddressOf GestionarFacultades_Load
    End Sub

    Private Async Sub GestionarFacultades_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await CargarFacultades()
    End Sub

    Private Async Function CargarFacultades() As Task
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrlFacultades)
                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim facultades As List(Of Facultades) = JsonConvert.DeserializeObject(Of List(Of Facultades))(jsonResponse)

                    dgvFacultades.Invoke(Sub()
                                             dgvFacultades.DataSource = facultades
                                             dgvFacultades.Columns("facultadId").HeaderText = "ID"
                                             dgvFacultades.Columns("nombreFacultad").HeaderText = "Nombre de Facultad"
                                             dgvFacultades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                                         End Sub)
                Else
                    MessageBox.Show("Error al obtener facultades.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error de conexión: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function
End Class
