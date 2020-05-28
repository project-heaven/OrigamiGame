using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class AboutActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_about);

            FindViewById<ImageButton>(Resource.Id.back_button).Click
            += (s, e) => { StartActivity(typeof(MainMenuActivity)); };
        }
    }
}