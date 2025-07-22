Imports Newtonsoft.Json

Public Class Staff
    Public Property StaffId As Integer
    Public Property FirstName As String
    Public Property LastName As String
    Public Property Position As String
    Public Property Email As String
    Public Property PhoneNumber As Integer
    Public Property HireDate As Date
    Public Property Salary As Decimal
    Public Property ImagePath As String ' Path to staff photo

    Public ReadOnly Property FullName As String
        Get
            Return $"{FirstName} {LastName}"
        End Get
    End Property
End Class