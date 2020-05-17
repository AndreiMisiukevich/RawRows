using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FormsControls.Base;
using TouchEffect;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace FifteenInRow
{
    public partial class GamePage : ContentPage, IAnimationPage
    {
        private const int Spacing = 10;
        private const int SideMargin = 15;

        public static readonly BindableProperty PerformTransitionCommandProperty = BindableProperty.Create(nameof(PerformTransitionCommand), typeof(ICommand), typeof(GamePage), null, BindingMode.OneWayToSource);

        private int _mapSize;
        private double _itemSize;
        private PancakeView[] _items;

        public GamePage()
        {
            var gameMap = new AbsoluteLayout
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                Margin = new Thickness(15)
            };
            gameMap.SetBinding(BindingContextProperty, nameof(GameViewModel.Numbers));
            gameMap.BindingContextChanged += OnGameMapBindingContextChanged;
            AbsoluteLayout.SetLayoutBounds(gameMap, new Rectangle(.5, .5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(gameMap, AbsoluteLayoutFlags.PositionProportional);

            var mainMenuButton = new PancakeView
            {
                BackgroundColor = Color.Black.MultiplyAlpha(.65),
                CornerRadius = new CornerRadius(0, 10, 10, 0),
                Padding = new Thickness(10, 5),
                Margin = new Thickness(15, Device.RuntimePlatform == Device.iOS ? 40 : 20),
                BorderColor = Color.White,
                BorderThickness = 2,
                HeightRequest = 40,
                Content = new Label
                {
                    FontSize = 30,
                    Text = "< MENU",
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            };
            AbsoluteLayout.SetLayoutBounds(mainMenuButton, new Rectangle(0, 0, -1, -1));
            AbsoluteLayout.SetLayoutFlags(mainMenuButton, AbsoluteLayoutFlags.PositionProportional);
            TouchEff.SetCommand(mainMenuButton, new Command(() =>
            {
                DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
                Navigation.PopAsync();
            }));
            TouchEff.SetNativeAnimation(mainMenuButton, true);

            var newGameButton = new PancakeView
            {
                BackgroundColor = Color.Black.MultiplyAlpha(.65),
                CornerRadius = new CornerRadius(0, 10, 10, 0),
                Padding = new Thickness(10, 5),
                Margin = new Thickness(15, Device.RuntimePlatform == Device.iOS ? 40 : 20),
                BorderColor = Color.White,
                BorderThickness = 2,
                HeightRequest = 40,
                Content = new Label
                {
                    FontSize = 30,
                    Text = "NEW GAME",
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                }
            };
            AbsoluteLayout.SetLayoutBounds(newGameButton, new Rectangle(1, 0, -1, -1));
            AbsoluteLayout.SetLayoutFlags(newGameButton, AbsoluteLayoutFlags.PositionProportional);
            Task.Run(async () =>
            {
                await Task.Delay(300);
                TouchEff.SetCommand(newGameButton, new Command(async () =>
                {
                    DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
                    await Navigation.PushAsync(new GamePage() { BindingContext = new GameViewModel() });
                    Navigation.RemovePage(this);
                }));
            });
            TouchEff.SetNativeAnimation(newGameButton, true);


            var swapsCountLabel = new Label();
            swapsCountLabel.SetBinding(Label.TextProperty, nameof(GameViewModel.SwapsCount));

            var backImage = new Image
            {
                Opacity = 0.98,
                Source = ImageSource.FromResource("back.jpg", Application.Current.GetType().Assembly),
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutBounds(backImage, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(backImage, AbsoluteLayoutFlags.All);

            var blurFrame = new PancakeView
            {
                CornerRadius = new CornerRadius(50, 10, 10, 50),
                BorderColor = Color.Red,
                BackgroundColor = Color.Black.MultiplyAlpha(.65)
            };
            AbsoluteLayout.SetLayoutBounds(blurFrame, new Rectangle(.5, .5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(blurFrame, AbsoluteLayoutFlags.PositionProportional);

            var countLabel = new Label
            {
                FontSize = 50,
                TextColor = Color.White,
                FontFamily = "MandaloreHalftone"
            };
            countLabel.SetBinding(Label.TextProperty, nameof(GameViewModel.SwapsCount));

            var labelStack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    new Label
                    {
                        FontSize = 50,
                        TextColor = Color.White,
                        Text = "SWAPS:",
                        FontFamily = "MandaloreHalftone"
                    },
                    countLabel
                }
            };
            AbsoluteLayout.SetLayoutBounds(labelStack, new Rectangle(.5, .5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(labelStack, AbsoluteLayoutFlags.PositionProportional);

            gameMap.SizeChanged += (s, e) =>
            {
                using (blurFrame.Batch())
                {
                    blurFrame.WidthRequest = gameMap.Width;
                    blurFrame.HeightRequest = gameMap.Height;
                }
                labelStack.TranslationY = -gameMap.Height / 2 - labelStack.Height / 2 - 20;
            };

            Content = new AbsoluteLayout
            {
                Children =
                {
                    backImage,
                    mainMenuButton,
                    newGameButton,
                    labelStack,
                    blurFrame,
                    gameMap
                }
            };

            PerformTransitionCommand = new Command(p =>
            {
                var transition = (TransitionModel)p;
                var item = _items[transition.Value - 1];
                item.InputTransparent = true;
                var fromPosition = GetPosition(transition.FromIndex);
                var toPosition = GetPosition(transition.ToIndex);
                var isVertical = Math.Abs(fromPosition.X - toPosition.X) < Math.Abs(fromPosition.Y - toPosition.Y);

                var start = fromPosition.X;
                var end = toPosition.X;
                if (isVertical)
                {
                    start = fromPosition.Y;
                    end = toPosition.Y;
                }

                item.Animate("PerformTransition", new Animation(v =>
                {
                    var x = v;
                    var y = fromPosition.Y;
                    if(isVertical)
                    {
                        x = fromPosition.X;
                        y = v;
                    }
                    AbsoluteLayout.SetLayoutBounds(item, new Rectangle(x, y, fromPosition.Width, fromPosition.Height));
                }, start, end), 16, 250, Easing.CubicInOut, (d, b) =>
                {
                    item.InputTransparent = false;
                    if(!b)
                    {
                        item.BorderThickness = transition.ToIndex == transition.Value - 1
                            ? 2
                            : 0;
                    }
                });
            });

            this.SetBinding(PerformTransitionCommandProperty, nameof(GameViewModel.PerformTransitionCommand));

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public ICommand PerformTransitionCommand
        {
            get => (ICommand)GetValue(PerformTransitionCommandProperty);
            set => SetValue(PerformTransitionCommandProperty, value);
        }

        public IPageAnimation PageAnimation { get; } = new FlipPageAnimation { Duration = AnimationDuration.Long, Subtype = AnimationSubtype.FromTop };

        public void OnAnimationFinished(bool isPopAnimation) { }

        public void OnAnimationStarted(bool isPopAnimation) { }

        private void OnGameMapBindingContextChanged(object sender, EventArgs e)
        {
            var gameMap = (AbsoluteLayout)sender;
            var deviceInfo = Device.Info;
            var infoSize = deviceInfo.PixelScreenSize;
            var deviceWidth = Math.Min(infoSize.Width, infoSize.Height) / deviceInfo.ScalingFactor;
            var mapWidth = deviceWidth - gameMap.Margin.HorizontalThickness;
            using (gameMap.Batch())
            {
                gameMap.Children.Clear();
                gameMap.WidthRequest = mapWidth;
                gameMap.HeightRequest = mapWidth;

                var numbers = (int[])gameMap.BindingContext;
                _items = new PancakeView[numbers.Length - 1];
                _mapSize = (int)Math.Sqrt(numbers.Length);
                _itemSize = (mapWidth - 2 * SideMargin - (_mapSize - 1) * Spacing) / _mapSize;

                for (int i = 0; i < numbers.Length; ++i)
                {
                    var number = numbers[i];
                    if(number == 0)
                    {
                        continue;
                    }

                    var item = CreateItemView(number);
                    if(i == number - 1)
                    {
                        item.BorderThickness = 2;
                    }
                    _items[number - 1] = item;
                    gameMap.Children.Add(item, GetPosition(i), AbsoluteLayoutFlags.None);
                }
            }
        }

        private Rectangle GetPosition(int index)
        {
            var row = index % _mapSize;
            var col = index / _mapSize;

            var shift = Spacing + _itemSize;
            var x = row * shift + SideMargin;
            var y = col * shift + SideMargin;

            return new Rectangle(x, y, _itemSize, _itemSize);
        }

        private PancakeView CreateItemView(int value)
        {
            var view = new PancakeView
            {
                BorderColor = Color.White,
                CornerRadius = new CornerRadius(_itemSize / 2, 0, 0, _itemSize / 2),
                BackgroundGradientAngle = 315,
                BackgroundGradientStops = new GradientStopCollection
                {
                    new GradientStop
                    {
                        Color = Color.FromRgb(204, 58, 53),
                        Offset = 0.2f
                    },
                    new GradientStop
                    {
                        Color = Color.FromRgb(170, 54, 52),
                        Offset = 0.45f
                    },
                    new GradientStop
                    {
                        Color = Color.Transparent,
                        Offset = 0.5f
                    },
                    new GradientStop
                    {
                        Color = Color.FromRgb(209, 84, 60),
                        Offset = 0.55f
                    },
                    new GradientStop
                    {
                        Color = Color.FromRgb(221, 142, 69),
                        Offset = 0.9f
                    },
                },
                Content = new Label
                {
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                    FontSize = _itemSize * 0.63,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Text = value.ToString()
                }
            };

            view.SetBinding(TouchEff.CommandProperty, new Binding
            {
                Path = $"{nameof(BindingContext)}.{nameof(GameViewModel.SwapCommand)}",
                Source = this
            });
            TouchEff.SetCommandParameter(view, value);

            TouchEff.SetRippleCount(view, -1);

            TouchEff.SetRegularOpacity(view, 1);
            TouchEff.SetRegularScale(view, 1);
            TouchEff.SetRegularAnimationDuration(view, 400);
            TouchEff.SetRegularAnimationEasing(view, Easing.CubicInOut);

            TouchEff.SetPressedOpacity(view, .85);
            TouchEff.SetPressedScale(view, 1.15);
            TouchEff.SetPressedAnimationDuration(view, 400);
            TouchEff.SetPressedAnimationEasing(view, Easing.CubicInOut);

            return view;
        }
    }
}
