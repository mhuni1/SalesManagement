Public Class ProductForm
    Inherits BaseForm

    Private _productList As List(Of Product)
    Private WithEvents dgvProducts As DataGridView
    Private WithEvents btnAdd As Button
    Private WithEvents btnEdit As Button
    Private WithEvents btnDelete As Button

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        LoadProductData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Product Management"
        Me.Size = New Size(800, 600)

        dgvProducts = New DataGridView()
        With dgvProducts
            .Dock = DockStyle.Fill
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AutoGenerateColumns = False
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "ProductId",
                .HeaderText = "ID",
                .Width = 50
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "ProductName",
                .HeaderText = "Name",
                .Width = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Category",
                .HeaderText = "Category",
                .Width = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "UnitPrice",
                .HeaderText = "Unit Price",
                .Width = 100
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StockQuantity",
                .HeaderText = "Stock",
                .Width = 80
            })
        End With

        Dim buttonPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Bottom,
            .FlowDirection = FlowDirection.LeftToRight,
            .Height = 40,
            .Padding = New Padding(5)
        }
        btnAdd = New Button With {
            .Text = "Add Product",
            .Width = 100
        }
        btnEdit = New Button With {
            .Text = "Edit Product",
            .Width = 100,
            .Enabled = False
        }
        btnDelete = New Button With {
            .Text = "Delete Product",
            .Width = 100,
            .Enabled = False
        }
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete})
        Me.Controls.Add(dgvProducts)
        Me.Controls.Add(buttonPanel)
    End Sub

    Private Sub LoadProductData()
        Try
            _productList = _dataService.GetProducts()
            dgvProducts.DataSource = Nothing
            dgvProducts.DataSource = _productList
        Catch ex As Exception
            ShowError("Error loading product data: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvProducts_SelectionChanged(sender As Object, e As EventArgs) Handles dgvProducts.SelectionChanged
        Dim hasSelection = dgvProducts.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim newProduct As New Product With {.ProductId = GetNextProductId()}
        Using detailsForm As New ProductDetailsForm(newProduct)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                If Not String.IsNullOrEmpty(newProduct.ImagePath) AndAlso IO.File.Exists(newProduct.ImagePath) Then
                    newProduct.ImagePath = ImageHelper.SaveImageAndGetPath(newProduct.ImagePath, "Product", newProduct.ProductId)
                End If
                _productList.Add(newProduct)
                _dataService.SaveProducts(_productList)
                LoadProductData()
                ShowInfo("Product added successfully.")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If dgvProducts.SelectedRows.Count > 0 Then
            Dim selectedProduct = DirectCast(dgvProducts.SelectedRows(0).DataBoundItem, Product)
            Dim originalImagePath = selectedProduct.ImagePath
            Using detailsForm As New ProductDetailsForm(selectedProduct)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    If Not String.IsNullOrEmpty(selectedProduct.ImagePath) AndAlso IO.File.Exists(selectedProduct.ImagePath) AndAlso selectedProduct.ImagePath <> originalImagePath Then
                        selectedProduct.ImagePath = ImageHelper.SaveImageAndGetPath(selectedProduct.ImagePath, "Product", selectedProduct.ProductId)
                    End If
                    _dataService.SaveProducts(_productList)
                    LoadProductData()
                    ShowInfo("Product updated successfully.")
                End If
            End Using
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvProducts.SelectedRows.Count > 0 Then
            If MessageBox.Show("Are you sure you want to delete this product?", "Confirm Delete",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Dim selectedProduct = DirectCast(dgvProducts.SelectedRows(0).DataBoundItem, Product)
                    _productList.Remove(selectedProduct)
                    _dataService.SaveProducts(_productList)
                    LoadProductData()
                    ShowInfo("Product deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting product: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Function GetNextProductId() As Integer
        If _productList Is Nothing OrElse _productList.Count = 0 Then Return 1
        Return _productList.Max(Function(p) p.ProductId) + 1
    End Function
End Class
