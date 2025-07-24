Imports System.Windows.Forms

Public Class LoginForm
    Inherits Form

    Private txtEmail As TextBox
    Private txtPassword As TextBox
    Private btnLogin As Button
    Private lblError As Label

    Public Sub New()
        Me.Text = "Login - MART Sales Management"
        Me.Size = New Size(600, 480)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.White

        Dim formWidth As Integer = Me.ClientSize.Width
        Dim titleFont As New Font("Segoe UI", 28, FontStyle.Bold)
        Dim lblTitle As New Label With {
            .Text = "MART Sales Management",
            .Font = titleFont,
            .ForeColor = Color.FromArgb(0, 120, 215),
            .AutoSize = True
        }
        ' Center the title after adding to Controls
        Me.Controls.Add(lblTitle)
        lblTitle.Left = (Me.ClientSize.Width - lblTitle.Width) \ 2
        lblTitle.Top = 50

        Dim leftPad As Integer = 80
        Dim ctrlWidth As Integer = 440
        Dim lblEmail As New Label With {.Text = "Email", .Location = New Point(leftPad, 130), .Font = New Font("Segoe UI", 13)}
        txtEmail = New TextBox With {.Location = New Point(leftPad, 160), .Width = ctrlWidth, .Font = New Font("Segoe UI", 13)}
        Dim lblPassword As New Label With {.Text = "Password", .Location = New Point(leftPad, 210), .Font = New Font("Segoe UI", 13)}
        txtPassword = New TextBox With {.Location = New Point(leftPad, 240), .Width = ctrlWidth, .Font = New Font("Segoe UI", 13), .UseSystemPasswordChar = True}
        btnLogin = New Button With {
            .Text = "Login",
            .Location = New Point(leftPad, 320),
            .Width = ctrlWidth,
            .Height = 50,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 16, FontStyle.Bold)
        }
        lblError = New Label With {
            .Text = "",
            .ForeColor = Color.Red,
            .Location = New Point(leftPad, 290),
            .AutoSize = True,
            .Font = New Font("Segoe UI", 12, FontStyle.Bold)
        }
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click
        Me.Controls.AddRange({lblEmail, txtEmail, lblPassword, txtPassword, btnLogin, lblError})
        ' Center the title again after all controls are added (for correct width)
        lblTitle.Left = (Me.ClientSize.Width - lblTitle.Width) \ 2
    End Sub

    Private Sub BtnLogin_Click(sender As Object, e As EventArgs)
        Dim email = txtEmail.Text.Trim().ToLower()
        Dim password = txtPassword.Text
        If email = "user@mart.com" AndAlso password = "password" Then
            Me.DialogResult = DialogResult.OK
        Else
            lblError.Text = "Invalid email or password."
        End If
    End Sub
End Class
