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

        public const int DEFAULT_HINTS = 20;

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
                MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh()));

            FindViewById<ImageButton>(Resource.Id.back_button).Click += (s, e) => StartActivity(typeof(LevelSelectActivity));

            //FindViewById<ImageButton>(Resource.Id.help_button).Click += (s, e) => ShowHelpModal();
            FindViewById<ImageButton>(Resource.Id.undo).Click += (s, e) => MainMenuActivity.Instance.core.CurrentLevel().Undo();
            FindViewById<ImageButton>(Resource.Id.restart).Click += (s, e) => MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh();
        }

        public void ShowHelpModal()
        {
            SetButtonsEnabled(false);

            View help_modal_view = LayoutInflater.Inflate(Resource.Layout.help_modal, null);

            int hints = Preferences.Get("hints", DEFAULT_HINTS);
            //help_modal_view.FindViewById<TextView>(Resource.Id.hints_remained).Text = $"{hints} ОСТАЛОСЬ";
            //if (hints == 0)
                //help_modal_view.FindViewById<ImageButton>(Resource.Id.help_button).Enabled = false;

            //help_modal_view.FindViewById<ImageButton>(Resource.Id.back_button).Click += (s, e) =>
            //{
            //    SetButtonsEnabled(true);
            //    ((ViewGroup)help_modal_view.Parent).RemoveView(help_modal_view);
            //};

            //help_modal_view.FindViewById<ImageButton>(Resource.Id.help_button).Click += (s, e) =>
            //{
            //    Preferences.Set("hints", Preferences.Get("hints", DEFAULT_HINTS) - 1);
            //
            //    MainMenuActivity.Instance.core.CurrentLevel().Help();
            //    SetButtonsEnabled(true);
            //    ((ViewGroup)help_modal_view.Parent).RemoveView(help_modal_view);
            //};

            var display_info = DeviceDisplay.MainDisplayInfo;
            AddContentView(help_modal_view, new ViewGroup.LayoutParams((int)display_info.Width, (int)display_info.Height));
        } 

        public void LevelFailed()
        {
            SetButtonsEnabled(false);

            View lvl_fail_view = LayoutInflater.Inflate(Resource.Layout.level_lose_modal, null);

            lvl_fail_view.FindViewById<ImageButton>(Resource.Id.back).Click += (s, e) =>
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_fail_view.Parent).RemoveView(lvl_fail_view);
                StartActivity(typeof(LevelSelectActivity));
            };

            lvl_fail_view.FindViewById<ImageButton>(Resource.Id.restart).Click += (s, e) =>
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_fail_view.Parent).RemoveView(lvl_fail_view);
                MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh();
            };

            var display_info = DeviceDisplay.MainDisplayInfo;
            AddContentView(lvl_fail_view, new ViewGroup.LayoutParams((int)display_info.Width, (int)display_info.Height));
        }

        public void LevelCompleted(int stars, bool last_level)
        {
            SetButtonsEnabled(false);

            View lvl_complete_view;

            if(last_level)
            {
                lvl_complete_view = LayoutInflater.Inflate(Resource.Layout.level_end_modal, null);
            }
            else
            {
                lvl_complete_view = LayoutInflater.Inflate(Resource.Layout.level_end_modal, null);

                lvl_complete_view.FindViewById<ImageButton>(Resource.Id.next).Click += (s, e) =>
                {
                    MainMenuActivity.Instance.core.NextLevel();
                    SetButtonsEnabled(true);
                    ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                };
            }

            var star_0 = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star0);
            var star_1 = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star1);
            var star_2 = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star2);

            var star_0_outline = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star0_outline);
            var star_1_outline = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star1_outline);
            var star_2_outline = lvl_complete_view.FindViewById<ImageView>(Resource.Id.star2_outline);

            if (stars >= 1)
                star_0.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star));
            else
                star_0.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

            if (stars >= 2)
                star_1.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star));
            else
                star_1.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

            if (stars >= 3)
                star_2.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star));
            else
                star_2.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

            lvl_complete_view.FindViewById<ImageButton>(Resource.Id.back).Click += (s, e) => 
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                StartActivity(typeof(LevelSelectActivity));
            };

            lvl_complete_view.FindViewById<ImageButton>(Resource.Id.restart).Click += (s, e) =>
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh();
            };

            var display_info = DeviceDisplay.MainDisplayInfo;
            AddContentView(lvl_complete_view, new ViewGroup.LayoutParams((int)display_info.Width, (int)display_info.Height));
        }

        void SetButtonsEnabled(bool state)
        {
            Level.FoldsDenied = !state;

            //FindViewById<ImageButton>(Resource.Id.help_button).Enabled = state;
            //FindViewById<ImageButton>(Resource.Id.undo_button).Enabled = state;
            //FindViewById<ImageButton>(Resource.Id.reset_button).Enabled = state;

            //FindViewById<ImageButton>(Resource.Id.back_button).Enabled = state;
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

        public int last_score = 1000;
        public void SetScore(int score)
        {
            last_score = score;
            //FindViewById<TextView>(Resource.Id.score).Text = $"{score}%"; 
        }

        public void SetFolds(int folds)
        {
            //FindViewById<TextView>(Resource.Id.folds).Text = folds.ToString();
        }

        public void SetFoldLimit(int limit)
        {
            //FindViewById<TextView>(Resource.Id.fold_limit).Text = limit.ToString();
        }

        public void RedrawField()
        {
            MainMenuActivity.Instance.core.CurrentLevel().RenderField(game_field);
        }
    }
}