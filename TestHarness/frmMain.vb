Imports NFLPlayerScraper
Imports CommonUtilities
Imports System.IO
Imports FantasyHelperObjects

Public Class frmMain

    Private _appIsOpen As Boolean = True
    Private _logFs As FileStream
    Private Delegate Sub GenDel(obj As Object)
    Private _playerScraper As FoxSportsPlayerScraper
    Private _newsScraper As FoxSportsPlayerNewsScraper
    Private _conStr As String
    Private _insertedPlayerCount As Integer = 0

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Util.SetupLog(_logFs, My.Application.Info.ProductName)
        Util.LogMeWithTimestamp("Player tracker utils started.")
        Threading.ThreadPool.QueueUserWorkItem(AddressOf LogUpdateThread)
        _conStr = My.Settings.ffConStrWork
    End Sub
    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        _appIsOpen = False
    End Sub

    Private Sub LogUpdateThread(obj As Object)
        While _appIsOpen
            Try
                Dim str As String = TraceLogger.GetLog().Dequeue
                If Not String.IsNullOrEmpty(str) Then
                    AppendToTextBox(str + vbNewLine)
                End If
            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error in LogUpdateThread.")
            End Try
        End While
    End Sub

    Private Sub AppendToTextBox(obj As Object)
        If Me.TextBox1.InvokeRequired Then
            Dim d As New GenDel(AddressOf AppendToTextBox)
            Me.Invoke(d, obj)
        Else
            Me.TextBox1.AppendText(CType(obj, String))
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            If Util.ShowMessageBox("Delete player table?", "Are you sure you want to delete the player table?", MsgBoxStyle.YesNo) = DialogResult.OK Then
                Dim league As IProLeagues = New ProLeague(_conStr, "NFL")
                DataUtility.DeletePlayerTable(_conStr, league)
                _playerScraper = New FoxSportsPlayerScraper(My.Settings.foxRootURL, My.Settings.foxPlayersURL, league)
                _playerScraper.FetchPlayers()
                Threading.ThreadPool.QueueUserWorkItem(AddressOf ImportPlayersToDB)
            End If
        Catch ex As Exception
            Util.LogMeWithTimestamp(ex, "Error getting players.")
        End Try
    End Sub

    Private Sub ImportPlayersToDB()
        Dim timesQueueWasEmpty As Integer = 0
        While True
            Try
                Dim p As Player = _playerScraper.Players.Dequeue()
                If p IsNot Nothing Then
                    DataUtility.InsertPlayer(_conStr, p)
                    _insertedPlayerCount += 1
                Else
                    timesQueueWasEmpty += 1
                    If timesQueueWasEmpty > 6 And _playerScraper.IsProcessingComplete() Then
                        Util.LogMeWithTimestamp("Player queue was empty " + timesQueueWasEmpty.ToString + " times.")
                        Util.LogMeWithTimestamp("Done! " + _insertedPlayerCount.ToString + " players inserted.")
                        Exit While
                    End If
                End If
            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error importing player.")
            End Try
        End While
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            Dim league As IProLeagues = New ProLeague(_conStr, "NFL")
            _newsScraper = New FoxSportsPlayerNewsScraper(_conStr, league)
            _newsScraper.FetchPlayerNews()

        Catch ex As Exception

        End Try


    End Sub
End Class
