MediaCapture mediaCapture;
bool isPreviewing;
mediaCapture = new MediaCapture();
await mediaCapture.InitializeAsync();
mediaCapture.Failed += MediaCapture_Failed;

// Prepare and capture photo
var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat.Bgra8));

var capturedPhoto = await lowLagCapture.CaptureAsync();
var softwareBitmap = capturedPhoto.Frame.SoftwareBitmap;

await lowLagCapture.FinishAsync();   

var myPictures = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Pictures);
StorageFile file = await myPictures.SaveFolder.CreateFileAsync("photo.jpg", CreationCollisionOption.GenerateUniqueName);

using (var captureStream = new InMemoryRandomAccessStream())
{
    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

    using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
    {
        var decoder = await BitmapDecoder.CreateAsync(captureStream);
        var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

        var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
        await encoder.BitmapProperties.SetPropertiesAsync(properties);

        await encoder.FlushAsync();
    }
}