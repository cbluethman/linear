using System.IO;
using System.Windows;
using NAudio.Wave;

namespace LinearAlgebra.App.Services;

public class SoundService
{
    private bool _muted;
    private float _volume = 0.5f;

    public bool IsMuted
    {
        get => _muted;
        set => _muted = value;
    }

    public float Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0f, 1f);
    }

    public void PlayClick()    => Play("Sounds/click.wav");
    public void PlaySnap()     => Play("Sounds/snap.wav");
    public void PlayWhoosh()   => Play("Sounds/whoosh.wav");
    public void PlayCorrect()  => Play("Sounds/correct.wav");
    public void PlayWrong()    => Play("Sounds/wrong.wav");
    public void PlayNavigate() => Play("Sounds/navigate.wav");

    private void Play(string resourcePath)
    {
        if (_muted) return;

        try
        {
            var uri = new Uri($"pack://application:,,,/{resourcePath}", UriKind.Absolute);
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo == null) return;

            var reader = new WaveFileReader(streamInfo.Stream);
            var volumeProvider = new VolumeWaveProvider16(new WaveChannel32(reader))
            {
                Volume = _volume
            };

            var outputDevice = new WaveOutEvent();
            outputDevice.Init(volumeProvider);
            outputDevice.PlaybackStopped += (_, _) =>
            {
                outputDevice.Dispose();
                reader.Dispose();
            };
            outputDevice.Play();
        }
        catch
        {
            // Sound playback is non-critical — fail silently
        }
    }
}
