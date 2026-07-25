Imports System.IO
Imports System.Linq
Imports Microsoft.Win32

Class MainWindow

    Private Const RepositoryUrl As String = "https://github.com/henriksb/ExtensionSpoofer"
    Private Const IconsFolderName As String = "Win10Icons"
    Private Const NoFileSelectedText As String = "Current file: None"
    Private Const RightToLeftOverrideCharacter As Char = ChrW(&H202E)
    Private ReadOnly SupportedSourceExtensions As String() = {"scr", "exe", "com"}
    Private iconFolderPath As String = IO.Path.Combine(My.Application.Info.DirectoryPath, IconsFolderName)

    Public Sub OnMainWindowLoaded(sender As Object, e As RoutedEventArgs)
        InitializeSourceExtensions()

        If Not TryInitializeIconFolder() Then
            Environment.Exit(0)
            Return
        End If

        UpdateName()
        SetStatus("Ready", Brushes.SeaGreen)
    End Sub

    Private Sub InitializeSourceExtensions()
        SourceExtension.Items.Clear()
        For Each sourceExtensionValue In SupportedSourceExtensions
            SourceExtension.Items.Add(sourceExtensionValue)
        Next

        If SourceExtension.Items.Count > 0 Then
            SourceExtension.SelectedIndex = 0
        End If
    End Sub

    Private Function TryInitializeIconFolder() As Boolean
        If Not Directory.Exists(iconFolderPath) Then
            Dim warningMessage = $"{IconsFolderName} folder not found. Please select a folder with .ico files, or download {IconsFolderName} from {RepositoryUrl}."
            MessageBox.Show(warningMessage, $"{IconsFolderName} folder not found", MessageBoxButton.OK, MessageBoxImage.Warning)

            Using iconFolderDialog As New Forms.FolderBrowserDialog()
                iconFolderDialog.Description = "Select icon folder"
                iconFolderDialog.ShowDialog()
                iconFolderPath = iconFolderDialog.SelectedPath
            End Using
        End If

        If String.IsNullOrWhiteSpace(iconFolderPath) OrElse Not Directory.Exists(iconFolderPath) Then
            Return False
        End If

        LoadSpoofExtensions(iconFolderPath)
        Return True
    End Function

    Private Sub LoadSpoofExtensions(selectedFolderPath As String)
        SpoofExtension.Items.Clear()

        Dim iconExtensionNames = Directory.GetFiles(selectedFolderPath, "*.ico") _
            .Select(Function(iconPath) IO.Path.GetFileNameWithoutExtension(iconPath)) _
            .Where(Function(extensionName) Not String.IsNullOrWhiteSpace(extensionName)) _
            .OrderBy(Function(extensionName) extensionName) _
            .ToArray()

        For Each iconExtensionName In iconExtensionNames
            SpoofExtension.Items.Add(iconExtensionName)
        Next

        If SpoofExtension.Items.Count > 0 Then
            SpoofExtension.SelectedIndex = 0
            SetStatus($"Loaded {SpoofExtension.Items.Count} icon extension(s)", Brushes.SeaGreen)
        Else
            SetStatus("No .ico files found in selected folder", Brushes.IndianRed)
        End If
    End Sub

    Private Sub OpenFile_Click(sender As Object, e As RoutedEventArgs) Handles OpenFile.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Select executable"
        fd.Filter = "Executable files (*.exe)|*.exe"
        fd.FilterIndex = 1
        fd.RestoreDirectory = True

        If fd.ShowDialog() <> True OrElse String.IsNullOrWhiteSpace(fd.FileName) Then
            Return
        End If

        CurrentFile.Text = fd.FileName
        SetStatus("File selected", Brushes.SeaGreen)
        UpdateName()
    End Sub

    'Next three methods are only for updating the live view of your output file
    Private Sub SpoofExtension_DropDownClosed(sender As Object, e As EventArgs) Handles SpoofExtension.DropDownClosed
        UpdateName()
    End Sub

    Private Sub SourceExtension_DropDownClosed(sender As Object, e As EventArgs) Handles SourceExtension.DropDownClosed
        UpdateName()
    End Sub

    Private Sub NameInput_TextChanged(sender As Object, e As TextChangedEventArgs) Handles NameInput.TextChanged
        UpdateName()
    End Sub

    Private Sub UpdateName()
        OutputName.Text = NameInput.Text + StrReverse(SourceExtension.Text) + "." + SpoofExtension.Text

        Dim iconPreviewPath = GetIconPath(SpoofExtension.Text)
        If String.IsNullOrWhiteSpace(iconPreviewPath) OrElse Not File.Exists(iconPreviewPath) Then
            IconImage.Source = Nothing
            Return
        End If

        Try
            IconImage.Source = New BitmapImage(New Uri(iconPreviewPath, UriKind.Absolute))
        Catch
            IconImage.Source = Nothing
        End Try
    End Sub

    Private Sub GenerateButton_Click(sender As Object, e As RoutedEventArgs) Handles GenerateButton.Click
        If Not ValidateInputs() Then
            SetStatus("Please fill in all fields before generating", Brushes.IndianRed)
            MessageBox.Show("Please fill in all fields before generating", "Not all fields filled in", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        If Not File.Exists(CurrentFile.Text) Then
            SetStatus("File does not exist. Please select a new file.", Brushes.IndianRed)
            MessageBox.Show("File does not exist. Please select a new file.", "File does not exist", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        If NameInput.Text.IndexOfAny(IO.Path.GetInvalidFileNameChars()) >= 0 Then
            SetStatus("File name contains invalid characters", Brushes.IndianRed)
            MessageBox.Show("The selected file name contains invalid characters.", "Invalid file name", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Dim iconFilePath = GetIconPath(SpoofExtension.Text)
        If String.IsNullOrWhiteSpace(iconFilePath) OrElse Not File.Exists(iconFilePath) Then
            SetStatus("Icon file not found for selected spoof extension", Brushes.IndianRed)
            MessageBox.Show("The icon file for the selected spoof extension was not found.", "Icon not found", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Dim SpoofName = NameInput.Text + RightToLeftOverrideCharacter + StrReverse(SpoofExtension.Text) + "." + SourceExtension.Text
        Dim originalFilePath = CurrentFile.Text
        Dim originalFileName = IO.Path.GetFileName(originalFilePath)
        Dim directoryPath = IO.Path.GetDirectoryName(CurrentFile.Text)
        If String.IsNullOrWhiteSpace(directoryPath) Then
            SetStatus("Selected file path is invalid", Brushes.IndianRed)
            MessageBox.Show("Selected file path is invalid.", "Invalid file path", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Dim newFilePath = IO.Path.Combine(directoryPath, SpoofName)
        Dim renamed As Boolean = False

        Try
            My.Computer.FileSystem.RenameFile(originalFilePath, SpoofName)
            renamed = True
            Iconchanger.InjectIcon(newFilePath, iconFilePath)
            CurrentFile.Text = newFilePath
            UpdateName()
        Catch ex As UnauthorizedAccessException
            Dim rollbackFailureMessage = GetRollbackFailureMessage(renamed, newFilePath, originalFileName)

            Dim accessDeniedMessage = $"No access to path: {originalFilePath}.{rollbackFailureMessage}"
            SetStatus(accessDeniedMessage, Brushes.IndianRed)
            MessageBox.Show(accessDeniedMessage, "Access denied", MessageBoxButton.OK, MessageBoxImage.Error)
            Return
        Catch ex As IOException
            Dim rollbackFailureMessage = GetRollbackFailureMessage(renamed, newFilePath, originalFileName)

            Dim ioErrorMessage = $"Unable to spoof file: {ex.Message}.{rollbackFailureMessage}"
            SetStatus(ioErrorMessage, Brushes.IndianRed)
            MessageBox.Show(ioErrorMessage, "File operation failed", MessageBoxButton.OK, MessageBoxImage.Error)
            Return
        Catch ex As Exception
            Dim rollbackFailureMessage = GetRollbackFailureMessage(renamed, newFilePath, originalFileName)

            Dim unexpectedErrorMessage = $"Unexpected error: {ex.Message}.{rollbackFailureMessage}"
            SetStatus(unexpectedErrorMessage, Brushes.IndianRed)
            MessageBox.Show(unexpectedErrorMessage, "Operation failed", MessageBoxButton.OK, MessageBoxImage.Error)
            Return
        End Try

        SetStatus("Successfully spoofed file", Brushes.SeaGreen)
        MessageBox.Show("Successfully spoofed file!", "Success", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Function ValidateInputs() As Boolean
        Return Not String.IsNullOrWhiteSpace(SpoofExtension.Text) _
            AndAlso Not String.IsNullOrWhiteSpace(SourceExtension.Text) _
            AndAlso Not String.IsNullOrWhiteSpace(CurrentFile.Text) _
            AndAlso CurrentFile.Text <> NoFileSelectedText
    End Function

    Private Function GetIconPath(extensionName As String) As String
        If String.IsNullOrWhiteSpace(extensionName) Then
            Return String.Empty
        End If

        Return IO.Path.Combine(iconFolderPath, $"{extensionName}.ico")
    End Function

    Private Function GetRollbackFailureMessage(renamed As Boolean, newFilePath As String, originalFileName As String) As String
        If Not renamed Then
            Return String.Empty
        End If

        If TryRollbackRename(newFilePath, originalFileName) Then
            Dim directoryPath = IO.Path.GetDirectoryName(newFilePath)
            If Not String.IsNullOrWhiteSpace(directoryPath) Then
                CurrentFile.Text = IO.Path.Combine(directoryPath, originalFileName)
            End If

            Return String.Empty
        End If

        Return " The original file name could not be restored automatically."
    End Function

    Private Function TryRollbackRename(newFilePath As String, originalFileName As String) As Boolean
        Try
            My.Computer.FileSystem.RenameFile(newFilePath, originalFileName)
            Return True
        Catch
            Return False
        End Try
    End Function

    Private Sub SetStatus(message As String, colorBrush As Brush)
        If StatusText Is Nothing Then
            Return
        End If

        StatusText.Text = message
        StatusText.Foreground = colorBrush
    End Sub
End Class
