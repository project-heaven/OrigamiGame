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

            var back_button = FindViewById<ImageButton>(Resource.Id.back_button);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            back_button.Click += (s, e) => StartActivity(typeof(MainMenuActivity));
        }

        void SetupChapterIcons()
        {
            ChapterPagerAdapter.chapters = new ChapterPagerAdapter.Chapter[]
            {
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.whale, level_count = 16, name = "whale", passed_count = 0 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.fox, level_count = 15, name = "fox", passed_count = 1 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.pony, level_count = 14, name = "pony", passed_count = 2 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.pigeons, level_count = 13, name = "pigeons", passed_count = 3 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.giraffe, level_count = 12, name = "giraffe", passed_count = 4 },
                new ChapterPagerAdapter.Chapter() { image_id = Resource.Drawable.coala, level_count = 11, name = "coala", passed_count = 5 }
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