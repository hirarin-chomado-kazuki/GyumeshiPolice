using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Uno.UI;

namespace GyumeshiPolice.Droid
{
    [Activity(
            MainLauncher = true,
            ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
            WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
        )]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        private const int PermissionRequestCode = 1111;

        protected override void OnStart()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var permissions = new string[]
                {
                    Android.Manifest.Permission.WriteExternalStorage,
                    Android.Manifest.Permission.ReadExternalStorage
                };

                foreach (var pm in permissions)
                {
                    if (ContextHelper.Current.CheckSelfPermission(pm) != Permission.Granted)
                    {
                        ((Activity)ContextHelper.Current).RequestPermissions(permissions, PermissionRequestCode);
                        break;
                    }
                }
            }

            base.OnStart();
        }
    }
}

