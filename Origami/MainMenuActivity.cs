using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;

using Origami.Logics;
using System.IO;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainMenuActivity : Activity
    {
        public static MainMenuActivity Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_mainmenu);

            FindViewById<Button>(Resource.Id.start_button).Click += (s, e) => { StartGame(); };

            if (Instance != null)
                return;

            Instance = this;

            core = new LogicCore();
            string levels_xml = new StreamReader(Assets.Open("levels.xml")).ReadToEnd();
            core.LoadLevels(levels_xml);

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