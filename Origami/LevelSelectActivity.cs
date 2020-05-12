using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Widget;

namespace Origami
{
    [Activity(Label = "LevelSelectActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class LevelSelectActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_levelselect);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_levelselect);

            var level_grid = FindViewById<GridView>(Resource.Id.level_grid);
            LevelsGridViewAdapter.button_count = 30;
            LevelsGridViewAdapter.ButtonClick += LevelSelected;
            level_grid.Adapter = new LevelsGridViewAdapter(this);

            FindViewById<Button>(Resource.Id.back_button).Click 
            += (s, e) => { Back(); };
        }

        void Back()
        {
            StartActivity(typeof(MainMenuActivity));
        }

        void LevelSelected(int level)
        {
            StartActivity(typeof(GameActivity));
        }
    }
}