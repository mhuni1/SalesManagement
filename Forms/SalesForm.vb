Public Class SalesForm
    Inherits BaseForm

    Private _salesList As List(Of Sale)
    Private _staffList As List(Of Staff)
    Private WithEvents dgvSales As DataGridView
    Private WithEvents btnAdd As Button
    Private WithEvents btnEdit As Button
    Private WithEvents btnDelete As Button
    Private tableLayoutPanel As TableLayoutPanel
    Private buttonPanel As FlowLayoutPanel

    Public Sub New()
        MyBase.New()
        _staffList = _dataService.GetStaff()
        InitializeComponent()
        LoadSalesData() ' Load data immediately after initialization
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        
        ' Create TableLayoutPanel for main layout
        tableLayoutPanel = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .RowCount = 2,
            .ColumnCount = 1,
            .BackColor = Color.White
        }
        tableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100))
        tableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 50))
        tableLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))

        ' Initialize DataGridView
        dgvSales = New DataGridView With {
            .Dock = DockStyle.Fill,
            .BackgroundColor = Color.White,
            .BorderStyle = BorderStyle.None,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .AllowUserToResizeRows = False,
            .MultiSelect = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoGenerateColumns = False,
            .RowHeadersVisible = False,
            .ReadOnly = True
        }

        ' Configure DataGridView columns
        With dgvSales.Columns
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "SaleId",
                .HeaderText = "ID",
                .Width = 60
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CustomerId",
                .HeaderText = "Customer",
                .Width = 150
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StaffId",
                .HeaderText = "Staff",
                .Width = 150
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "SaleDate",
                .HeaderText = "Date",
                .Width = 150
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "TotalAmount",
                .HeaderText = "Total Amount",
                .Width = 120
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "PaymentMethod",
                .HeaderText = "Payment Method",
                .Width = 120
            })
        End With

        ' Style DataGridView
        With dgvSales
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI", 10, FontStyle.Bold)
            .ColumnHeadersHeight = 40
            .RowTemplate.Height = 35
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245)
        End With

        ' Initialize button panel
        buttonPanel = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .FlowDirection = FlowDirection.LeftToRight,
            .Padding = New Padding(0, 5, 0, 5)
        }

        ' Initialize buttons
        btnAdd = New Button With {
            .Text = "Add Sale",
            .Width = 120,
            .Height = 35,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI", 10)
        }

        btnEdit = New Button With {
            .Text = "Edit Sale",
            .Width = 120,
            .Height = 35,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Font = New Font("Segoe UI", 10),
            .Enabled = False
        }

        btnDelete = New Button With {
            .Text = "Delete Sale",
            .Width = 120,
            .Height = 35,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Font = New Font("Segoe UI", 10),
            .Enabled = False
        }

        ' Add buttons to panel
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete})

        ' Add controls to TableLayoutPanel
        tableLayoutPanel.Controls.Add(dgvSales, 0, 0)
        tableLayoutPanel.Controls.Add(buttonPanel, 0, 1)

        ' Add TableLayoutPanel to form
        Me.Controls.Add(tableLayoutPanel)

        ' Wire up events
        AddHandler dgvSales.SelectionChanged, AddressOf dgvSales_SelectionChanged
        AddHandler btnAdd.Click, AddressOf btnAdd_Click
        AddHandler btnEdit.Click, AddressOf btnEdit_Click
        AddHandler btnDelete.Click, AddressOf btnDelete_Click

        Me.ResumeLayout(False)
    End Sub

    Public Sub LoadSalesData()
        Try
            _salesList = _dataService.GetSales()
            
            ' Update DataGridView
            dgvSales.DataSource = Nothing
            dgvSales.DataSource = _salesList
            
            ' Force refresh
            dgvSales.Refresh()
            Me.Refresh()
        Catch ex As Exception
            ShowError("Error loading sales data: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvSales_SelectionChanged(sender As Object, e As EventArgs)
        Dim hasSelection = dgvSales.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs)
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

    Private Sub btnEdit_Click(sender As Object, e As EventArgs)
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

    Private Sub btnDelete_Click(sender As Object, e As EventArgs)
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
