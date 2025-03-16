Public Class BienvenidaForm
    Inherits Form

    Private lblTitulo As Label
    Private lblDescripcion As Label
    Private picIcono As PictureBox
    Private bottomPanel As Panel
    Private btnInicio As Button

    Public Sub New()
        ' Configuración del formulario
        Me.Text = "Bienvenida - UcneGuide"
        Me.Size = New Size(350, 600)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.White

        ' Icono superior
        picIcono = New PictureBox With {
            .Size = New Size(120, 120),
            .Location = New Point((Me.ClientSize.Width - 120) \ 2, 50),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .Image = My.Resources.guia_turistico_3' Reemplazar con el recurso adecuado
        }
        Me.Controls.Add(picIcono)

        ' Título
        lblTitulo = New Label With {
            .Text = "Bienvenidos a UcneGuide",
            .Font = New Font("Arial", 14, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point((Me.ClientSize.Width - 250) \ 2, 180),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblTitulo)

        ' Descripción
        lblDescripcion = New Label With {
            .Text = "¡Bienvenido a nuestra app móvil para estudiantes de Ingeniería en Sistemas y Cómputos! " &
                   "Aquí encontrarás información completa sobre tus asignaturas, perfiles de profesores, " &
                   "calificaciones y reseñas. Explora, aprende y conecta en nuestra app. ¡Disfruta de la experiencia académica completa!",
            .Font = New Font("Arial", 10),
            .AutoSize = False,
            .Size = New Size(300, 120),
            .Location = New Point((Me.ClientSize.Width - 300) \ 2, 220),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblDescripcion)

        ' Panel inferior con fondo azul y borde amarillo
        bottomPanel = New Panel With {
            .Size = New Size(Me.ClientSize.Width, 100),
            .Location = New Point(0, Me.ClientSize.Height - 100),
            .BackColor = ColorTranslator.FromHtml("#074788")
        }
        Me.Controls.Add(bottomPanel)

        ' Botón de inicio dentro del panel inferior
        btnInicio = New Button With {
            .Size = New Size(60, 60),
            .Location = New Point((bottomPanel.Width - 60) \ 2, 20),
            .BackgroundImageLayout = ImageLayout.Zoom,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .Image = My.Resources.Inicio' Reemplazar con el recurso adecuado
        }
        btnInicio.FlatAppearance.BorderSize = 0
        bottomPanel.Controls.Add(btnInicio)
    End Sub

    Private Sub BienvenidaForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Puedes agregar código aquí si necesitas realizar alguna acción al cargar el formulario
    End Sub
End Class
