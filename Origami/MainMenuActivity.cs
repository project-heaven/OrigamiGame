using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;

using Origami.Logics;
using System.IO;
using Xamarin.Essentials;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainMenuActivity : Activity
    {
        public static MainMenuActivity Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_mainmenu);

            FindViewById<ImageButton>(Resource.Id.start_button).Click += (s, e) => { StartGame(); };

            // DEBUG
            FindViewById<Button>(Resource.Id.dbg_reset_button).Click += (s, e) => { Preferences.Clear(); };
            

            if (Instance != null)
                return;

            Instance = this;

            core = new LogicCore();
            string levels_xml = new StreamReader(Assets.Open("levels.xml")).ReadToEnd();
            core.Init(levels_xml);

            Level.FoldLineColor = Resources.GetColor(Resource.Color.resultOutlineColor);

            PaperSheet.Color = Resources.GetColor(Resource.Color.sheetColor);
            PaperSheet.Color.A = 100;
        }

        public LogicCore core;

        void StartGame()
        {
            StartActivity(typeof(LevelSelectActivity));
        }
    }
}