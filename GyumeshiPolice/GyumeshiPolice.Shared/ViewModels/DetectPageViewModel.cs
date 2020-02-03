using System.Windows.Input;

namespace GyumeshiPolice.ViewModels
{
    public class DetectPageViewModel : Observable
    {
        private string imageSource;
        public string ImageSource
        {
            get { return imageSource; }
            set { this.Set(ref this.imageSource, value); }
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
            FileCommand = new DelegateCommand(async () => await ImageFilePredictionService.PredictImageFileAsync());
        }
    }
}
