Imports Newtonsoft.Json
Imports System.Net.Http
Imports Newtonsoft.Json.Serialization

Public Class GestionarDocentes
    Inherits Form

    Private ReadOnly apiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"
    Private WithEvents flowPanel As FlowLayoutPanel
    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    Private WithEvents txtFiltro As TextBox
    Private _docentes As List(Of Docente)
    Private filteredDocentes As List(Of Docente)

    Public Sub New()
        InitializeComponents()
    End Sub

    Private Sub InitializeComponents()
        Me.Text = "Gestión de Docentes"
        Me.Size = New Size(800, 600)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Panel superior
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 120,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        ' Título centrado
        Dim lblTitle As New Label With {
            .Text = "CONTROL DOCENTES",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.White,
            .AutoSize = True
        }

        ' Línea amarilla
        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }

        ' Icono
        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(90, 90),
            .Location = New Point(25, 15),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()

        ' Barra de búsqueda
        txtFiltro = New TextBox With {
            .Text = "Buscar maestros...",
            .ForeColor = Color.Gray,
            .Size = New Size(250, 30)
        }

        ' Configurar posición dinámica
        AddHandler topPanel.Resize, Sub()
                                        txtFiltro.Location = New Point(
                                            iconoPictureBox.Right + 10,
                                            (topPanel.Height - txtFiltro.Height) \ 2
                                        )
                                        lblTitle.Location = New Point(
                                            (topPanel.Width - lblTitle.Width) \ 2,
                                            (topPanel.Height - lblTitle.Height) \ 2
                                        )
                                    End Sub

        ' Placeholder
        AddHandler txtFiltro.GotFocus, Sub(s, e)
                                           If txtFiltro.Text = "Buscar maestros..." Then
                                               txtFiltro.Text = ""
                                               txtFiltro.ForeColor = Color.Black
                                           End If
                                       End Sub

        AddHandler txtFiltro.LostFocus, Sub(s, e)
                                            If String.IsNullOrWhiteSpace(txtFiltro.Text) Then
                                                txtFiltro.Text = "Buscar maestros..."
                                                txtFiltro.ForeColor = Color.Gray
                                            End If
                                        End Sub

        AddHandler txtFiltro.TextChanged, AddressOf FiltrarDocentes

        ' Agregar controles
        topPanel.Controls.Add(iconoPictureBox)
        topPanel.Controls.Add(lblTitle)
        topPanel.Controls.Add(txtFiltro)
        topPanel.Controls.Add(bottomBorder)

        ' Contenedor principal
        flowPanel = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .Padding = New Padding(20)
        }

        Me.Controls.Add(flowPanel)
        Me.Controls.Add(topPanel)

        AddHandler Me.Load, AddressOf GestionarDocentes_Load
    End Sub

    Private Async Sub GestionarDocentes_Load(sender As Object, e As EventArgs)
        Await CargarDocentes()
    End Sub

    Private Async Function CargarDocentes() As Task
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(apiUrlDocentes)

                If response.IsSuccessStatusCode Then
                    Dim jsonResponse As String = Await response.Content.ReadAsStringAsync()
                    Dim settings = New JsonSerializerSettings With {
                        .ContractResolver = New CamelCasePropertyNamesContractResolver()
                    }
                    _docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(jsonResponse, settings)
                    filteredDocentes = _docentes.ToList() ' <- Carga completa
                    RenderizarDocentes()
                Else
                    MessageBox.Show($"Error HTTP: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Function

    Private Sub RenderizarDocentes()
        flowPanel.SuspendLayout()
        flowPanel.Controls.Clear()

        If filteredDocentes Is Nothing OrElse filteredDocentes.Count = 0 Then
            Dim lblMensaje As New Label With {
                .Text = "No hay docentes registrados.",
                .Font = New Font("Arial", 12),
                .ForeColor = Color.Gray,
                .TextAlign = ContentAlignment.MiddleCenter,
                .Dock = DockStyle.Fill
            }
            flowPanel.Controls.Add(lblMensaje)
        Else
            For Each docente In filteredDocentes
                Dim card As New Panel With {
                    .Size = New Size(300, 100),
                    .BackColor = Color.White,
                    .Margin = New Padding(10),
                    .BorderStyle = BorderStyle.FixedSingle
                }

                Dim infoPanel As New Panel With {
                    .Dock = DockStyle.Fill,
                    .Padding = New Padding(10)
                }

                Dim nameLabel As New Label With {
                    .Text = $"{docente.nombre} {docente.apellido}",
                    .Font = New Font("Arial", 12, FontStyle.Bold),
                    .AutoSize = True
                }

                Dim detailsLabel As New Label With {
                    .Text = $"{docente.rol}",
                    .Top = 25,
                    .AutoSize = True,
                    .ForeColor = Color.DimGray
                }

                infoPanel.Controls.Add(nameLabel)
                infoPanel.Controls.Add(detailsLabel)
                card.Controls.Add(infoPanel)
                flowPanel.Controls.Add(card)
            Next
        End If

        flowPanel.ResumeLayout(True)
    End Sub

    Private Sub FiltrarDocentes(sender As Object, e As EventArgs)
        If _docentes Is Nothing Then Return

        Dim searchTerm = If(txtFiltro.Text?.Trim().ToLower(), "")
        If searchTerm = "buscar maestros..." Then searchTerm = ""

        filteredDocentes = _docentes.
            Where(Function(d) d IsNot Nothing AndAlso
                (d.nombre?.ToLower().Contains(searchTerm) OrElse
                 d.apellido?.ToLower().Contains(searchTerm))).
            ToList()

        RenderizarDocentes()
    End Sub
End Class

