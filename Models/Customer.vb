Imports Newtonsoft.Json

Public Class Customer
    Public Property CustomerId As Integer
    Public Property FirstName As String
    Public Property LastName As String
    Public Property Address As String
    Public Property PhoneNumber As Integer

    Public ReadOnly Property FullName As String
        Get
            Return $"{FirstName} {LastName}"
        End Get
    End Property
End Class