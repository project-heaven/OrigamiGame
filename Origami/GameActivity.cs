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

        public const int DEFAULT_HINTS = 5;

        static bool another_activity_entered = false;

        protected override void OnPause()
        {
            base.OnPause();

            if (!another_activity_entered)
                MainMenuActivity.audioPlayer.PauseAmbient();
            else
                another_activity_entered = true;
        }

        protected override void OnResume()
        {
            base.OnResume();

            MainMenuActivity.audioPlayer.ResumeAmbient();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_game);

            game_field = FindViewById<ImageView>(Resource.Id.game_field);
            game_field.Touch += Touch;

            int screen_height = (int)DeviceDisplay.MainDisplayInfo.Height;
            game_field.SetImageBitmap(Bitmap.CreateBitmap(screen_height, screen_height, Bitmap.Config.Argb8888));

            game_field.Post(new System.Action(() =>
                MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh()));

            var back_button = FindViewById<ImageButton>(Resource.Id.back_button);
            var help_button = FindViewById<ImageButton>(Resource.Id.help);
            var undo_button = FindViewById<ImageButton>(Resource.Id.undo);
            var restart_button = FindViewById<ImageButton>(Resource.Id.restart);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            help_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            undo_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            restart_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();

            back_button.Click += (s, e) =>
            {
                another_activity_entered = true;
                StartActivity(typeof(LevelSelectActivity));
            };
            help_button.Click += (s, e) => Help();
            undo_button.Click += (s, e) => MainMenuActivity.Instance.core.CurrentLevel().Undo();
            restart_button.Click += (s, e) => MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh();

            int hints = Preferences.Get("hints", DEFAULT_HINTS);
            FindViewById<TextView>(Resource.Id.help_count).Text = hints.ToString();
        }

        public void Help()
        {
            int hints = Preferences.Get("hints", DEFAULT_HINTS);
            if(hints == 0)
                OpenNoHints();
            else
            {
                Preferences.Set("hints", --hints);
                FindViewById<TextView>(Resource.Id.help_count).Text = hints.ToString();
                MainMenuActivity.Instance.core.CurrentLevel().Help();
            }
        }

        void OpenNoHints()
        {
            View no_hints_view = LayoutInflater.Inflate(Resource.Layout.modal_no_hints, null);

            var show_ads_button = no_hints_view.FindViewById<ImageButton>(Resource.Id.show_ads);

            show_ads_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            show_ads_button.Click += (s, e) => Ads.ShowAds(this);

            var display_info = DeviceDisplay.MainDisplayInfo;
            AddContentView(no_hints_view, new ViewGroup.LayoutParams((int)display_info.Width, (int)display_info.Height));

            SetButtonsEnabled(false);

            no_hints_view.Click += (s, e) => CloseNoHints();
        }

        void CloseNoHints()
        {
            var no_hints_view = FindViewById<LinearLayout>(Resource.Id.no_hints);
            if (no_hints_view != null)
                (no_hints_view.Parent as ViewGroup).RemoveView(no_hints_view);

            SetButtonsEnabled(true);
        }

        public enum RewardAdState
        {
            FAIILED,
            LOADING,
            REWARDED
        };

        public RewardAdState AdState { 
            set 
            { 
                switch(value)
                {
                    case RewardAdState.FAIILED: 
                        { 
                            Toast.MakeText(this, "Failed to show ad. Try again later.", ToastLength.Short).Show();

                            CloseNoHints();

                            break; 
                        }
                    case RewardAdState.LOADING: 
                        { 
                            Toast.MakeText(this, "Loading your ad...", ToastLength.Short).Show(); 

                            break; 
                        }
                    case RewardAdState.REWARDED: 
                        {
                            CloseNoHints();

                            int hints = Preferences.Get("hints", DEFAULT_HINTS);
                            FindViewById<TextView>(Resource.Id.help_count).Text = (++hints).ToString();
                            Preferences.Set("hints", hints);

                            break; 
                        }
                }
            } 
        }

        public void LevelFailed(int level)
        {
            MainMenuActivity.audioPlayer.PlayLose();

            SetButtonsEnabled(false);

            View lvl_fail_view = LayoutInflater.Inflate(Resource.Layout.modal_level_lose, null);

            lvl_fail_view.FindViewById<TextView>(Resource.Id.level_name).Text = $"Level {level + 1}";

            var back_button = lvl_fail_view.FindViewById<ImageButton>(Resource.Id.back);
            var restart_button = lvl_fail_view.FindViewById<ImageButton>(Resource.Id.restart);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            restart_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();

            back_button.Click += (s, e) =>
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_fail_view.Parent).RemoveView(lvl_fail_view);
                another_activity_entered = true;
                StartActivity(typeof(LevelSelectActivity));
            };

            restart_button.Click += (s, e) =>
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_fail_view.Parent).RemoveView(lvl_fail_view);
                MainMenuActivity.Instance.core.CurrentLevel().ResetAndRefresh();
            };

            var display_info = DeviceDisplay.MainDisplayInfo;
            AddContentView(lvl_fail_view, new ViewGroup.LayoutParams((int)display_info.Width, (int)display_info.Height));
        }

        static int interestial_counter = 0;
        const int interestial_rate = 4;

        public void LevelCompleted(int level, int stars, bool last_level)
        {
            interestial_counter++;
            if (interestial_counter >= interestial_rate)
            {
                interestial_counter = 0;
                Ads.ShowInterestitial();
            }
                
            MainMenuActivity.audioPlayer.PlayWin();

            SetButtonsEnabled(false);

            View lvl_complete_view = LayoutInflater.Inflate(Resource.Layout.modal_level_end, null); ;

            lvl_complete_view.FindViewById<TextView>(Resource.Id.level_number).Text = $"Level {level + 1}";

            if(!last_level)
            {
                var next_button = lvl_complete_view.FindViewById<ImageButton>(Resource.Id.next);

                next_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();

                next_button.Click += (s, e) =>
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

            var back_button = lvl_complete_view.FindViewById<ImageButton>(Resource.Id.back);
            var restart_button = lvl_complete_view.FindViewById<ImageButton>(Resource.Id.restart);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            restart_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();

            back_button.Click += (s, e) =>
            {
                SetButtonsEnabled(true);
                ((ViewGroup)lvl_complete_view.Parent).RemoveView(lvl_complete_view);
                another_activity_entered = true;
                StartActivity(typeof(LevelSelectActivity));
            };

            restart_button.Click += (s, e) =>
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

            FindViewById<ImageButton>(Resource.Id.help).Enabled = state;
            FindViewById<ImageButton>(Resource.Id.undo).Enabled = state;
            FindViewById<ImageButton>(Resource.Id.restart).Enabled = state;

            FindViewById<ImageButton>(Resource.Id.back_button).Enabled = state;
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
            FindViewById<TextView>(Resource.Id.score).Text = $"{score}%"; 
        }

        public void SetLevelNumber(int number)
        {
            FindViewById<TextView>(Resource.Id.level_number).Text = "level " + number.ToString();
        }

        public void SetLevelName(string name)
        {
            FindViewById<TextView>(Resource.Id.level_name).Text = name;
        }

        public void SetFolds(int folds)
        {
            FindViewById<TextView>(Resource.Id.folds).Text = folds.ToString();
        }

        public void SetFoldLimit(int limit)
        {
            FindViewById<TextView>(Resource.Id.fold_limit).Text = limit.ToString();
        }

        public void RedrawField()
        {
            MainMenuActivity.Instance.core.CurrentLevel().RenderField(game_field);
        }
    }
}