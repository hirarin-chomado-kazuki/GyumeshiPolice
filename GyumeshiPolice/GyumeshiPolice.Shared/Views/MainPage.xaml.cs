using GyumeshiPolice.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace GyumeshiPolice.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        public MainPage()
        {
            this.InitializeComponent();

            // 初期表示
            RadioButtonDetect.IsChecked = true;
        }

        private void RadioButtonDetect_Checked(object sender, RoutedEventArgs e)
        {
            NavigateTo(typeof(DetectPage));
        }

        private void RadioButtonExam_Checked(object sender, RoutedEventArgs e)
        {
            NavigateTo(typeof(ExamPage));
        }

        private void RadioButtonAbout_Checked(object sender, RoutedEventArgs e)
        {
            NavigateTo(typeof(AboutPage));
        }

        private void NavigateTo(Type page)
        {
            ContentFrame.Navigate(page, null, new SuppressNavigationTransitionInfo());

            // 画面が狭いとき（CompactOverlay のとき）だけ閉じるようにする
            SplitView.IsPaneOpen = (SplitView.DisplayMode != SplitViewDisplayMode.CompactOverlay);
        }
    }

    public static class SymbolEx
    {
        public static Symbol GlobalNavigationButton { get; } = (Symbol)59136;
    }
}
