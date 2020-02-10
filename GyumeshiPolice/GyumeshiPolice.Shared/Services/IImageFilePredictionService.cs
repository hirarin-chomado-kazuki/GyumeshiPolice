using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace GyumeshiPolice.Services
{
    public interface IImageFilePredictionService
    {
        Task<(ImageSource Source, Stream Stream)> SelectFileAsync();
    }
}
