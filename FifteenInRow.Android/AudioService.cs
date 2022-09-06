using System.Collections.Generic;
using Android.Media;
using FifteenInRow.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioService))]
namespace FifteenInRow.Droid
{
    public class AudioService : IAudioService
    {
        private Dictionary<string, MediaPlayer> _players = new Dictionary<string, MediaPlayer>();

        public void Play(string resource, bool isLoop)
        {
            if (_players.TryGetValue(resource, out MediaPlayer player))
            {
                player.Stop();
                player.Dispose();
            }
            var fd = global::Android.App.Application.Context.Assets.OpenFd(resource);
            player = new MediaPlayer { Looping = isLoop };
            player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
            _players[resource] = player;
            player.Prepared += (s, e) => ((MediaPlayer)s).Start();
            player.Prepare();
        }

        public void Stop(string resource)
        {
            if(!_players.TryGetValue(resource, out MediaPlayer player))
            {
                return;
            }
            player.Stop();
        }
    }
}
