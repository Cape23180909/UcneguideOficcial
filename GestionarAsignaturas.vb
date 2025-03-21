Imports System.Net.Http
Imports Newtonsoft.Json
Imports System.Threading.Tasks
Imports System.ComponentModel
Imports System.Security
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
        .Height = 120,
        .BackColor = ColorTranslator.FromHtml("#074788")
}

        iconoPictureBox = New PictureBox With {
            .Image = My.Resources.guia_turistico_3,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Size = New Size(90, 90),
            .Location = New Point(25, 15),
            .Anchor = AnchorStyles.Left,
            .Cursor = Cursors.Hand ' Cambia el cursor al pasar sobre el icono
        }
        AddHandler iconoPictureBox.Click, Sub(sender, e) Me.Close()
        topPanel.Controls.Add(iconoPictureBox)

        ' Panel de información con sombra
        Dim infoPanel As New Panel With {
        .Dock = DockStyle.Right,
        .Width = 300,
        .BackColor = ColorTranslator.FromHtml("#0A5AA8"),
        .Padding = New Padding(15, 10, 15, 10)
    }

        ' Contenedor principal
        Dim mainContainer As New TableLayoutPanel With {
        .Dock = DockStyle.Fill,
        .ColumnCount = 2,
        .RowCount = 2,
        .AutoSize = True
    }

        ' Icono de carrera
        Dim iconoCarrera As New PictureBox With {
        .Image = SystemIcons.Shield.ToBitmap(),
        .SizeMode = PictureBoxSizeMode.Zoom,
        .Size = New Size(32, 32),
        .Dock = DockStyle.Left,
        .Margin = New Padding(0, 0, 10, 0)
    }

        ' Etiqueta modificada para mostrar "Usuario: NOMBRE"
        Dim lblUsuario As New Label With {
        .Text = "Usuario: " & UserSession.nombre.ToUpper(), ' Texto modificado aquí
        .Font = New Font("Segoe UI", 12, FontStyle.Bold),
        .ForeColor = Color.White,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Dock = DockStyle.Fill,
        .AutoSize = True
    }

        Dim lblCarrera As New Label With {
        .Text = "nombreCarrera: " & UserSession.nombreCarrera.ToUpper(), ' Texto modificado aquí
        .Font = New Font("Segoe UI", 11, FontStyle.Bold),
        .ForeColor = Color.White,
        .TextAlign = ContentAlignment.MiddleLeft,
        .Dock = DockStyle.Fill,
        .AutoSize = True
    }

        ' Configurar layout
        mainContainer.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 40))
        mainContainer.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))

        mainContainer.Controls.Add(lblUsuario, 1, 0)
        mainContainer.Controls.Add(iconoCarrera, 0, 1)
        mainContainer.Controls.Add(lblCarrera, 1, 1)

        ' Separador visual
        Dim separador As New Panel With {
        .Height = 1,
        .Dock = DockStyle.Bottom,
        .BackColor = Color.FromArgb(50, 255, 255, 255)
    }

        infoPanel.Controls.Add(mainContainer)
        infoPanel.Controls.Add(separador)
        topPanel.Controls.Add(infoPanel)

        ' Efecto de sombra
        AddShadowEffect(infoPanel)
    End Sub

    Private Sub AddShadowEffect(panel As Panel)
        Dim shadow As New Panel With {
        .BackColor = Color.FromArgb(50, 0, 0, 0),
        .Dock = DockStyle.Fill,
        .Padding = New Padding(0, 0, 5, 5)
    }
        panel.Controls.Add(shadow)
        shadow.SendToBack()
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

        ' Columnas con DataPropertyName en PascalCase (igual que el DataTable)
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
            .DataPropertyName = "DescripcionAsignatura",
            .HeaderText = "DescripcionAsignatura",
            .Width = 250
        },
        New DataGridViewTextBoxColumn With {
            .DataPropertyName = "NombreDocenteCompleto",
            .HeaderText = "NombreDocenteCompleto",
            .Width = 200
        },
        New DataGridViewTextBoxColumn With {
            .DataPropertyName = "NombreCarrera",
            .HeaderText = "NombreCarrera",
            .Width = 200
        }
    })

        AddHandler dgvAsignaturas.CellDoubleClick, AddressOf dgvAsignaturas_CellDoubleClick
    End Sub




    Private Sub dgvAsignaturas_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvAsignaturas.CellDoubleClick
        If e.RowIndex < 0 Then Return ' Evitar clic en encabezados

        ' Obtener la fila seleccionada
        Dim selectedRow As DataGridViewRow = dgvAsignaturas.Rows(e.RowIndex)

        ' Intentar obtener el código de la asignatura con nombre de columna
        Dim codigoAsignatura As String = ""
        Try
            codigoAsignatura = selectedRow.Cells("codigoAsignatura").Value.ToString()
        Catch ex As ArgumentException
            ' Si no encuentra la columna, intenta con un índice (ajústalo según corresponda)
            codigoAsignatura = selectedRow.Cells(0).Value.ToString()
        End Try

        ' Verificar si se obtuvo correctamente el código
        If String.IsNullOrEmpty(codigoAsignatura) Then
            MessageBox.Show("No se pudo obtener el código de la asignatura.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Crear y mostrar el formulario de descripción de asignatura
        Dim detallesForm As New DescripcionAsignaturas(codigoAsignatura)
        detallesForm.ShowDialog()
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

    ' Función para convertir la lista de asignaturas en DataTable
    Private Function ConvertirListaADataTable(asignaturas As List(Of Asignaturas)) As DataTable
        Dim dt As New DataTable()

        dt.Columns.Add("CodigoAsignatura", GetType(String))
        dt.Columns.Add("NombreAsignatura", GetType(String))
        dt.Columns.Add("DescripcionAsignatura", GetType(String))
        dt.Columns.Add("NombreDocenteCompleto", GetType(String))
        dt.Columns.Add("NombreCarrera", GetType(String))

        For Each a In asignaturas
            dt.Rows.Add(
            a.CodigoAsignatura,
            a.NombreAsignatura,
            a.DescripcionAsignatura,
            a.NombreDocenteCompleto,
            a.NombreCarrera
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
                    Dim docente = docentes.FirstOrDefault(Function(d) d.docenteId = asignatura.docenteId)
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
