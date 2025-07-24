Imports System.Windows.Forms

Public Class SalesDetailsForm
    Inherits BaseForm

    Public Property SaleItem As Sale
    Private cmbCustomer As ComboBox
    Private cmbStaff As ComboBox
    Private dgvItems As DataGridView
    Private btnAddProduct As Button
    Private btnRemoveProduct As Button
    Private lblTotal As Label
    Private cmbPaymentMethod As ComboBox
    Private btnSave As Button
    Private btnCancel As Button
    Private _customers As List(Of Customer)
    Private _products As List(Of Product)
    Private _staffList As List(Of Staff)
    Private dtpSaleDate As DateTimePicker

    Public Sub New(Optional sale As Sale = Nothing)
        MyBase.New()
        Me.SaleItem = If(sale, New Sale())
        _customers = _dataService.GetCustomers()
        _products = _dataService.GetProducts()
        _staffList = _dataService.GetStaff()
        InitializeComponent()
        If sale IsNot Nothing Then
            LoadSaleData()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Sale Details"
        Me.Size = New Size(900, 700)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterParent

        Dim lblTitle As New Label With {
            .Text = "New Sale",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Location = New Point(30, 20),
            .AutoSize = True
        }

        Dim lblCustomer As New Label With {
            .Text = "Customer",
            .Location = New Point(30, 70),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        cmbCustomer = New ComboBox With {
            .Location = New Point(120, 65),
            .Width = 220,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .DataSource = _customers,
            .DisplayMember = "FullName",
            .ValueMember = "CustomerId",
            .Font = New Font("Segoe UI", 10)
        }

        Dim lblStaff As New Label With {
            .Text = "Staff",
            .Location = New Point(370, 70),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        cmbStaff = New ComboBox With {
            .Location = New Point(450, 65),
            .Width = 220,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .DataSource = _staffList,
            .DisplayMember = "FullName",
            .ValueMember = "StaffId",
            .Font = New Font("Segoe UI", 10)
        }

        Dim lblSaleDate As New Label With {
            .Text = "Sale Date",
            .Location = New Point(700, 70),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        dtpSaleDate = New DateTimePicker With {
            .Location = New Point(780, 65),
            .Width = 100,
            .Format = DateTimePickerFormat.Custom,
            .CustomFormat = "yyyy-MM-dd HH:mm:ss",
            .Font = New Font("Segoe UI", 10)
        }

        dgvItems = New DataGridView With {
            .Location = New Point(30, 120),
            .Size = New Size(820, 320),
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoGenerateColumns = False,
            .BackgroundColor = Color.White,
            .Font = New Font("Segoe UI", 10)
        }
        ' Add image column for product
        Dim imgCol As New DataGridViewImageColumn With {
            .HeaderText = "Image",
            .Width = 80,
            .ImageLayout = DataGridViewImageCellLayout.Zoom
        }
        dgvItems.Columns.Add(imgCol)
        dgvItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .HeaderText = "Product",
            .DataPropertyName = "ProductId",
            .Width = 180
        })
        dgvItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .HeaderText = "Quantity",
            .DataPropertyName = "Quantity",
            .Width = 80
        })
        dgvItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .HeaderText = "Unit Price",
            .DataPropertyName = "UnitPrice",
            .Width = 100
        })
        dgvItems.Columns.Add(New DataGridViewTextBoxColumn With {
            .HeaderText = "Total",
            .DataPropertyName = "Total",
            .Width = 100,
            .ReadOnly = True
        })
        AddHandler dgvItems.CellFormatting, AddressOf DgvItems_CellFormatting

        btnAddProduct = New Button With {
            .Text = "Add Product",
            .Location = New Point(30, 460),
            .Width = 140,
            .Height = 40,
            .Font = New Font("Segoe UI", 10),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        btnRemoveProduct = New Button With {
            .Text = "Remove Product",
            .Location = New Point(190, 460),
            .Width = 140,
            .Height = 40,
            .Font = New Font("Segoe UI", 10),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215)
        }
        btnRemoveProduct.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215)
        AddHandler btnAddProduct.Click, AddressOf BtnAddProduct_Click
        AddHandler btnRemoveProduct.Click, AddressOf BtnRemoveProduct_Click

        lblTotal = New Label With {
            .Text = "Total: $0.00",
            .Location = New Point(700, 460),
            .Font = New Font("Segoe UI", 14, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .AutoSize = True
        }

        Dim lblPayment As New Label With {
            .Text = "Payment Method",
            .Location = New Point(30, 520),
            .Font = New Font("Segoe UI", 10),
            .AutoSize = True
        }
        cmbPaymentMethod = New ComboBox With {
            .Location = New Point(180, 515),
            .Width = 220,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .Font = New Font("Segoe UI", 10)
        }
        cmbPaymentMethod.Items.AddRange(New String() {"Cash", "Card", "Mobile Money", "Bank Transfer"})

        btnSave = New Button With {
            .Text = "Save",
            .Location = New Point(600, 580),
            .Width = 120,
            .Height = 40,
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(740, 580),
            .Width = 120,
            .Height = 40,
            .Font = New Font("Segoe UI", 12, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215)
        }
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215)
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        Me.Controls.AddRange({lblTitle, lblCustomer, cmbCustomer, lblStaff, cmbStaff, lblSaleDate, dtpSaleDate, dgvItems, btnAddProduct, btnRemoveProduct, lblTotal, lblPayment, cmbPaymentMethod, btnSave, btnCancel})
        dgvItems.DataSource = New BindingSource(SaleItem.Items, Nothing)
        UpdateTotal()
    End Sub

    Private Sub BtnAddProduct_Click(sender As Object, e As EventArgs)
        Using dlg As New AddSaleProductDialog(_products)
            If dlg.ShowDialog() = DialogResult.OK Then
                SaleItem.Items.Add(dlg.SaleItem)
                dgvItems.DataSource = Nothing
                dgvItems.DataSource = New BindingSource(SaleItem.Items, Nothing)
                UpdateTotal()
            End If
        End Using
    End Sub

    Private Sub BtnRemoveProduct_Click(sender As Object, e As EventArgs)
        If dgvItems.SelectedRows.Count > 0 Then
            Dim selected = DirectCast(dgvItems.SelectedRows(0).DataBoundItem, SaleItem)
            SaleItem.Items.Remove(selected)
            dgvItems.DataSource = Nothing
            dgvItems.DataSource = New BindingSource(SaleItem.Items, Nothing)
            UpdateTotal()
        End If
    End Sub

    Private Sub UpdateTotal()
        Dim total = SaleItem.Items.Sum(Function(i) i.Total)
        lblTotal.Text = $"Total: ${total:F2}"
        SaleItem.TotalAmount = total
    End Sub

    Private Sub LoadSaleData()
        If SaleItem IsNot Nothing Then
            cmbCustomer.SelectedValue = SaleItem.CustomerId
            cmbStaff.SelectedValue = SaleItem.StaffId
            cmbPaymentMethod.SelectedItem = SaleItem.PaymentMethod
            dtpSaleDate.Value = If(SaleItem.SaleDate = Date.MinValue, Date.Now, SaleItem.SaleDate)
            dgvItems.DataSource = New BindingSource(SaleItem.Items, Nothing)
            UpdateTotal()
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If cmbCustomer.SelectedItem Is Nothing OrElse cmbStaff.SelectedItem Is Nothing OrElse SaleItem.Items.Count = 0 OrElse cmbPaymentMethod.SelectedItem Is Nothing Then
            ShowError("Please select a customer, staff, add at least one product, and select a payment method.")
            Return
        End If
        SaleItem.CustomerId = DirectCast(cmbCustomer.SelectedItem, Customer).CustomerId
        SaleItem.StaffId = DirectCast(cmbStaff.SelectedItem, Staff).StaffId
        SaleItem.SaleDate = dtpSaleDate.Value
        SaleItem.PaymentMethod = cmbPaymentMethod.SelectedItem.ToString()
        Me.DialogResult = DialogResult.OK
    End Sub

    ' Display product image in the grid
    Private Sub DgvItems_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs)
        If dgvItems.Columns(e.ColumnIndex).HeaderText = "Image" Then
            Dim saleItem As SaleItem = CType(dgvItems.Rows(e.RowIndex).DataBoundItem, SaleItem)
            Dim prod = _products.FirstOrDefault(Function(p) p.ProductId = saleItem.ProductId)
            If prod IsNot Nothing AndAlso Not String.IsNullOrEmpty(prod.ImagePath) AndAlso IO.File.Exists(prod.ImagePath) Then
                e.Value = ImageHelper.LoadImage(prod.ImagePath)
            Else
                e.Value = Nothing
            End If
        End If
    End Sub
End Class
