using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Origami
{
    [Activity(Label = "ChapterSelectActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class ChapterSelectActivity : Activity
    {
        public static ChapterSelectActivity Instance;
        public static int selected_chapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_chapterselect);

            SetupChapterIcons();

            FindViewById<ImageButton>(Resource.Id.back_button).Click
            += (s, e) => { StartActivity(typeof(MainMenuActivity)); };
        }

        void SetupChapterIcons()
        {
            ChapterPagerAdapter.chapters = new ChapterPagerAdapter.Chapter[]
            {
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 16, name = "ch0", passed_count = 0 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 15, name = "ch1", passed_count = 1 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 14, name = "ch2", passed_count = 2 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 13, name = "ch3", passed_count = 3 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 12, name = "ch4", passed_count = 4 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 11, name = "ch5", passed_count = 5 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 10, name = "ch6", passed_count = 6 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 9, name = "ch7", passed_count = 7 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 8, name = "ch8", passed_count = 8 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 7, name = "ch9", passed_count = 9 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 6, name = "ch10", passed_count = 10 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.origami_sample, level_count = 5, name = "ch11", passed_count = 11 }
            };

            var chapters_pager = FindViewById<ViewPager>(Resource.Id.chapters_pager);
            chapters_pager.Adapter = new ChapterPagerAdapter(this, this);
            chapters_pager.ScrollChange += ScrollChange;
        }

        private void ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            float scroll_x_norm = e.ScrollX / (float)e.V.Width;

            if(scroll_x_norm > 0.5f)
            {
                FindViewById<LinearLayout>(Resource.Id.screen_selected0).SetBackgroundResource(Resource.Drawable.transparent);
                FindViewById<LinearLayout>(Resource.Id.screen_selected1).SetBackgroundResource(Resource.Drawable.roundedCorners);
            }
            else
            {
                FindViewById<LinearLayout>(Resource.Id.screen_selected0).SetBackgroundResource(Resource.Drawable.roundedCorners);
                FindViewById<LinearLayout>(Resource.Id.screen_selected1).SetBackgroundResource(Resource.Drawable.transparent);
            }
        }

        public void ChapterSelected(int chapter)
        {
            selected_chapter = chapter;
            StartActivity(typeof(LevelSelectActivity)); 
        }
    }
}