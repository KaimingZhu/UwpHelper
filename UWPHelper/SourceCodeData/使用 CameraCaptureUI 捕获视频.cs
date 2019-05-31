CameraCaptureUI captureUI = new CameraCaptureUI();
captureUI.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;

StorageFile videoFile = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Video);

if (videoFile == null)
{
    // User cancelled photo capture
    return;
}