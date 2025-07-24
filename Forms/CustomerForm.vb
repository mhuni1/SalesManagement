Public Class CustomerForm
    Inherits BaseForm

    Private _customers As List(Of Customer)
    Private WithEvents txtSearch As TextBox

    Public Sub New()
        MyBase.New()
        Me.Text = "Customers Management"
        lblTitle.Text = "Customers Management"
        InitializeCustomersUI()
        LoadCustomersData()
    End Sub

    Private Sub InitializeCustomersUI()
        ' Create Table Layout
        Dim tableLayoutPanel = CreateTableLayoutPanel()

        ' Search Panel
        Dim searchPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 0, 0, 10)
        }

        txtSearch = CreateSearchTextBox()
        txtSearch.PlaceholderText = "Search customers..."
        txtSearch.Location = New Point(0, 10)
        AddHandler txtSearch.TextChanged, AddressOf FilterCustomers

        searchPanel.Controls.Add(txtSearch)
        tableLayoutPanel.Controls.Add(searchPanel, 0, 0)

        ' Initialize DataGridView
        dgvData = CreateModernDataGridView()

        ' Configure columns
        With dgvData.Columns
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CustomerId",
                .HeaderText = "ID",
                .Width = 80,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "FullName",
                .HeaderText = "Full Name",
                .Width = 200,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Address",
                .HeaderText = "Address",
                .Width = 200,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "PhoneNumber",
                .HeaderText = "Phone",
                .Width = 150,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
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
        btnAdd = CreateModernButton("Add Customer", True)
        btnEdit = CreateModernButton("Edit Customer", False)
        btnDelete = CreateModernButton("Delete Customer", False)

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
        AddHandler dgvData.SelectionChanged, AddressOf DgvCustomers_SelectionChanged
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnEdit.Click, AddressOf BtnEdit_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click

        ' Add TableLayoutPanel to content panel
        contentPanel.Controls.Add(tableLayoutPanel)
    End Sub

    Private Sub LoadCustomersData()
        Try
            _customers = _dataService.GetCustomers()
            dgvData.DataSource = Nothing
            dgvData.DataSource = _customers
            dgvData.Refresh()
        Catch ex As Exception
            ShowError("Error loading customers data: " & ex.Message)
        End Try
    End Sub

    Private Sub FilterCustomers(sender As Object, e As EventArgs)
        If _customers Is Nothing Then Return
        Dim searchText = txtSearch.Text.ToLower()
        Dim filtered = _customers.Where(Function(c) _
            c.FullName.ToLower().Contains(searchText) OrElse _
            c.Address.ToLower().Contains(searchText) OrElse _
            c.PhoneNumber.ToString().Contains(searchText) OrElse _
            c.CustomerId.ToString().Contains(searchText)
        ).ToList()
        dgvData.DataSource = Nothing
        dgvData.DataSource = filtered
        dgvData.Refresh()
    End Sub

    Private Sub DgvCustomers_SelectionChanged(sender As Object, e As EventArgs)
        Dim hasSelection = dgvData.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Dim newCustomer As New Customer()
        Using detailsForm As New CustomerDetailsForm(newCustomer)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                _customers.Add(newCustomer)
                _dataService.SaveCustomers(_customers)
                LoadCustomersData()
            End If
        End Using
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
        If dgvData.SelectedRows.Count > 0 Then
            Dim customer = DirectCast(dgvData.SelectedRows(0).DataBoundItem, Customer)
            Using detailsForm As New CustomerDetailsForm(customer)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    _dataService.SaveCustomers(_customers)
                    LoadCustomersData()
                End If
            End Using
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If dgvData.SelectedRows.Count > 0 Then
            Dim customer = DirectCast(dgvData.SelectedRows(0).DataBoundItem, Customer)
            If MessageBox.Show($"Are you sure you want to delete customer '{customer.FullName}'?",
                             "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    _customers.Remove(customer)
                    _dataService.SaveCustomers(_customers)
                    LoadCustomersData()
                    ShowInfo("Customer deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting customer: " & ex.Message)
                End Try
            End If
        End If
    End Sub
End Class
