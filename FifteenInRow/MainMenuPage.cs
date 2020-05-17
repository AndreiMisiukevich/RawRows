using System;
using FormsControls.Base;
using TouchEffect;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace FifteenInRow
{
    public class MainMenuPage : ContentPage, IAnimationPage
    {
        public MainMenuPage()
        {
            var backImage = new Image
            {
                Opacity = 0.98,
                Source = ImageSource.FromResource("back.jpg", Application.Current.GetType().Assembly),
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutBounds(backImage, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(backImage, AbsoluteLayoutFlags.All);

            var settingButton = new PancakeView
            {
                Margin = new Thickness(0, 30, 0, 0),
                HeightRequest = 60,
                BorderColor = Color.White,
                BorderThickness = 2,
                Content = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 50,
                    Text = "SETTINGS",
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                }
            };
            TouchEff.SetPressedOpacity(settingButton, 0.7);
            TouchEff.SetPressedScale(settingButton, 0.95);
            TouchEff.SetCommand(settingButton, new Command(OnSettingsClicked));
            //TouchEff.SetNativeAnimation(settingButton, true);

            var startGameButton = new PancakeView
            {
                Margin = new Thickness(0, 15, 0, 0),
                HeightRequest = 60,
                BorderColor = Color.White,
                BorderThickness = 2,
                Content = new Label
                {
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 50,
                    Text = "START GAME",
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                }
            };
            TouchEff.SetPressedOpacity(startGameButton, 0.7);
            TouchEff.SetPressedScale(startGameButton, 0.95);
            TouchEff.SetCommand(startGameButton, new Command(OnStartGameClicked));
            //TouchEff.SetNativeAnimation(startGameButton, true);

            var buttonsView = new PancakeView
            {
                Margin = new Thickness(25, 0),
                Padding = new Thickness(25, 10, 25, 20),
                CornerRadius = new CornerRadius(50, 10, 10, 50),
                BackgroundColor = Color.Black.MultiplyAlpha(.65),
                Content = new StackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        new Label
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            FontSize = 50,
                            Text = "FIFTEEN IN ROW",
                            TextColor = Color.White,
                            FontFamily = "MandaloreHalftone",
                        },
                        settingButton,
                        startGameButton
                    }
                }
            };
            AbsoluteLayout.SetLayoutBounds(buttonsView, new Rectangle(.5, .5, 1, -1));
            AbsoluteLayout.SetLayoutFlags(buttonsView, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            Content = new AbsoluteLayout
            {
                Children =
                {
                    backImage,
                    buttonsView
                }
            };

            NavigationPage.SetHasNavigationBar(this, false);
        }

        public IPageAnimation PageAnimation { get; } = Device.RuntimePlatform == Device.iOS
            ? new FlipPageAnimation { Duration = AnimationDuration.Long, Subtype = AnimationSubtype.FromTop }
            : (IPageAnimation)new LandingPageAnimation { Duration = AnimationDuration.Medium, Subtype = AnimationSubtype.FromTop };

        public void OnAnimationFinished(bool isPopAnimation) { }

        public void OnAnimationStarted(bool isPopAnimation) { }

        private void OnSettingsClicked()
        {
            if (Preferences.Get("ShouldPlaySound", true))
                DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
            Navigation.PushAsync(new SettingsPage());
        }

        private void OnStartGameClicked()
        {
            if (Preferences.Get("ShouldPlaySound", true))
                DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
            Navigation.PushAsync(new GamePage
            {
                BindingContext = new GameViewModel()
            });
        }
    }
}
