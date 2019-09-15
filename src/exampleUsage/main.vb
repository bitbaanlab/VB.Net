Imports VBLib
Imports Newtonsoft.Json.Linq
Imports System.IO

Module main

    Sub Main()
        Console.Write(" ____  _ _   ____                      __  __    _    _          _     " + vbCrLf)
        Console.Write("| __ )(_) |_| __ )  __ _  __ _ _ __   |  \/  |  / \  | |    __ _| |__  " + vbCrLf)
        Console.Write("|  _ \| | __|  _ \ / _` |/ _` | '_ \  | |\/| | / _ \ | |   / _` | '_ \ " + vbCrLf)
        Console.Write("| |_) | | |_| |_) | (_| | (_| | | | | | |  | |/ ___ \| |__| (_| | |_) |" + vbCrLf)
        Console.Write("|____/|_|\__|____/ \__,_|\__,_|_| |_| |_|  |_/_/   \_\_____\__,_|_.__/ " + vbCrLf + vbCrLf)

        Dim email As String = "", password As String = "", serveraddress As String = ""
        Dim file_path As String = ""

        Console.Write("Please insert API server address [Default=https://apimalab.bitbaan.com]: ")
        serveraddress = Console.ReadLine()
        If serveraddress = "" Then
            serveraddress = "https://apimalab.bitbaan.com"
        End If

        Console.Write("Please insert email address: ")
        email = Console.ReadLine()

        Console.Write("Please insert your password: ")
        password = Console.ReadLine()

        Dim MALabLib As MALabLib = New MALabLib(serveraddress)
        Dim params1 As JObject = New JObject
        params1.Add("email", email)
        params1.Add("password", password)
        Dim return_value As JObject = MALabLib.call_with_json_input("user/login", params1)
        If (return_value.Item("success").ToObject(Of Boolean) = True) Then
            Console.Write("You are logged in successfully." + vbCrLf)
        Else
            Console.Write(MALabLib.get_error(return_value))
            Console.ReadLine()
            Return
        End If
        Console.Write("Please enter the path of file to scan: ")
        file_path = Console.ReadLine()
        Dim apikey = return_value.Item("apikey").ToString
        Dim file_name = Path.GetFileName(file_path)
        Dim params2 As JObject = New JObject
        params2.Add("file_name", file_name)
        params2.Add("apikey", apikey)
        return_value = MALabLib.call_with_form_input("file/scan", params2, "file_data", file_path)
        If (return_value.Item("success").ToObject(Of Boolean) = True) Then
            'getting scan results:
            Dim is_finished = False
            Dim file_hash = MALabLib.get_sha256(file_path)
            Dim scan_id = return_value.Item("scan_id").ToObject(Of Integer)
            While is_finished = False
                Console.Write("Waiting for getting results..." + vbCrLf)
                Dim params3 As JObject = New JObject
                params3.Add("hash", file_hash)
                params3.Add("apikey", apikey)
                return_value = MALabLib.call_with_json_input("file/scan/result/get", params3)
                If return_value.Item("success").ToObject(Of Boolean) = False Then
                    Console.Write(MALabLib.get_error(return_value))
                    Console.ReadLine()
                End If
                Console.Clear()
                For Each current_av_result As JObject In return_value.Item("scan").Item("results").ToArray()
                    If current_av_result.Item("result").ToObject(Of String) = "malware" Then 'file Is malware
                        Console.Write("{0} ==> {1}" + vbCrLf, current_av_result.Item("av_name").ToString, current_av_result.Item("malware_name").ToString)
                    ElseIf current_av_result.Item("result").ToObject(Of String) = "clean" Then 'file Is clean
                        Console.Write("{0} ==> {1}" + vbCrLf, current_av_result.Item("av_name").ToString, "Clean")
                    End If
                Next
                is_finished = return_value.Item("scan").Item("is_finished").ToObject(Of Boolean)
                System.Threading.Thread.Sleep(2000)
            End While
        Else
            Console.Write(MALabLib.get_error(return_value))
        End If
        Console.ReadLine()
    End Sub

End Module
