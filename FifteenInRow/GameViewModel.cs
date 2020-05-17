using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace FifteenInRow
{
    public sealed class GameViewModel: INotifyPropertyChanged
    {
        private const int MapSize = 4;

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int[] _winSequence = GetEmptyMap().Skip(1).ToArray();
        private int[] _numbers;
        private ICommand _swapCommand;
        private ICommand _initGameCommand;
        private int _swapsCount;
        private int _emptyIndex;

        public GameViewModel()
            => InitGameCommand.Execute(null);

        public int[] Numbers
        {
            get => _numbers;
            set
            {
                _numbers = value;
                OnPropertyChanged();
            }
        }

        public int SwapsCount
        {
            get => _swapsCount;
            set
            {
                _swapsCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand PerformTransitionCommand { get; set; }

        public ICommand HandleWinCommand { get; set; }

        public ICommand SwapCommand => _swapCommand ?? (_swapCommand = new Command(p =>
        {
            var value = (int)p;
            var index = Numbers.IndexOf(value);
            if(Math.Abs(index / MapSize - _emptyIndex / MapSize) + Math.Abs(index % MapSize - _emptyIndex % MapSize) != 1)
            {
                return;
            }
            PerformTransitionCommand?.Execute(new TransitionModel(index, _emptyIndex, Numbers[index]));
            SwapElements(Numbers, index, _emptyIndex);
            _emptyIndex = index;
            if (Preferences.Get("ShouldPlaySound", true))
                DependencyService.Resolve<IAudioService>().Play("swap.mp3", false);

            ++SwapsCount;
            if (Numbers.Take(Numbers.Length - 1).SequenceEqual(_winSequence))
            {
                HandleWinCommand?.Execute(SwapsCount);
            }
        }));

        public ICommand InitGameCommand => _initGameCommand ?? (_initGameCommand = new Command(() =>
        {
            Numbers = new int []{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 0, 15 }; //ShuffleArray(GetEmptyMap());
            _emptyIndex = Numbers.IndexOf(0);
            SwapsCount = 0;
        }));

        private static int[] GetEmptyMap()
            => Enumerable.Range(0, MapSize * MapSize).ToArray();

        private static TValue[] ShuffleArray<TValue>(TValue[] array)
        {
            var random = new Random();
            for (int i = 0; i < array.Length; ++i)
            {
                var randomIndex = random.Next(0, array.Length - 1);
                SwapElements(array, i, randomIndex);
            }
            return array;
        }

        private static void SwapElements<TValue>(TValue[] array, int firstIndex, int secondIndex)
        {
            var firstValue = array[firstIndex];
            array[firstIndex] = array[secondIndex];
            array[secondIndex] = firstValue;
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}