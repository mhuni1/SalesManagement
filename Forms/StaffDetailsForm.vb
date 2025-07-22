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
        Me.Size = New Size(400, 600)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False

        pictureBox = New PictureBox With {
            .Size = New Size(120, 120),
            .Location = New Point(20, 20),
            .BorderStyle = BorderStyle.FixedSingle,
            .SizeMode = PictureBoxSizeMode.Zoom
        }
        btnUpload = New Button With {
            .Text = "Upload Photo",
            .Location = New Point(160, 60),
            .Width = 120
        }
        openFileDialog = New OpenFileDialog With {
            .Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
        }
        AddHandler btnUpload.Click, AddressOf BtnUpload_Click
        Me.Controls.Add(pictureBox)
        Me.Controls.Add(btnUpload)

        Dim lblFirstName As New Label With {.Text = "First Name", .Location = New Point(20, 160)}
        txtFirstName = New TextBox With {.Location = New Point(120, 155), .Width = 220}
        Dim lblLastName As New Label With {.Text = "Last Name", .Location = New Point(20, 200)}
        txtLastName = New TextBox With {.Location = New Point(120, 195), .Width = 220}
        Dim lblPosition As New Label With {.Text = "Position", .Location = New Point(20, 240)}
        txtPosition = New TextBox With {.Location = New Point(120, 235), .Width = 220}
        Dim lblEmail As New Label With {.Text = "Email", .Location = New Point(20, 280)}
        txtEmail = New TextBox With {.Location = New Point(120, 275), .Width = 220}
        Dim lblPhoneNumber As New Label With {.Text = "Phone Number", .Location = New Point(20, 320)}
        txtPhoneNumber = New TextBox With {.Location = New Point(120, 315), .Width = 220}
        Dim lblHireDate As New Label With {.Text = "Hire Date", .Location = New Point(20, 360)}
        dtpHireDate = New DateTimePicker With {.Location = New Point(120, 355), .Width = 220, .Format = DateTimePickerFormat.Short}
        Dim lblSalary As New Label With {.Text = "Salary", .Location = New Point(20, 400)}
        txtSalary = New TextBox With {.Location = New Point(120, 395), .Width = 220}

        btnSave = New Button With {.Text = "Save", .Location = New Point(120, 460), .Width = 80}
        btnCancel = New Button With {.Text = "Cancel", .Location = New Point(220, 460), .Width = 80}
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        AddHandler btnCancel.Click, Sub() Me.DialogResult = DialogResult.Cancel

        Me.Controls.AddRange({lblFirstName, txtFirstName, lblLastName, txtLastName, lblPosition, txtPosition, lblEmail, txtEmail, lblPhoneNumber, txtPhoneNumber, lblHireDate, dtpHireDate, lblSalary, txtSalary, btnSave, btnCancel})
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
