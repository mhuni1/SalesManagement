Public Class ProductForm
    Inherits BaseForm

    Private _products As List(Of Product)
    Private WithEvents txtSearch As TextBox

    Public Sub New()
        MyBase.New()
        Me.Text = "Products Management"
        lblTitle.Text = "Products Management"
        InitializeProductsUI()
        LoadProductsData()
    End Sub

    Private Sub InitializeProductsUI()
        ' Create Table Layout
        Dim tableLayoutPanel = CreateTableLayoutPanel()

        ' Search Panel
        Dim searchPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 0, 0, 10)
        }

        txtSearch = CreateSearchTextBox()
        txtSearch.PlaceholderText = "Search products..."
        txtSearch.Location = New Point(0, 10)
        AddHandler txtSearch.TextChanged, AddressOf FilterProducts

        searchPanel.Controls.Add(txtSearch)
        tableLayoutPanel.Controls.Add(searchPanel, 0, 0)

        ' Initialize DataGridView
        dgvData = CreateModernDataGridView()

        ' Configure columns
        With dgvData.Columns
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "ProductId",
                .HeaderText = "ID",
                .Width = 80,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "ProductName",
                .HeaderText = "Product Name",
                .Width = 250,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Category",
                .HeaderText = "Category",
                .Width = 150,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "UnitPrice",
                .HeaderText = "Unit Price",
                .Width = 120,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .DefaultCellStyle = New DataGridViewCellStyle With {
                    .Format = "C2",
                    .Alignment = DataGridViewContentAlignment.MiddleRight
                }
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StockQuantity",
                .HeaderText = "Stock",
                .Width = 100,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .DefaultCellStyle = New DataGridViewCellStyle With {
                    .Alignment = DataGridViewContentAlignment.MiddleRight
                }
            })
        End With

        ' Add DataGridView to a panel for padding
        Dim gridPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(1)
        }
        gridPanel.Controls.Add(dgvData)
        tableLayoutPanel.Controls.Add(gridPanel, 0, 1)

        ' Button Panel
        Dim buttonPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 10, 0, 0)
        }

        ' Initialize buttons
        btnAdd = CreateModernButton("Add Product", True)
        btnEdit = CreateModernButton("Edit Product", False)
        btnDelete = CreateModernButton("Delete Product", False)

        ' Disable edit/delete buttons initially
        btnEdit.Enabled = False
        btnDelete.Enabled = False

        ' Add buttons to panel with proper spacing
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete})
        For i = 0 To buttonPanel.Controls.Count - 1
            buttonPanel.Controls(i).Left = i * 140 + 10
            buttonPanel.Controls(i).Top = 10
        Next

        tableLayoutPanel.Controls.Add(buttonPanel, 0, 2)

        ' Wire up events
        AddHandler dgvData.SelectionChanged, AddressOf DgvProducts_SelectionChanged
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnEdit.Click, AddressOf BtnEdit_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click

        ' Add TableLayoutPanel to content panel
        contentPanel.Controls.Add(tableLayoutPanel)
    End Sub

    Private Sub LoadProductsData()
        Try
            _products = _dataService.GetProducts()
            dgvData.DataSource = Nothing
            dgvData.DataSource = _products
            dgvData.Refresh()
        Catch ex As Exception
            ShowError("Error loading products data: " & ex.Message)
        End Try
    End Sub

    Private Sub FilterProducts(sender As Object, e As EventArgs)
        If _products Is Nothing Then Return

        Dim searchText = txtSearch.Text.ToLower()
        Dim filtered = _products.Where(Function(p) _
            p.ProductName.ToLower().Contains(searchText) OrElse _
            p.Category.ToLower().Contains(searchText) OrElse _
            p.ProductId.ToString().Contains(searchText)
        ).ToList()

        dgvData.DataSource = Nothing
        dgvData.DataSource = filtered
        dgvData.Refresh()
    End Sub

    Private Sub DgvProducts_SelectionChanged(sender As Object, e As EventArgs)
        Dim hasSelection = dgvData.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Dim newProduct As New Product()
        Using detailsForm As New ProductDetailsForm(newProduct)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                _products.Add(newProduct)
                _dataService.SaveProducts(_products)
                LoadProductsData()
            End If
        End Using
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
        If dgvData.SelectedRows.Count > 0 Then
            Dim product = DirectCast(dgvData.SelectedRows(0).DataBoundItem, Product)
            Using detailsForm As New ProductDetailsForm(product)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    _dataService.SaveProducts(_products)
                    LoadProductsData()
                End If
            End Using
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If dgvData.SelectedRows.Count > 0 Then
            Dim product = DirectCast(dgvData.SelectedRows(0).DataBoundItem, Product)
            If MessageBox.Show($"Are you sure you want to delete product '{product.ProductName}'?",
                             "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    _products.Remove(product)
                    _dataService.SaveProducts(_products)
                    LoadProductsData()
                    ShowInfo("Product deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting product: " & ex.Message)
                End Try
            End If
        End If
    End Sub
End Class
