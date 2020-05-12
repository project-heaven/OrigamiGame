using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Origami.Logics;

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