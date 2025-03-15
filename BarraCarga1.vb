
Imports System.Drawing.Drawing2D
Public Class BarraCarga1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Aplicar forma circular al PictureBox
        MakeCircularPictureBox(PictureBox1)

        ' Configurar la barra de carga
        ProgressBar1.Style = ProgressBarStyle.Continuous
        ProgressBar1.ForeColor = Color.DarkBlue
        ProgressBar1.Value = 0

        ' Iniciar el Timer
        Timer2.Interval = 100 ' Cada 100 ms
        Timer2.Start()
    End Sub

    Private Sub MakeCircularPictureBox(pb As PictureBox)
        Dim path As New GraphicsPath()
        path.AddEllipse(0, 0, pb.Width, pb.Height)
        pb.Region = New Region(path)
        pb.SizeMode = PictureBoxSizeMode.StretchImage ' Ajustar imagen sin deformar
    End Sub

    Private Sub Timer2_Tick(sender As Object, e As EventArgs) Handles Timer2.Tick
        ' Avanzar la barra de progreso
        If ProgressBar1.Value < 100 Then
            ProgressBar1.Value += 2 ' Incremento progresivo
        Else
            Timer2.Stop() ' Detener cuando llegue al 100%
            Me.Hide() ' Ocultar Form1
            Dim LoginForm As New LoginForm() ' Crear instancia de LoginForm
            LoginForm.Show() ' Mostrar LoginForm
        End If
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub ProgressBar1_Click(sender As Object, e As EventArgs) Handles ProgressBar1.Click

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click

    End Sub
End Class
