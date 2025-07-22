Imports System.IO
Imports Newtonsoft.Json

Public Class DataService
    Private ReadOnly _dataPath As String = Path.Combine(Application.StartupPath, "Data")

    Public Sub New()
        If Not Directory.Exists(_dataPath) Then
            Directory.CreateDirectory(_dataPath)
        End If
        InitializeDataFiles()
    End Sub

    Private Sub InitializeDataFiles()
        InitializeFile(Of List(Of Staff))("staff.json")
        InitializeFile(Of List(Of Customer))("customers.json")
        InitializeFile(Of List(Of Product))("products.json")
        InitializeFile(Of List(Of Sale))("sales.json")
    End Sub

    Private Sub InitializeFile(Of T)(fileName As String)
        Dim filePath = Path.Combine(_dataPath, fileName)
        If Not File.Exists(filePath) Then
            File.WriteAllText(filePath, JsonConvert.SerializeObject(Activator.CreateInstance(Of T)))
        End If
    End Sub

    Public Function LoadData(Of T)(fileName As String) As T
        Dim filePath = Path.Combine(_dataPath, fileName)
        Dim jsonData = File.ReadAllText(filePath)
        Return JsonConvert.DeserializeObject(Of T)(jsonData)
    End Function

    Public Sub SaveData(Of T)(fileName As String, data As T)
        Dim filePath = Path.Combine(_dataPath, fileName)
        Dim jsonData = JsonConvert.SerializeObject(data, Formatting.Indented)
        File.WriteAllText(filePath, jsonData)
    End Sub

    ' Helper methods for each entity type
    Public Function GetStaff() As List(Of Staff)
        Return LoadData(Of List(Of Staff))("staff.json")
    End Function

    Public Sub SaveStaff(staff As List(Of Staff))
        SaveData("staff.json", staff)
    End Sub

    Public Function GetCustomers() As List(Of Customer)
        Return LoadData(Of List(Of Customer))("customers.json")
    End Function

    Public Sub SaveCustomers(customers As List(Of Customer))
        SaveData("customers.json", customers)
    End Sub

    Public Function GetProducts() As List(Of Product)
        Return LoadData(Of List(Of Product))("products.json")
    End Function

    Public Sub SaveProducts(products As List(Of Product))
        SaveData("products.json", products)
    End Sub

    Public Function GetSales() As List(Of Sale)
        Return LoadData(Of List(Of Sale))("sales.json")
    End Function

    Public Sub SaveSales(sales As List(Of Sale))
        SaveData("sales.json", sales)
    End Sub
End Class