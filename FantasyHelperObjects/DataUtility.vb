Imports System.Data.SqlClient
Imports CommonUtilities

Public Class DataUtility

    Public Shared Function GetProPositionObject(row As DataRow) As ProPosition
        Dim p As New ProPosition()
        p.PositionId = Util.GetStringSafely(row("position_id"))
        p.PositionName = Util.GetStringSafely(row("position_name"))
        p.PositionLongName = Util.GetStringSafely(row("position_long_name"))
        p.SportId = Util.GetStringSafely(row("sport_id"))
        p.NBCId = Util.GetStringSafely(row("nbc_id"))
        Return p
    End Function

    Public Shared Function GetProTeamObject(row As DataRow) As ProTeam
        Dim p As New ProTeam
        p.SportId = Util.GetStringSafely(row("sport_id"))
        p.TeamId = Util.GetStringSafely(row("team_id"))
        p.TeamName = Util.GetStringSafely(row("team_name"))
        p.TeamLongName = Util.GetStringSafely(row("team_long_name"))
        Return p
    End Function

    Public Shared Function GetPlayerObject(ByVal row As DataRow) As Player
        Dim p As New Player()
        p.DbID = Util.GetStringSafely(row("player_id"))
        p.WebId = Util.GetStringSafely(row("player_id_web"))
        p.FirstName = Util.GetStringSafely(row("player_first_name"))
        p.LastName = Util.GetStringSafely(row("player_last_name"))
        p.Position = Util.GetStringSafely(row("position_name"))
        p.TeamName = Util.GetStringSafely(row("team_name"))
        p.Sport = Util.GetStringSafely(row("sport_name"))
        p.Link = Util.GetStringSafely(row("player_link"))
        p.TwitterHandle = Util.GetStringSafely(row("player_twitter_handle"))
        p.Height = Util.GetStringSafely(row("player_height"))
        p.Weight = Util.GetStringSafely(row("player_weight"))
        p.Age = Util.GetStringSafely(row("player_age"))
        p.Birthdate = Util.GetStringSafely(row("player_birthday"))
        p.YearsExperience = Util.GetStringSafely(row("player_experience"))
        p.College = Util.GetStringSafely(row("player_college"))
        Return p
    End Function

    Public Shared Function GetAllPlayers(ByVal conStr As String) As List(Of Player)
        Dim players As New List(Of Player)
        Try
            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
         "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
            "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
            " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        players.Add(GetPlayerObject(row))
                    Next
                End Using
            End Using
            Return players
        Catch ex As Exception
            Util.LogMeWithTimestamp(ex, "Error getting all players from database. ")
            Return Nothing
        End Try

    End Function

    Public Shared Function GetAllPlayersBySport(ByVal conStr As String, ByVal sport As String) As List(Of Player)
        Dim players As New List(Of Player)
        Try
            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers where player_sport = @sport"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
         "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
            "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
            " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web " + _
             "where b.sport_name = @sport"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet
                    sa.SelectCommand.Parameters.AddWithValue("@sport", sport)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        players.Add(GetPlayerObject(row))
                    Next
                End Using
            End Using
            Return players
        Catch ex As Exception
            LogMe(ex, "Error getting all players from database. ")
            Return Nothing
        End Try

    End Function

    Public Shared Function GetPlayerByFullName(ByVal conStr As String, ByVal name As String, ByVal sport As String) As Player
        Try
            Dim p As Player = Nothing

            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers where player_full_name = @name " + _
                "and player_sport = @sport"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
         "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
            "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
            " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web " + _
            "where a.player_full_name = @name and b.sport_name = @sport"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@name", name)
                    sa.SelectCommand.Parameters.AddWithValue("@sport", sport)
                    sa.Fill(ds)
                    If ds.Tables(0).Rows.Count > 0 Then
                        p = GetPlayerObject(ds.Tables(0).Rows(0))
                    End If

                End Using
            End Using
            Return p

        Catch ex As Exception
            LogMe(ex, "Error getting player(s) by name. ")
            Return Nothing
        End Try

    End Function


    Public Shared Function GetPlayerByName(ByVal conStr As String, ByVal name As String, ByVal sport As String) As List(Of Player)
        Try
            Dim players As New List(Of Player)

            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers where player_last_name like @name " + _
                "and player_sport = @sport order by player_last_name, player_first_name"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
         "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
            "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
            " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web " + _
            "where a.player_last_name like @name and b.sport_name = @sport order by a.player_last_name, a.player_first_name"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@name", name + "%")
                    sa.SelectCommand.Parameters.AddWithValue("@sport", sport)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        players.Add(GetPlayerObject(row))
                    Next
                End Using
            End Using
            Return players

        Catch ex As Exception
            LogMe(ex, "Error getting player(s) by name. ")
            Return Nothing
        End Try

    End Function

    Public Shared Function GetPlayerByTeam(ByVal conStr As String, ByVal team As String, ByVal sport As String) As List(Of Player)
        Try
            Dim players As New List(Of Player)

            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers where player_team = @team " + _
                "and player_sport = @sport order by player_last_name, player_first_name"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
         "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
            "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
            " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web " + _
             " where c.team_name = @team and b.sport_name = @sport order by a.player_last_name, a.player_first_name"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@team", team)
                    sa.SelectCommand.Parameters.AddWithValue("@sport", sport)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        players.Add(GetPlayerObject(row))
                    Next
                End Using
            End Using
            Return players

        Catch ex As Exception
            LogMe(ex, "Error getting players by team. ")
            Return Nothing
        End Try
    End Function

    Public Shared Function GetPlayerByPosition(ByVal conStr As String, ByVal position As String, ByVal sport As String) As List(Of Player)
        Try
            Dim players As New List(Of Player)

            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers where player_position = @position " + _
                "and player_sport = @sport order by player_last_name, player_first_name"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
        "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
           "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
           " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web " + _
            " where d.position_name = @position and b.sport_name = @sport order by a.player_last_name, a.player_first_name"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@position", position)
                    sa.SelectCommand.Parameters.AddWithValue("@sport", sport)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        players.Add(GetPlayerObject(row))
                    Next
                End Using
            End Using
            Return players

        Catch ex As Exception
            LogMe(ex, "Error getting players by position. ")
            Return Nothing
        End Try

    End Function

    Public Shared Function GetPlayerByID(ByVal conStr As String, ByVal id As String, ByVal sport As String) As Player
        Try
            Dim players As New List(Of Player)

            Dim qry As String = "SELECT [player_id],[player_id_web],[player_first_name],[player_last_name]," + _
                "[player_full_name],[player_position],[player_team],[player_sport],[player_link] FROM " + _
                "FantasySports..FantasyPlayers where player_id_web = @id " + _
                "and player_sport = @sport"

            qry = "select a.player_id,a.player_id_web,player_first_name,player_last_name," + _
         "a.player_full_name,d.position_name,c.team_name,b.sport_name,a.player_link,f.player_twitter_handle, " + _
         " a.player_height,a.player_weight,a.player_age,a.player_birthday,a.player_experience,a.player_college FROM " + _
      "FantasySports..FantasyPlayers a join Sports b on a.player_sport_id = b.sport_id " + _
         "join Teams c on a.player_team_id = c.team_id join Positions d on a.player_position_id = d.position_id " + _
         " left join FantasyPlayersTwitter f on a.player_id_web = f.player_id_web " + _
          " where a.player_id_web = @id and b.sport_name = @sport"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@id", id)
                    sa.SelectCommand.Parameters.AddWithValue("@sport", sport)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows 'only 1 result expected
                        Return GetPlayerObject(row)
                    Next
                End Using
            End Using

        Catch ex As Exception
            LogMe(ex, "Error getting player by id. ")
        End Try

        Return Nothing
    End Function

    Public Shared Function GetNews(ByVal conStr As String, ByVal id As String, ByVal newsType As PlayerNewsType) As String

        Dim strNewsType As String
        If newsType = PlayerNewsType.Fantasy Then
            strNewsType = "fantasy_news"
        ElseIf newsType = PlayerNewsType.Injury Then
            strNewsType = "injuries"
        ElseIf newsType = PlayerNewsType.Latest Then
            strNewsType = "recent_news"
        Else
            Return Nothing
        End If

        Try
            Dim qry As String = "select " + strNewsType + " from " + _
                "FantasyPlayersNews where player_id_web = @id"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@id", id)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows 'only 1 result expected
                        Return Util.GetStringSafely(row(0))
                    Next
                End Using
            End Using

        Catch ex As Exception
            LogMe(ex, "Error getting player news. ")
        End Try

        Return Nothing
    End Function

    Public Shared Sub UpdateUserLastVisited(ByVal conStr As String, ByVal user As String)
        Try
            Dim qry As String = "update WebUsers set last_visited = getdate() WHERE user_id = @user"
            Using cmd As New SqlCommand(qry, New SqlConnection(conStr))
                cmd.Connection.Open()
                cmd.Parameters.AddWithValue("@user", user)
                cmd.ExecuteNonQuery()
                cmd.Connection.Close()
            End Using
        Catch ex As Exception
            LogMe(ex, "Error updating user last visited time. ")
        End Try
    End Sub

    Public Shared Function AuthenticateUser(ByVal conStr As String, ByVal user As String, ByVal pass As String) As String
        Try
            Dim qry As String = "SELECT user_id FROM " + _
                   "WebUsers WHERE user_id = @user AND password = @pass"
            Dim id As String = Nothing
            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@user", user)
                    sa.SelectCommand.Parameters.AddWithValue("@pass", pass)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        id = Util.GetStringSafely(row(0))
                    Next
                End Using
            End Using
            Return id

        Catch ex As Exception
            LogMe(ex, "Error authenticating user. ")
        End Try

        Return Nothing
    End Function

    Public Shared Function CreateUser(ByVal conStr As String, ByVal user As String, ByVal pass As String) As Boolean
        Try
            Dim qry As String = "SELECT user_id FROM " + _
                  "WebUsers WHERE user_id = @user"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@user", user)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        Return False 'user id already exists
                    Next
                End Using
            End Using

            qry = "INSERT INTO WebUsers (user_id,password,date_updated,date_inserted,last_visited) VALUES " + _
                    "(@user,@pass,getdate(),getdate(),getdate())"
            Using cmd As New SqlCommand(qry, New SqlConnection(conStr))
                cmd.Connection.Open()
                cmd.Parameters.AddWithValue("@user", user)
                cmd.Parameters.AddWithValue("@pass", pass)
                cmd.ExecuteNonQuery()
                cmd.Connection.Close()
            End Using

            Return True
        Catch ex As Exception
            LogMe(ex, "Error creating user. ")
            Return False
        End Try

    End Function

    Public Shared Function TrackPlayer(ByVal conStr As String, ByVal user As String, ByVal playerID As String) As TrackResult
        Try
            Dim qry As String = "SELECT user_id FROM " + _
                "PlayerTrackingKeys WHERE user_id = @user AND player_id_web = @id"
            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@user", user)
                    sa.SelectCommand.Parameters.AddWithValue("@id", playerID)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        Return TrackResult.Duplicate 'player already being tracked
                    Next
                End Using
            End Using
            qry = "INSERT INTO PlayerTrackingKeys (user_id, player_id_web, date_inserted) VALUES " + _
            "(@user, @id, getdate())"
            Using cmd As New SqlCommand(qry, New SqlConnection(conStr))
                cmd.Connection.Open()
                cmd.Parameters.AddWithValue("@user", user)
                cmd.Parameters.AddWithValue("@id", playerID)
                cmd.ExecuteNonQuery()
                cmd.Connection.Close()
            End Using
            Return TrackResult.Success
        Catch ex As Exception
            LogMe(ex, "Error tracking player. ")
            Return TrackResult.Fail
        End Try

    End Function

    Public Shared Function RemovedTrackedPlayer(ByVal conStr As String, ByVal user As String, ByVal playerID As String, sport As IProLeagues) As TrackResult
        Try
            Dim qry As String = "DELETE FROM " + _
                "PlayerTrackingKeys WHERE user_id = @user AND player_id_web = @id;" + _
                "select count(1) as 'count' from " + _
                "PlayerTrackingKeys a join FantasyPlayers b on a.player_id_web = b.player_id_web " + _
                "WHERE a.user_id = @user and b.player_sport_id = @sportId"

            Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                Using ds As New DataSet()
                    sa.SelectCommand.Parameters.AddWithValue("@user", user)
                    sa.SelectCommand.Parameters.AddWithValue("@id", playerID)
                    sa.SelectCommand.Parameters.AddWithValue("@sportId", sport.GetSportId)
                    sa.Fill(ds)
                    For Each row As DataRow In ds.Tables(0).Rows
                        Dim count As String = Util.GetStringSafely(row("count"))
                        If count = "0" Then
                            Return TrackResult.NoPlayersTracked
                        End If
                    Next
                End Using
            End Using
            Return TrackResult.Success
          
        Catch ex As Exception
            LogMe(ex, "Error removing tracked player. ")
            Return TrackResult.Fail
        End Try
    End Function

    Public Shared Function RemovedTrackedPlayer(ByVal conStr As String, ByVal user As String, ByVal playerID As String) As TrackResult
        Try
            Dim qry As String = "DELETE FROM " + _
                "PlayerTrackingKeys WHERE user_id = @user AND player_id_web = @id; "

            Using cmd As New SqlCommand(qry, New SqlConnection(conStr))
                cmd.Connection.Open()
                cmd.Parameters.AddWithValue("@user", user)
                cmd.Parameters.AddWithValue("@id", playerID)
                cmd.ExecuteNonQuery()
                cmd.Connection.Close()
            End Using
            Return TrackResult.Success

        Catch ex As Exception
            LogMe(ex, "Error removing tracked player. ")
            Return TrackResult.Fail
        End Try
    End Function

    Public Shared Function GetTrackedPlayers(ByVal conStr As String, ByVal user As String, ByVal sport As IProLeagues) As List(Of Player)
        Dim positions As List(Of ProPosition) = sport.GetAllPositions()
        Dim players As New List(Of Player)

        Try
            For i As Integer = 0 To positions.Count - 1 'orders by position
                Dim qry As String = "SELECT p.player_id, p.player_id_web,p.player_first_name,p.player_last_name," + _
                 "p.player_full_name,p.player_position,p.player_team,p.player_sport,p.player_link FROM " + _
                 "PlayerTrackingKeys t join FantasyPlayers p on t.player_id_web = p.player_id_web WHERE " + _
                 "t.user_id = @user AND p.player_position = @position and p.player_sport = @sport " + _
                 "order by player_last_name, player_first_name"

                qry = "SELECT p.player_id, p.player_id_web,p.player_first_name,p.player_last_name," + _
    "p.player_full_name,d.position_name,c.team_name,b.sport_name,p.player_link,f.player_twitter_handle " + _
    "FROM PlayerTrackingKeys t join FantasyPlayers p on t.player_id_web = p.player_id_web " + _
     "join Sports b on p.player_sport_id = b.sport_id join Teams c on p.player_team_id = c.team_id " + _
       "join Positions d on p.player_position_id = d.position_id " + _
       " left join FantasyPlayersTwitter f on p.player_id_web = f.player_id_web " + _
       "WHERE t.user_id = @user AND d.position_name = @position and b.sport_name = @sport " + _
       "order by p.player_last_name, p.player_first_name"

                Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
                    Using ds As New DataSet()
                        sa.SelectCommand.Parameters.AddWithValue("@user", user)
                        sa.SelectCommand.Parameters.AddWithValue("@position", positions(i).PositionName)
                        sa.SelectCommand.Parameters.AddWithValue("@sport", sport.GetSportName())
                        sa.Fill(ds)
                        For Each row As DataRow In ds.Tables(0).Rows
                            players.Add(GetPlayerObject(row))
                        Next
                    End Using
                End Using
            Next

            Return players
        Catch ex As Exception
            LogMe(ex, "Error getting tracked players. ")
            Return Nothing
        End Try

    End Function

    Public Shared Sub TransferTrackedPlayers(ByVal conStr As String, ByVal userSource As String, ByVal userDest As String)
        Dim playerIds As String = Nothing
        Dim qry As String = "SELECT player_id_web from PlayerTrackingKeys where user_id = @user"
        Using sa As New SqlDataAdapter(qry, New SqlConnection(conStr))
            Using ds As New DataSet()
                sa.SelectCommand.Parameters.AddWithValue("@user", userSource)
                sa.Fill(ds)
                For Each row As DataRow In ds.Tables(0).Rows
                    playerIds += "," + CType(row(0), String)
                Next
            End Using
        End Using
        playerIds = Mid(playerIds, 2)
        If Not String.IsNullOrEmpty(playerIds) Then
            Dim arrPlayerIds() As String = playerIds.Split(",")
            For Each id As String In arrPlayerIds
                TrackPlayer(conStr, userDest, id)
                RemovedTrackedPlayer(conStr, userSource, id)
            Next
        End If
    End Sub

    Private Shared Sub LogMe(str As String)
        Util.LogMeWithTimestamp(str)
    End Sub
    Private Shared Sub LogMe(ex As Exception, str As String)
        Util.LogMeWithTimestamp(ex, str)
    End Sub

    Public Shared Sub InsertPlayer(conStr As String, p As Player)
        Dim qry As String = "insert into FantasyPlayers (player_id_web,player_first_name,player_last_name," +
                           "player_full_name,player_position_id,player_team_id,player_sport_id,player_link," +
                           "player_height,player_weight,player_age,player_birthday,player_experience," +
                           "player_college,date_inserted) values (@id,@fname,@lname,@fullname,@position,@team," +
                           "@sport,@link,@height,@weight,@age,@bday,@exp,@college,@dateins)"

        Try
            Using cmd As New SqlCommand(qry, New SqlConnection(conStr))
                cmd.Parameters.AddWithValue("@id", p.WebId)
                cmd.Parameters.AddWithValue("@fname", p.FirstName)
                cmd.Parameters.AddWithValue("@lname", p.LastName)
                cmd.Parameters.AddWithValue("@fullname", p.FullName)
                cmd.Parameters.AddWithValue("@position", p.Position)
                cmd.Parameters.AddWithValue("@team", p.TeamName)
                cmd.Parameters.AddWithValue("@sport", p.Sport)
                cmd.Parameters.AddWithValue("@link", p.Link)
                cmd.Parameters.AddWithValue("@height", p.Height)
                cmd.Parameters.AddWithValue("@weight", p.Weight)
                cmd.Parameters.AddWithValue("@age", p.Age)
                cmd.Parameters.AddWithValue("@bday", p.Birthdate)
                cmd.Parameters.AddWithValue("@exp", p.YearsExperience)
                cmd.Parameters.AddWithValue("@college", p.College)
                cmd.Parameters.AddWithValue("@dateins", Now)
                cmd.Connection.Open()
                LogMe("Inserting player - " + p.FullName + ".")
                cmd.ExecuteNonQuery()
                cmd.Connection.Close()
            End Using
        Catch ex As Exception
            Util.LogMeWithTimestamp(ex, "Error inserting player.")
        End Try


    End Sub

    Public Shared Sub DeletePlayerTable(conStr As String, l As IProLeagues)
        Try
            Dim qry1 As String = ""

            Using cmd As New SqlCommand(qry1, New SqlConnection(conStr))
                If l.GetSportName.ToLower = "nfl" Then
                    'qry1 = "delete from FantasyPlayers where player_sport_id = '" & _league.GetSportId & "' and player_position_id <> '" + _league.GetPositionId("D") + "'"
                    qry1 = "delete from FantasyPlayers where player_sport_id = @sportId and player_position_id <> @posId"
                    cmd.CommandText = qry1
                    cmd.Parameters.AddWithValue("@sportId", l.GetSportId)
                    cmd.Parameters.AddWithValue("@posId", l.GetPositionId("D"))
                Else
                    'qry1 = "delete from FantasyPlayers where player_sport_id = '" & _league.GetSportId & "'"
                    qry1 = "delete from FantasyPlayers where player_sport_id = @sportId"
                    cmd.CommandText = qry1
                    cmd.Parameters.AddWithValue("@sportId", l.GetSportId)
                End If
                cmd.Connection.Open()
                Util.LogMeWithTimestamp("Removing all " & l.GetSportName & " players - " & qry1)
                cmd.ExecuteNonQuery()
                Util.LogMeWithTimestamp("Success!")
                cmd.Connection.Close()
            End Using
        Catch ex As Exception
            Util.LogMeWithTimestamp(ex, "Exception occurred deleting player table: ")
        End Try
    End Sub

End Class

Public Enum TrackResult
    Success = 0
    Fail = 1
    Duplicate = 2
    NoPlayersTracked = 3

End Enum