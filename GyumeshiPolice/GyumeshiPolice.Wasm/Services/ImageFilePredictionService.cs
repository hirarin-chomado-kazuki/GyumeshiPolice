using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Uno.Foundation;
using Uno.UI.Wasm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GyumeshiPolice.Services
{
    public class ImageFilePredictionService : IImageFilePredictionService
    {
        private static HttpClient HttpClient { get; } = new HttpClient(new WasmHttpHandler());

        public async Task<(ImageSource, Stream)> SelectFileAsync()
        {
            var htmlId = $"tempimg_{Guid.NewGuid().ToString()}";

            WebAssemblyRuntime.InvokeJS($"createImgFromFile('{htmlId}')");

            // ファイル選択が完了するまで待機
            var url = await GetImageUrlAsync(htmlId);

            // ローカル画像の Blob URL から画像の Stream を取得
            var stream = await HttpClient.GetStreamAsync(url);

            return (new BitmapImage { UriSource = new Uri(url) }, stream);
        }

        private async Task<string> GetImageUrlAsync(string htmlId)
        {
            string url;

            // 画像 URL が取得できるまで（選択が終了し反映されるまで）繰り返す
            do
            {
                url = WebAssemblyRuntime.InvokeJS($"getImageSrc('{htmlId}');");

                await Task.Delay(100).ConfigureAwait(false);

            } while (string.IsNullOrEmpty(url));

            return url;
        }
    }
}
