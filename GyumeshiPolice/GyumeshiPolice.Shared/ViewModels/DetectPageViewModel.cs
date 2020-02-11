using GyumeshiPolice.Views;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Media.Capture;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GyumeshiPolice.ViewModels
{
    public class DetectPageViewModel : Observable
    {
        private ImageSource imageSource;
        public ImageSource ImageSource
        {
            get { return imageSource; }
            set { this.Set(ref this.imageSource, value); }
        }

        private string message;
        public string Message
        {
            get { return message; }
            set { this.Set(ref this.message, value); }
        }

        private bool isProgressRingActive;
        public bool IsProgressRingActive
        {
            get { return isProgressRingActive; }
            set { this.Set(ref this.isProgressRingActive, value); }
        }

        private Services.IImageFilePredictionService imageFilePredictionService;
        public Services.IImageFilePredictionService ImageFilePredictionService
        {
            get { return this.imageFilePredictionService; }
            set { this.Set(ref this.imageFilePredictionService, value); }
        }

        public ICommand TweetCommand { get; }
        public ICommand CameraCommand { get; }
        public ICommand FileCommand { get; }

        public DetectPageViewModel()
        {
            // 初期表示
            ImageSource = new BitmapImage
            {
#if __WASM__
                UriSource = new Uri("ms-appx:///Assets/square150x150logo.scale-200.png")
#else
                UriSource = new Uri("ms-appx:///Assets/square150x150logo.png")
#endif
            };
            Message = "カメラを起動して牛めし/牛丼判定をしてみよう！\nすでに撮影済みの写真も使えるよ";

            CameraCommand = new DelegateCommand(async () =>
            {
                var captureUI = new CameraCaptureUI();
                captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

                if (photo == null)
                {
                    return;
                }
                else
                {
                    var source = new BitmapImage(new Uri(photo.Path));
                    var stream = await photo.OpenStreamForReadAsync();


                    await ConfirmAndPredict(source, stream);
                }
            });

            FileCommand = new DelegateCommand(async () =>
            {
                var imageData = await ImageFilePredictionService.SelectFileAsync();

                if (imageData.Source != null)
                {
                    await ConfirmAndPredict(imageData.Source, imageData.Stream);
                }
            });
        }

        private async Task ConfirmAndPredict(ImageSource source, Stream stream)
        {
            // 確認ダイアログ
            var dialog = new ConfirmDialog { ImageSource = source };
            var dialogResult = await dialog.ShowAsync();

            if (dialogResult == ContentDialogResult.Primary)
            {
                IsProgressRingActive = true;

                var result = await CustomVisionHelper.ClassifyImageAsync(stream);

                var predictions = result.Predictions
                    .OrderByDescending(r => r.Probability)
                    .Select(r => (ShopName: CustomVisionHelper.ToShopName(r.TagName), Probability: Math.Round(r.Probability * 100, 3)));

                Message = $@"{predictions.First().ShopName}の確率: {predictions.First().Probability}％
({string.Join(", ", predictions.Skip(1).Select(r => $"{r.ShopName}: {r.Probability}％"))})";

                ImageSource = source;

                IsProgressRingActive = false;
            }
        }
    }
}
