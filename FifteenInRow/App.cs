using System;
using FormsControls.Base;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FifteenInRow
{
    public class App : Application
    {
        public App()
        {
            MainPage = new AnimationNavigationPage(new MainMenuPage())
            {
                BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
            base.OnStart();
            if (Preferences.Get("ShouldPlayMusic", true))
                DependencyService.Resolve<IAudioService>().Play("backMusic.wav", true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (Preferences.Get("ShouldPlayMusic", true))
                DependencyService.Resolve<IAudioService>().Play("backMusic.wav", true);
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            DependencyService.Resolve<IAudioService>().Stop("backMusic.wav");
        }
    }
}
