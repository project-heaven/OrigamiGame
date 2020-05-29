using Android.App;
using Android.Content.PM;
using Android.OS;
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

        public const string ad_id = "ca-app-pub-3940256099942544/5224354917"/*"ca-app-pub-3824185688302776~6278298542"*/;

        public LogicCore core;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_mainmenu);

            FindViewById<ImageButton>(Resource.Id.start_button).Click += (s, e) => StartActivity(typeof(ChapterSelectActivity));

            //FindViewById<Button>(Resource.Id.dbg).Click += (s, e) => Preferences.Clear();

            FindViewById<TextView>(Resource.Id.about_button).Click += (s, e) => StartActivity(typeof(AboutActivity));

            if (Instance != null)
                return;

            Instance = this;

            core = new LogicCore();
            string levels_xml = new StreamReader(Assets.Open("levels.xml")).ReadToEnd();
            core.Init(levels_xml);

            Level.FoldLineColor = Resources.GetColor(Resource.Color.resultOutlineColor);

            var sheet_color = Resources.GetColor(Resource.Color.sheetColor);
            sheet_color.A = 100;
            PaperSheet.Color = sheet_color;

            PaperSheet.OutlineColor = Resources.GetColor(Resource.Color.resultOutlineColor);
        }
    }
}