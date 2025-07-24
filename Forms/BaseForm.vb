Public Class BaseForm
    Inherits Form

    Protected _dataService As DataService
    Protected contentPanel As Panel
    Protected headerPanel As Panel
    Protected lblTitle As Label
    Protected dgvData As DataGridView
    Protected WithEvents btnAdd As Button
    Protected WithEvents btnEdit As Button
    Protected WithEvents btnDelete As Button

    Public Sub New()
        _dataService = New DataService()
        SetupBaseUI()
    End Sub

    Protected Overridable Sub SetupBaseUI()
        Me.WindowState = FormWindowState.Maximized
        Me.BackColor = Color.White

        ' Header Panel
        headerPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 60,
            .BackColor = Color.FromArgb(0, 120, 215),
            .Padding = New Padding(20, 0, 20, 0)
        }

        lblTitle = New Label With {
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.White,
            .Dock = DockStyle.Left,
            .TextAlign = ContentAlignment.MiddleLeft,
            .AutoSize = True
        }
        headerPanel.Controls.Add(lblTitle)

        ' Main content panel
        contentPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.FromArgb(245, 245, 245),
            .Padding = New Padding(20)
        }

        Me.Controls.Add(contentPanel)
        Me.Controls.Add(headerPanel)
    End Sub

    Protected Function CreateModernButton(text As String, isPrimary As Boolean) As Button
        Dim btn = New Button With {
            .Text = text,
            .Width = 130,
            .Height = 40,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI Semibold", 10),
            .Cursor = Cursors.Hand
        }

        If isPrimary Then
            btn.BackColor = Color.FromArgb(0, 120, 215)
            btn.ForeColor = Color.White
            btn.FlatAppearance.BorderSize = 0
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 100, 200)
        Else
            btn.BackColor = Color.White
            btn.ForeColor = Color.FromArgb(70, 70, 70)
            btn.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210)
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245)
        End If

        Return btn
    End Function

    Protected Function CreateModernDataGridView() As DataGridView
        Dim dgv = New DataGridView With {
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
            .ReadOnly = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }

        With dgv
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 241, 250)
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 11)
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(10)
            .ColumnHeadersHeight = 45
            .RowTemplate.Height = 40
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .DefaultCellStyle.Padding = New Padding(10, 5, 10, 5)
            .DefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255)
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 246, 252)
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 245)
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .GridColor = Color.FromArgb(220, 225, 230)
        End With

        Return dgv
    End Function

    Protected Function CreateSearchTextBox() As TextBox
        Return New TextBox With {
            .Width = 300,
            .Height = 35,
            .Font = New Font("Segoe UI", 12),
            .PlaceholderText = "Search...",
            .BorderStyle = BorderStyle.FixedSingle
        }
    End Function

    Protected Function CreateTableLayoutPanel() As TableLayoutPanel
        Dim tlp = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 3,
            .Padding = New Padding(0),
            .BackColor = Color.White
        }
        tlp.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        tlp.RowStyles.Add(New RowStyle(SizeType.Absolute, 60)) ' Search panel
        tlp.RowStyles.Add(New RowStyle(SizeType.Percent, 100)) ' Grid
        tlp.RowStyles.Add(New RowStyle(SizeType.Absolute, 60)) ' Buttons
        Return tlp
    End Function

    Protected Sub ShowError(message As String)
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Protected Sub ShowInfo(message As String)
        MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class