Imports System.Windows.Forms

Public Class CustomerDetailsForm
    Inherits BaseForm

    Public Property CustomerItem As Customer
    Private txtFirstName As TextBox
    Private txtLastName As TextBox
    Private txtAddress As TextBox
    Private txtPhoneNumber As TextBox
    Private btnSave As Button
    Private btnCancel As Button

    Public Sub New(Optional customer As Customer = Nothing)
        MyBase.New()
        Me.CustomerItem = If(customer, New Customer())
        InitializeComponent()
        If customer IsNot Nothing Then
            LoadCustomerData()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Customer Details"
        Me.Size = New Size(500, 400)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterParent

        Dim lblTitle As New Label With {
            .Text = "Customer Details",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Location = New Point(30, 20),
            .AutoSize = True
        }

        Dim lblFirstName As New Label With {.Text = "First Name", .Location = New Point(30, 80), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtFirstName = New TextBox With {.Location = New Point(150, 75), .Width = 280, .Font = New Font("Segoe UI", 10)}
        Dim lblLastName As New Label With {.Text = "Last Name", .Location = New Point(30, 130), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtLastName = New TextBox With {.Location = New Point(150, 125), .Width = 280, .Font = New Font("Segoe UI", 10)}
        Dim lblAddress As New Label With {.Text = "Address", .Location = New Point(30, 180), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtAddress = New TextBox With {.Location = New Point(150, 175), .Width = 280, .Font = New Font("Segoe UI", 10)}
        Dim lblPhoneNumber As New Label With {.Text = "Phone Number", .Location = New Point(30, 230), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtPhoneNumber = New TextBox With {.Location = New Point(150, 225), .Width = 280, .Font = New Font("Segoe UI", 10)}

        btnSave = New Button With {
            .Text = "Save",
            .Location = New Point(150, 300),
            .Width = 90,
            .Height = 40,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(250, 300),
            .Width = 90,
            .Height = 40,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.White,
            .ForeColor = Color.FromArgb(0, 120, 215)
        }
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215)
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        contentPanel.Controls.AddRange({lblTitle, lblFirstName, txtFirstName, lblLastName, txtLastName, lblAddress, txtAddress, lblPhoneNumber, txtPhoneNumber, btnSave, btnCancel})
    End Sub

    Private Sub LoadCustomerData()
        txtFirstName.Text = CustomerItem.FirstName
        txtLastName.Text = CustomerItem.LastName
        txtAddress.Text = CustomerItem.Address
        txtPhoneNumber.Text = CustomerItem.PhoneNumber.ToString()
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtFirstName.Text) OrElse String.IsNullOrWhiteSpace(txtLastName.Text) Then
            ShowError("First and Last Name are required.")
            Return
        End If
        CustomerItem.FirstName = txtFirstName.Text.Trim()
        CustomerItem.LastName = txtLastName.Text.Trim()
        CustomerItem.Address = txtAddress.Text.Trim()
        Integer.TryParse(txtPhoneNumber.Text.Trim(), CustomerItem.PhoneNumber)
        Me.DialogResult = DialogResult.OK
    End Sub
End Class
