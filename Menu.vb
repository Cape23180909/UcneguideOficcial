Public Class Menu
    Inherits Form

    Public Sub New()
        Me.Text = "Pantalla Principal"
        Me.Size = New Size(800, 600)
        Me.BackColor = Color.White

        Dim lblBienvenida As New Label With {
            .Text = "¡Bienvenido al Sistema!",
            .Font = New Font("Arial", 24, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(250, 250)
        }

        Me.Controls.Add(lblBienvenida)
    End Sub

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


End Class


