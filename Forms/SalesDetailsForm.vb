Imports System.Windows.Forms

Public Class SalesDetailsForm
    Inherits BaseForm

    Public Property SaleItem As Sale
    Private cmbCustomer As ComboBox
    Private cmbStaff As ComboBox ' New: Staff selection
    Private dgvItems As DataGridView
    Private btnAddProduct As Button
    Private btnRemoveProduct As Button
    Private lblTotal As Label
    Private cmbPaymentMethod As ComboBox
    Private btnSave As Button
    Private btnCancel As Button
    Private _customers As List(Of Customer)
    Private _products As List(Of Product)
    Private _staffList As List(Of Staff) ' New: Staff list
    Private dtpSaleDate As DateTimePicker

    Public Sub New(Optional sale As Sale = Nothing)
        MyBase.New()
        Me.SaleItem = If(sale, New Sale())
        _customers = _dataService.GetCustomers()
        _products = _dataService.GetProducts()
        _staffList = _dataService.GetStaff() ' New: Load staff
        InitializeComponent()
        If sale IsNot Nothing Then
            LoadSaleData()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Sale Details"
        Me.Size = New Size(700, 700)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Dim lblCustomer As New Label With {.Text = "Customer", .Location = New Point(20, 30)}
        cmbCustomer = New ComboBox With {
            .Location = New Point(120, 25),
            .Width = 220,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .DataSource = _customers,
            .DisplayMember = "FullName",
            .ValueMember = "CustomerId"
        }

        Dim lblStaff As New Label With {.Text = "Staff", .Location = New Point(370, 30)}
        cmbStaff = New ComboBox With {
            .Location = New Point(450, 25),
            .Width = 180,
            .DropDownStyle = ComboBoxStyle.DropDownList,
            .DataSource = _staffList,
            .DisplayMember = "FullName",
            .ValueMember = "StaffId"
        }

        Dim lblSaleDate As New Label With {.Text = "Sale Date", .Location = New Point(20, 70)}
        dtpSaleDate = New DateTimePicker With {
            .Location = New Point(120, 65),
            .Width = 220,
            .Format = DateTimePickerFormat.Custom,
            .CustomFormat = "yyyy-MM-dd HH:mm:ss"
        }

        dgvItems = New DataGridView With {
            .Location = New Point(20, 110),
            .Size = New Size(640, 300),
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoGenerateColumns = False
        }
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

        btnAddProduct = New Button With {.Text = "Add Product", .Location = New Point(20, 430), .Width = 120}
        btnRemoveProduct = New Button With {.Text = "Remove Product", .Location = New Point(160, 430), .Width = 120}
        AddHandler btnAddProduct.Click, AddressOf BtnAddProduct_Click
        AddHandler btnRemoveProduct.Click, AddressOf BtnRemoveProduct_Click

        lblTotal = New Label With {.Text = "Total: $0.00", .Location = New Point(500, 430), .AutoSize = True, .Font = New Font("Segoe UI", 12, FontStyle.Bold)}

        Dim lblPayment As New Label With {.Text = "Payment Method", .Location = New Point(20, 480)}
        cmbPaymentMethod = New ComboBox With {
            .Location = New Point(160, 475),
            .Width = 180,
            .DropDownStyle = ComboBoxStyle.DropDownList
        }
        cmbPaymentMethod.Items.AddRange(New String() {"Cash", "Card", "Mobile Money", "Bank Transfer"})

        btnSave = New Button With {.Text = "Save", .Location = New Point(160, 550), .Width = 100}
        btnCancel = New Button With {.Text = "Cancel", .Location = New Point(280, 550), .Width = 100}
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        Me.Controls.AddRange({lblCustomer, cmbCustomer, lblStaff, cmbStaff, lblSaleDate, dtpSaleDate, dgvItems, btnAddProduct, btnRemoveProduct, lblTotal, lblPayment, cmbPaymentMethod, btnSave, btnCancel})
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
End Class
