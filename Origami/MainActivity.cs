using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using Origami.Logics;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity Instance;
        ImageView game_field;
        LogicCore core;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            game_field = FindViewById<ImageView>(Resource.Id.game_field);
            game_field.Touch += Touch;

            core = new LogicCore();
            core.Init();
        }

        void Touch(object sender, View.TouchEventArgs e)
        {
            float touch_x = e.Event.GetX() / game_field.Width;
            float touch_y = e.Event.GetY() / game_field.Height;

            Vector2 touch_coords = new Vector2(touch_x, touch_y);

            switch(e.Event.Action)
            {
                case MotionEventActions.Down:
                    core.level.TouchStart(touch_coords);
                    break;
                case MotionEventActions.Move:
                    core.level.TouchMove(touch_coords);
                    break;
                case MotionEventActions.Up:
                    core.level.TouchEnd(touch_coords);
                    break;
            }
        }

        public void FieldUpdated()
        {
            core.level.RenderField(game_field);
        }
	}
}

