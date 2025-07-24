Imports System.Windows.Forms

Public Class LoginForm
    Inherits Form

    Private txtEmail As TextBox
    Private txtPassword As TextBox
    Private btnLogin As Button
    Private lblError As Label

    Public Sub New()
        Me.Text = "Login - MART Sales Management"
        Me.Size = New Size(400, 350)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.White

        Dim lblTitle As New Label With {
            .Text = "MART Sales Management",
            .Font = New Font("Segoe UI", 18, FontStyle.Bold),
            .ForeColor = Color.FromArgb(0, 120, 215),
            .Location = New Point(40, 30),
            .AutoSize = True
        }
        Dim lblEmail As New Label With {.Text = "Email", .Location = New Point(40, 90), .Font = New Font("Segoe UI", 11)}
        txtEmail = New TextBox With {.Location = New Point(40, 120), .Width = 300, .Font = New Font("Segoe UI", 11)}
        Dim lblPassword As New Label With {.Text = "Password", .Location = New Point(40, 160), .Font = New Font("Segoe UI", 11)}
        txtPassword = New TextBox With {.Location = New Point(40, 190), .Width = 300, .Font = New Font("Segoe UI", 11), .UseSystemPasswordChar = True}
        btnLogin = New Button With {
            .Text = "Login",
            .Location = New Point(40, 240),
            .Width = 300,
            .Height = 40,
            .BackColor = Color.FromArgb(0, 120, 215),
            .ForeColor = Color.White,
            .FlatStyle = FlatStyle.Flat,
            .Font = New Font("Segoe UI", 12, FontStyle.Bold)
        }
        lblError = New Label With {
            .Text = "",
            .ForeColor = Color.Red,
            .Location = New Point(40, 220),
            .AutoSize = True,
            .Font = New Font("Segoe UI", 10, FontStyle.Bold)
        }
        AddHandler btnLogin.Click, AddressOf BtnLogin_Click
        Me.Controls.AddRange({lblTitle, lblEmail, txtEmail, lblPassword, txtPassword, btnLogin, lblError})
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
