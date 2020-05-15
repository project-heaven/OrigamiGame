using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Origami.Logics;
using System.Runtime.CompilerServices;
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
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

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

        View lvl_complete_view;

        public void LevelComplete(int stars)
        {
            lvl_complete_view = LayoutInflater.Inflate(Resource.Layout.level_end_modal, null);

            var star_0 = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star0);
            var star_1 = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star1);
            var star_2 = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star2);

            if (stars >= 1)
                star_0.SetImageResource(Resource.Drawable.star_filled);
            else
                star_0.SetImageResource(Resource.Drawable.star);

            if (stars >= 2)
                star_1.SetImageResource(Resource.Drawable.star_filled);
            else
                star_1.SetImageResource(Resource.Drawable.star);

            if (stars >= 3)
                star_2.SetImageResource(Resource.Drawable.star_filled);
            else
                star_2.SetImageResource(Resource.Drawable.star);

            lvl_complete_view.FindViewById<ImageButton>(Resource.Id.back_button).Click += (s, e) => 
            {
                ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                StartActivity(typeof(LevelSelectActivity));
                MainMenuActivity.Instance.core.CurrentLevel().Reset();
            };

            lvl_complete_view.FindViewById<ImageButton>(Resource.Id.reset_button).Click += (s, e) =>
            {
                ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                MainMenuActivity.Instance.core.CurrentLevel().Reset();
            };

            lvl_complete_view.FindViewById<ImageButton>(Resource.Id.next_button).Click += (s, e) =>
            {
                ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                MainMenuActivity.Instance.core.NextLevel();
                MainMenuActivity.Instance.core.CurrentLevel().Reset();
            };

            var display_info = DeviceDisplay.MainDisplayInfo;
            AddContentView(lvl_complete_view, new ViewGroup.LayoutParams((int)display_info.Width, (int)display_info.Height));
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