using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using SastImg.Client.Service.API;

namespace SastImg.Client.Views.Dialogs;

public sealed partial class UploadImageDialog : ContentDialog
{
    private List<StorageFile> _selectedFiles = new();
    private readonly long _albumId;

    public bool UploadSuccessful { get; private set; }

    public UploadImageDialog(long albumId)
    {
        this.InitializeComponent();
        _albumId = albumId;
    }

    private async void ChooseFiles_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".gif");
        picker.FileTypeFilter.Add(".bmp");
        picker.FileTypeFilter.Add(".webp");

        var files = await picker.PickMultipleFilesAsync();
        if (files != null && files.Count > 0)
        {
            _selectedFiles = files.ToList();
            FileCountText.Text = $"{_selectedFiles.Count} file(s) selected";
        }
    }

    private async void UploadButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var deferral = args.GetDeferral();

        try
        {
            if (_selectedFiles.Count == 0)
            {
                ErrorInfoBar.Message = "Please select at least one image file.";
                ErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                return;
            }

            // Validate file sizes (max 10MB per file)
            const long maxFileSize = 10 * 1024 * 1024;
            foreach (var file in _selectedFiles)
            {
                var props = await file.GetBasicPropertiesAsync();
                if (props.Size > maxFileSize)
                {
                    ErrorInfoBar.Message = $"File '{file.Name}' exceeds 10MB limit.";
                    ErrorInfoBar.IsOpen = true;
                    args.Cancel = true;
                    return;
                }
            }

            // Parse tags
            var tags = new List<long>();
            if (!string.IsNullOrWhiteSpace(TagsTextBox.Text))
            {
                var tagStrings = TagsTextBox.Text.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var tagStr in tagStrings)
                {
                    if (long.TryParse(tagStr.Trim(), out var tagId))
                    {
                        tags.Add(tagId);
                    }
                }

                if (tags.Count > 10)
                {
                    ErrorInfoBar.Message = "Maximum 10 tags allowed.";
                    ErrorInfoBar.IsOpen = true;
                    args.Cancel = true;
                    return;
                }
            }

            // Show progress
            ProgressPanel.Visibility = Visibility.Visible;
            IsPrimaryButtonEnabled = false;
            IsSecondaryButtonEnabled = false;

            // Upload files
            int uploaded = 0;
            int total = _selectedFiles.Count;

            foreach (var file in _selectedFiles)
            {
                try
                {
                    var buffer = await FileIO.ReadBufferAsync(file);
                    var bytes = new byte[buffer.Length];
                    using (var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                    {
                        reader.ReadBytes(bytes);
                    }

                    var title = string.IsNullOrWhiteSpace(TitleTextBox.Text)
                        ? Path.GetFileNameWithoutExtension(file.Name)
                        : TitleTextBox.Text;

                    var body = new Body2
                    {
                        Title = title,
                        Image = bytes,
                        Tags = tags
                    };

                    var response = await App.API.Image.AddImageAsync(_albumId, body);
                    if (response.IsSuccessStatusCode)
                    {
                        uploaded++;
                        UploadProgressBar.Value = (uploaded * 100.0) / total;
                        ProgressText.Text = $"{uploaded} / {total} uploaded";
                    }
                    else
                    {
                        ErrorInfoBar.Message = $"Failed to upload '{file.Name}': {response.Error?.Content}";
                        ErrorInfoBar.IsOpen = true;
                    }
                }
                catch (Exception ex)
                {
                    ErrorInfoBar.Message = $"Error uploading '{file.Name}': {ex.Message}";
                    ErrorInfoBar.IsOpen = true;
                }
            }

            UploadSuccessful = uploaded > 0;

            if (uploaded == total)
            {
                // All uploaded successfully, close dialog
                args.Cancel = false;
            }
            else
            {
                // Some failed, keep dialog open
                args.Cancel = true;
                IsPrimaryButtonEnabled = true;
                IsSecondaryButtonEnabled = true;
            }
        }
        finally
        {
            deferral.Complete();
        }
    }
}
