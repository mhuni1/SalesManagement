Public Class StaffForm
    Inherits BaseForm

    Private _staffList As List(Of Staff)
    Private WithEvents dgvStaff As DataGridView
    Private WithEvents btnAdd As Button
    Private WithEvents btnEdit As Button
    Private WithEvents btnDelete As Button

    Public Sub New()
        MyBase.New()
        InitializeComponent()
        LoadStaffData()
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Staff Management"
        Me.Size = New Size(800, 600)

        ' Create DataGridView
        dgvStaff = New DataGridView()
        With dgvStaff
            .Dock = DockStyle.Fill
            .AllowUserToAddRows = False
            .AllowUserToDeleteRows = False
            .MultiSelect = False
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect
            .AutoGenerateColumns = False
            ' Add columns
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StaffId",
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
                .DataPropertyName = "Position",
                .HeaderText = "Position",
                .Width = 120
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Email",
                .HeaderText = "Email",
                .Width = 150
            })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "PhoneNumber",
                .HeaderText = "Phone",
                .Width = 100
            })
        End With

        ' Create Buttons
        Dim buttonPanel As New FlowLayoutPanel With {
            .Dock = DockStyle.Bottom,
            .FlowDirection = FlowDirection.LeftToRight,
            .Height = 40,
            .Padding = New Padding(5)
        }
        btnAdd = New Button With {
            .Text = "Add Staff",
            .Width = 100
        }
        btnEdit = New Button With {
            .Text = "Edit Staff",
            .Width = 100,
            .Enabled = False
        }
        btnDelete = New Button With {
            .Text = "Delete Staff",
            .Width = 100,
            .Enabled = False
        }
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete})
        Me.Controls.Add(dgvStaff)
        Me.Controls.Add(buttonPanel)
    End Sub

    Private Sub LoadStaffData()
        Try
            _staffList = _dataService.GetStaff()
            dgvStaff.DataSource = Nothing
            dgvStaff.DataSource = _staffList
        Catch ex As Exception
            ShowError("Error loading staff data: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvStaff_SelectionChanged(sender As Object, e As EventArgs) Handles dgvStaff.SelectionChanged
        Dim hasSelection = dgvStaff.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        Dim newStaff As New Staff With {.StaffId = GetNextStaffId()}
        Using detailsForm As New StaffDetailsForm(newStaff)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                ' Save image if uploaded
                If Not String.IsNullOrEmpty(newStaff.ImagePath) AndAlso IO.File.Exists(newStaff.ImagePath) Then
                    newStaff.ImagePath = ImageHelper.SaveImageAndGetPath(newStaff.ImagePath, "Staff", newStaff.StaffId)
                End If
                _staffList.Add(newStaff)
                _dataService.SaveStaff(_staffList)
                LoadStaffData()
                ShowInfo("Staff member added successfully.")
            End If
        End Using
    End Sub

    Private Sub btnEdit_Click(sender As Object, e As EventArgs) Handles btnEdit.Click
        If dgvStaff.SelectedRows.Count > 0 Then
            Dim selectedStaff = DirectCast(dgvStaff.SelectedRows(0).DataBoundItem, Staff)
            Dim originalImagePath = selectedStaff.ImagePath
            Using detailsForm As New StaffDetailsForm(selectedStaff)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    ' Save image if changed
                    If Not String.IsNullOrEmpty(selectedStaff.ImagePath) AndAlso IO.File.Exists(selectedStaff.ImagePath) AndAlso selectedStaff.ImagePath <> originalImagePath Then
                        selectedStaff.ImagePath = ImageHelper.SaveImageAndGetPath(selectedStaff.ImagePath, "Staff", selectedStaff.StaffId)
                    End If
                    _dataService.SaveStaff(_staffList)
                    LoadStaffData()
                    ShowInfo("Staff member updated successfully.")
                End If
            End Using
        End If
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvStaff.SelectedRows.Count > 0 Then
            If MessageBox.Show("Are you sure you want to delete this staff member?", "Confirm Delete",
                             MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    Dim selectedStaff = DirectCast(dgvStaff.SelectedRows(0).DataBoundItem, Staff)
                    _staffList.Remove(selectedStaff)
                    _dataService.SaveStaff(_staffList)
                    LoadStaffData()
                    ShowInfo("Staff member deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting staff member: " & ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Function GetNextStaffId() As Integer
        If _staffList Is Nothing OrElse _staffList.Count = 0 Then Return 1
        Return _staffList.Max(Function(s) s.StaffId) + 1
    End Function
End Class