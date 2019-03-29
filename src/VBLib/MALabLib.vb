Imports System.IO
Imports System.Net
Imports Newtonsoft.Json.Linq

Public Class MALabLib
    Private Const USER_AGENT = "BitBaan-API-Sample-VBNet"

    Private server_address As String
    Private api_key As String
    Dim unknownerror_respone_json As JObject

    Public Sub New(ByVal server_address As String, Optional ByVal api_key As String = "")
        Me.server_address = server_address
        Me.api_key = api_key

        unknownerror_respone_json = New JObject
        unknownerror_respone_json.Add("error_code", 900)
        unknownerror_respone_json.Add("success", False)
    End Sub

    Private Function call_api_with_json_input(ByVal api As String, ByVal json_input As JObject) As JObject
        Dim HttpWebRequest As HttpWebRequest = WebRequest.Create(Me.server_address + "/" + api)
        HttpWebRequest.ContentType = "application/json"
        HttpWebRequest.Method = "POST"
        HttpWebRequest.UserAgent = USER_AGENT
        Using streamWriter = New StreamWriter(HttpWebRequest.GetRequestStream())
            Dim parsedContent As String = json_input.ToString()
            streamWriter.Write(parsedContent)
            streamWriter.Flush()
            streamWriter.Close()
        End Using
        Dim result As String
        Try
            Dim httpResponse As HttpWebResponse = HttpWebRequest.GetResponse()
            Using streamReader = New StreamReader(httpResponse.GetResponseStream())
                result = streamReader.ReadToEnd()
            End Using
        Catch ex As WebException
            If ex.Response Is Nothing Then
                Return unknownerror_respone_json
            End If
            Using stream = ex.Response.GetResponseStream()
                Using reader = New StreamReader(stream)
                    result = reader.ReadToEnd()
                End Using
            End Using
        End Try
        Try
            Dim srtr As JObject
            srtr = JObject.Parse(result)
            Return srtr
        Catch ex As Exception
            Return unknownerror_respone_json
        End Try
    End Function

    Public Function login(ByVal email As String, ByVal password As String) As JObject
        Dim params As JObject = New JObject
        params.Add("email", email)
        params.Add("password", password)
        Return call_api_with_json_input("api/v1/user/login", params)
    End Function
End Class
