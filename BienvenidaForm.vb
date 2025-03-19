Imports System.Drawing.Drawing2D

Public Class BienvenidaForm
    Inherits Form

    Private lblTitulo As Label
    Private lblDescripcion As Label
    Private picIcono As PictureBox
    Private bottomPanel As Panel
    Private btnInicio As Button
    Private bottomBorder1 As Panel
    Private bottomBorder2 As Panel

    Public Sub New()
        ' Configuración del formulario en pantalla completa
        Me.Text = "Bienvenida - UcneGuide"
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.None
        Me.BackColor = Color.White

        ' Icono superior con diseño circular
        picIcono = New PictureBox With {
            .Size = New Size(150, 150),
            .SizeMode = PictureBoxSizeMode.StretchImage
        }
        Me.Controls.Add(picIcono)
        DrawCircularIcon(picIcono)

        ' Título
        lblTitulo = New Label With {
            .Text = "Bienvenidos a UcneGuide",
            .Font = New Font("Arial", 18, FontStyle.Bold),
            .AutoSize = True,
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblTitulo)

        ' Descripción
        lblDescripcion = New Label With {
            .Text = "¡Bienvenido a nuestra app móvil para estudiantes de Ingeniería en Sistemas y Cómputos! " & vbCrLf &
                    "Aquí encontrarás información sobre asignaturas, perfiles de profesores, " & vbCrLf &
                    "calificaciones y reseñas. Explora, aprende y conecta en nuestra app. " & vbCrLf &
                    "¡Disfruta de la experiencia académica completa!",
            .Font = New Font("Arial", 12),
            .AutoSize = False,
            .Size = New Size(500, 120),
            .TextAlign = ContentAlignment.MiddleCenter
        }
        Me.Controls.Add(lblDescripcion)

        ' Panel inferior con fondo azul
        bottomPanel = New Panel With {
            .BackColor = ColorTranslator.FromHtml("#074788")
        }
        Me.Controls.Add(bottomPanel)

        ' Líneas amarillas (arriba y abajo del panel azul)
        bottomBorder1 = New Panel With {
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        bottomPanel.Controls.Add(bottomBorder1)

        bottomBorder2 = New Panel With {
            .Height = 5,
            .BackColor = ColorTranslator.FromHtml("#F7D917")
        }
        bottomPanel.Controls.Add(bottomBorder2)

        ' Botón de inicio (Circular)
        btnInicio = New Button With {
            .Size = New Size(70, 70),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .Text = "▶"
        }
        btnInicio.FlatAppearance.BorderSize = 0
        bottomPanel.Controls.Add(btnInicio)

        ' Asignar evento de clic al botón
        AddHandler btnInicio.Click, AddressOf BtnInicio_Click

        ' Aplicar forma circular al botón
        MakeButtonCircular(btnInicio)

        ' Ajustar todos los elementos en pantalla
        ResizeElements()
    End Sub



    Private Sub IrAMenu(sender As Object, e As EventArgs)
        Dim menuForm As New Menu()
        menuForm.Show()
        Me.Close()
    End Sub

    ' Método para dibujar el ícono dentro de un círculo azul
    Private Sub DrawCircularIcon(pb As PictureBox)
        Dim bmp As New Bitmap(pb.Width, pb.Height)
        Using g As Graphics = Graphics.FromImage(bmp)
            g.SmoothingMode = SmoothingMode.AntiAlias

            ' Color #074788
            Dim customColor As Color = ColorTranslator.FromHtml("#074788")
            Dim brush As New SolidBrush(customColor)

            ' Dibujar el círculo
            Dim rect As New Rectangle(0, 0, pb.Width, pb.Height)
            g.FillEllipse(brush, rect)

            ' Cargar el ícono desde los recursos
            Dim icono As Image = My.Resources.guia_turistico_3

            ' Ajustar tamaño del ícono dentro del círculo
            Dim iconSize As Integer = pb.Width \ 2
            Dim iconRect As New Rectangle((pb.Width - iconSize) \ 2, (pb.Height - iconSize) \ 2, iconSize, iconSize)

            ' Dibujar el ícono en el centro
            g.DrawImage(icono, iconRect)
        End Using

        pb.Image = bmp
    End Sub

    ' Método para hacer el botón circular
    Private Sub MakeButtonCircular(btn As Button)
        Dim path As New GraphicsPath()
        path.AddEllipse(0, 0, btn.Width, btn.Height)
        btn.Region = New Region(path)
    End Sub

    ' Evento al cargar el formulario
    Private Sub BienvenidaForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ResizeElements()
    End Sub

    ' Método para redimensionar elementos según el tamaño de la pantalla
    Private Sub ResizeElements()
        Dim formWidth As Integer = Me.ClientSize.Width
        Dim formHeight As Integer = Me.ClientSize.Height

        ' Centrar ícono
        picIcono.Location = New Point((formWidth - picIcono.Width) \ 2, formHeight \ 6)

        ' Centrar título debajo del ícono
        lblTitulo.Location = New Point((formWidth - lblTitulo.Width) \ 2, picIcono.Bottom + 20)

        ' Centrar descripción debajo del título
        lblDescripcion.Location = New Point((formWidth - lblDescripcion.Width) \ 2, lblTitulo.Bottom + 20)

        ' Ajustar tamaño del panel inferior
        bottomPanel.Size = New Size(formWidth, 100)
        bottomPanel.Location = New Point(0, formHeight - bottomPanel.Height)

        ' Ajustar las líneas amarillas dentro del panel azul
        bottomBorder1.Size = New Size(bottomPanel.Width, 5)
        bottomBorder1.Location = New Point(0, 0) ' Línea amarilla superior

        bottomBorder2.Size = New Size(bottomPanel.Width, 5)
        bottomBorder2.Location = New Point(0, bottomPanel.Height - 5) ' Línea amarilla inferior

        ' Centrar botón de inicio dentro del panel inferior
        btnInicio.Location = New Point((bottomPanel.Width - btnInicio.Width) \ 2, (bottomPanel.Height - btnInicio.Height) \ 2)

        ' Aplicar nuevamente la forma circular al botón
        MakeButtonCircular(btnInicio)
    End Sub

    ' Evento al hacer clic en el botón de inicio
    Private Sub BtnInicio_Click(sender As Object, e As EventArgs)
        Dim menuForm As New Menu()
        menuForm.Show()
        Me.Hide()
    End Sub
End Class
