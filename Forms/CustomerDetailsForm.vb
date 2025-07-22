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
        Me.Size = New Size(400, 350)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        Dim lblFirstName As New Label With {.Text = "First Name", .Location = New Point(20, 30)}
        txtFirstName = New TextBox With {.Location = New Point(120, 25), .Width = 220}
        Dim lblLastName As New Label With {.Text = "Last Name", .Location = New Point(20, 70)}
        txtLastName = New TextBox With {.Location = New Point(120, 65), .Width = 220}
        Dim lblAddress As New Label With {.Text = "Address", .Location = New Point(20, 110)}
        txtAddress = New TextBox With {.Location = New Point(120, 105), .Width = 220}
        Dim lblPhoneNumber As New Label With {.Text = "Phone Number", .Location = New Point(20, 150)}
        txtPhoneNumber = New TextBox With {.Location = New Point(120, 145), .Width = 220}

        btnSave = New Button With {.Text = "Save", .Location = New Point(120, 200), .Width = 80}
        btnCancel = New Button With {.Text = "Cancel", .Location = New Point(220, 200), .Width = 80}
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        Me.Controls.AddRange({lblFirstName, txtFirstName, lblLastName, txtLastName, lblAddress, txtAddress, lblPhoneNumber, txtPhoneNumber, btnSave, btnCancel})
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
