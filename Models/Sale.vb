Imports Newtonsoft.Json

Public Class Sale
    Public Property SaleId As Integer
    Public Property CustomerId As Integer
    Public Property StaffId As Integer ' New: Staff who made the sale
    Public Property SaleDate As DateTime
    Public Property TotalAmount As Decimal
    Public Property PaymentMethod As String
    Public Property Items As List(Of SaleItem)

    Public Sub New()
        Items = New List(Of SaleItem)()
    End Sub
End Class

Public Class SaleItem
    Public Property SaleItemId As Integer
    Public Property SaleId As Integer
    Public Property ProductId As Integer
    Public Property Quantity As Integer
    Public Property UnitPrice As Decimal

    Public ReadOnly Property Total As Decimal
        Get
            Return Quantity * UnitPrice
        End Get
    End Property
End Class