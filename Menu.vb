Public Class Menu
    Inherits Form

    Public Sub New()
        ' Configuración del formulario
        Me.Text = "Menú"
        Me.Size = New Size(400, 700)
        Me.BackColor = Color.White

        ' Panel superior con título
        Dim panelSuperior As New Panel With {
            .Size = New Size(Me.Width, 60),
            .BackColor = Color.Navy,
            .Dock = DockStyle.Top
        }

        Dim lblMenu As New Label With {
            .Text = "Menú",
            .ForeColor = Color.White,
            .Font = New Font("Arial", 16, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(20, 15)
        }

        panelSuperior.Controls.Add(lblMenu)
        Me.Controls.Add(panelSuperior)

        ' Crear las opciones del menú sin imágenes
        CrearOpcionMenu("Facultades", 100)
        CrearOpcionMenu("Sugerencias", 200)
        CrearOpcionMenu("Maestros", 300)

        ' Botón de salida
        Dim btnSalir As New Button With {
            .Text = "Salir",
            .Font = New Font("Arial", 12, FontStyle.Bold),
            .Size = New Size(100, 40),
            .Location = New Point(20, Me.Height - 100)
        }
        AddHandler btnSalir.Click, AddressOf Me.CerrarAplicacion
        Me.Controls.Add(btnSalir)
    End Sub

    Private Sub CrearOpcionMenu(texto As String, yPos As Integer)
        Dim panelOpcion As New Panel With {
            .Size = New Size(Me.Width - 40, 80),
            .Location = New Point(20, yPos),
            .BackColor = Color.LightGray,
            .BorderStyle = BorderStyle.FixedSingle
        }

        Dim lblTexto As New Label With {
            .Text = texto,
            .Font = New Font("Arial", 14, FontStyle.Bold),
            .ForeColor = Color.Black,
            .AutoSize = False,
            .TextAlign = ContentAlignment.MiddleCenter,
            .Dock = DockStyle.Fill
        }

        panelOpcion.Controls.Add(lblTexto)
        Me.Controls.Add(panelOpcion)
    End Sub

    Private Sub CerrarAplicacion(sender As Object, e As EventArgs)
        Me.Close()
    End Sub
End Class
