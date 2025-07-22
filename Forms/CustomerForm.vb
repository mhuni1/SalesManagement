Public Class CustomerForm
    Inherits BaseForm

    Private _customerList As List(Of Customer)
    Private WithEvents dgvCustomers As DataGridView
    Private WithEvents btnAdd As Button
    Private WithEvents btnEdit As Button
    Private WithEvents btnDelete As Button

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        LoadCustomerData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Customer Management"
        Me.Size = New Size(800, 600)

        dgvCustomers = New DataGridView()
        With dgvCustomers
            .Dock = DockStyle.Fill
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AutoGenerateColumns = False
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CustomerId",
                .HeaderText = "ID",
                .Width = 50
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "FirstName",
                .HeaderText = "First Name",
                .Width = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "LastName",
                .HeaderText = "Last Name",
                .Width = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Address",
                .HeaderText = "Address",
                .Width = 200
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "PhoneNumber",
                .HeaderText = "Phone",
                .Width = 100
            })
        End With

        Dim buttonPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Bottom,
            .FlowDirection = FlowDirection.LeftToRight,
            .Height = 40,
            .Padding = New Padding(5)
        }
        btnAdd = New Button With {
            .Text = "Add Customer",
            .Width = 100
        }
        btnEdit = New Button With {
            .Text = "Edit Customer",
            .Width = 100,
            .Enabled = False
        }
        btnDelete = New Button With {
            .Text = "Delete Customer",
            .Width = 100,
            .Enabled = False
        }
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete})
        Me.Controls.Add(dgvCustomers)
        Me.Controls.Add(buttonPanel)
    End Sub

    Private Sub LoadCustomerData()
        Try
            _customerList = _dataService.GetCustomers()
            dgvCustomers.DataSource = Nothing
            dgvCustomers.DataSource = _customerList
        Catch ex As Exception
            ShowError("Error loading customer data: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvCustomers_SelectionChanged(sender As Object, e As EventArgs) Handles dgvCustomers.SelectionChanged
        Dim hasSelection = dgvCustomers.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim newCustomer As New Customer With {.CustomerId = GetNextCustomerId()}
        Using detailsForm As New CustomerDetailsForm(newCustomer)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                _customerList.Add(newCustomer)
                _dataService.SaveCustomers(_customerList)
                LoadCustomerData()
                ShowInfo("Customer added successfully.")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If dgvCustomers.SelectedRows.Count > 0 Then
            Dim selectedCustomer = DirectCast(dgvCustomers.SelectedRows(0).DataBoundItem, Customer)
            Using detailsForm As New CustomerDetailsForm(selectedCustomer)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    _dataService.SaveCustomers(_customerList)
                    LoadCustomerData()
                    ShowInfo("Customer updated successfully.")
                End If
            End Using
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvCustomers.SelectedRows.Count > 0 Then
            If MessageBox.Show("Are you sure you want to delete this customer?", "Confirm Delete",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Dim selectedCustomer = DirectCast(dgvCustomers.SelectedRows(0).DataBoundItem, Customer)
                    _customerList.Remove(selectedCustomer)
                    _dataService.SaveCustomers(_customerList)
                    LoadCustomerData()
                    ShowInfo("Customer deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting customer: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Function GetNextCustomerId() As Integer
        If _customerList Is Nothing OrElse _customerList.Count = 0 Then Return 1
        Return _customerList.Max(Function(c) c.CustomerId) + 1
    End Function
End Class
