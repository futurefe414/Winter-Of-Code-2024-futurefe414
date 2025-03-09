using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using SastImg.Client.Service.API;
using SastImg.Client.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SastImg.Client.Views
{
    public partial class ImageViewModel : ObservableObject
    {
        [ObservableProperty]
        private byte[]? imageData;
        private Image img; // Add this line to declare the img field

        public async Task<bool> ShowImageAsync(long id)
        {
            var imageResponse = await App.API?.Image.GetImageAsync(id, 0);
            if (!imageResponse.IsSuccessful) return false;

            using var m = new MemoryStream();
            await imageResponse.Content.CopyToAsync(m);
            ImageData = m.ToArray();
            return true;
        }

        public ImageViewModel()
        {
            PropertyChanged += async (sender, e) =>
            {
                if (e.PropertyName == nameof(ImageData)) // 如果属性的名字是“ImageData”
                {
                    await UpdateImageAsync();
                }
            };
        }

        private async Task UpdateImageAsync()
        {
            if (imageData is null)
            {
                img.Source = null;
                return;
            }
            var s = new MemoryStream(imageData);
            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(s.AsRandomAccessStream());
            img.Source = bitmap;
        }
    }

    internal class Image
    {
        public object Source { get; internal set; }
    }
}
