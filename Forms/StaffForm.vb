Public Class StaffForm
    Inherits BaseForm

    Private _staffList As List(Of Staff)
    Private WithEvents txtSearch As TextBox

    Public Sub New()
        MyBase.New()
        Me.Text = "Staff Management"
        lblTitle.Text = "Staff Management"
        InitializeStaffUI()
        LoadStaffData()
    End Sub

    Private Sub InitializeStaffUI()
        ' Create Table Layout
        Dim tableLayoutPanel = CreateTableLayoutPanel()

        ' Search Panel
        Dim searchPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 0, 0, 10)
        }

        txtSearch = CreateSearchTextBox()
        txtSearch.PlaceholderText = "Search staff..."
        txtSearch.Location = New Point(0, 10)
        AddHandler txtSearch.TextChanged, AddressOf FilterStaff

        searchPanel.Controls.Add(txtSearch)
        tableLayoutPanel.Controls.Add(searchPanel, 0, 0)

        ' Initialize DataGridView
        dgvData = CreateModernDataGridView()

        ' Configure columns
        With dgvData.Columns
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StaffId",
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
                .DataPropertyName = "Position",
                .HeaderText = "Position",
                .Width = 150,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "Email",
                .HeaderText = "Email",
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
        btnAdd = CreateModernButton("Add Staff", True)
        btnEdit = CreateModernButton("Edit Staff", False)
        btnDelete = CreateModernButton("Delete Staff", False)

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
        AddHandler dgvData.SelectionChanged, AddressOf DgvStaff_SelectionChanged
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnEdit.Click, AddressOf BtnEdit_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click

        ' Add TableLayoutPanel to content panel
        contentPanel.Controls.Add(tableLayoutPanel)
    End Sub

    Private Sub LoadStaffData()
        Try
            _staffList = _dataService.GetStaff()
            dgvData.DataSource = Nothing
            dgvData.DataSource = _staffList
            dgvData.Refresh()
        Catch ex As Exception
            ShowError("Error loading staff data: " & ex.Message)
        End Try
    End Sub

    Private Sub FilterStaff(sender As Object, e As EventArgs)
        If _staffList Is Nothing Then Return

        Dim searchText = txtSearch.Text.ToLower()
        Dim filtered = _staffList.Where(Function(s) _
            s.FullName.ToLower().Contains(searchText) OrElse
            s.Position.ToLower().Contains(searchText) OrElse
            s.Email.ToLower().Contains(searchText) OrElse
            s.PhoneNumber.ToString().Contains(searchText) OrElse
            s.StaffId.ToString().Contains(searchText)
        ).ToList()

        dgvData.DataSource = Nothing
        dgvData.DataSource = filtered
        dgvData.Refresh()
    End Sub

    Private Sub DgvStaff_SelectionChanged(sender As Object, e As EventArgs)
        Dim hasSelection = dgvData.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Dim newStaff As New Staff()
        Using detailsForm As New StaffDetailsForm(newStaff)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                _staffList.Add(newStaff)
                _dataService.SaveStaff(_staffList)
                LoadStaffData()
            End If
        End Using
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
        If dgvData.SelectedRows.Count > 0 Then
            Dim staff = DirectCast(dgvData.SelectedRows(0).DataBoundItem, Staff)
            Using detailsForm As New StaffDetailsForm(staff)
                If detailsForm.ShowDialog() = DialogResult.OK Then
                    _dataService.SaveStaff(_staffList)
                    LoadStaffData()
                End If
            End Using
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If dgvData.SelectedRows.Count > 0 Then
            Dim staff = DirectCast(dgvData.SelectedRows(0).DataBoundItem, Staff)
            If MessageBox.Show($"Are you sure you want to delete staff member '{staff.FullName}'?",
                             "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                Try
                    _staffList.Remove(staff)
                    _dataService.SaveStaff(_staffList)
                    LoadStaffData()
                    ShowInfo("Staff member deleted successfully")
                Catch ex As Exception
                    ShowError("Error deleting staff member: " & ex.Message)
                End Try
            End If
        End If
    End Sub
End Class