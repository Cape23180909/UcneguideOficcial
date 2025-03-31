Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Windows.Forms

Public Class EliminarComentarios
    Inherits Form

    ' URLs de la API
    Private ReadOnly ApiBaseUrl As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api"
    Private ReadOnly ApiComentarios As String = $"{ApiBaseUrl}/Comentarios"
    Private ReadOnly ApiAsignaturas As String = $"{ApiBaseUrl}/Asignaturas"
    Private ReadOnly ApiDocentes As String = $"{ApiBaseUrl}/Docentes"

    ' Controles
    Private WithEvents btnConfirmar As Button
    Private panelHeader, panelBody As Panel
    Private iconoAlerta As PictureBox
    Private httpClient As New HttpClient()
    Private comentarioId As Integer
    Private comentarioData As Comentarios
    Private asignaturaData As Asignaturas
    Private docenteData As Docente

    Private topPanel As Panel
    Private iconoPictureBox As PictureBox
    ' Elementos de datos
    Private lblAsignaturaValor As Label
    Private lblDocenteValor As Label
    Private lblFechaValor As Label
    Private txtComentarioContenido As TextBox

    Public Sub New(comentarioId As Integer)
        Me.comentarioId = comentarioId
        InitializeComponents()
        CrearPanelSuperior()
        Me.Size = New Size(750, 650)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
    End Sub

    Private Sub InitializeComponents()
        Me.BackColor = Color.White
        Me.Font = New Font("Segoe UI", 10)
        Me.Padding = New Padding(20)

        ' Panel Body
        panelBody = New Panel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .Padding = New Padding(20)
        }

        CrearContenidoDetallado()
        Me.Controls.Add(panelHeader)
        Me.Controls.Add(panelBody)
    End Sub

    Private Sub CrearPanelSuperior()
        topPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 100,
            .BackColor = ColorTranslator.FromHtml("#074788")
        }

        iconoPictureBox = New PictureBox With {
             .Image = Image.FromFile("C:\Ucneguide\Resources\logo.jpg"),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(80, 80),
            .Location = New Point(20, 10),
            .Cursor = Cursors.Hand
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)
        ' Título centrado
        Dim lblTitle As New Label With {
        .Text = "ELIMINAR COMENTARIO",
        .Font = New Font("Segoe UI", 18, FontStyle.Bold),
        .ForeColor = Color.White,
        .AutoSize = True
    }

        ' Posicionamiento dinámico del título
        AddHandler topPanel.Resize, Sub()
                                        lblTitle.Location = New Point(
                                        (topPanel.Width - lblTitle.Width) \ 2,
                                        (topPanel.Height - lblTitle.Height) \ 2)
                                    End Sub

        topPanel.Controls.Add(lblTitle)
        lblTitle.BringToFront() ' Asegurar que está sobre el borde

        Dim bottomBorder As New Panel With {
            .Dock = DockStyle.Bottom,
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        topPanel.Controls.Add(bottomBorder)

        Me.Controls.Add(topPanel)
    End Sub
    Private Sub CrearContenidoDetallado()
        Dim contenedorPrincipal As New TableLayoutPanel With {
        .Dock = DockStyle.Fill,
        .RowCount = 6, ' Añadimos una fila para espaciado superior
        .ColumnCount = 2,
        .Padding = New Padding(25, 40, 25, 15), ' Más padding superior
        .BackColor = Color.White,
        .Margin = New Padding(0, 50, 0, 0), ' Margen superior para bajar el contenido
        .CellBorderStyle = TableLayoutPanelCellBorderStyle.None
    }

        ' Configuración de columnas
        contenedorPrincipal.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35))
        contenedorPrincipal.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65))

        ' Configuración de filas con espacio superior
        contenedorPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 0))    ' Espaciado superior
        contenedorPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 55))   ' Docente
        contenedorPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 55))   ' Asignatura
        contenedorPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 55))   ' Fecha
        contenedorPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 100))  ' Comentario
        contenedorPrincipal.RowStyles.Add(New RowStyle(SizeType.Absolute, 60))   ' Botón

        ' Etiquetas (posición inicial en fila 1)
        contenedorPrincipal.Controls.Add(CrearEtiquetaTitulo("DOCENTE:"), 0, 1)
        contenedorPrincipal.Controls.Add(CrearEtiquetaTitulo("ASIGNATURA:"), 0, 2)
        contenedorPrincipal.Controls.Add(CrearEtiquetaTitulo("FECHA:"), 0, 3)
        contenedorPrincipal.Controls.Add(CrearEtiquetaTitulo("COMENTARIO:"), 0, 4)

        ' Valores
        lblDocenteValor = CrearEtiquetaValor()
        lblAsignaturaValor = CrearEtiquetaValor()
        lblFechaValor = CrearEtiquetaValor()

        ' Cuadro de comentario
        txtComentarioContenido = New TextBox With {
        .Multiline = True,
        .ReadOnly = True,
        .ScrollBars = ScrollBars.Vertical,
        .Font = New Font("Segoe UI", 9.5),
        .BackColor = Color.FromArgb(245, 245, 245),
        .Dock = DockStyle.Fill,
        .Margin = New Padding(5),
        .BorderStyle = BorderStyle.None,
        .Height = 80
    }

        ' Posicionamiento de controles
        contenedorPrincipal.Controls.Add(lblDocenteValor, 1, 1)
        contenedorPrincipal.Controls.Add(lblAsignaturaValor, 1, 2)
        contenedorPrincipal.Controls.Add(lblFechaValor, 1, 3)
        contenedorPrincipal.Controls.Add(txtComentarioContenido, 1, 4)

        ' Botón compacto
        btnConfirmar = New Button With {
        .Text = "CONFIRMAR ELIMINACIÓN",
        .Size = New Size(180, 35),
        .BackColor = Color.FromArgb(220, 53, 69),
        .ForeColor = Color.White,
        .FlatStyle = FlatStyle.Flat,
        .Font = New Font("Arial", 9, FontStyle.Bold),
        .Cursor = Cursors.Hand,
        .Anchor = AnchorStyles.None
    }
        btnConfirmar.FlatAppearance.BorderSize = 0

        Dim btnContainer As New Panel With {
        .Dock = DockStyle.Fill,
        .Padding = New Padding(0, 10, 0, 0)
    }
        btnContainer.Controls.Add(btnConfirmar)
        btnConfirmar.Location = New Point((btnContainer.Width - btnConfirmar.Width) \ 2, 0)

        contenedorPrincipal.SetColumnSpan(btnContainer, 2)
        contenedorPrincipal.Controls.Add(btnContainer, 0, 5)

        panelBody.Controls.Add(contenedorPrincipal)
    End Sub
    Private Function CrearEtiquetaTitulo(texto As String) As Label
        Return New Label With {
        .Text = texto,
        .Font = New Font("Arial", 11, FontStyle.Bold),
       .ForeColor = ColorTranslator.FromHtml("#074788"), ' Color azul corporativo
        .TextAlign = ContentAlignment.MiddleRight,
        .Dock = DockStyle.Fill,
        .Margin = New Padding(0, 0, 15, 0)
    }
    End Function

    Private Function CrearEtiquetaValor() As Label
        Return New Label With {
        .Font = New Font("Segoe UI", 11),
        .ForeColor = Color.Black,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Dock = DockStyle.Fill,
        .Margin = New Padding(15, 0, 0, 0)
    }
    End Function

    Private Async Sub EliminarComentarios_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await CargarDatosComentario()
        VerificarPermisos()
        MostrarDatos()
    End Sub

    Private Async Function CargarDatosComentario() As Task
        Try
            Dim response = Await httpClient.GetAsync($"{ApiComentarios}/{comentarioId}")
            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                comentarioData = JsonConvert.DeserializeObject(Of Comentarios)(json)
            End If

            If comentarioData IsNot Nothing Then
                Dim t1 = httpClient.GetAsync($"{ApiAsignaturas}/{comentarioData.AsignaturaId}")
                Dim t2 = httpClient.GetAsync($"{ApiDocentes}/{comentarioData.DocenteId}")
                Await Task.WhenAll(t1, t2)

                If t1.Result.IsSuccessStatusCode Then
                    asignaturaData = JsonConvert.DeserializeObject(Of Asignaturas)(
                        Await t1.Result.Content.ReadAsStringAsync())
                End If

                If t2.Result.IsSuccessStatusCode Then
                    docenteData = JsonConvert.DeserializeObject(Of Docente)(
                        Await t2.Result.Content.ReadAsStringAsync())
                End If
            End If

        Catch ex As Exception
            MessageBox.Show($"Error cargando datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End Try
    End Function

    Private Sub VerificarPermisos()
        If UserSession.usuarioId <> comentarioData.UsuarioId Then
            MessageBox.Show("No tienes permiso para eliminar este comentario", "Acceso Denegado",
                          MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Me.Close()
        End If
    End Sub

    Private Sub MostrarDatos()
        ' Verificar si los objetos están inicializados antes de usarlos
        lblAsignaturaValor.Text = If(asignaturaData?.nombreAsignatura, "N/A")

        ' Usar operador null-conditional en ambas propiedades
        Dim nombreDocente As String = If(docenteData?.nombre, "")
        Dim apellidoDocente As String = If(docenteData?.apellido, "")
        lblDocenteValor.Text = If($"{nombreDocente} {apellidoDocente}".Trim(), "N/A")

        ' Asegurar que comentarioData no sea Nothing y manejar FechaComentario
        Dim fecha As String = If(comentarioData?.FechaComentario.ToString("dd/MM/yyyy HH:mm"), "N/A")
        lblFechaValor.Text = fecha

        txtComentarioContenido.Text = If(comentarioData?.Comentario, "N/A")
    End Sub

    Private Async Sub btnConfirmar_Click(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Try
            Dim response = Await httpClient.DeleteAsync($"{ApiComentarios}/{comentarioId}")

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Comentario eliminado correctamente", "Éxito",
                              MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                Dim errorContent = Await response.Content.ReadAsStringAsync()
                MessageBox.Show($"Error al eliminar: {response.StatusCode}{Environment.NewLine}{errorContent}",
                              "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class