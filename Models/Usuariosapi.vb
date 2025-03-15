Imports RestSharp

Public Class Usuariosapi
    Public Function Obtener(url As String, headers As List(Of Parametro), parametros As List(Of Parametro)) As String
        Return EjecutarSolicitud(url, Method.Get, headers, parametros, Nothing)
    End Function

    Public Function Post(url As String, headers As List(Of Parametro), parametros As List(Of Parametro), objeto As Object) As String
        Return EjecutarSolicitud(url, Method.Post, headers, parametros, objeto)
    End Function

    Public Function Put(url As String, headers As List(Of Parametro), parametros As List(Of Parametro), objeto As Object) As String
        Return EjecutarSolicitud(url, Method.Put, headers, parametros, objeto)
    End Function

    Public Function Delete(url As String, headers As List(Of Parametro), parametros As List(Of Parametro)) As String
        Return EjecutarSolicitud(url, Method.Delete, headers, parametros, Nothing)
    End Function

    Private Function EjecutarSolicitud(url As String, metodo As Method, headers As List(Of Parametro),
                                      parametros As List(Of Parametro), objeto As Object) As String
        Try
            Dim client = New RestClient(url)
            Dim request = New RestRequest() With {.Method = metodo}

            ' Agregar headers
            For Each header In headers
                request.AddHeader(header.Clave, header.Valor)
            Next

            ' Agregar parámetros
            For Each param In parametros
                request.AddParameter(param.Clave, param.Valor)
            Next

            ' Agregar cuerpo si existe
            If objeto IsNot Nothing Then
                request.AddJsonBody(objeto)
            End If

            Dim response = client.Execute(request)
            If response.IsSuccessful Then
                Return response.Content
            Else
                Throw New Exception($"Error: {response.StatusCode} - {response.ErrorMessage}")
            End If
        Catch ex As Exception
            Throw New Exception($"Error en la solicitud: {ex.Message}")
        End Try
    End Function
End Class

