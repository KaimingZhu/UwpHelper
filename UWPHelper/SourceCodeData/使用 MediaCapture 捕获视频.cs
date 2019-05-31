LowLagMediaRecording _mediaRecording;
var myVideos = await Windows.Storage.StorageLibrary.GetLibraryAsync(Windows.Storage.KnownLibraryId.Videos);
StorageFile file = await myVideos.SaveFolder.CreateFileAsync("video.mp4", CreationCollisionOption.GenerateUniqueName);
_mediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
        MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), file);
await _mediaRecording.StartAsync();

await _mediaRecording.StopAsync();

await _mediaRecording.FinishAsync();

mediaCapture.RecordLimitationExceeded += MediaCapture_RecordLimitationExceeded;

var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
StorageFile file = await localFolder.CreateFileAsync("audio.mp3", CreationCollisionOption.GenerateUniqueName);
_mediaRecording = await mediaCapture.PrepareLowLagRecordToStorageFileAsync(
        MediaEncodingProfile.CreateMp3(AudioEncodingQuality.High), file);
await _mediaRecording.StartAsync();