Public Class SalesForm
    Inherits BaseForm

    Private _salesList As List(Of Sale)
    Private _staffList As List(Of Staff)
    Private WithEvents dgvSales As DataGridView
    Private WithEvents btnAdd As Button
    Private WithEvents btnEdit As Button
    Private WithEvents btnDelete As Button

    Public Sub New()
        MyBase.New()
        _staffList = _dataService.GetStaff()
        InitializeComponent()
        LoadSalesData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Sales Management"
        Me.Size = New Size(1050, 600)

        dgvSales = New DataGridView()
        With dgvSales
            .Dock = DockStyle.Fill
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AutoGenerateColumns = False
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "SaleId",
                .HeaderText = "ID",
                .Width = 50
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CustomerId",
                .HeaderText = "Customer",
                .Width = 150
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StaffId",
                .HeaderText = "Staff",
                .Width = 150
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "SaleDate",
                .HeaderText = "Date",
                .Width = 150
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "TotalAmount",
                .HeaderText = "Total Amount",
                .Width = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "PaymentMethod",
                .HeaderText = "Payment Method",
                .Width = 120
            })
        End With

        Dim buttonPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Bottom,
            .FlowDirection = FlowDirection.LeftToRight,
            .Height = 40,
            .Padding = New Padding(5)
        }
        btnAdd = New Button With {
            .Text = "Add Sale",
            .Width = 100
        }
        btnEdit = New Button With {
            .Text = "Edit Sale",
            .Width = 100,
            .Enabled = False
        }
        btnDelete = New Button With {
            .Text = "Delete Sale",
            .Width = 100,
            .Enabled = False
        }
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete})
        Me.Controls.Add(dgvSales)
        Me.Controls.Add(buttonPanel)
    End Sub

    Private Sub LoadSalesData()
        Try
            _salesList = _dataService.GetSales()
            ' Optionally, show staff name instead of ID
            For Each sale In _salesList
                Dim staff = _staffList.FirstOrDefault(Function(s) s.StaffId = sale.StaffId)
                If staff IsNot Nothing Then
                    sale.PaymentMethod = $"{sale.PaymentMethod} (By: {staff.FullName})"
                End If
            Next
            dgvSales.DataSource = Nothing
            dgvSales.DataSource = _salesList
        Catch ex As Exception
            ShowError("Error loading sales data: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvSales_SelectionChanged(sender As Object, e As EventArgs) Handles dgvSales.SelectionChanged
        Dim hasSelection = dgvSales.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim newSale As New Sale With {.SaleId = GetNextSaleId()}
        Using detailsForm As New SalesDetailsForm(newSale)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                _salesList.Add(newSale)
                _dataService.SaveSales(_salesList)
                LoadSalesData()
                ShowInfo("Sale added successfully.")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If dgvSales.SelectedRows.Count > 0 Then
            Dim selectedSale = DirectCast(dgvSales.SelectedRows(0).DataBoundItem, Sale)
            Using detailsForm As New SalesDetailsForm(selectedSale)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    _dataService.SaveSales(_salesList)
                    LoadSalesData()
                    ShowInfo("Sale updated successfully.")
                End If
            End Using
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvSales.SelectedRows.Count > 0 Then
            If MessageBox.Show("Are you sure you want to delete this sale?", "Confirm Delete",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Dim selectedSale = DirectCast(dgvSales.SelectedRows(0).DataBoundItem, Sale)
                    _salesList.Remove(selectedSale)
                    _dataService.SaveSales(_salesList)
                    LoadSalesData()
                    ShowInfo("Sale deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting sale: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Function GetNextSaleId() As Integer
        If _salesList Is Nothing OrElse _salesList.Count = 0 Then Return 1
        Return _salesList.Max(Function(s) s.SaleId) + 1
    End Function
End Class
