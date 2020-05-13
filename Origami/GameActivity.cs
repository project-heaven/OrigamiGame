using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Origami.Logics;
using Xamarin.Essentials;

namespace Origami
{
    [Activity(Label = "GameActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class GameActivity : Activity
    {
        public static GameActivity Instance;

        ImageView game_field;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_game);

            game_field = FindViewById<ImageView>(Resource.Id.game_field);
            game_field.Touch += Touch;

            int screen_height = (int)DeviceDisplay.MainDisplayInfo.Height;
            game_field.SetImageBitmap(Bitmap.CreateBitmap(screen_height, screen_height, Bitmap.Config.Argb8888));

            game_field.Post(new System.Action(() =>
                MainMenuActivity.Instance.core.CurrentLevel().RenderField(game_field)));

            FindViewById<ImageButton>(Resource.Id.back_button).Click += (s, e) => StartActivity(typeof(LevelSelectActivity));

            //FindViewById<ImageButton>(Resource.Id.help_button).Click += (s, e) => ;
            FindViewById<ImageButton>(Resource.Id.undo_button).Click += (s, e) => MainMenuActivity.Instance.core.CurrentLevel().Undo();
            FindViewById<ImageButton>(Resource.Id.reset_button).Click += (s, e) => MainMenuActivity.Instance.core.CurrentLevel().Reset();
        }

        void Touch(object sender, View.TouchEventArgs e)
        {
            float touch_x = e.Event.GetX() / game_field.Width;
            float touch_y = e.Event.GetY() / game_field.Height;

            Vector2 touch_coords = new Vector2(touch_x, touch_y);
            var current_level = MainMenuActivity.Instance.core.CurrentLevel();

            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    current_level.TouchStart(touch_coords);
                    break;
                case MotionEventActions.Move:
                    current_level.TouchMove(touch_coords);
                    break;
                case MotionEventActions.Up:
                    current_level.TouchEnd(touch_coords);
                    break;
            }
        }
    
        public void RedrawField()
        {
            MainMenuActivity.Instance.core.CurrentLevel().RenderField(game_field);
        }
    }
}