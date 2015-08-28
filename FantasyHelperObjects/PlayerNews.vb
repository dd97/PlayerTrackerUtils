Public Class PlayerNews

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

    Private _player As Player
    Public Property PlayerInfo() As Player
        Get
            Return _player
        End Get
        Set(ByVal value As Player)
            _player = value
        End Set
    End Property

    Private _latestNews As String
    Public Property LatestNews() As String
        Get
            Return _latestNews
        End Get
        Set(ByVal value As String)
            _latestNews = value
        End Set
    End Property


    Private _injuryNews As String
    Public Property InjuryNews() As String
        Get
            Return _injuryNews
        End Get
        Set(ByVal value As String)
            _injuryNews = value
        End Set
    End Property


    Private _fantasyNews As String
    Public Property FantasyNews() As String
        Get
            Return _fantasyNews
        End Get
        Set(ByVal value As String)
            _fantasyNews = value
        End Set
    End Property

    Private _dateUpdated As String
    Public Property DateUpdated() As String
        Get
            Return _dateUpdated
        End Get
        Set(ByVal value As String)
            _dateUpdated = value
        End Set
    End Property



End Class


Public Enum PlayerNewsType
    Latest = 0
    Injury = 1
    Fantasy = 2
End Enum
