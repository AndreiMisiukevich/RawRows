using System.Collections.Generic;
using AVFoundation;
using FifteenInRow.iOS;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace FifteenInRow.iOS
{
    public class AudioService : IAudioService
    {
        private Dictionary<string, AVAudioPlayer> _players = new Dictionary<string, AVAudioPlayer>();

        public void Play(string resource, bool isLoop)
        {
            if (_players.TryGetValue(resource, out AVAudioPlayer player))
            {
                player.Stop();
                player.Dispose();
            }
            player = AVAudioPlayer.FromUrl(NSUrl.FromFilename(resource));
            player.NumberOfLoops = isLoop ? -1 : 0;
            _players[resource] = player;
            player.Play();
        }

        public void Stop(string resource)
        {
            if(!_players.TryGetValue(resource, out AVAudioPlayer player))
            {
                return;
            }
            player.Stop();
        }
    }
}

/*
public AudioService() {}

    private MediaPlayer _mediaPlayer;

    public void PlaySound()
    {
        _mediaPlayer = MediaPlayer.Create(global::Android.App.Application.Context, Resource.Raw.Monkeys);
    _mediaPlayer.Start();
    }

    public void StopSound()
    {
        if (_mediaPlayer != null)
            _mediaPlayer.Stop();
    }
*/