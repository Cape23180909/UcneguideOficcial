Imports Newtonsoft.Json
Imports System.Net.Http
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows.Forms

Public Class ModificarComentarios
    Inherits Form

    ' URLs de la API
    Private ReadOnly ApiUrlComentarios As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Comentarios"
    Private ReadOnly ApiUrlAsignaturas As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Asignaturas"
    Private ReadOnly ApiUrlDocentes As String = "https://api-ucne-emfugwekcfefc3ef.eastus-01.azurewebsites.net/api/Docentes"

    ' Componentes del formulario
    Private WithEvents cmbAsignaturas As ComboBox
    Private WithEvents cmbDocentes As ComboBox
    Private WithEvents txtComentario As TextBox
    Private lblFecha As Label
    Private WithEvents btnActualizar As Button

    ' Variables de estado
    Private httpClient As New HttpClient()
    Private asignaturas As List(Of Asignaturas)
    Private docentes As List(Of Docente)
    Private comentarioActual As Comentarios
    Private comentarioId As Integer

    Public Sub New(comentarioId As Integer, asignaturas As List(Of Asignaturas), docentes As List(Of Docente))
        Me.comentarioId = comentarioId
        Me.asignaturas = asignaturas
        Me.docentes = docentes
        InitializeFormComponents()
        AddHandler Me.Load, AddressOf ModificarComentarios_Load
    End Sub

    Private Sub InitializeFormComponents()
        Me.SuspendLayout()

        ' Configuración básica del formulario
        Me.Text = "Editar Comentario"
        Me.Size = New Size(600, 400)
        Me.StartPosition = FormStartPosition.CenterScreen

        ' Crear controles
        CreateControls()

        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub

    Private Sub CreateControls()
        ' Configurar ComboBox de asignaturas
        cmbAsignaturas = New ComboBox With {
            .Location = New Point(20, 60),
            .Size = New Size(400, 30),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        ' Configurar ComboBox de docentes
        cmbDocentes = New ComboBox With {
            .Location = New Point(20, 140),
            .Size = New Size(400, 30),
            .DropDownStyle = ComboBoxStyle.DropDownList
        }

        ' Configurar TextBox de comentario
        txtComentario = New TextBox With {
            .Location = New Point(20, 220),
            .Size = New Size(400, 100),
            .Multiline = True
        }

        ' Configurar etiqueta de fecha
        lblFecha = New Label With {
            .Location = New Point(20, 20),
            .Size = New Size(300, 20),
            .Text = "Fecha:"
        }

        ' Configurar botón de actualización
        btnActualizar = New Button With {
            .Location = New Point(20, 340),
            .Size = New Size(150, 30),
            .Text = "Actualizar"
        }

        ' Agregar controles al formulario
        Me.Controls.AddRange({cmbAsignaturas, cmbDocentes, txtComentario, lblFecha, btnActualizar})
    End Sub

    Private Async Sub ModificarComentarios_Load(sender As Object, e As EventArgs)
        Await LoadInitialData()
        ConfigureControls()
    End Sub

    Private Async Function LoadInitialData() As Task
        Try
            ' Cargar comentario
            Dim response = Await httpClient.GetAsync($"{ApiUrlComentarios}/{comentarioId}")
            If response.IsSuccessStatusCode Then
                Dim json = Await response.Content.ReadAsStringAsync()
                comentarioActual = JsonConvert.DeserializeObject(Of Comentarios)(json)

                If comentarioActual Is Nothing Then
                    MessageBox.Show("El comentario no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Me.Close()
                End If
            Else
                MessageBox.Show("Error al cargar el comentario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Me.Close()
            End If

            ' Cargar asignaturas y docentes
            Dim asignaturasResponse = Await httpClient.GetAsync(ApiUrlAsignaturas)
            If asignaturasResponse.IsSuccessStatusCode Then
                asignaturas = JsonConvert.DeserializeObject(Of List(Of Asignaturas))(
                    Await asignaturasResponse.Content.ReadAsStringAsync())
            End If

            Dim docentesResponse = Await httpClient.GetAsync(ApiUrlDocentes)
            If docentesResponse.IsSuccessStatusCode Then
                docentes = JsonConvert.DeserializeObject(Of List(Of Docente))(
                    Await docentesResponse.Content.ReadAsStringAsync())
            End If

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.Close()
        End Try
    End Function

    Private Sub ConfigureControls()
        Try
            ' Asignaturas
            cmbAsignaturas.DataSource = asignaturas
            cmbAsignaturas.DisplayMember = "NombreAsignatura"
            cmbAsignaturas.ValueMember = "AsignaturaId"
            cmbAsignaturas.SelectedValue = comentarioActual.AsignaturaId

            ' Docentes
            cmbDocentes.DataSource = docentes
            cmbDocentes.DisplayMember = "Nombre"
            cmbDocentes.ValueMember = "DocenteId"
            cmbDocentes.SelectedValue = comentarioActual.DocenteId

            ' Otros controles
            lblFecha.Text = $"Fecha: {comentarioActual.FechaComentario:dd/MM/yyyy HH:mm}"
            txtComentario.Text = comentarioActual.Comentario

        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Async Sub btnActualizar_Click(sender As Object, e As EventArgs) Handles btnActualizar.Click
        If Not ValidateInputs() Then Return

        Try
            Dim updatedComment = CreateUpdatedComment()
            Dim response = Await UpdateComment(updatedComment)

            If response.IsSuccessStatusCode Then
                MessageBox.Show("Comentario actualizado correctamente", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Me.DialogResult = DialogResult.OK
                Me.Close()
            Else
                ShowApiError(response)
            End If
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function ValidateInputs() As Boolean
        If String.IsNullOrWhiteSpace(txtComentario.Text) Then
            MessageBox.Show("El comentario no puede estar vacío", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If cmbAsignaturas.SelectedValue Is Nothing OrElse cmbDocentes.SelectedValue Is Nothing Then
            MessageBox.Show("Debe seleccionar una asignatura y un docente", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True
    End Function

    Private Function CreateUpdatedComment() As Comentarios
        Return New Comentarios With {
            .ComentarioId = comentarioId,
            .AsignaturaId = CInt(cmbAsignaturas.SelectedValue),
            .DocenteId = CInt(cmbDocentes.SelectedValue),
            .Comentario = txtComentario.Text.Trim(),
            .FechaComentario = comentarioActual.FechaComentario,
            .UsuarioId = comentarioActual.UsuarioId
        }
    End Function

    Private Async Function UpdateComment(comment As Comentarios) As Task(Of HttpResponseMessage)
        Dim json = JsonConvert.SerializeObject(comment)
        Dim content = New StringContent(json, Encoding.UTF8, "application/json")
        Return Await httpClient.PutAsync($"{ApiUrlComentarios}/{comentarioId}", content)
    End Function

    Private Async Sub ShowApiError(response As HttpResponseMessage)
        Dim errorContent = Await response.Content.ReadAsStringAsync()
        MessageBox.Show($"Error del servidor: {response.StatusCode}{Environment.NewLine}{errorContent}",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error)
    End Sub

    Private Sub ModificarComentarios_Load_1(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class