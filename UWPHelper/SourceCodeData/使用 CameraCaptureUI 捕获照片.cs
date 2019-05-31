CameraCaptureUI captureUI = new CameraCaptureUI();
captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200); 

StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

if (photo == null)
{
    // User cancelled photo capture
    return;
}

