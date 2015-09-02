Public Class PlayerNews

    Public Sub New()

    End Sub

    Public Sub New(ByVal player As Player, ByVal latest As String, ByVal injury As String, ByVal fantasy As String)
        Me.PlayerInfo = player
        Me.LatestNews = latest
        Me.InjuryNews = injury
        Me.FantasyNews = fantasy
    End Sub

    Public Sub New(ByVal player As Player, ByVal latest As String, ByVal injury As String, ByVal fantasy As String, dteUpdated As String)
        Me.PlayerInfo = player
        Me.LatestNews = latest
        Me.InjuryNews = injury
        Me.DateUpdated = dteUpdated
        Me.FantasyNews = fantasy
    End Sub

    Public Property PlayerInfo() As Player = Nothing
    Public Property LatestNews() As String = String.Empty
    Public Property InjuryNews() As String = String.Empty
    Public Property FantasyNews() As String = String.Empty
    Public Property DateUpdated() As String = Date.MinValue
    Public Property RawNews As String = String.Empty

End Class


Public Enum PlayerNewsType
    Latest = 0
    Injury = 1
    Fantasy = 2
End Enum
