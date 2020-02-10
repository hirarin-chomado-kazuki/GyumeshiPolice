using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GyumeshiPolice.Services
{
    public class ImageFilePredictionService : IImageFilePredictionService
    {
        public async Task<(ImageSource, Stream)> SelectFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".gif");

            // 選択したファイル情報を保持
            var file = await picker.PickSingleFileAsync();

            var stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream);

            // 返却用の Stream
            stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);
            return (bitmapImage, stream.AsStream());
        }
    }
}
