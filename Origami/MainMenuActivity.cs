using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

using Origami.Logics;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainMenuActivity : Activity
    {
        public static MainMenuActivity Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_mainmenu);

            FindViewById<Button>(Resource.Id.start_button).Click += (s, e) => { StartGame(); };
        }

        public LogicCore core;

        void StartGame()
        {
            StartActivity(typeof(LevelSelectActivity));

            core = new LogicCore();
        }
    }
}