using GyumeshiPolice.Services;
using GyumeshiPolice.ViewModels;
using Windows.UI.Xaml.Controls;

namespace GyumeshiPolice.Views
{
    public sealed partial class DetectPage : Page
    {
        public DetectPageViewModel ViewModel { get; } = new DetectPageViewModel();

        public DetectPage()
        {
            this.InitializeComponent();

            ViewModel.ImageFilePredictionService = new ImageFilePredictionService();
        }
    }
}
