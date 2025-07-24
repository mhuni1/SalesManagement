Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO

Public Class Form1
    Inherits Form

    Private contentPanel As Panel
    Private widgetsPanel As Panel
    Private dataPanel As Panel
    Private toolStrip As ToolStrip
    Private WithEvents dgvSales As DataGridView
    Private WithEvents btnExport As Button
    Private WithEvents btnAdd As Button
    Private WithEvents btnEdit As Button
    Private WithEvents btnDelete As Button
    Private _salesList As List(Of Sale)
    Private _dataService As New DataService()

    Public Sub New()
        InitializeComponent()
        SetupModernSalesUI()
    End Sub

    Private Sub SetupModernSalesUI()
        Me.Text = "Sales - MART Sales Management"
        Me.WindowState = FormWindowState.Maximized
        Me.BackColor = Color.White
        Me.Controls.Clear()

        ' ToolStrip navigation at the top
        toolStrip = New ToolStrip With {
            .Dock = DockStyle.Top,
            .ImageScalingSize = New Size(32, 32),
            .BackColor = Color.FromArgb(0, 120, 215),
            .Renderer = New ToolStripProfessionalRenderer(New CustomToolStripColorTable())
        }
        Dim btnSales As New ToolStripButton("Sales") With {
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .DisplayStyle = ToolStripItemDisplayStyle.Text,
            .Enabled = False
        }
        Dim btnProducts As New ToolStripButton("Products") With {
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .DisplayStyle = ToolStripItemDisplayStyle.Text
        }
        Dim btnCustomers As New ToolStripButton("Customers") With {
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .DisplayStyle = ToolStripItemDisplayStyle.Text
        }
        Dim btnStaff As New ToolStripButton("Staff") With {
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .ForeColor = Color.White,
            .DisplayStyle = ToolStripItemDisplayStyle.Text
        }
        toolStrip.Items.AddRange({btnSales, btnProducts, btnCustomers, btnStaff})
        Me.Controls.Add(toolStrip)

        ' Main content panel
        contentPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.FromArgb(245, 245, 245),
            .Padding = New Padding(20, 10, 20, 20)
        }
        Me.Controls.Add(contentPanel)

        ' Widgets panel
        widgetsPanel = New Panel With {
            .Dock = DockStyle.Top,
            .Height = 200,
            .BackColor = Color.White,
            .Padding = New Padding(0)
        }
        contentPanel.Controls.Add(widgetsPanel)

        ' Data panel
        dataPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White,
            .Padding = New Padding(10)
        }
        contentPanel.Controls.Add(dataPanel)

        ' Initialize sales table
        InitializeSalesTable()

        ' Add handlers for navigation
        AddHandler btnProducts.Click, AddressOf ShowProductsWindow
        AddHandler btnCustomers.Click, AddressOf ShowCustomersWindow
        AddHandler btnStaff.Click, AddressOf ShowStaffWindow

        ' Load widgets and data
        LoadDashboardWidgets()
        LoadSalesData()
    End Sub

    Private Sub InitializeSalesTable()
        ' Create Table Layout
        Dim tableLayoutPanel As New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 4, ' 4 rows: label, search, grid, buttons
            .Padding = New Padding(0),
            .BackColor = Color.White
        }
        tableLayoutPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100))
        tableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 40)) ' Label row fixed height
        tableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 60)) ' Search panel
        tableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100)) ' Grid
        tableLayoutPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 60)) ' Buttons

        ' Panel for the label
        Dim labelPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .BackColor = Color.White
        }
        Dim lblTableTitle As New Label With {
            .Text = "Sales Records",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(60, 60, 60),
            .Dock = DockStyle.Left,
            .TextAlign = ContentAlignment.MiddleLeft,
            .AutoSize = True,
            .Padding = New Padding(10, 0, 0, 0)
        }
        labelPanel.Controls.Add(lblTableTitle)
        tableLayoutPanel.Controls.Add(labelPanel, 0, 0)

        ' Search Panel
        Dim searchPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 0, 0, 10)
        }

        Dim txtSearch As New TextBox With {
            .Width = 300,
            .Height = 35,
            .Location = New Point(0, 10),
            .Font = New Font("Segoe UI", 12),
            .PlaceholderText = "Search sales...",
            .BorderStyle = BorderStyle.FixedSingle
        }
        AddHandler txtSearch.TextChanged, Sub(sender, e)
                                              FilterSalesData(txtSearch.Text)
                                          End Sub

        searchPanel.Controls.Add(txtSearch)
        tableLayoutPanel.Controls.Add(searchPanel, 0, 1)

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
            .ReadOnly = True,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }

        ' Faded/soft color scheme for DataGridView
        With dgvSales
            .EnableHeadersVisualStyles = False
            .ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(235, 241, 250) ' Soft blue
            .ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60)
            .ColumnHeadersDefaultCellStyle.Font = New Font("Segoe UI Semibold", 11)
            .ColumnHeadersDefaultCellStyle.Padding = New Padding(10)
            .ColumnHeadersHeight = 45
            .RowTemplate.Height = 40
            .DefaultCellStyle.Font = New Font("Segoe UI", 10)
            .DefaultCellStyle.Padding = New Padding(10, 5, 10, 5)
            .DefaultCellStyle.BackColor = Color.FromArgb(250, 252, 255) ' Very light blue
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 246, 252) ' Faded blue
            .DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 225, 245) ' Soft selection
            .DefaultCellStyle.SelectionForeColor = Color.Black
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            .GridColor = Color.FromArgb(220, 225, 230)
        End With

        ' Configure columns
        With dgvSales.Columns
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "SaleId",
                .HeaderText = "ID",
                .Width = 80,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "CustomerName",
                .HeaderText = "Customer",
                .Width = 200,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "StaffName",
                .HeaderText = "Staff",
                .Width = 200,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "SaleDate",
                .HeaderText = "Date",
                .Width = 150,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .DefaultCellStyle = New DataGridViewCellStyle With {
                    .Format = "dd MMM yyyy HH:mm"
                }
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "TotalAmount",
                .HeaderText = "Total Amount",
                .Width = 150,
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                .DefaultCellStyle = New DataGridViewCellStyle With {
                    .Format = "C2",
                    .Alignment = DataGridViewContentAlignment.MiddleRight
                }
            })
            .Add(New DataGridViewTextBoxColumn With {
                .DataPropertyName = "PaymentMethod",
                .HeaderText = "Payment",
                .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            })
        End With

        ' Add DataGridView to a panel for padding
        Dim gridPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(1)
        }
        gridPanel.Controls.Add(dgvSales)
        tableLayoutPanel.Controls.Add(gridPanel, 0, 2)

        ' Button Panel
        Dim buttonPanel As New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(0, 10, 0, 0)
        }

        ' Initialize buttons with modern styling
        btnAdd = New Button With {
            .Text = "Add Sale",
            .Width = 130,
            .Height = 40,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI Semibold", 10),
            .Cursor = Cursors.Hand
        }
        btnAdd.FlatAppearance.BorderSize = 0
        btnAdd.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 100, 200)

        btnEdit = New Button With {
            .Text = "Edit Sale",
            .Width = 130,
            .Height = 40,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(70, 70, 70),
            .Font = New Font("Segoe UI Semibold", 10),
            .Enabled = False,
            .Cursor = Cursors.Hand
        }
        btnEdit.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210)
        btnEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245)

        btnDelete = New Button With {
            .Text = "Delete Sale",
            .Width = 130,
            .Height = 40,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(70, 70, 70),
            .Font = New Font("Segoe UI Semibold", 10),
            .Enabled = False,
            .Cursor = Cursors.Hand
        }
        btnDelete.FlatAppearance.BorderColor = Color.FromArgb(210, 210, 210)
        btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 245, 245)

        btnExport = New Button With {
            .Text = "Export to CSV",
            .Width = 130,
            .Height = 40,
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 153, 51),
            .ForeColor = Color.White,
            .Font = New Font("Segoe UI Semibold", 10),
            .Cursor = Cursors.Hand
        }
        btnExport.FlatAppearance.BorderSize = 0
        btnExport.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 130, 40)

        ' Add buttons to panel with proper spacing
        buttonPanel.Controls.AddRange({btnAdd, btnEdit, btnDelete, btnExport})
        For i = 0 To buttonPanel.Controls.Count - 1
            buttonPanel.Controls(i).Left = i * 140 + 10
            buttonPanel.Controls(i).Top = 10
        Next

        tableLayoutPanel.Controls.Add(buttonPanel, 0, 3)

        ' Wire up events
        AddHandler dgvSales.SelectionChanged, AddressOf DgvSales_SelectionChanged
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        AddHandler btnEdit.Click, AddressOf BtnEdit_Click
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click
        AddHandler btnExport.Click, AddressOf BtnExport_Click

        ' Add TableLayoutPanel to data panel
        dataPanel.Controls.Add(tableLayoutPanel)
    End Sub

    Private Sub LoadSalesData()
        Try
            _salesList = _dataService.GetSales()
            Dim staffList = _dataService.GetStaff()
            Dim customerList = _dataService.GetCustomers()

            ' Create view model with names instead of IDs
            Dim salesView As New List(Of SaleView)
            For Each s In _salesList
                Dim staff = staffList.FirstOrDefault(Function(st) st.StaffId = s.StaffId)
                Dim customer = customerList.FirstOrDefault(Function(c) c.CustomerId = s.CustomerId)
                salesView.Add(New SaleView With {
                    .SaleId = s.SaleId,
                    .CustomerName = If(customer IsNot Nothing, customer.FullName, "Unknown"),
                    .StaffName = If(staff IsNot Nothing, staff.FullName, "Unknown"),
                    .SaleDate = s.SaleDate,
                    .TotalAmount = s.TotalAmount,
                    .PaymentMethod = s.PaymentMethod
                })
            Next
            dgvSales.DataSource = salesView
            dgvSales.Refresh()
        Catch ex As Exception
            MessageBox.Show("Error loading sales data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DgvSales_SelectionChanged(sender As Object, e As EventArgs)
        Dim hasSelection = dgvSales.SelectedRows.Count > 0
        btnEdit.Enabled = hasSelection
        btnDelete.Enabled = hasSelection
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Dim newSale As New Sale With {.SaleId = GetNextSaleId()}
        Using detailsForm As New SalesDetailsForm(newSale)
            If detailsForm.ShowDialog() = DialogResult.OK Then
                _salesList.Add(newSale)
                _dataService.SaveSales(_salesList)
                LoadSalesData()
                LoadDashboardWidgets() ' Refresh widgets to show updated totals
            End If
        End Using
    End Sub

    Private Sub BtnEdit_Click(sender As Object, e As EventArgs)
        If dgvSales.SelectedRows.Count > 0 Then
            Dim view As SaleView = DirectCast(dgvSales.SelectedRows(0).DataBoundItem, SaleView)
            Dim selectedSale = _salesList.FirstOrDefault(Function(s) s.SaleId = view.SaleId)
            If selectedSale IsNot Nothing Then
                Using detailsForm As New SalesDetailsForm(selectedSale)
                    If detailsForm.ShowDialog() = DialogResult.OK Then
                        _dataService.SaveSales(_salesList)
                        LoadSalesData()
                        LoadDashboardWidgets() ' Refresh widgets to show updated totals
                    End If
                End Using
            End If
        End If
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        If dgvSales.SelectedRows.Count > 0 Then
            Dim view As SaleView = DirectCast(dgvSales.SelectedRows(0).DataBoundItem, SaleView)
            Dim selectedSale = _salesList.FirstOrDefault(Function(s) s.SaleId = view.SaleId)
            If selectedSale IsNot Nothing Then
                If MessageBox.Show("Are you sure you want to delete this sale?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
                    Try
                        _salesList.Remove(selectedSale)
                        _dataService.SaveSales(_salesList)
                        LoadSalesData()
                        LoadDashboardWidgets() ' Refresh widgets to show updated totals
                    Catch ex As Exception
                        MessageBox.Show("Error deleting sale: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub BtnExport_Click(sender As Object, e As EventArgs)
        Try
            Using sfd As New SaveFileDialog With {
                .Filter = "CSV files (*.csv)|*.csv",
                .FileName = $"Sales_Export_{DateTime.Now:yyyyMMdd}.csv"
            }
                If sfd.ShowDialog() = DialogResult.OK Then
                    Using writer As New StreamWriter(sfd.FileName)
                        ' Write headers
                        Dim headers As New List(Of String)
                        For Each c As DataGridViewColumn In dgvSales.Columns
                            headers.Add("""" & c.HeaderText & """")
                        Next
                        writer.WriteLine(String.Join(",", headers))
                        ' Write data rows
                        For Each row As DataGridViewRow In dgvSales.Rows
                            If Not row.IsNewRow Then
                                Dim cells As New List(Of String)
                                For Each c As DataGridViewCell In row.Cells
                                    cells.Add("""" & If(c.Value, "") & """")
                                Next
                                writer.WriteLine(String.Join(",", cells))
                            End If
                        Next
                    End Using
                End If
            End Using
            MessageBox.Show("Data exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error exporting data: " & ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetNextSaleId() As Integer
        Return If(_salesList.Any(), _salesList.Max(Function(s) s.SaleId) + 1, 1)
    End Function

    Private Sub FilterSalesData(searchText As String)
        If _salesList Is Nothing Then Return
        Dim staffList = _dataService.GetStaff()
        Dim customerList = _dataService.GetCustomers()
        Dim filteredSales As New List(Of SaleView)
        For Each s In _salesList
            Dim staff = staffList.FirstOrDefault(Function(st) st.StaffId = s.StaffId)
            Dim customer = customerList.FirstOrDefault(Function(c) c.CustomerId = s.CustomerId)
            Dim view = New SaleView With {
                .SaleId = s.SaleId,
                .CustomerName = If(customer IsNot Nothing, customer.FullName, "Unknown"),
                .StaffName = If(staff IsNot Nothing, staff.FullName, "Unknown"),
                .SaleDate = s.SaleDate,
                .TotalAmount = s.TotalAmount,
                .PaymentMethod = s.PaymentMethod
            }
            If view.CustomerName.ToLower().Contains(searchText.ToLower()) OrElse
               view.StaffName.ToLower().Contains(searchText.ToLower()) OrElse
               view.PaymentMethod.ToLower().Contains(searchText.ToLower()) OrElse
               view.SaleId.ToString().Contains(searchText) Then
                filteredSales.Add(view)
            End If
        Next
        dgvSales.DataSource = filteredSales
        dgvSales.Refresh()
    End Sub

    ' Custom ToolStrip color table for modern look
    Private Class CustomToolStripColorTable
        Inherits ProfessionalColorTable
        Public Overrides ReadOnly Property ToolStripGradientBegin As Color
            Get
                Return Color.FromArgb(0, 120, 215)
            End Get
        End Property
        Public Overrides ReadOnly Property ToolStripGradientEnd As Color
            Get
                Return Color.FromArgb(0, 120, 215)
            End Get
        End Property
        Public Overrides ReadOnly Property ButtonSelectedHighlight As Color
            Get
                Return Color.FromArgb(0, 153, 255)
            End Get
        End Property
        Public Overrides ReadOnly Property ButtonSelectedBorder As Color
            Get
                Return Color.White
            End Get
        End Property
    End Class

    Private Sub ShowProductsWindow(sender As Object, e As EventArgs)
        Dim productsForm As New ProductForm()
        productsForm.WindowState = FormWindowState.Maximized
        productsForm.Show()
    End Sub

    Private Sub ShowCustomersWindow(sender As Object, e As EventArgs)
        Dim customersForm As New CustomerForm()
        customersForm.WindowState = FormWindowState.Maximized
        customersForm.Show()
    End Sub

    Private Sub ShowStaffWindow(sender As Object, e As EventArgs)
        Dim staffForm As New StaffForm()
        staffForm.WindowState = FormWindowState.Maximized
        staffForm.Show()
    End Sub

    Private Sub LoadDashboardWidgets()
        widgetsPanel.Controls.Clear()
        Dim ds As New DataService()
        Dim salesCount = ds.GetSales().Count
        Dim productsCount = ds.GetProducts().Count
        Dim customersCount = ds.GetCustomers().Count
        Dim staffCount = ds.GetStaff().Count
        Dim totalSales = ds.GetSales().Sum(Function(s) s.TotalAmount)

        Dim widgetColors = {Color.FromArgb(0, 120, 215), Color.FromArgb(0, 153, 51), Color.FromArgb(255, 140, 0), Color.FromArgb(220, 0, 80)}
        Dim widgetTitles = {"Total Sales", "Products", "Customers", "Staff"}
        Dim widgetValues = {$"{totalSales:C2}", productsCount.ToString(), customersCount.ToString(), staffCount.ToString()}
        Dim widgetIcons = {"💰", "📦", "👥", "👤"}

        Dim widgetWidth = 340
        Dim widgetHeight = 180
        Dim widgetSpacing = 30
        For i = 0 To 3
            Dim card As New Panel With {
                .Width = widgetWidth,
                .Height = widgetHeight,
                .Left = 10 + i * (widgetWidth + widgetSpacing),
                .Top = 10,
                .BackColor = widgetColors(i),
                .BorderStyle = BorderStyle.FixedSingle,
                .Padding = New Padding(0),
                .Margin = New Padding(0)
            }
            ' Icon on the left, vertically centered
            Dim iconSize = 30
            Dim lblIcon As New Label With {
                .Text = widgetIcons(i),
                .Font = New Font("Segoe UI Emoji", iconSize, FontStyle.Bold),
                .ForeColor = Color.White,
                .Location = New Point(30, (widgetHeight - iconSize) \ 2),
                .AutoSize = True,
                .TextAlign = ContentAlignment.MiddleCenter
            }
            ' Text panel to the right of the icon, vertically centered
            Dim textPanelTop = (widgetHeight - 60) \ 2
            Dim textPanel As New Panel With {
                .Location = New Point(120, textPanelTop),
                .Size = New Size(widgetWidth - 160, 70),
                .BackColor = Color.Transparent
            }
            Dim lblTitle As New Label With {
                .Text = widgetTitles(i),
                .Font = New Font("Segoe UI", 16, FontStyle.Bold),
                .ForeColor = Color.White,
                .Location = New Point(0, 0),
                .AutoSize = True
            }
            Dim lblValue As New Label With {
                .Text = widgetValues(i),
                .Font = New Font("Segoe UI", 18, FontStyle.Bold),
                .ForeColor = Color.White,
                .Location = New Point(0, 38),
                .AutoSize = True
            }
            textPanel.Controls.AddRange({lblTitle, lblValue})
            card.Controls.Add(lblIcon)
            card.Controls.Add(textPanel)
            widgetsPanel.Controls.Add(card)
        Next
    End Sub
End Class

Public Class SaleView
    Public Property SaleId As Integer
    Public Property CustomerName As String
    Public Property StaffName As String
    Public Property SaleDate As DateTime
    Public Property TotalAmount As Decimal
    Public Property PaymentMethod As String
End Class
