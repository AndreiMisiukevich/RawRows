using System;
using FormsControls.Base;
using TouchEffect;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace FifteenInRow
{
    public class SettingsPage : ContentPage, IAnimationPage
    {
        public SettingsPage()
        {
            BindingContext = new SettingsViewModel();

            var backImage = new Image
            {
                Opacity = 0.98,
                Source = "back",
                Aspect = Aspect.Fill
            };
            AbsoluteLayout.SetLayoutBounds(backImage, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(backImage, AbsoluteLayoutFlags.All);

            var musicLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 40,
                Text = "MUSIC",
                TextColor = Color.White,
                FontFamily = "MandaloreRegular",
            };
            AbsoluteLayout.SetLayoutBounds(musicLabel, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(musicLabel, AbsoluteLayoutFlags.All);

            var musicCrossOne = new BoxView
            {
                VerticalOptions = LayoutOptions.Center,
                Color = Color.White,
                HeightRequest = 2,
                Rotation = 10
            };
            musicCrossOne.SetBinding(IsVisibleProperty, nameof(SettingsViewModel.IsMusicDisabled));
            AbsoluteLayout.SetLayoutBounds(musicCrossOne, new Rectangle(0, .5, 1, 2));
            AbsoluteLayout.SetLayoutFlags(musicCrossOne, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            var musicCrossTwo = new BoxView
            {
                VerticalOptions = LayoutOptions.Center,
                Color = Color.White,
                HeightRequest = 2,
                Rotation = -10
            };
            musicCrossTwo.SetBinding(IsVisibleProperty, nameof(SettingsViewModel.IsMusicDisabled));
            AbsoluteLayout.SetLayoutBounds(musicCrossTwo, new Rectangle(0, .5, 1, 2));
            AbsoluteLayout.SetLayoutFlags(musicCrossTwo, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);


            var musicLayout = new PancakeView
            {
                Margin = new Thickness(0, 30, 0, 0),
                HeightRequest = 60,
                BorderColor = Color.White,
                BorderThickness = 2,
                Content = new AbsoluteLayout
                {
                    Children =
                    {
                        musicLabel,
                        musicCrossOne,
                        musicCrossTwo
                    }
                }
            };
            musicLayout.SetBinding(TouchEff.CommandProperty, nameof(SettingsViewModel.ChangeSettingCommand));
            TouchEff.SetCommandParameter(musicLayout, "music");
            TouchEff.SetNativeAnimation(musicLayout, true);

            var soundLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = 40,
                Text = "SOUND",
                TextColor = Color.White,
                FontFamily = "MandaloreRegular",
            };
            AbsoluteLayout.SetLayoutBounds(soundLabel, new Rectangle(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(soundLabel, AbsoluteLayoutFlags.All);

            var soundCrossOne = new BoxView
            {
                VerticalOptions = LayoutOptions.Center,
                Color = Color.White,
                HeightRequest = 2,
                Rotation = 10
            };
            soundCrossOne.SetBinding(IsVisibleProperty, nameof(SettingsViewModel.IsSoundDisabled));
            AbsoluteLayout.SetLayoutBounds(soundCrossOne, new Rectangle(0, .5, 1, 2));
            AbsoluteLayout.SetLayoutFlags(soundCrossOne, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            var soundCrossTwo = new BoxView
            {
                VerticalOptions = LayoutOptions.Center,
                Color = Color.White,
                HeightRequest = 2,
                Rotation = -10
            };
            soundCrossTwo.SetBinding(IsVisibleProperty, nameof(SettingsViewModel.IsSoundDisabled));
            AbsoluteLayout.SetLayoutBounds(soundCrossTwo, new Rectangle(0, .5, 1, 2));
            AbsoluteLayout.SetLayoutFlags(soundCrossTwo, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

            var soundLayout = new PancakeView
            {
                Margin = new Thickness(0, 15, 0, 0),
                HeightRequest = 60,
                BorderColor = Color.White,
                BorderThickness = 2,
                Content = new AbsoluteLayout
                {
                    Children =
                    {
                        soundLabel,
                        soundCrossOne,
                        soundCrossTwo
                    }
                }
            };
            soundLayout.SetBinding(TouchEff.CommandProperty, nameof(SettingsViewModel.ChangeSettingCommand));
            TouchEff.SetCommandParameter(soundLayout, "sound");
            TouchEff.SetNativeAnimation(soundLayout, true);


            var decreaseMapSizeLayout = new PancakeView
            {
                HeightRequest = 60,
                WidthRequest = 60,
                BorderColor = Color.White,
                BorderThickness = 2,
                Content = new Label
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 50,
                    Text = "-",
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                }
            };
            decreaseMapSizeLayout.SetBinding(TouchEff.CommandProperty, nameof(SettingsViewModel.ChangeSettingCommand));
            TouchEff.SetCommandParameter(decreaseMapSizeLayout, "decreaseMapSize");
            TouchEff.SetNativeAnimation(decreaseMapSizeLayout, true);

            var increaseMapSizeLayout = new PancakeView
            {
                HeightRequest = 60,
                WidthRequest = 60,
                BorderColor = Color.White,
                BorderThickness = 2,
                Content = new Label
                {
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 50,
                    Text = "+",
                    TextColor = Color.White,
                    FontFamily = "MandaloreRegular",
                }
            };
            increaseMapSizeLayout.SetBinding(TouchEff.CommandProperty, nameof(SettingsViewModel.ChangeSettingCommand));
            TouchEff.SetCommandParameter(increaseMapSizeLayout, "increaseMapSize");
            TouchEff.SetNativeAnimation(increaseMapSizeLayout, true);

            var mapSizeLabel = new Label
            {

                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 30,
                TextColor = Color.White,
                FontFamily = "MandaloreRegular",
            };
            mapSizeLabel.SetBinding(Label.TextProperty, nameof(SettingsViewModel.MapSizeText));

            var buttonsView = new PancakeView
            {
                Margin = new Thickness(Device.Idiom == TargetIdiom.Phone ? 15 : 125, 0),
                Padding = new Thickness(25),
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
                            VerticalTextAlignment = TextAlignment.Center,
                            FontSize = 50,
                            Text = "SETTINGS",
                            TextColor = Color.White,
                            FontFamily = "MandaloreHalftone",
                        },
                        musicLayout,
                        soundLayout,
                        new StackLayout
                        {
                            HeightRequest = 60,
                            Margin = new Thickness(0, 15, 0, 0),
                            Orientation = StackOrientation.Horizontal,
                            Children = {

                                decreaseMapSizeLayout,

                                new StackLayout
                                {
                                    Spacing = 0,
                                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                                    Children =
                                    {
                                        new Label
                                        {
                                            
                                            HorizontalTextAlignment = TextAlignment.Center,
                                            FontSize = 30,
                                            Text = "MAP SIZE",
                                            TextColor = Color.White,
                                            FontFamily = "MandaloreRegular",
                                        },
                                        mapSizeLabel
                                    }
                                },

                                increaseMapSizeLayout
                            }
                        }
                    }
                }
            };
            AbsoluteLayout.SetLayoutBounds(buttonsView, new Rectangle(.5, .5, 1, -1));
            AbsoluteLayout.SetLayoutFlags(buttonsView, AbsoluteLayoutFlags.PositionProportional | AbsoluteLayoutFlags.WidthProportional);

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
                if (Preferences.Get("ShouldPlaySound", true))
                    DependencyService.Resolve<IAudioService>().Play("click.mp3", false);
                Navigation.PopAsync();
            }));
            TouchEff.SetNativeAnimation(mainMenuButton, true);

            Content = new AbsoluteLayout
            {
                Children =
                {
                    backImage,
                    mainMenuButton,
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
    }
}
