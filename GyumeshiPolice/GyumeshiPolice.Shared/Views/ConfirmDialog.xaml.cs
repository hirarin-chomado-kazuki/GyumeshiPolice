using GyumeshiPolice.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace GyumeshiPolice.Views
{
    public sealed partial class ConfirmDialog : ContentDialog
    {
        public ImageSource ImageSource { get; set; }

        public ConfirmDialog()
        {
            this.InitializeComponent();
        }
    }
}
