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
    Private picProduct As PictureBox
    Private pnlProduct As Panel
    Private lblTotal As Label

    Public Sub New(products As List(Of Product))
        MyBase.New()
        _products = products
        SaleItem = New SaleItem()
        InitializeComponent()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Add Product to Sale"
        Me.Size = New Size(750, 400)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterParent

        ' Title Label
        Dim lblTitle As New Label With {
            .Text = "Add Product",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Location = New Point(30, 20),
            .AutoSize = True
        }

        ' Left Panel for Product Image
        pnlProduct = New Panel With {
            .Location = New Point(30, 70),
            .Size = New Size(250, 250),
            .BorderStyle = BorderStyle.None,
            .BackColor = Color.WhiteSmoke
        }

        picProduct = New PictureBox With {
            .Size = New Size(230, 230),
            .Location = New Point(10, 10),
            .SizeMode = PictureBoxSizeMode.Zoom,
            .BackColor = Color.Transparent
        }
        pnlProduct.Controls.Add(picProduct)

        ' Right Panel for Input Controls
        Dim pnlControls As New Panel With {
            .Location = New Point(300, 70),
            .Size = New Size(420, 250),
            .BackColor = Color.White
        }

        ' Product Selection
        Dim lblProduct As New Label With {
            .Text = "Product",
            .Location = New Point(0, 10),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        cmbProduct = New ComboBox With {
            .Location = New Point(0, 35),
            .Width = 380,
            .Height = 30,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .DataSource = _products,
            .DisplayMember = "ProductName",
            .ValueMember = "ProductId",
            .Font = New Font("Segoe UI", 10)
        }

        ' Quantity
        Dim lblQuantity As New Label With {
            .Text = "Quantity",
            .Location = New Point(0, 80),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        nudQuantity = New NumericUpDown With {
            .Location = New Point(0, 105),
            .Width = 150,
            .Height = 30,
            .Minimum = 1,
            .Maximum = 1000,
            .Value = 1,
            .Font = New Font("Segoe UI", 10)
        }
        AddHandler nudQuantity.ValueChanged, AddressOf UpdateTotalAmount

        ' Unit Price
        Dim lblUnitPrice As New Label With {
            .Text = "Unit Price",
            .Location = New Point(200, 80),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        txtUnitPrice = New TextBox With {
            .Location = New Point(200, 105),
            .Width = 180,
            .Height = 30,
            .ReadOnly = True,
            .Font = New Font("Segoe UI", 10),
            .BackColor = Color.WhiteSmoke,
            .TextAlign = HorizontalAlignment.Right
        }

        ' Total Amount
        lblTotal = New Label With {
            .Text = "Total: $0.00",
            .Location = New Point(0, 150),
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .AutoSize = True
        }

        pnlControls.Controls.AddRange({lblProduct, cmbProduct, lblQuantity, nudQuantity,
                                     lblUnitPrice, txtUnitPrice, lblTotal})

        ' Buttons with modern styling
        btnOK = New Button With {
            .Text = "Add to Cart",
            .Location = New Point(300, 330),
            .Width = 120,
            .Height = 40,
            .Font = New Font("Segoe UI", 10),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }

        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(430, 330),
            .Width = 120,
            .Height = 40,
            .Font = New Font("Segoe UI", 10),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215)
        }
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215)

        AddHandler btnOK.Click, AddressOf BtnOK_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel
        AddHandler cmbProduct.SelectedIndexChanged, AddressOf CmbProduct_SelectedIndexChanged

        Me.Controls.AddRange({lblTitle, pnlProduct, pnlControls, btnOK, btnCancel})

        ' Initialize product selection
        If _products IsNot Nothing AndAlso _products.Count > 0 AndAlso cmbProduct.Items.Count > 0 Then
            cmbProduct.SelectedIndex = 0
            UpdateUnitPrice()
            UpdateProductImage()
        Else
            cmbProduct.SelectedIndex = -1
            txtUnitPrice.Text = ""
        End If
    End Sub

    Private Sub CmbProduct_SelectedIndexChanged(sender As Object, e As EventArgs)
        UpdateUnitPrice()
        UpdateProductImage()
        UpdateTotalAmount()
    End Sub

    Private Sub UpdateUnitPrice()
        If cmbProduct.SelectedItem IsNot Nothing Then
            Dim prod = DirectCast(cmbProduct.SelectedItem, Product)
            txtUnitPrice.Text = prod.UnitPrice.ToString("C2")
        Else
            txtUnitPrice.Text = ""
        End If
    End Sub

    Private Sub UpdateProductImage()
        If cmbProduct.SelectedItem IsNot Nothing Then
            Dim prod = DirectCast(cmbProduct.SelectedItem, Product)
            If Not String.IsNullOrEmpty(prod.ImagePath) AndAlso IO.File.Exists(prod.ImagePath) Then
                picProduct.Image = ImageHelper.LoadImage(prod.ImagePath)
            Else
                picProduct.Image = Nothing
            End If
        Else
            picProduct.Image = Nothing
        End If
    End Sub

    Private Sub UpdateTotalAmount()
        If cmbProduct.SelectedItem IsNot Nothing Then
            Dim prod = DirectCast(cmbProduct.SelectedItem, Product)
            Dim total = prod.UnitPrice * nudQuantity.Value
            lblTotal.Text = $"Total: {total:C2}"
        Else
            lblTotal.Text = "Total: $0.00"
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
