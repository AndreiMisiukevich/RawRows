using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FifteenInRow
{
    public class SettingsViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ICommand _changeSettingCommand;
        private bool _isMusicDisabled = !Preferences.Get("ShouldPlayMusic", true);
        private bool _isSoundDisabled = !Preferences.Get("ShouldPlaySound", true);
        private int _mapSize = Preferences.Get("MapSize", 4);

        public ICommand ChangeSettingCommand => _changeSettingCommand ?? (_changeSettingCommand = new Command(p =>
        {
            if (Preferences.Get("ShouldPlaySound", true))
                DependencyService.Resolve<IAudioService>().Play("click.mp3", false);

            switch (p.ToString())
            {
                case "music":
                    IsMusicDisabled = !IsMusicDisabled;
                    break;
                case "sound":
                    IsSoundDisabled = !IsSoundDisabled;
                    break;
                case "decreaseMapSize":
                    if (Preferences.Get("MapSize", 4) > 3)
                    {
                        Preferences.Set("MapSize", --_mapSize);
                        OnPropertyChanged(nameof(MapSizeText));
                    }
                    break;
                case "increaseMapSize":
                    if (Preferences.Get("MapSize", 4) < 6)
                    {
                        Preferences.Set("MapSize", ++_mapSize);
                        OnPropertyChanged(nameof(MapSizeText));
                    }
                    break;
            }
        }));

        public bool IsMusicDisabled
        {
            get => _isMusicDisabled;
            set {
                _isMusicDisabled = value;
                Preferences.Set("ShouldPlayMusic", !value);
                var service = DependencyService.Resolve<IAudioService>();
                if(value)
                {
                    service.Stop("backMusic.wav");
                }
                else
                {
                    service.Play("backMusic.wav", true);
                }
                
                OnPropertyChanged();
            }
        }

        public bool IsSoundDisabled
        {
            get => _isSoundDisabled;
            set
            {
                _isSoundDisabled = value;
                Preferences.Set("ShouldPlaySound", !value);
                OnPropertyChanged();
            }
        }

        public string MapSizeText => $"{_mapSize} X {_mapSize}";

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
