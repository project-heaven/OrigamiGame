using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Origami
{
    [Activity(Label = "LevelSelectActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class LevelSelectActivity : Activity
    {
        public static LevelSelectActivity Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_levelselect);

            InflateLevelIcons();

            FindViewById<ImageButton>(Resource.Id.back_button).Click
            += (s, e) => { StartActivity(typeof(MainMenuActivity)); };
        }

        
        void InflateLevelIcons()
        {
            var level_pager = FindViewById<ViewPager>(Resource.Id.levels_pager);
            level_pager.Adapter = new LevelPagerAdapter(this, this);
        }
        
        public void LevelSelected(int level)
        {
            if(MainMenuActivity.Instance.core.GetLevelRating(level).unlocked)
            {
                StartActivity(typeof(GameActivity));
                MainMenuActivity.Instance.core.SetLevel(level);
            }
        }
    }
}