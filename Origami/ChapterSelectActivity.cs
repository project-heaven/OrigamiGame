using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Origami
{
    [Activity(Label = "ChapterSelectActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class ChapterSelectActivity : Activity
    {
        public static ChapterSelectActivity Instance;

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

        }
        
        void ChapterSelected(int chapter)
        {/*
            if(MainMenuActivity.Instance.core.GetLevelRating(level).unlocked)
            {
                StartActivity(typeof(GameActivity));
                MainMenuActivity.Instance.core.SetLevel(level);
            }*/
        }
    }
}