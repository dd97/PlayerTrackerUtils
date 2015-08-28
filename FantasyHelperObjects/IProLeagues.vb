Public Interface IProLeagues

    Function GetTeamAbbrev(ByVal name As String) As String
    Function GetTeamLongName(ByVal name As String) As String
    Function IsTeam(ByVal name As String) As Boolean
    Function IsPosition(ByVal name As String) As Boolean
    Function GetAllTeams() As List(Of ProTeam)
    Function GetAllPositions() As List(Of ProPosition)
    Function GetSportName() As String
    Function GetNBCSportName() As String
    Function GetPositionId(positionName As String) As String
    Function GetTeamId(teamName As String) As String
    Function GetSportId() As String

End Interface
