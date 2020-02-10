using Foundation;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GyumeshiPolice.Services
{
    public class ImageFilePredictionService : IImageFilePredictionService
    {
        private NSData _data;
        private TaskCompletionSource<(ImageSource, Stream)> taskCompletionSource;
        private UIImagePickerController imagePicker;

        public Task<(ImageSource, Stream)> SelectFileAsync()
        {
            imagePicker = new UIImagePickerController
            {
                SourceType = UIImagePickerControllerSourceType.PhotoLibrary,
                MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.PhotoLibrary)
            };

            imagePicker.FinishedPickingMedia += OnImagePickerFinishedPickingMedia;
            imagePicker.Canceled += OnImagePickerCancelled;

            var window = UIApplication.SharedApplication.KeyWindow;
            var viewController = window.RootViewController;
            viewController.PresentModalViewController(imagePicker, true);

            taskCompletionSource = new TaskCompletionSource<(ImageSource, Stream)>();
            return taskCompletionSource.Task;
        }

        void OnImagePickerFinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs args)
        {
            var image = args.EditedImage ?? args.OriginalImage;

            if (image != null)
            {
                // UIImage から .NET の BitmapImage へ変換 
                if (args.ReferenceUrl.PathExtension.Equals("PNG") || args.ReferenceUrl.PathExtension.Equals("png"))
                {
                    _data = image.AsPNG();
                }
                else
                {
                    _data = image.AsJPEG(1);
                }
                var stream = _data.AsStream();
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(stream);

                UnregisterEventHandlers();

                // 返却用
                stream = _data.AsStream();

                taskCompletionSource.SetResult((bitmapImage, stream));
            }
            else
            {
                UnregisterEventHandlers();
                taskCompletionSource.SetResult((null, null));
            }
            imagePicker.DismissModalViewController(true);
        }

        void OnImagePickerCancelled(object sender, EventArgs args)
        {
            UnregisterEventHandlers();
            taskCompletionSource.SetResult((null, null));
            imagePicker.DismissModalViewController(true);
        }

        void UnregisterEventHandlers()
        {
            imagePicker.FinishedPickingMedia -= OnImagePickerFinishedPickingMedia;
            imagePicker.Canceled -= OnImagePickerCancelled;
        }
    }
}
