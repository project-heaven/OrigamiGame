using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_about);

            var back_button = FindViewById<ImageButton>(Resource.Id.back_button);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            back_button.Click += (s, e) => { StartActivity(typeof(MainMenuActivity)); };

            FindViewById<Button>(Resource.Id.dbgg).Click += (s, e) => 
            {
                for (int i = 0; i < 144; i++)
                    Preferences.Set($"level {i} rating", 3);

                Preferences.Set($"chapter 0 passed", 24);
                Preferences.Set($"chapter 1 passed", 24);
                Preferences.Set($"chapter 2 passed", 24);
                Preferences.Set($"chapter 3 passed", 24);
                Preferences.Set($"chapter 4 passed", 24);
                Preferences.Set($"chapter 5 passed", 24);
            };
        }
    }
}