Public Class Form1
    Private _dataService As DataService

    Public Sub New()
        InitializeComponent()
        _dataService = New DataService()
        SetupUI()
    End Sub

    Private Sub SetupUI()
        Me.Text = "Sales Management System"
        Me.WindowState = FormWindowState.Maximized

        ' Configure ToolStrip
        Dim toolStrip As New ToolStrip()
        With toolStrip
            .ImageScalingSize = New Size(32, 32)
            .Items.Add(CreateToolStripButton("Sales", "sale.png", AddressOf ShowSalesForm))
            .Items.Add(New ToolStripSeparator())
            .Items.Add(CreateToolStripButton("Products", "product.png", AddressOf ShowProductsForm))
            .Items.Add(New ToolStripSeparator())
            .Items.Add(CreateToolStripButton("Customers", "customer.png", AddressOf ShowCustomersForm))
            .Items.Add(New ToolStripSeparator())
            .Items.Add(CreateToolStripButton("Staff", "staff.png", AddressOf ShowStaffForm))
        End With

        Me.Controls.Add(toolStrip)
    End Sub

    Private Function CreateToolStripButton(text As String, iconName As String, handler As EventHandler) As ToolStripButton
        Dim button = New ToolStripButton(text)
        button.ImageTransparentColor = Color.Magenta
        button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
        button.Text = text
        AddHandler button.Click, handler
        Return button
    End Function

    Private Sub ShowSalesForm(sender As Object, e As EventArgs)
        Using salesForm As New SalesForm()
            salesForm.ShowDialog()
        End Using
    End Sub

    Private Sub ShowProductsForm(sender As Object, e As EventArgs)
        Using productForm As New ProductForm()
            productForm.ShowDialog()
        End Using
    End Sub

    Private Sub ShowCustomersForm(sender As Object, e As EventArgs)
        Using customerForm As New CustomerForm()
            customerForm.ShowDialog()
        End Using
    End Sub

    Private Sub ShowStaffForm(sender As Object, e As EventArgs)
        Using staffForm As New StaffForm()
            staffForm.ShowDialog()
        End Using
    End Sub
End Class
