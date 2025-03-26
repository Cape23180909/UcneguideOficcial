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

        btnCrear = New Button With {
            .Text = "Crear Nuevo",
            .Size = New Size(150, 40),
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Arial", 10, FontStyle.Bold),
            .Anchor = AnchorStyles.Right,
            .Cursor = Cursors.Hand
        }
        btnCrear.FlatAppearance.BorderSize = 0

        AddHandler btnCrear.Click, AddressOf AbrirGenerarComentarios
        btnCrear.Location = New Point(footerPanel.Width - btnCrear.Width - 20, 10)

        AddHandler footerPanel.Resize, Sub(s, e)
                                           btnCrear.Left = footerPanel.Width - btnCrear.Width - 20
                                       End Sub

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

    Private Async Function CargarComentarios() As Task
        Try
            Dim response = Await httpClient.GetAsync(ApiUrlComentarios)
            If response.IsSuccessStatusCode Then
                Dim comentarios = JsonConvert.DeserializeObject(Of List(Of Comentarios))(
                    Await response.Content.ReadAsStringAsync())

                For Each c In comentarios
                    c.NombreAsignatura = If(asignaturas?.Any(Function(a) a.AsignaturaId = c.AsignaturaId),
                        asignaturas.First(Function(a) a.AsignaturaId = c.AsignaturaId).NombreAsignatura,
                        "N/A")

                    c.NombreDocenteCompleto = If(docentes?.Any(Function(d) d.docenteId = c.DocenteId),
                        docentes.First(Function(d) d.docenteId = c.DocenteId).nombre,
                        "N/A")
                Next

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

        Dim columnStyle = New DataGridViewCellStyle With {
            .Padding = New Padding(5),
            .Alignment = DataGridViewContentAlignment.MiddleLeft
        }

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Comentario",
            .HeaderText = "COMENTARIO",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            .DefaultCellStyle = columnStyle
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "NombreAsignatura",
            .HeaderText = "NombreAsignatura",
            .Width = 200,
            .DefaultCellStyle = columnStyle
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "NombreDocenteCompleto",
            .HeaderText = "NombreDocenteCompleto",
            .Width = 200,
            .DefaultCellStyle = columnStyle
        })

        dgvComentarios.Columns.Add(New DataGridViewTextBoxColumn With {
            .DataPropertyName = "FechaComentario",
            .HeaderText = "FechaComentario",
            .Width = 150,
            .DefaultCellStyle = New DataGridViewCellStyle With {
                .Format = "dd/MM/yyyy HH:mm",
                .Alignment = DataGridViewContentAlignment.MiddleCenter
            }
        })

        dgvComentarios.RowTemplate.Height = 35
        dgvComentarios.ColumnHeadersHeight = 40
    End Sub

    Private Sub AbrirGenerarComentarios(sender As Object, e As EventArgs)
        Dim formGenerar As New GenerarComentarios()
        formGenerar.ShowDialog()
    End Sub

End Class