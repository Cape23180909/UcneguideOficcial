Imports System.Drawing.Drawing2D

Public Class Menu
    Inherits Form

    Private topPanel As Panel
    Private bottomPanel As Panel
    Private lblTitulo As Label
    Private btnBack As PictureBox
    Private btnUser As PictureBox
    Private flowPanel As FlowLayoutPanel
    Private cardFacultades As Panel
    Private cardSugerencias As Panel
    Private cardMaestros As Panel
    Private imgFacultades As PictureBox
    Private imgSugerencias As PictureBox
    Private imgMaestros As PictureBox
    Private lblFacultades As Label
    Private lblSugerencias As Label
    Private lblMaestros As Label

    Public Sub New()
        ' Configuración del formulario
        Me.Text = "Menú"
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.None
        Me.BackColor = Color.White

        ' Panel superior
        topPanel = New Panel With {
            .BackColor = ColorTranslator.FromHtml("#074788"),
            .Height = 80,
            .Dock = DockStyle.Top
        }
        Me.Controls.Add(topPanel)

        ' Icono de menú (botón de regreso)
        btnBack = New PictureBox With {
            .Size = New Size(40, 40),
            .SizeMode = PictureBoxSizeMode.StretchImage,
            .Location = New Point(10, 20)
        }
        btnBack.Image = LoadImage("C:\\Ucneguide\\Resources\\guia-turistico 3.png")
        AddMouseHoverEffects(btnBack)
        topPanel.Controls.Add(btnBack)

        ' Título del menú
        lblTitulo = New Label With {
            .Text = "Menú",
            .ForeColor = Color.White,
            .Font = New Font("Arial", 18, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(60, 25)
        }
        topPanel.Controls.Add(lblTitulo)

        ' Icono de usuario
        btnUser = New PictureBox With {
            .Size = New Size(40, 40),
            .SizeMode = PictureBoxSizeMode.StretchImage
        }
        btnUser.Image = LoadImage("C:\\Ucneguide\\Resources\\Usuarios.png")
        AddMouseHoverEffects(btnUser)
        AddHandler btnUser.Click, AddressOf btnUser_Click
        topPanel.Controls.Add(btnUser)

        ' Panel para tarjetas
        flowPanel = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoScroll = True,
            .Padding = New Padding(50, 200, 50, 20)
        }
        Me.Controls.Add(flowPanel)

        ' Crear tarjetas
        CreateCard(cardFacultades, imgFacultades, lblFacultades, LoadImage("C:\\Ucneguide\\Resources\\Facultad.png"), "Asignaturas")
        CreateCard(cardSugerencias, imgSugerencias, lblSugerencias, LoadImage("C:\\Ucneguide\\Resources\\Sugerencia.png"), "Sugerencias")
        CreateCard(cardMaestros, imgMaestros, lblMaestros, LoadImage("C:\\Ucneguide\\Resources\\Maestro.png"), "Maestros")

        ' Panel inferior
        bottomPanel = New Panel With {
            .BackColor = Color.White,
            .Height = 50,
            .Dock = DockStyle.Bottom
        }
        Me.Controls.Add(bottomPanel)

        ' Botón de regreso
        Dim btnRegreso As New PictureBox With {
            .Size = New Size(40, 40),
            .SizeMode = PictureBoxSizeMode.StretchImage
        }
        btnRegreso.Image = LoadImage("C:\\Ucneguide\\Resources\\Salir.png")
        AddMouseHoverEffects(btnRegreso)
        AddHandler btnRegreso.Click, AddressOf BtnRegreso_Click
        bottomPanel.Controls.Add(btnRegreso)

        ResizeElements()
    End Sub


    ' Evento para manejar el clic en el icono de usuario
    Private Sub btnUser_Click(sender As Object, e As EventArgs)
        Dim perfilForm As New ActualizarPerfil()
        perfilForm.ShowDialog()
    End Sub

    Private Sub CreateCard(ByRef card As Panel, ByRef img As PictureBox, ByRef lbl As Label, image As Image, text As String)
        card = New Panel With {
        .Size = New Size(250, 250),
        .BackColor = Color.White,
        .Margin = New Padding(50, 0, 50, 0)
    }
        flowPanel.Controls.Add(card)

        img = New PictureBox With {
        .Size = New Size(250, 250),
        .SizeMode = PictureBoxSizeMode.StretchImage,
        .Image = AddTextToImage(image, text)
    }
        AddMouseHoverEffects(img)


        ' Evento Click para abrir el formulario correspondiente
        AddHandler img.Click, Sub(sender, e)
                                  Select Case text
                                      Case "Asignaturas"
                                          Dim gestionAsignaturas As New GestionarAsignaturas(UserSession.CarreraId)
                                          gestionAsignaturas.Show()
                                      Case "Sugerencias"
                                          Dim formSugerencias As New GestionarSugerencias()
                                          formSugerencias.Show()
                                      Case "Maestros"
                                          Dim formDocentes As New GestionarDocentes()
                                          formDocentes.Show()
                                  End Select
                              End Sub


        card.Controls.Add(img)
    End Sub


    ' Método para agregar texto a la imagen
    Private Function AddTextToImage(originalImage As Image, text As String) As Image
        Dim newImage As Bitmap = New Bitmap(originalImage)
        Using g As Graphics = Graphics.FromImage(newImage)
            Dim font As New Font("Arial", 16, FontStyle.Bold)
            Dim brush As New SolidBrush(Color.White)
            Dim rect As New Rectangle(0, newImage.Height - 50, newImage.Width, 50)
            Dim sf As New StringFormat() With {
                .Alignment = StringAlignment.Center,
                .LineAlignment = StringAlignment.Center
            }
            g.DrawString(text, font, brush, rect, sf)
        End Using
        Return newImage
    End Function

    ' Método para hacer que los iconos sean responsivos al pasar el mouse
    Private Sub AddMouseHoverEffects(picBox As PictureBox)
        AddHandler picBox.MouseEnter, Sub(sender, e) picBox.Size = New Size(picBox.Width + 10, picBox.Height + 10)
        AddHandler picBox.MouseLeave, Sub(sender, e) picBox.Size = New Size(picBox.Width - 10, picBox.Height - 10)
    End Sub

    ' Método para organizar los elementos
    Private Sub ResizeElements()
        btnUser.Location = New Point(topPanel.Width - btnUser.Width - 10, (topPanel.Height - btnUser.Height) \ 2)
        bottomPanel.Controls(0).Location = New Point(10, 5)
    End Sub

    ' Evento de clic para el botón de regreso
    Private Sub BtnRegreso_Click(sender As Object, e As EventArgs)
        Dim bienvenidaForm As New BienvenidaForm()
        bienvenidaForm.Show()
        Me.Close()
    End Sub

    ' Método para cargar imágenes con verificación
    Private Function LoadImage(imagePath As String) As Image
        If System.IO.File.Exists(imagePath) Then
            Return Image.FromFile(imagePath)
        Else
            MessageBox.Show("No se encontró la imagen: " & imagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return Nothing
        End If
    End Function

    Private Sub Menu_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ResizeElements()
    End Sub
End Class
