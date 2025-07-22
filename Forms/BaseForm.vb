Public Class BaseForm
    Inherits Form

    Protected _dataService As DataService

    Public Sub New()
        _dataService = New DataService()
        SetupBaseUI()
    End Sub

    Private Sub SetupBaseUI()
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.MinimizeBox = False
        Me.MaximizeBox = False
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
    End Sub

    Protected Sub ShowError(message As String)
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Sub

    Protected Sub ShowInfo(message As String)
        MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class