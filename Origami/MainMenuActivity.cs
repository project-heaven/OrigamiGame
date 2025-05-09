﻿using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Origami.Logics;
using System;
using System.IO;
using Xamarin.Essentials;

namespace Origami
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainMenuActivity : Activity
    {
        public static MainMenuActivity Instance;
        public LogicCore core;
        public static Audio audioPlayer;

        static bool music_enabled = true;
        static bool sfx_enabled = true;

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

            audioPlayer.ResumeAmbient();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.AddFlags(WindowManagerFlags.Fullscreen);
            
            base.OnCreate(savedInstanceState);

            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_mainmenu);

            var start_button = FindViewById<ImageButton>(Resource.Id.start_button);
            var about_button = FindViewById<TextView>(Resource.Id.about_button);

            var music_button = FindViewById<ImageButton>(Resource.Id.music);
            var sfx_button = FindViewById<ImageButton>(Resource.Id.sfx);

            music_button.Click += (s, e) => audioPlayer.PlayClick();

            music_button.BackgroundTintList =
                ColorStateList.ValueOf(Resources.GetColor(music_enabled ? Resource.Color.baseColor : Resource.Color.grayoutColor));

            sfx_button.BackgroundTintList =
                ColorStateList.ValueOf(Resources.GetColor(sfx_enabled ? Resource.Color.baseColor : Resource.Color.grayoutColor));

            music_button.Click += (s, e) =>
            {
                music_enabled = !music_enabled;
                audioPlayer.SetAmbientEnabled(music_enabled);
                music_button.BackgroundTintList = 
                ColorStateList.ValueOf(Resources.GetColor(music_enabled ? Resource.Color.baseColor : Resource.Color.grayoutColor));
            };

            sfx_button.Click += (s, e) =>
            {
                sfx_enabled = !sfx_enabled;
                audioPlayer.SetSfxEnabled(sfx_enabled);
                audioPlayer.PlayClick();
                sfx_button.BackgroundTintList =
                ColorStateList.ValueOf(Resources.GetColor(sfx_enabled ? Resource.Color.baseColor : Resource.Color.grayoutColor));
            };

            start_button.Click += (s, e) => audioPlayer.PlayClick();
            about_button.Click += (s, e) => audioPlayer.PlayClick();

            start_button.Click += (s, e) =>
            {
                another_activity_entered = true;
                StartActivity(typeof(ChapterSelectActivity));
                
            };

            about_button.Click += (s, e) =>
            {
                another_activity_entered = true;
                StartActivity(typeof(AboutActivity));
            };

            if (Instance != null)
                return;

            Instance = this;

            Ads.Init(this);

            core = new LogicCore();
            string levels_xml = new StreamReader(Assets.Open("levels.xml")).ReadToEnd();
            core.Init(levels_xml);

            audioPlayer = new Audio(this);

            Level.FoldLineColor = Resources.GetColor(Resource.Color.resultOutlineColor);
            PaperSheet.Color = Resources.GetColor(Resource.Color.sheetColor);
            PaperSheet.OutlineColor = Resources.GetColor(Resource.Color.resultOutlineColor);
        }
    }
}