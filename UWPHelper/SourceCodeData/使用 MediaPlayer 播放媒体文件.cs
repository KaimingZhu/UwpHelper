mediaPlayer = new MediaPlayer();
mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/example_video.mkv"));
mediaPlayer.Play();

_mediaPlayerElement.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/example_video.mkv"));
mediaPlayer = _mediaPlayerElement.MediaPlayer;
mediaPlayer.Play();

