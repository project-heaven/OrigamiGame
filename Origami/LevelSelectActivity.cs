﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Xamarin.Essentials;

namespace Origami
{
    [Activity(Label = "LevelSelectActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class LevelSelectActivity : Activity
    {
        public static LevelSelectActivity Instance;

        static int start_scroll_offset = -1;

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

            SetContentView(Resource.Layout.activity_levelselect);

            var level_pager = FindViewById<ViewPager>(Resource.Id.levels_pager);
            level_pager.Adapter = new LevelPagerAdapter(this, this);
            level_pager.ScrollChange += ScrollChange;

            if (start_scroll_offset == -1)
            {
                start_scroll_offset = 0;
                if (Preferences.ContainsKey($"level {8 + ChapterSelectActivity.selected_chapter * 24} rating"))
                    start_scroll_offset++;
                if (Preferences.ContainsKey($"level {16 + ChapterSelectActivity.selected_chapter * 24} rating"))
                    start_scroll_offset++;
            }          
            else
                start_scroll_offset = last_scroll;

            UpdateScrollDots(start_scroll_offset);
            level_pager.SetCurrentItem(start_scroll_offset, false);

            var back_button = FindViewById<ImageButton>(Resource.Id.back_button);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            back_button.Click += (s, e) => { another_activity_entered = true; StartActivity(typeof(ChapterSelectActivity)); };
        }

        static int last_scroll;

        private void ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            float scroll_x_norm = e.ScrollX / ((float)e.V.Width);
            int visible_screen = Math.Round(scroll_x_norm);

            visible_screen += start_scroll_offset;
            if (visible_screen < 0)
                visible_screen = 0;

            UpdateScroll(visible_screen);
        }

        void UpdateScrollDots(int position)
        {
            int[] level_selected_view_ids = new int[] { Resource.Id.screen_selected0, Resource.Id.screen_selected1, Resource.Id.screen_selected2 };

            if (position >= level_selected_view_ids.Length)
                position = level_selected_view_ids.Length - 1;

            for (int i = 0; i < level_selected_view_ids.Length; i++)
            {
                var view = FindViewById<LinearLayout>(level_selected_view_ids[i]);
                view.SetBackgroundResource(i == position ? Resource.Drawable.roundedCorners : Resource.Drawable.transparent);
            }
        }

        void UpdateScroll(int position)
        {
            if(position != last_scroll)
            {
                last_scroll = position;
                MainMenuActivity.audioPlayer.PlayScroll();
            }

            UpdateScrollDots(position);
        }

        public void LevelSelected(int level)
        {
            if(MainMenuActivity.Instance.core.GetLevelRating(level).unlocked)
            {
                another_activity_entered = true;
                StartActivity(typeof(GameActivity));
                MainMenuActivity.Instance.core.SetLevel(level);
            }
        }
    }
}