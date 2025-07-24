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
        Me.Size = New Size(600, 500)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterParent

        Dim lblTitle As New Label With {
            .Text = "Product Details",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Location = New Point(30, 20),
            .AutoSize = True
        }

        pictureBox = New PictureBox With {
            .Size = New Size(160, 160),
            .Location = New Point(30, 70),
            .BorderStyle = BorderStyle.FixedSingle,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .BackColor = Color.WhiteSmoke
        }
        btnUpload = New Button With {
            .Text = "Upload Image",
            .Location = New Point(30, 240),
            .Width = 160,
            .Height = 36,
            .Font = New Font("Segoe UI", 10),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        openFileDialog = New OpenFileDialog With {
            .Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        }
        AddHandler btnUpload.Click, AddressOf BtnUpload_Click

        Dim lblProductName As New Label With {.Text = "Product Name", .Location = New Point(220, 70), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtProductName = New TextBox With {.Location = New Point(340, 65), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblCategory As New Label With {.Text = "Category", .Location = New Point(220, 120), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtCategory = New TextBox With {.Location = New Point(340, 115), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblUnitPrice As New Label With {.Text = "Unit Price", .Location = New Point(220, 170), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtUnitPrice = New TextBox With {.Location = New Point(340, 165), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblStockQuantity As New Label With {.Text = "Stock Quantity", .Location = New Point(220, 220), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtStockQuantity = New TextBox With {.Location = New Point(340, 215), .Width = 200, .Font = New Font("Segoe UI", 10)}

        btnSave = New Button With {
            .Text = "Save",
            .Location = New Point(340, 300),
            .Width = 90,
            .Height = 40,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(450, 300),
            .Width = 90,
            .Height = 40,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215)
        }
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215)
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        contentPanel.Controls.AddRange({lblTitle, pictureBox, btnUpload, lblProductName, txtProductName, lblCategory, txtCategory, lblUnitPrice, txtUnitPrice, lblStockQuantity, txtStockQuantity, btnSave, btnCancel})
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
