using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Support.V4.App;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GyumeshiPolice.Services
{
	public class ImageFilePredictionService : IImageFilePredictionService
    {
        private const int ImageRequestCode = 1000;

        public async Task<(ImageSource, Stream)> SelectFileAsync()
        {
			var imagePickerActivity = await StartImagePickerActivity();

			var intent = new Intent();
			intent.SetType("image/*");
			intent.SetAction(Intent.ActionOpenDocument);

			var result = await imagePickerActivity.GetActivityResult(
				Intent.CreateChooser(intent, "Select photo"), ImageRequestCode);

			if (result.ResultCode != Result.Ok)
			{
				if (this.Log().IsEnabled(LogLevel.Information))
				{
					this.Log().LogInformation($"Image not selected. Result: {result.ResultCode}");
				}
				return (null, null);
			}

			var uri = result.Intent.Data;

			ContextHelper.Current.ContentResolver.TakePersistableUriPermission(uri, ActivityFlags.GrantReadUriPermission);

			var stream = ContextHelper.Current.ContentResolver.OpenInputStream(uri);

			return (new BitmapImage(new Uri(await FileHelper.GetPathFromUri(uri))), stream);
		}
		
        private async Task<ImagePickerActivity> StartImagePickerActivity()
		{
			var tcs = new TaskCompletionSource<ImagePickerActivity>();

			void handler(ImagePickerActivity instance) => tcs.TrySetResult(instance);

			try
			{
				ImagePickerActivity.Resumed += handler;
				ContextHelper.Current.StartActivity(typeof(ImagePickerActivity));
				return await tcs.Task;
			}
			finally
			{
				ImagePickerActivity.Resumed -= handler;
			}
		}


		[Activity(
			Theme = "@style/Theme.AppCompat.Translucent",
			ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize
		)]
		internal class ImagePickerActivity : FragmentActivity
		{
			public static event Action<ImagePickerActivity> Resumed;
			private static Dictionary<int, ImagePickerActivity> _originalActivities = new Dictionary<int, ImagePickerActivity>();

			private TaskCompletionSource<ImagePickerActivityResult> _resultCompletionSource = new TaskCompletionSource<ImagePickerActivityResult>();
			private int _requestCode;

			internal async Task<ImagePickerActivityResult> GetActivityResult(Intent intent, int requestCode = 0)
			{
				try
				{
					_originalActivities[requestCode] = this;
					_requestCode = requestCode;

					StartActivityForResult(intent, requestCode);

					return await _resultCompletionSource.Task;
				}
				finally
				{
					Finish();
					_originalActivities.Remove(requestCode);
				}
			}

			protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent intent)
			{
				base.OnActivityResult(requestCode, resultCode, intent);

				var isCurrentActivityANewOne = false;

				if (_originalActivities.TryGetValue(requestCode, out var originalActivity))
				{
					isCurrentActivityANewOne = originalActivity != this;
				}
				else
				{
					originalActivity = this;
				}

				if (isCurrentActivityANewOne)
				{
					// Close this activity (because we are not the original)
					Finish();
				}

				// Push a new request code in the calling activity
				if (originalActivity._requestCode == requestCode)
				{
					originalActivity._resultCompletionSource.TrySetResult(new ImagePickerActivityResult(_requestCode, resultCode, intent));
				}
			}

			protected override void OnDestroy()
			{
				base.OnDestroy();
				_resultCompletionSource?.TrySetResult(new ImagePickerActivityResult(_requestCode, Result.Canceled, null));
			}

			protected override void OnResume()
			{
				base.OnResume();

				Resumed?.Invoke(this);
			}
		}

		internal class ImagePickerActivityResult
		{
			public ImagePickerActivityResult(int requestCode, Result resultCode, Intent intent)
			{
				RequestCode = requestCode;
				ResultCode = resultCode;
				Intent = intent;
			}

			public int RequestCode { get; }

			public Result ResultCode { get; }

			public Intent Intent { get; }
		}
    }
}
