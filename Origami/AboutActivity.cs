﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class AboutActivity : Activity
    {
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
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_about);

            var back_button = FindViewById<ImageButton>(Resource.Id.back_button);

            back_button.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
            back_button.Click += (s, e) => { another_activity_entered = true;  StartActivity(typeof(MainMenuActivity)); };
        }
    }
}