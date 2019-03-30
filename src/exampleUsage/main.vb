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

        Console.Write("Please insert API server address [Default=https://malab.bitbaan.com]: ")
        serveraddress = Console.ReadLine()
        If serveraddress = "" Then
            serveraddress = "https://malab.bitbaan.com"
        End If

        Console.Write("Please insert email address: ")
        email = Console.ReadLine()

        Console.Write("Please insert your password: ")
        password = Console.ReadLine()

        Dim MALabLib As MALabLib = New MALabLib(serveraddress)
        Dim returnValue As JObject = MALabLib.login(email, password)
        If (returnValue.Item("success").ToObject(Of Boolean) = True) Then
            Console.Write("You are logged in successfully." + vbCrLf)
        Else
            Console.Write("error code {0} occurred." + vbCrLf, returnValue.Item("error_code").ToObject(Of Integer))
            Console.ReadLine()
            Return
        End If
        Console.Write("Please enter the path of file to scan: ")
        file_path = Console.ReadLine()
        returnValue = MALabLib.scan(file_path, Path.GetFileName(file_path))
        If (returnValue.Item("success").ToObject(Of Boolean) = True) Then
            'getting scan results:
            Dim is_finished = False
            Dim file_hash = MALabLib.get_sha256(file_path)
            Dim scan_id = returnValue.Item("scan_id").ToObject(Of Integer)
            While is_finished = False
                Console.Write("Waiting for getting results..." + vbCrLf)
                returnValue = MALabLib.results(file_hash, scan_id)
                If returnValue.Item("success").ToObject(Of Boolean) = False Then
                    Console.Write("error code {0} occurred." + vbCrLf, returnValue.Item("error_code").ToObject(Of Integer))
                    Console.ReadLine()
                End If
                Console.Clear()
                For Each current_av_result As JObject In returnValue.Item("results").ToArray()
                    If current_av_result.Item("result_state").ToObject(Of Integer) = 32 Then 'file Is malware
                        Console.Write("{0} ==> {1}" + vbCrLf, current_av_result.Item("av_name").ToString, current_av_result.Item("virus_name").ToString)
                    ElseIf current_av_result.Item("result_state").ToObject(Of Integer) = 33 Then 'file Is clean
                        Console.Write("{0} ==> {1}" + vbCrLf, current_av_result.Item("av_name").ToString, "Clean")
                    End If
                Next
                is_finished = returnValue.Item("is_finished")
                System.Threading.Thread.Sleep(2000)
            End While
        Else
            Console.Write("error code {0} occurred." + vbCrLf, returnValue.Item("error_code").ToObject(Of Integer))
            Console.ReadLine()
            Return
        End If
    End Sub

End Module
