Option Explicit On
Option Strict On

Imports System.IO

Public Class Form_Main

    ' Object NotifyIcon - tray form
    Private WithEvents mNotifyIcon As New NotifyIcon

    Private Sub btnPath_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPath.Click

        'Open to select folder and files
        Dim oFolderBrowser As New FolderBrowserDialog

        With oFolderBrowser
            .SelectedPath = ""
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                txtPath.Text = .SelectedPath
                .Dispose()
                cambiar_FSW(FileSystemWatcher1)
            End If
        End With

    End Sub

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        mNotifyIcon.Dispose()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' Started a FilseSystemWatcher properties
        With FileSystemWatcher1
            ' Include Directory to perform a monitoring
            .IncludeSubdirectories = False
            .EnableRaisingEvents = False
            ' Monitor al files
            .Filter = "*.*"
            ' Filters: Create, Change, Read, Write
            .NotifyFilter = NotifyFilters.CreationTime Or _
                            NotifyFilters.Size Or _
                            NotifyFilters.FileName Or _
                            NotifyFilters.LastAccess

            'MsgBox(.NotifyFilter)
            chkActivar.Text = "Notification"
        End With

        ' Add columns (path, type and date)
        With ListView1
            .View = View.Details
            .Columns.Add("Files ", 300)
            .Columns.Add("Type of Modification", 120)
            .Columns.Add("Date", 200)
        End With

        btnPath.Text = "Select"

        ' NotifyIcon
        With mNotifyIcon
            .Icon = Me.Icon
        End With

    End Sub

    Private Sub chkActivar_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkActivar.CheckedChanged
        cambiar_FSW(FileSystemWatcher1)
    End Sub

    Sub cambiar_FSW(ByVal FSW As FileSystemWatcher)

        Try
            ' Check if directory exist
            If Directory.Exists(txtPath.Text) = True Then
                With FSW
                    .Path = txtPath.Text
                    ' Enable or disable FileSystemWatcher
                    .EnableRaisingEvents = chkActivar.Checked
                    .IncludeSubdirectories = True
                End With
            End If
            ' error
        Catch sError As Exception
            MsgBox(sError.Message, MsgBoxStyle.Critical)
        End Try

    End Sub

    ' Add info to listview
    Sub Notificar_Cambio(ByVal Path As String, ByVal type As String)

        Dim oItem As New ListViewItem(Path)
        With oItem
            .SubItems.Add(type)
            .SubItems.Add(Date.Now.ToString)
            ListView1.Items.Add(oItem)
        End With

        With mNotifyIcon
            .ShowBalloonTip(5000, type, Path, ToolTipIcon.Info)
        End With
    End Sub

    ' Change in Folders or Files
    Private Sub FileSystemWatcher1_Changed(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Changed
        Notificar_Cambio(e.Name, "Change by " & Environment.UserName)
    End Sub

    ' Create Files or Folders
    Private Sub FileSystemWatcher1_Created(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Created
        Notificar_Cambio(e.Name, "Create by " & Environment.UserName)
    End Sub

    ' Delete Files or Folders
    Private Sub FileSystemWatcher1_Deleted(ByVal sender As Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Deleted
        Notificar_Cambio(e.Name, "Delete by " & Environment.UserName)
    End Sub

    ' Error event
    Private Sub FileSystemWatcher1_Error(ByVal sender As Object, ByVal e As System.IO.ErrorEventArgs) Handles FileSystemWatcher1.Error
        MsgBox(e.GetException.ToString)
    End Sub

    ' NotifyIcon event
    Private Sub mNotifyIcon_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles mNotifyIcon.Click
        Me.Visible = True
        Me.WindowState = FormWindowState.Normal
    End Sub

    ' Do enable the NotifyIcon
    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize

        Select Case Me.WindowState
            Case FormWindowState.Minimized
                Me.Visible = False
                mNotifyIcon.Visible = True
        End Select
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        'Save content to text file
        Dim directory As String = My.Application.Info.DirectoryPath
        'Declare IO StreamWriter
        Dim streamWriter As IO.StreamWriter = New IO.StreamWriter(directory & "chusma-output.txt")

        'Save de result in a text file
        For Each o As Object In ListView1.Items()
            streamWriter.WriteLine(o.ToString())
        Next
        streamWriter.Flush()
        streamWriter.Close()

        MsgBox(directory & "chusma-output.txt - Saved")

    End Sub
End Class
