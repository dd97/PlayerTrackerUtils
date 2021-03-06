﻿Imports CommonUtilities
Imports FantasyHelperObjects

Public Class FoxSportsPlayerNewsScraper

    Private _league As IProLeagues
    Private _players As New ThreadSafeBlockingQueue(Of Player)
    Private _conStr As String
    Private _finishedQueueingPages As Boolean = False
    Private _numPagesQueued As Integer = 0
    Private _rawNewsPages As New ThreadSafeBlockingQueue(Of PlayerNews)
    Private _processedNewsPages As New ThreadSafeBlockingQueue(Of PlayerNews)
    Private _playersCount As Integer = 0
    Private _playerNewPagesCount As Integer = 0
    Private _numNewsPagesProcessed As Integer = 0


    Public Sub New(conStr As String, l As IProLeagues)
        _league = l
        _conStr = conStr
    End Sub

    Public Sub FetchPlayerNews()
        Util.LogMeWithTimestamp("Filling players queue.")
        FillPlayersQueue()
        Util.LogMeWithTimestamp("Starting get news pages thread.")
        Threading.ThreadPool.QueueUserWorkItem(AddressOf QueueNewsPages)
        Threading.ThreadPool.QueueUserWorkItem(AddressOf ParsePlayerNewsPage)
    End Sub

    Private Sub ParseNews(ByVal playerNewsInfo As PlayerNews, row As String)
        Dim tagBegin, tagEnd As Integer
        tagBegin = row.IndexOf("<span class=""wisfb_newsDate"">", 0)
        If tagBegin <> -1 Then
            tagBegin = row.IndexOf(">", tagBegin)
            tagEnd = row.IndexOf("</span>", tagBegin)
            Dim dateAndSource As String = row.Substring(tagBegin + 1, tagEnd - tagBegin - 1)
            Dim source As String
            Dim strDate As String
            Dim d As Date
            Dim tag1, tag2 As Integer
            tag1 = dateAndSource.IndexOf("<strong>", 0)
            If tag1 <> -1 Then
                tag1 = dateAndSource.IndexOf(">", tag1)
                tag2 = dateAndSource.IndexOf("</strong>", tag1)
                source = dateAndSource.Substring(tag1 + 1, tag2 - tag1 - 1).Trim()
                strDate = dateAndSource.Substring(tag2 + 9).Trim()
                'If strDate.Contains("/") Then
                '    Dim left As String = strDate.Split("/")(0)
                '    Dim right As String = strDate.Split("/")(1)
                '    ParseFoxDate(left, right)
                'End If
                strDate = strDate.Replace("/", Date.Now.Year.ToString)
                strDate = strDate.Substring(0, strDate.Length - 3) + "m"

                If Not Date.TryParse(strDate, d) Then
                    Util.LogMeWithTimestamp("Bad date format - '" + strDate + "'.")
                End If


            End If

        Else
            Util.LogMeWithTimestamp("Bad data found while parsing news.")
        End If


    End Sub

    Private Function ParseFoxDate(left As String, right As String) As String
        'left side will be short month name and the day
        'right side will be the time of day
        Dim d As Date = Date.MinValue
        left = left.Trim()
        left += " " + Date.Now.Year.ToString
        Date.TryParse(left, d)


    End Function


    Private Sub ParsePlayerNewsPage()
        Dim timesQueueWasEmpty As Integer = 0
        While True
            Try
                Dim playerNewsInfo As PlayerNews = _rawNewsPages.Dequeue()
                If playerNewsInfo IsNot Nothing Then
                    Dim news As String = playerNewsInfo.RawNews
                    If Not String.IsNullOrEmpty(news) Then
                        Dim tagBegin, tagEnd As Integer
                        While True
                            Try
                                tagBegin = news.IndexOf("<tr", tagEnd)
                                If tagBegin = -1 Then
                                    Exit While
                                End If
                                tagEnd = news.IndexOf("</tr>", tagBegin)
                                Dim row As String = news.Substring(tagBegin, tagEnd - tagBegin + 5)
                                ParseNews(playerNewsInfo, row)
                            Catch ex As Exception
                                Util.LogMeWithTimestamp(ex, "Error while parsing player.")
                            End Try
                        End While
                        _processedNewsPages.Enqueue(playerNewsInfo)
                        _numNewsPagesProcessed += 1
                    Else
                        timesQueueWasEmpty += 1
                        If timesQueueWasEmpty > 6 And IsProcessingComplete() Then
                            Util.LogMeWithTimestamp("Pages queue was empty " + timesQueueWasEmpty.ToString + " times.")
                            Exit While
                        End If
                    End If
                End If

            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error while parsing page.")
            End Try

        End While
    End Sub

    Private Function IsProcessingComplete() As Boolean
        Return (_numPagesQueued = _numNewsPagesProcessed) And _finishedQueueingPages
    End Function

    Private Sub FillPlayersQueue()
        Util.LogMeWithTimestamp("Filling player queue.")
        Dim allNFLPlayers As List(Of Player) = DataUtility.GetAllPlayersBySport(_conStr, _league.GetSportName)
        _playersCount = allNFLPlayers.Count
        For Each player As Player In allNFLPlayers
            _players.Enqueue(player)
        Next
        Util.LogMeWithTimestamp("Finished filling player queue.")
    End Sub

    Private Sub QueueNewsPages()
        Dim count As Integer = 1
        While True
            Try
                Dim p As Player = _players.Dequeue()
                If p IsNot Nothing Then
                    Dim rawNews As New PlayerNews()
                    rawNews.PlayerInfo = p
                    Dim newsLink As String = p.Link + "-news"
                    Util.LogMeWithTimestamp("Getting page - " + newsLink + ".")
                    _playerNewPagesCount += 1
                    Dim page As String = Util.GetWebPageAsString(newsLink)
                    Dim tagBegin As Integer = page.IndexOf("<table id=""wisfb_newsTable"">")
                    Dim tagEnd As Integer
                    If tagBegin <> -1 Then
                        tagEnd = page.IndexOf("</table>", tagBegin)
                        rawNews.RawNews = page.Substring(tagBegin, tagEnd - tagBegin)
                        _rawNewsPages.Enqueue(rawNews)
                        _numPagesQueued += 1
                    Else
                        Util.LogMeWithTimestamp("No news for player.")
                    End If
                Else
                    If _playerNewPagesCount = _playersCount Then
                        _finishedQueueingPages = True
                        Util.LogMeWithTimestamp("No more data. Exiting news pages thread.")
                        Exit While
                    End If
                End If

            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error while queueing pages.")
            End Try
        End While
    End Sub

End Class
