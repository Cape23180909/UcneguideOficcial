Imports System.Drawing.Drawing2D

Public Class BarraCarga1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Configurar pantalla completa
        Me.WindowState = FormWindowState.Maximized
        Me.FormBorderStyle = FormBorderStyle.None
        Me.BackColor = Color.White

        ' Configurar PictureBox1
        PictureBox1.SizeMode = PictureBoxSizeMode.StretchImage
        DrawCircularIcon(PictureBox1)

        ' Configurar ProgressBar1
        ProgressBar1.Style = ProgressBarStyle.Continuous
        ProgressBar1.ForeColor = Color.DarkBlue
        ProgressBar1.BackColor = Color.LightGray
        ProgressBar1.Value = 0
        ProgressBar1.Size = New Size(Me.ClientSize.Width \ 2, 30)

        ' Configurar Timer
        Timer2.Interval = 50
        Timer2.Start()

        ' Centrar elementos
        ResizeElements()
    End Sub

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

            '' Cargar el ícono desde los recursos
            'Dim As Image = My.Resources.guia_turistico_3
            Dim icono As Image = Image.FromFile("C:\Ucneguide\Resources\logo.jpg")
            ' Ajustar tamaño del ícono dentro del círculo
            Dim iconSize As Integer = pb.Width \ 2
            Dim iconRect As New Rectangle((pb.Width - iconSize) \ 2, (pb.Height - iconSize) \ 2, iconSize, iconSize)

            ' Dibujar el ícono en el centro
            g.DrawImage(icono, iconRect)
        End Using

        pb.Image = bmp
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        If ProgressBar1.Value < 100 Then
            ProgressBar1.Value += 2
            Label1.Text = $"Cargando... {ProgressBar1.Value}%"
        Else
            Timer2.Stop()
            Me.Hide()
            Dim LoginForm As New LoginForm()
            LoginForm.Show()
        End If
    End Sub

    ' Permitir salir con ESC
    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Application.Exit()
        End If
    End Sub

    ' Manejar redimensionado
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ResizeElements()
    End Sub

    Private Sub ResizeElements()
        PictureBox1.Location = New Point((Me.ClientSize.Width - PictureBox1.Width) \ 2, (Me.ClientSize.Height \ 3) - (PictureBox1.Height \ 2))
        ProgressBar1.Location = New Point((Me.ClientSize.Width - ProgressBar1.Width) \ 2, PictureBox1.Bottom + 50)
        ProgressBar1.Size = New Size(Me.ClientSize.Width \ 2, 30)
        Label1.Location = New Point((Me.ClientSize.Width - Label1.Width) \ 2, ProgressBar1.Bottom + 20)
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub
End Class
