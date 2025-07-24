Imports System.Windows.Forms

Public Class StaffDetailsForm
    Inherits BaseForm

    Public Property StaffMember As Staff
    Private pictureBox As PictureBox
    Private btnUpload As Button
    Private openFileDialog As OpenFileDialog
    Private txtFirstName As TextBox
    Private txtLastName As TextBox
    Private txtPosition As TextBox
    Private txtEmail As TextBox
    Private txtPhoneNumber As TextBox
    Private dtpHireDate As DateTimePicker
    Private txtSalary As TextBox
    Private btnSave As Button
    Private btnCancel As Button

    Public Sub New(Optional staff As Staff = Nothing)
        MyBase.New()
        Me.StaffMember = If(staff, New Staff())
        InitializeComponent()
        If staff IsNot Nothing Then
            LoadStaffData()
        End If
    End Sub

    Private Sub InitializeComponent()
        Me.Text = "Staff Details"
        Me.Size = New Size(600, 600)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.BackColor = Color.White
        Me.StartPosition = FormStartPosition.CenterParent

        Dim lblTitle As New Label With {
            .Text = "Staff Details",
            .Font = New Font("Segoe UI", 16, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Location = New Point(30, 20),
            .AutoSize = True
        }

        pictureBox = New PictureBox With {
            .Size = New Size(160, 160),
            .Location = New Point(30, 70),
            .BorderStyle = BorderStyle.FixedSingle,
            .SizeMode = PictureBoxSizeMode.Zoom,
            .BackColor = Color.WhiteSmoke
        }
        btnUpload = New Button With {
            .Text = "Upload Photo",
            .Location = New Point(30, 240),
            .Width = 160,
            .Height = 36,
            .Font = New Font("Segoe UI", 10),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        openFileDialog = New OpenFileDialog With {
            .Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        }
        AddHandler btnUpload.Click, AddressOf BtnUpload_Click

        Dim lblFirstName As New Label With {.Text = "First Name", .Location = New Point(220, 70), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtFirstName = New TextBox With {.Location = New Point(340, 65), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblLastName As New Label With {.Text = "Last Name", .Location = New Point(220, 120), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtLastName = New TextBox With {.Location = New Point(340, 115), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblPosition As New Label With {.Text = "Position", .Location = New Point(220, 170), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtPosition = New TextBox With {.Location = New Point(340, 165), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblEmail As New Label With {.Text = "Email", .Location = New Point(220, 220), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtEmail = New TextBox With {.Location = New Point(340, 215), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblPhoneNumber As New Label With {.Text = "Phone Number", .Location = New Point(220, 270), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtPhoneNumber = New TextBox With {.Location = New Point(340, 265), .Width = 200, .Font = New Font("Segoe UI", 10)}
        Dim lblHireDate As New Label With {.Text = "Hire Date", .Location = New Point(220, 320), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        dtpHireDate = New DateTimePicker With {.Location = New Point(340, 315), .Width = 200, .Format = DateTimePickerFormat.Short, .Font = New Font("Segoe UI", 10)}
        Dim lblSalary As New Label With {.Text = "Salary", .Location = New Point(220, 370), .Font = New Font("Segoe UI", 10), .AutoSize = True}
        txtSalary = New TextBox With {.Location = New Point(340, 365), .Width = 200, .Font = New Font("Segoe UI", 10)}

        btnSave = New Button With {
            .Text = "Save",
            .Location = New Point(340, 440),
            .Width = 90,
            .Height = 40,
            .Font = New Font("Segoe UI", 11, FontStyle.Bold),
            .FlatStyle = FlatStyle.Flat,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White
        }
        btnCancel = New Button With {
            .Text = "Cancel",
            .Location = New Point(450, 440),
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

        Me.Controls.AddRange({lblTitle, pictureBox, btnUpload, lblFirstName, txtFirstName, lblLastName, txtLastName, lblPosition, txtPosition, lblEmail, txtEmail, lblPhoneNumber, txtPhoneNumber, lblHireDate, dtpHireDate, lblSalary, txtSalary, btnSave, btnCancel})
    End Sub

    Private Sub BtnUpload_Click(sender As Object, e As EventArgs)
        If openFileDialog.ShowDialog() = DialogResult.OK Then
            pictureBox.Image = Image.FromFile(openFileDialog.FileName)
            StaffMember.ImagePath = openFileDialog.FileName ' Will be saved properly on Save
        End If
    End Sub

    Private Sub LoadStaffData()
        If Not String.IsNullOrEmpty(StaffMember.ImagePath) AndAlso IO.File.Exists(StaffMember.ImagePath) Then
            pictureBox.Image = Image.FromFile(StaffMember.ImagePath)
        End If
        txtFirstName.Text = StaffMember.FirstName
        txtLastName.Text = StaffMember.LastName
        txtPosition.Text = StaffMember.Position
        txtEmail.Text = StaffMember.Email
        txtPhoneNumber.Text = StaffMember.PhoneNumber.ToString()
        dtpHireDate.Value = If(StaffMember.HireDate = Date.MinValue, Date.Today, StaffMember.HireDate)
        txtSalary.Text = StaffMember.Salary.ToString("F2")
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        If String.IsNullOrWhiteSpace(txtFirstName.Text) OrElse String.IsNullOrWhiteSpace(txtLastName.Text) Then
            ShowError("First and Last Name are required.")
            Return
        End If
        StaffMember.FirstName = txtFirstName.Text.Trim()
        StaffMember.LastName = txtLastName.Text.Trim()
        StaffMember.Position = txtPosition.Text.Trim()
        StaffMember.Email = txtEmail.Text.Trim()
        Integer.TryParse(txtPhoneNumber.Text.Trim(), StaffMember.PhoneNumber)
        StaffMember.HireDate = dtpHireDate.Value
        Decimal.TryParse(txtSalary.Text.Trim(), StaffMember.Salary)
        Me.DialogResult = DialogResult.OK
    End Sub
End Class
