using ImageMagick;
using System.Windows.Input;

namespace DllUsageDemo.ViewModels;

public partial class MainViewModel : BaseViewModel
{
	[ObservableProperty]
	string errorMessage;

	[RelayCommand]
	async void SelectImage()
	{
		var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
		{
			Title = "Pick Image"
		});

		if (photo == null)
			return;

		await using var stream = new MemoryStream();
		await using var readStream = await photo.OpenReadAsync();
		await readStream.CopyToAsync(stream);
		stream.Position = 0;
		var data = stream.ToArray();
		try
		{
			using MagickImage image = new(data);
			if (image.Format.ToString().ToLower() is "heic" || image.Format.ToString().ToLower() is "heif")
			{
				image.Format = MagickFormat.Jpg;
				image.Write(stream);
				data = stream.ToArray();
			}
		}
		catch (Exception ex)
		{
			ErrorMessage = ex.ToString();
		}

		return;
	}
	private class TemporaryMediaFile
	{
		public string Key { get; set; }

		public byte[] FileContentAsByteArray { get; set; }
	}
}
