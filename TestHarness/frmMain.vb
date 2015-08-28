Imports NFLPlayerScraper
Imports CommonUtilities
Imports System.IO
Imports FantasyHelperObjects

Public Class frmMain

    Private _logFs As FileStream
    Private _scraper As FoxSportsPlayerScraper
    Private _conStr As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            _scraper = New FoxSportsPlayerScraper(My.Settings.foxRootURL, My.Settings.foxPlayersURL)
            _scraper.FetchPlayers()

        Catch ex As Exception
            Util.LogMeWithTimestamp(ex, "Error getting players.")
        End Try
    End Sub

    Private Sub ImportPlayersToDB()
        While True
            Try
                Dim p As Player = _scraper.Players.Dequeue()
                If p IsNot Nothing Then
                    DataUtility.InsertPlayer(_conStr, p)
                End If
            Catch ex As Exception

            End Try
        End While
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Util.SetupLog(_logFs, My.Application.Info.ProductName)
        _conStr = My.Settings.ffConStrWork
    End Sub
End Class
