using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#if __WASM__
using System.Net.Http;
using Newtonsoft.Json;
using Uno.UI.Wasm;
#endif

namespace GyumeshiPolice
{
    public static class CustomVisionHelper
    {
        private static readonly AppConfig _config = new AppConfig();

#if __WASM__
        private static HttpClient HttpClient { get; } = new HttpClient(new WasmHttpHandler());
#endif

        private static CustomVisionPredictionClient Client { get; } = new CustomVisionPredictionClient()
        {
            ApiKey = _config.CustomVisionSettings.ApiKey,
            Endpoint = _config.CustomVisionSettings.Endpoint
        };

        public static async Task<ImagePrediction> ClassifyImageAsync(Stream stream)
        {
#if __WASM__
            // Custom Vision SDK は Wasm の HttpClient を使用しても
            // Custom Vision 側で BadRequestImageFormat になるため、直接 Web API を叩くようにする
            var req = new HttpRequestMessage
            {
                RequestUri = new Uri($"{_config.CustomVisionSettings.Endpoint}/customvision/v3.0/Prediction/{_config.CustomVisionSettings.ProjectId}/classify/iterations/{_config.CustomVisionSettings.PublishedName}/image"),
                Method = HttpMethod.Post,
                Content = new StreamContent(stream)
            };
            req.Content.Headers.Add("Content-Type", "application/octet-stream");
            req.Headers.Add("Prediction-key", _config.CustomVisionSettings.ApiKey);

            var res = await HttpClient.SendAsync(req);
            var content = await res.Content.ReadAsStringAsync();

            // JSON のデシリアライズをするためには LinkerConfig.xml で設定が必要
            return JsonConvert.DeserializeObject<ImagePrediction>(content);
#else
            return await Client.ClassifyImageWithNoStoreAsync(
                new Guid(_config.CustomVisionSettings.ProjectId),
                _config.CustomVisionSettings.PublishedName, stream);
#endif
        }
    }
}
