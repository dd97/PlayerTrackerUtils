Public Class PlayerNews

    Public Sub New()

    End Sub

    Public Sub New(ByVal player As Player, ByVal latest As String, ByVal injury As String, ByVal fantasy As String)
        Me.PlayerInfo = player
        Me.LatestNews.Add(latest)
        Me.InjuryNews.Add(injury)
        Me.FantasyNews.Add(fantasy)
    End Sub

    Public Sub New(ByVal player As Player, ByVal latest As String, ByVal injury As String, ByVal fantasy As String, dteUpdated As String)
        Me.PlayerInfo = player
        Me.LatestNews.Add(latest)
        Me.InjuryNews.Add(injury)
        Me.FantasyNews.Add(fantasy)
        Me.DateUpdated = dteUpdated
    End Sub

    Public Property PlayerInfo() As Player = Nothing
    Public Property LatestNews() As New List(Of String)
    Public Property InjuryNews() As New List(Of String)
    Public Property FantasyNews() As New List(Of String)
    Public Property DateUpdated() As String = Date.MinValue
    Public Property RawNews As String = String.Empty

End Class


Public Enum PlayerNewsType
    Latest = 0
    Injury = 1
    Fantasy = 2
End Enum
