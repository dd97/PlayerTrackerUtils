Imports CommonUtilities
Imports FantasyHelperObjects

Public Class FoxSportsPlayerNewsScraper

    Private _league As IProLeagues
    Private _players As New ThreadSafeBlockingQueue(Of Player)
    Private _conStr As String
    Private _finishedQueueingPages As Boolean = False
    Private _numPagesQueued As Integer = 0
    Private _pages As New ThreadSafeBlockingQueue(Of String)


    Public Sub New(conStr As String, l As IProLeagues)
        _league = l
        _conStr = conStr
    End Sub

    Public Sub FetchPlayerNews()
        Util.LogMeWithTimestamp("Starting get pages thread.")
        Threading.ThreadPool.QueueUserWorkItem(AddressOf QueuePages)

    End Sub

    Private Sub FillPlayersQueue()
        Util.LogMeWithTimestamp("Filling player queue.")
        Dim allNFLPlayers As List(Of Player) = DataUtility.GetAllPlayersBySport(_conStr, _league.GetSportName)
        For Each player As Player In allNFLPlayers
            _players.Enqueue(player)
        Next
        Util.LogMeWithTimestamp("Finished filling player queue.")
    End Sub

    Private Sub QueuePages()
        Dim count As Integer = 1
        While True
            Try
                Dim p As Player = _players.Dequeue()
                If p IsNot Nothing Then
                    Util.LogMeWithTimestamp("Getting page - " + p.Link + ".")
                    Dim page As String = Util.GetWebPageAsString(p.Link)
                    Dim tagBegin As Integer = page.IndexOf("<table class=""wisfb_standard"">")
                    Dim tagEnd As Integer
                    If tagBegin = -1 Then
                        Util.LogMeWithTimestamp("No more data. Exiting thread.")
                        _finishedQueueingPages = True
                        Exit While
                    Else
                        tagEnd = page.IndexOf("</table>", tagBegin)
                        Dim playerTable As String = page.Substring(tagBegin, tagEnd - tagBegin)
                        _pages.Enqueue(playerTable)
                        _numPagesQueued += 1
                    End If
                End If

            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error while queueing pages.")
            End Try
        End While
    End Sub

End Class
