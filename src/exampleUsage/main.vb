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

        Dim email As String = "", password As String = "", serveraddress As String = "", use_https_str As String = ""
        Dim file_path As String = ""
        Dim use_https As Boolean = True

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
            Console.Write("error code {0} occured." + vbCrLf, returnValue.Item("error_code").ToObject(Of Integer))
            Console.ReadLine()
            Return
        End If
        Console.Write("Please enter the path of file to scan: ")
        file_path = Console.ReadLine()
        returnValue = MALabLib.scan(file_path, Path.GetFileName(file_path))
        If (returnValue.Item("success").ToObject(Of Boolean) = True) Then
            Console.Write("Scan completed successfully." + vbCrLf)
        Else
            Console.Write("error code {0} occured." + vbCrLf, returnValue.Item("error_code").ToObject(Of Integer))
            Console.ReadLine()
            Return
            Console.ReadLine()
        End If
        Console.ReadLine()
    End Sub

End Module
