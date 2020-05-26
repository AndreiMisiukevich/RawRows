using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FormsControls.Base;
using TouchEffect;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace FifteenInRow
{
    public partial class GamePage : ContentPage, IAnimationPage
    {
        private const int Spacing = 10;
        private const int SideMargin = 15;

        public static readonly BindableProperty PerformTransitionCommandProperty = BindableProperty.Create(nameof(PerformTransitionCommand), typeof(ICommand), typeof(GamePage), null, BindingMode.OneWayToSource);
        public static readonly BindableProperty HandleWinCommandProperty = BindableProperty.Create(nameof(HandleWinCommand), typeof(ICommand), typeof(GamePage), null, BindingMode.OneWayToSource);

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

            var isClosing = false;

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
            Task.Run(async () =>
            {
                await Task.Delay(300);
                TouchEff.SetCommand(mainMenuButton, new Command(() =>
                {
                    if (isClosing || Navigation.NavigationStack.OfType<GamePage>().Count() > 1) return;
                    isClosing = true;
                    if (Preferences.Get("ShouldPlaySound", true))
                        DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
                    Navigation.PopAsync();
                }));
            });


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
                    if (isClosing) return;
                    isClosing = true;
                    if (Preferences.Get("ShouldPlaySound", true))
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
                IsVisible = Device.RuntimePlatform != Device.Android,
                Opacity = 0.98,
                Source = "back",
                Aspect = Aspect.AspectFill
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

            HandleWinCommand = new Command(v =>
            {
                var okButton = new PancakeView
                {
                    Margin = new Thickness(0, 30, 0, 0),
                    HeightRequest = 60,
                    BorderColor = Color.White,
                    BorderThickness = 2,
                    Content = new Label
                    {
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 40,
                        Text = "HOORAY!",
                        TextColor = Color.White,
                        FontFamily = "MandaloreRegular",
                    }
                };
                TouchEff.SetPressedOpacity(okButton, 0.7);
                TouchEff.SetPressedScale(okButton, 0.95);
                TouchEff.SetCommand(okButton, new Command(async () =>
                {
                    if (Preferences.Get("ShouldPlaySound", true))
                        DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
                    await Navigation.PushAsync(new GamePage() { BindingContext = new GameViewModel() });
                    Navigation.RemovePage(this);
                }));

                var popupView = new PancakeView
                {
                    Scale = 0,
                    Margin = new Thickness(25, 0),
                    Padding = new Thickness(25, 10, 25, 20),
                    CornerRadius = new CornerRadius(50, 10, 10, 50),
                    BackgroundGradientStops = new GradientStopCollection
                    {
                        new GradientStop
                        {
                            Color = Color.FromRgb(41, 36, 88),
                            Offset = .3f
                        },
                        new GradientStop
                        {
                            Color = Color.FromRgb(16, 15, 29),
                            Offset = .7f
                        },
                        new GradientStop
                        {
                            Color = Color.Black,
                            Offset = 1f
                        }
                    },
                    BorderColor = Color.White,
                    BorderThickness = 2,
                    Content = new StackLayout
                    {
                        Spacing = 0,
                        Children =
                        {
                            new Label
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                FontSize = 50,
                                Text = "CONGRATILATIONS",
                                TextColor = Color.White,
                                FontFamily = "MandaloreHalftone",
                            },
                            new Label
                            {
                                HorizontalTextAlignment = TextAlignment.Center,
                                FontSize = 40,
                                Text = $"YOUR SCORE IS: {v}",
                                TextColor = Color.White,
                                FontFamily = "MandaloreHalftone",
                            },
                            okButton
                        }
                    }
                };
                AbsoluteLayout.SetLayoutBounds(popupView, new Rectangle(.5, .5, 1, -1));
                AbsoluteLayout.SetLayoutFlags(popupView, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

                var popup = new AbsoluteLayout
                {
                    Opacity = 0,
                    BackgroundColor = Color.Black.MultiplyAlpha(.85),
                    Children =
                    {
                        popupView
                    }
                };
                AbsoluteLayout.SetLayoutBounds(popup, new Rectangle(0, 0, 1, 1));
                AbsoluteLayout.SetLayoutFlags(popup, AbsoluteLayoutFlags.All);
                (Content as AbsoluteLayout).Children.Add(popup);
                popup.FadeTo(1, 350, Easing.CubicInOut);
                popupView.ScaleTo(1, 500, Easing.CubicInOut);
                if (Preferences.Get("ShouldPlaySound", true))
                    Vibration.Vibrate(250);
            });

            this.SetBinding(PerformTransitionCommandProperty, nameof(GameViewModel.PerformTransitionCommand));
            this.SetBinding(HandleWinCommandProperty, nameof(GameViewModel.HandleWinCommand));

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public ICommand PerformTransitionCommand
        {
            get => (ICommand)GetValue(PerformTransitionCommandProperty);
            set => SetValue(PerformTransitionCommandProperty, value);
        }

        public ICommand HandleWinCommand
        {
            get => (ICommand)GetValue(HandleWinCommandProperty);
            set => SetValue(HandleWinCommandProperty, value);
        }

        public IPageAnimation PageAnimation { get; } = Device.RuntimePlatform == Device.iOS
            ? new FlipPageAnimation { Duration = AnimationDuration.Long, Subtype = AnimationSubtype.FromTop }
            : (IPageAnimation)new LandingPageAnimation { Duration = AnimationDuration.Medium, Subtype = AnimationSubtype.FromTop };

        public void OnAnimationFinished(bool isPopAnimation) { }

        public void OnAnimationStarted(bool isPopAnimation) { }

        private void OnGameMapBindingContextChanged(object sender, EventArgs e)
        {
            var gameMap = (AbsoluteLayout)sender;
            var deviceInfo = Device.Info;
            var infoSize = deviceInfo.PixelScreenSize;
            var width = Width > 0
                ? Width
                : infoSize.Width / deviceInfo.ScalingFactor;
            var height = Height > 0
                ? Height
                : infoSize.Height / deviceInfo.ScalingFactor;

            var minSize = Math.Min(width, height);
            var halfMaxSize = Math.Max(width, height) / 2;
            var deviceWidth = minSize > halfMaxSize ? halfMaxSize : minSize;
            var mapWidth = deviceWidth - gameMap.Margin.HorizontalThickness;
            using (gameMap.Batch())
            {
                gameMap.Children.Clear();
                gameMap.MinimumWidthRequest = mapWidth;
                gameMap.MinimumHeightRequest = mapWidth;
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

            var upSwipeGesture = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Up,
                CommandParameter = value
            };
            upSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, new Binding
            {
                Path = $"{nameof(BindingContext)}.{nameof(GameViewModel.SwipeUpCommand)}",
                Source = this
            });
            var downSwipeGesture = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Down,
                CommandParameter = value
            };
            downSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, new Binding
            {
                Path = $"{nameof(BindingContext)}.{nameof(GameViewModel.SwipeDownCommand)}",
                Source = this
            });
            var leftSwipeGesture = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Left,
                CommandParameter = value
            };
            leftSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, new Binding
            {
                Path = $"{nameof(BindingContext)}.{nameof(GameViewModel.SwipeLeftCommand)}",
                Source = this
            });
            var rightSwipeGesture = new SwipeGestureRecognizer
            {
                Direction = SwipeDirection.Right,
                CommandParameter = value
            };
            rightSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, new Binding
            {
                Path = $"{nameof(BindingContext)}.{nameof(GameViewModel.SwipeRightCommand)}",
                Source = this
            });

            view.GestureRecognizers.Add(upSwipeGesture);
            view.GestureRecognizers.Add(downSwipeGesture);
            view.GestureRecognizers.Add(leftSwipeGesture);
            view.GestureRecognizers.Add(rightSwipeGesture);

            return view;
        }
    }
}
