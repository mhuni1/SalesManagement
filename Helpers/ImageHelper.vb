Imports System.IO
Imports System.Drawing

Public Class ImageHelper
    Public Shared Function SaveImageAndGetPath(sourceFilePath As String, entityType As String, entityId As Integer) As String
        Dim imagesDir = Path.Combine(Application.StartupPath, "Images", entityType)
        If Not Directory.Exists(imagesDir) Then
            Directory.CreateDirectory(imagesDir)
        End If
        Dim ext = Path.GetExtension(sourceFilePath)
        Dim destFileName = $"{entityType}_{entityId}{ext}"
        Dim destFilePath = Path.Combine(imagesDir, destFileName)
        File.Copy(sourceFilePath, destFilePath, True)
        Return destFilePath
    End Function

    Public Shared Function LoadImage(imagePath As String) As Image
        If Not String.IsNullOrEmpty(imagePath) AndAlso File.Exists(imagePath) Then
            Using fs As New FileStream(imagePath, FileMode.Open, FileAccess.Read)
                Return Image.FromStream(fs)
            End Using
        End If
        Return Nothing
    End Function
End Class