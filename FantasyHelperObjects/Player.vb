<Serializable()> _
Public Class Player

    Property WebId As String = String.Empty
    Property DbID As String = String.Empty
    Property Sport As String = String.Empty
    Property LastName As String = String.Empty
    Property FirstName As String = String.Empty
    Property Position As String = String.Empty
    Property TeamName As String = String.Empty
    Property Link As String = String.Empty
    Property TwitterHandle As String = String.Empty
    Property Height As String = String.Empty
    Property Weight As String = String.Empty
    Property Age As String = String.Empty
    Property Birthdate As String = String.Empty
    Property YearsExperience As String = String.Empty
    Property College As String = String.Empty
    Property Number As String = String.Empty

    ReadOnly Property FullName As String
        Get
            Return FirstName + " " + LastName
        End Get
    End Property
    ReadOnly Property FullNameLast As String
        Get
            Return LastName + ", " + FirstName
        End Get
    End Property

   
End Class
