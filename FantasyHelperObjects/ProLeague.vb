Imports System.Data.SqlClient

<Serializable()> _
Public Class ProLeague
    Implements IProLeagues

    Private _conStr As String
    Private _sportName As String
    Private _teams As List(Of ProTeam)
    Private _positions As List(Of ProPosition)

    Public Sub New(conStr As String, sport As String)
        _conStr = conStr
        If sport.ToUpper <> "MLB" And sport.ToUpper <> "NFL" And sport.ToUpper <> "NHL" And sport.ToUpper <> "NBA" Then
            Throw New Exception("Invalid sport passed.")
        End If
        _sportName = sport.ToUpper
        _teams = New List(Of ProTeam)
        _positions = New List(Of ProPosition)
        FillTeams()
        FillPositions()

    End Sub
    Private Sub FillTeams()
        Dim qry As String = "select team_id, a.sport_id, team_name, team_long_name from Teams a " + _
                            "join Sports b on a.sport_id = b.sport_id where b.sport_name = '" + GetSportName() + "'"

        Using sa As New SqlDataAdapter(qry, New SqlConnection(_conStr))
            Using ds As New DataSet
                sa.Fill(ds)
                For Each r As DataRow In ds.Tables(0).Rows
                    _teams.Add(DataUtility.GetProTeamObject(r))
                Next
            End Using
        End Using
    End Sub
    Private Sub FillPositions()
        Dim qry As String = "select position_id, a.sport_id, a.nbc_id, position_name, position_long_name from Positions a " + _
                            "join Sports b on a.sport_id = b.sport_id where b.sport_name = '" + GetSportName() + "'"

        Using sa As New SqlDataAdapter(qry, New SqlConnection(_conStr))
            Using ds As New DataSet
                sa.Fill(ds)
                For Each r As DataRow In ds.Tables(0).Rows
                    _positions.Add(DataUtility.GetProPositionObject(r))
                Next
            End Using
        End Using
    End Sub
    Public Function GetTeamAbbrev(ByVal name As String) As String Implements IProLeagues.GetTeamAbbrev

        For Each team As ProTeam In _teams
            If team.TeamLongName.ToLower = name.ToLower Then
                Return team.TeamName
            End If
        Next

        Return Nothing
    End Function

    Public Function GetTeamLongName(name As String) As String Implements IProLeagues.GetTeamLongName
        For Each team As ProTeam In _teams
            If team.TeamName.ToLower = name.ToLower Then
                Return team.TeamLongName
            End If
        Next

        Return Nothing
    End Function

    Public Function IsTeam(ByVal name As String) As Boolean Implements IProLeagues.IsTeam
        For Each t As ProTeam In _teams
            If t.TeamName.ToLower = name.ToLower Or t.TeamLongName.ToLower = name.ToLower Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function IsPosition(ByVal name As String) As Boolean Implements IProLeagues.IsPosition
        For Each p As ProPosition In _positions
            If p.PositionName.ToLower = name.ToLower Then
                Return True
            End If
        Next
        Return False
    End Function

    Public Function GetSportName() As String Implements IProLeagues.GetSportName
        Return _sportName
    End Function

    Public Function GetSportId() As String Implements IProLeagues.GetSportId
        Return _teams(0).SportId
    End Function

    Public Function GetAllPositions() As System.Collections.Generic.List(Of ProPosition) Implements IProLeagues.GetAllPositions
        Return _positions
    End Function

    Public Function GetAllTeams() As System.Collections.Generic.List(Of ProTeam) Implements IProLeagues.GetAllTeams
        Return _teams
    End Function

    Public Function GetPositionId(positionName As String) As String Implements IProLeagues.GetPositionId
        For Each p As ProPosition In _positions
            If p.PositionName.ToLower = positionName.ToLower Then
                Return p.PositionId
            End If
        Next
        Return Nothing
    End Function

    Public Function GetTeamId(teamName As String) As String Implements IProLeagues.GetTeamId
        For Each p As ProTeam In _teams
            If p.TeamName.ToLower = teamName.ToLower Or p.TeamLongName.ToLower = teamName.ToLower Then
                Return p.TeamId
            End If
        Next
        Return Nothing
    End Function

    Public Function GetNBCSportName() As String Implements IProLeagues.GetNBCSportName
        Select Case _sportName.ToLower
            Case "nfl"
                Return "fb"
            Case "mlb"
                Return "mlb"
            Case "nba"
                Return "nba"
            Case "nhl"
                Return "nhl"
            Case Else
                Return Nothing
        End Select
    End Function
End Class
