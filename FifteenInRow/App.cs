using System;
using FormsControls.Base;
using Xamarin.Forms;

namespace FifteenInRow
{
    public class App : Application
    {
        public App()
        {
#if DEBUG
            HotReloader.Current.Run(this);
#endif
            MainPage = new AnimationNavigationPage(new MainMenuPage())
            {
                BarTextColor = Color.White
            };
        }

        protected override void OnStart()
        {
            base.OnStart();
            DependencyService.Resolve<IAudioService>().Play("backMusic.wav", true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            DependencyService.Resolve<IAudioService>().Play("backMusic.wav", true);
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            DependencyService.Resolve<IAudioService>().Stop("backMusic.wav");
        }
    }
}
