using System;
using System.Collections.Generic;
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
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly int[] _winSequence = GetEmptyMap(Preferences.Get("MapSize", 4));
        private int[] _numbers;
        private ICommand _swipeUpCommand;
        private ICommand _swipeDownCommand;
        private ICommand _swipeLeftCommand;
        private ICommand _swipeRightCommand;
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

        public ICommand SwipeUpCommand => _swipeUpCommand ?? (_swipeUpCommand = new Command(p =>
        {
            var value = (int)p;
            var index = Numbers.IndexOf(value);

            var currentRow = index / MapSize;
            var currentCol = index % MapSize;

            var emptyRow = _emptyIndex / MapSize;
            var emptyCol = _emptyIndex % MapSize;

            if(currentCol == emptyCol && currentRow > emptyRow)
            {
                SwapCommand.Execute(value);
            }
        }));

        public ICommand SwipeDownCommand => _swipeDownCommand ?? (_swipeDownCommand = new Command(p =>
        {
            var value = (int)p;
            var index = Numbers.IndexOf(value);

            var currentRow = index / MapSize;
            var currentCol = index % MapSize;

            var emptyRow = _emptyIndex / MapSize;
            var emptyCol = _emptyIndex % MapSize;

            if (currentCol == emptyCol && currentRow < emptyRow)
            {
                SwapCommand.Execute(value);
            }
        }));

        public ICommand SwipeLeftCommand => _swipeLeftCommand ?? (_swipeLeftCommand = new Command(p =>
        {
            var value = (int)p;
            var index = Numbers.IndexOf(value);

            var currentRow = index / MapSize;
            var currentCol = index % MapSize;

            var emptyRow = _emptyIndex / MapSize;
            var emptyCol = _emptyIndex % MapSize;

            if (currentRow == emptyRow && currentCol > emptyCol)
            {
                SwapCommand.Execute(value);
            }

        }));

        public ICommand SwipeRightCommand => _swipeRightCommand ?? (_swipeRightCommand = new Command(p =>
        {
            var value = (int)p;
            var index = Numbers.IndexOf(value);

            var currentRow = index / MapSize;
            var currentCol = index % MapSize;

            var emptyRow = _emptyIndex / MapSize;
            var emptyCol = _emptyIndex % MapSize;

            if (currentRow == emptyRow && currentCol < emptyCol)
            {
                SwapCommand.Execute(value);
            }
        }));

        public ICommand SwapCommand => _swapCommand ?? (_swapCommand = new Command(p =>
        {
            var value = (int)p;
            var index = Numbers.IndexOf(value);

            var currentRow = index / MapSize;
            var currentCol = index % MapSize;

            var emptyRow = _emptyIndex / MapSize;
            var emptyCol = _emptyIndex % MapSize;

            var transitions = new List<TransitionModel>();

            if (currentRow == emptyRow)
            {
                var step = Math.Sign(emptyCol - currentCol);
                var i = currentCol;
                while (i != emptyCol)
                {
                    var from = currentRow * MapSize + i;
                    i += step;
                    var to = currentRow * MapSize + i;
                    transitions.Insert(0, new TransitionModel(from, to, Numbers[from]));
                }
            }

            if (currentCol == emptyCol)
            {
                var step = Math.Sign(emptyRow - currentRow);
                var i = currentRow;
                while (i != emptyRow)
                {
                    var from = i * MapSize + currentCol;
                    i += step;
                    var to = i * MapSize + currentCol;
                    transitions.Insert(0, new TransitionModel(from, to, Numbers[from]));
                }
            }

            if(!transitions.Any())
            {
                return;
            }

            _emptyIndex = index;

            if (Preferences.Get("ShouldPlaySound", true))
                DependencyService.Resolve<IAudioService>().Play("swap.mp3", false);

            foreach (var t in transitions)
            {
                PerformTransitionCommand?.Execute(t);
                SwapElements(Numbers, t.FromIndex, t.ToIndex);
            }

            ++SwapsCount;
            if (Numbers.SequenceEqual(_winSequence))
            {
                HandleWinCommand?.Execute(SwapsCount);
            }
        }));

        public ICommand InitGameCommand => _initGameCommand ?? (_initGameCommand = new Command(() =>
        {
            Numbers = ShuffleArray(GetEmptyMap(MapSize));
            _emptyIndex = Numbers.IndexOf(0);
            SwapsCount = 0;
        }));

        private int MapSize { get; } = Preferences.Get("MapSize", 4);

        private static int[] GetEmptyMap(int mapSize)
            => Enumerable.Range(1, mapSize * mapSize - 1).Union(Enumerable.Repeat(0, 1)).ToArray();

        private static int[] ShuffleArray(int[] array)
        {
            var mapSize = (int)Math.Sqrt(array.Length);
            var random = new Random();

            var allowedActions = new int[3];
            allowedActions[0] = -1;
            allowedActions[1] = -mapSize;
            var allowedActionsCount = 2;
            var emptyIndex = array.Length - 1;

            for (var i = 0; i < 1300; ++i)
            {
                var randomAction = allowedActions[random.Next(allowedActionsCount)];
                var newIndex = emptyIndex + randomAction;
                SwapElements(array, emptyIndex, newIndex);
                var newIndexRow = newIndex / mapSize;
                var newIndexCol = newIndex % mapSize;
                allowedActionsCount = 0;
                if (newIndexCol > 0 && randomAction != -1)
                {
                    allowedActions[allowedActionsCount++] = -1;
                }
                if (newIndexCol < mapSize - 1 && randomAction != 1)
                {
                    allowedActions[allowedActionsCount++] = 1;
                }
                if (newIndexRow > 0 && randomAction != -mapSize)
                {
                    allowedActions[allowedActionsCount++] = -mapSize;
                }
                if (newIndexRow < mapSize - 1 && randomAction != mapSize)
                {
                    allowedActions[allowedActionsCount++] = mapSize;
                }
                emptyIndex = newIndex;
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