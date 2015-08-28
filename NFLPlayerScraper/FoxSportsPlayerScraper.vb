Imports CommonUtilities
Imports FantasyHelperObjects
Imports System.Web



Public Class FoxSportsPlayerScraper

    Public Players As New ThreadSafeBlockingQueue(Of Player)

    Private _rootUrl As String = String.Empty
    Private _playersUrl As String = String.Empty
    Private _pages As New ThreadSafeBlockingQueue(Of String)
    Private _numPagesQueued As Integer = 0
    Private _numPagesProcessed As Integer = 0
    Private _numPlayersFound As Integer = 0
    Private _league As IProLeagues
    Private _finishedQueueingPages As Boolean = False

    Public Sub New(rootUrl As String, playersUrl As String, l As IProLeagues)
        _rootUrl = rootUrl
        _playersUrl = playersUrl
        _league = l
    End Sub

    Public Sub FetchPlayers()
        Util.LogMeWithTimestamp("Starting get pages thread.")
        Threading.ThreadPool.QueueUserWorkItem(AddressOf QueuePages, _playersUrl)
        Util.LogMeWithTimestamp("Starting parse pages threads.")
        Threading.ThreadPool.QueueUserWorkItem(AddressOf ParsePage)
        Threading.ThreadPool.QueueUserWorkItem(AddressOf ParsePage)
    End Sub

    Public Function IsProcessingComplete() As Boolean
        Return (_numPagesQueued = _numPagesProcessed) And _finishedQueueingPages
    End Function

    Private Sub ParsePage()
        Dim timesQueueWasEmpty As Integer = 0
        While True
            Try
                Dim page As String = _pages.Dequeue()
                If Not String.IsNullOrEmpty(page) Then
                    Dim tagBegin, tagEnd As Integer
                    tagBegin = page.IndexOf("<tr", 0) 'skip first row (column names)
                    tagEnd = page.IndexOf("</tr>", tagBegin)
                    While True
                        Try
                            tagBegin = page.IndexOf("<tr", tagEnd)
                            If tagBegin = -1 Then
                                Exit While
                            End If
                            tagEnd = page.IndexOf("</tr>", tagBegin)
                            Dim row As String = page.Substring(tagBegin, tagEnd - tagBegin + 5)
                            ParsePlayer(row)
                        Catch ex As Exception
                            Util.LogMeWithTimestamp(ex, "Error while parsing player.")
                        End Try
                    End While
                    _numPagesProcessed += 1
                Else
                    timesQueueWasEmpty += 1
                    If timesQueueWasEmpty > 6 And IsProcessingComplete() Then
                        Util.LogMeWithTimestamp("Pages queue was empty " + timesQueueWasEmpty.ToString + " times.")
                        Exit While
                    End If
                End If
            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error while parsing page.")
            End Try

        End While
    End Sub

    ''' <summary>
    ''' Convert html table row into a player object.
    ''' columns expected are player, team, number, position, experience, height, weight, age, school
    ''' </summary>
    ''' <param name="row">row in html table containing player data</param>
    Private Sub ParsePlayer(row As String)
        Dim p As New Player()

        p.Sport = _league.GetSportId()

        Dim outerTagBegin, outerTagEnd, innerTagBegin, innerTagEnd As Integer
        outerTagBegin = row.IndexOf("<td", 0)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        Dim nameChunk As String = row.Substring(outerTagBegin, outerTagEnd - outerTagBegin + 5)

        Dim l As Link = Link.GetFirstLink(nameChunk)
        p.Link = _rootUrl + l.URL

        Util.LogMeWithTimestamp("Parsing player - " + p.Link + ".")

        innerTagBegin = nameChunk.IndexOf("<span", 0)
        innerTagBegin = nameChunk.IndexOf(">", innerTagBegin)
        innerTagEnd = nameChunk.IndexOf("</span>", innerTagBegin)
        p.FirstName = HttpUtility.HtmlDecode(nameChunk.Substring(innerTagBegin + 1, innerTagEnd - innerTagBegin - 1))

        innerTagBegin = nameChunk.IndexOf("<span", innerTagEnd)
        innerTagBegin = nameChunk.IndexOf(">", innerTagBegin)
        innerTagEnd = nameChunk.IndexOf("</span>", innerTagBegin)
        p.LastName = HttpUtility.HtmlDecode(nameChunk.Substring(innerTagBegin + 1, innerTagEnd - innerTagBegin - 1))

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        Dim teamChunk As String = row.Substring(outerTagBegin, outerTagEnd - outerTagBegin + 5)

        l = Link.GetFirstLink(teamChunk)
        Dim teamId As String = Nothing
        If l IsNot Nothing Then
            teamId = _league.GetTeamId(HttpUtility.HtmlDecode(l.Label).Trim())
        End If
        If String.IsNullOrEmpty(teamId) Then
            teamId = "-1"
        End If
        p.TeamName = teamId

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        p.Number = HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1))

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        p.Position = _league.GetPositionId(HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1)))

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        p.YearsExperience = HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1))

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        p.Height = HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1))

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        p.Weight = HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1))

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        Dim age As String = HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1))
        If Not Integer.TryParse(age, New Integer) Then
            age = "-1"
        End If
        p.Age = age

        outerTagBegin = row.IndexOf("<td", outerTagEnd)
        outerTagBegin = row.IndexOf(">", outerTagBegin)
        outerTagEnd = row.IndexOf("</td>", outerTagBegin)
        p.College = HttpUtility.HtmlDecode(row.Substring(outerTagBegin + 1, outerTagEnd - outerTagBegin - 1))

        _numPlayersFound += 1
        Players.Enqueue(p)
    End Sub

    Private Sub QueuePages(url As String)
        Dim count As Integer = 1
        While True
            Try
                Util.LogMeWithTimestamp("Getting page - " + url + ".")
                Dim page As String = Util.GetWebPageAsString(url)
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
                    Dim token As String = "&page=" + count.ToString
                    count += 1
                    url = url.Replace(token, "&page=" + count.ToString)
                End If
            Catch ex As Exception
                Util.LogMeWithTimestamp(ex, "Error while queueing pages.")
            End Try
        End While
    End Sub
End Class
