using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GyumeshiPolice.Services
{
    public class ImageFilePredictionService : IImageFilePredictionService
    {
        public async Task PredictImageFileAsync()
        {
            await new Windows.UI.Popups.MessageDialog("ここでファイルを選択して Custom Vision を呼ぶ").ShowAsync();
        }
    }
}
