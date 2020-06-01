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

            SetupLevelIcons();

            var back_button = FindViewById<ImageButton>(Resource.Id.back_button);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            back_button.Click += (s, e) => { StartActivity(typeof(ChapterSelectActivity)); };
        }

        void SetupLevelIcons()
        {
            var level_pager = FindViewById<ViewPager>(Resource.Id.levels_pager);
            level_pager.Adapter = new LevelPagerAdapter(this, this);
            level_pager.ScrollChange += ScrollChange;
        }

        private void ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            float scroll_x_norm = e.ScrollX / ((float)e.V.Width * 2);
            int visible_screen = (int)(scroll_x_norm * 3.0f);
            int[] level_selected_view_ids = new int[] { Resource.Id.screen_selected0, Resource.Id.screen_selected1, Resource.Id.screen_selected2 };
            if (visible_screen >= level_selected_view_ids.Length)
                visible_screen = level_selected_view_ids.Length - 1;

            for(int i = 0; i < level_selected_view_ids.Length; i++)
            {
                var view = FindViewById<LinearLayout>(level_selected_view_ids[i]);
                view.SetBackgroundResource(i == visible_screen ? Resource.Drawable.roundedCorners : Resource.Drawable.transparent);
            }
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