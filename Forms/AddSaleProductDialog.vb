Imports System.Windows.Forms

Public Class AddSaleProductDialog
    Inherits BaseForm

    Public Property SaleItem As SaleItem
    Private cmbProduct As ComboBox
    Private nudQuantity As NumericUpDown
    Private txtUnitPrice As TextBox
    Private btnOK As Button
    Private btnCancel As Button
    Private _products As List(Of Product)

    Public Sub New(products As List(Of Product))
        MyBase.New()
        _products = products
        SaleItem = New SaleItem()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Add Product to Sale"
        Me.Size = New Size(420, 320)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Dim lblProduct As New Label With {.Text = "Product", .Location = New Point(30, 40), .AutoSize = True}
        cmbProduct = New ComboBox With {
            .Location = New Point(140, 35),
            .Width = 220,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .DataSource = _products,
            .DisplayMember = "ProductName",
            .ValueMember = "ProductId"
        }
        Dim lblQuantity As New Label With {.Text = "Quantity", .Location = New Point(30, 100), .AutoSize = True}
        nudQuantity = New NumericUpDown With {
            .Location = New Point(140, 95),
            .Width = 100,
            .Minimum = 1,
            .Maximum = 1000,
            .Value = 1
        }
        Dim lblUnitPrice As New Label With {.Text = "Unit Price", .Location = New Point(30, 160), .AutoSize = True}
        txtUnitPrice = New TextBox With {.Location = New Point(140, 155), .Width = 120, .ReadOnly = True}

        btnOK = New Button With {.Text = "OK", .Location = New Point(120, 220), .Width = 80}
        btnCancel = New Button With {.Text = "Cancel", .Location = New Point(220, 220), .Width = 80}
        AddHandler btnOK.Click, AddressOf BtnOK_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel
        AddHandler cmbProduct.SelectedIndexChanged, AddressOf CmbProduct_SelectedIndexChanged

        Me.Controls.AddRange({lblProduct, cmbProduct, lblQuantity, nudQuantity, lblUnitPrice, txtUnitPrice, btnOK, btnCancel})
        ' Only set SelectedIndex if there are items
        If _products IsNot Nothing AndAlso _products.Count > 0 AndAlso cmbProduct.Items.Count > 0 Then
            cmbProduct.SelectedIndex = 0
            UpdateUnitPrice()
        Else
            cmbProduct.SelectedIndex = -1
            txtUnitPrice.Text = ""
        End If
    End Sub

    Private Sub CmbProduct_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateUnitPrice()
    End Sub

    Private Sub UpdateUnitPrice()
        If cmbProduct.SelectedItem IsNot Nothing Then
            Dim prod = DirectCast(cmbProduct.SelectedItem, Product)
            txtUnitPrice.Text = prod.UnitPrice.ToString("F2")
        Else
            txtUnitPrice.Text = ""
        End If
    End Sub

    Private Sub BtnOK_Click(sender As Object, e As EventArgs)
        If cmbProduct.SelectedItem Is Nothing Then
            ShowError("Please select a product.")
            Return
        End If
        Dim prod = DirectCast(cmbProduct.SelectedItem, Product)
        SaleItem.ProductId = prod.ProductId
        SaleItem.Quantity = CInt(nudQuantity.Value)
        SaleItem.UnitPrice = prod.UnitPrice
        Me.DialogResult = DialogResult.OK
    End Sub
End Class
