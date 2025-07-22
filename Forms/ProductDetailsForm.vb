Imports System.Windows.Forms

Public Class ProductDetailsForm
    Inherits BaseForm

    Public Property ProductItem As Product
    Private pictureBox As PictureBox
    Private btnUpload As Button
    Private openFileDialog As OpenFileDialog
    Private txtProductName As TextBox
    Private txtCategory As TextBox
    Private txtUnitPrice As TextBox
    Private txtStockQuantity As TextBox
    Private btnSave As Button
    Private btnCancel As Button

    Public Sub New(Optional product As Product = Nothing)
        MyBase.New()
        Me.ProductItem = If(product, New Product())
        InitializeComponent()
        If product IsNot Nothing Then
            LoadProductData()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Product Details"
        Me.Size = New Size(400, 500)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        pictureBox = New PictureBox With {
            .Size = New Size(120, 120),
            .Location = New Point(20, 20),
            .BorderStyle = BorderStyle.FixedSingle,
            .SizeMode = PictureBoxSizeMode.Zoom
        }
        btnUpload = New Button With {
            .Text = "Upload Image",
            .Location = New Point(160, 60),
            .Width = 120
        }
        openFileDialog = New OpenFileDialog With {
            .Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        }
        AddHandler btnUpload.Click, AddressOf BtnUpload_Click
        Me.Controls.Add(pictureBox)
        Me.Controls.Add(btnUpload)

        Dim lblProductName As New Label With {.Text = "Product Name", .Location = New Point(20, 160)}
        txtProductName = New TextBox With {.Location = New Point(140, 155), .Width = 200}
        Dim lblCategory As New Label With {.Text = "Category", .Location = New Point(20, 200)}
        txtCategory = New TextBox With {.Location = New Point(140, 195), .Width = 200}
        Dim lblUnitPrice As New Label With {.Text = "Unit Price", .Location = New Point(20, 240)}
        txtUnitPrice = New TextBox With {.Location = New Point(140, 235), .Width = 200}
        Dim lblStockQuantity As New Label With {.Text = "Stock Quantity", .Location = New Point(20, 280)}
        txtStockQuantity = New TextBox With {.Location = New Point(140, 275), .Width = 200}

        btnSave = New Button With {.Text = "Save", .Location = New Point(120, 340), .Width = 80}
        btnCancel = New Button With {.Text = "Cancel", .Location = New Point(220, 340), .Width = 80}
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        Me.Controls.AddRange({lblProductName, txtProductName, lblCategory, txtCategory, lblUnitPrice, txtUnitPrice, lblStockQuantity, txtStockQuantity, btnSave, btnCancel})
    End Sub

    Private Sub BtnUpload_Click(sender As Object, e As EventArgs)
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            pictureBox.Image = Image.FromFile(openFileDialog.FileName)
            ProductItem.ImagePath = openFileDialog.FileName ' Will be saved properly on Save
        End If
    End Sub

    Private Sub LoadProductData()
        If Not String.IsNullOrEmpty(ProductItem.ImagePath) AndAlso IO.File.Exists(ProductItem.ImagePath) Then
            pictureBox.Image = Image.FromFile(ProductItem.ImagePath)
        End If
        txtProductName.Text = ProductItem.ProductName
        txtCategory.Text = ProductItem.Category
        txtUnitPrice.Text = ProductItem.UnitPrice.ToString("F2")
        txtStockQuantity.Text = ProductItem.StockQuantity.ToString()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtProductName.Text) Then
            ShowError("Product Name is required.")
            Return
        End If
        ProductItem.ProductName = txtProductName.Text.Trim()
        ProductItem.Category = txtCategory.Text.Trim()
        Decimal.TryParse(txtUnitPrice.Text.Trim(), ProductItem.UnitPrice)
        Integer.TryParse(txtStockQuantity.Text.Trim(), ProductItem.StockQuantity)
        Me.DialogResult = DialogResult.OK
    End Sub
End Class
