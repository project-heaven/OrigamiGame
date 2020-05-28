using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace Origami
{
    [Activity(Label = "LevelSelectActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = ScreenOrientation.Landscape)]
    public class LevelSelectActivity : Activity
    {
        public static LevelSelectActivity Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_levelselect);

            InflateLevelIcons();

            FindViewById<ImageButton>(Resource.Id.back_button).Click
            += (s, e) => { StartActivity(typeof(MainMenuActivity)); };
        }

        
        void InflateLevelIcons()
        {
            LinearLayout[] rows = new LinearLayout[4];

            rows[0] = FindViewById<LinearLayout>(Resource.Id.first_row);
            rows[1] = FindViewById<LinearLayout>(Resource.Id.second_row);
            rows[2] = FindViewById<LinearLayout>(Resource.Id.third_row);
            rows[3] = FindViewById<LinearLayout>(Resource.Id.fourth_row);

            for(int i = 0; i < 4; i++)
            {
                for(int lvl = 0; lvl < 4; lvl++)
                {
                    int level_id = lvl + i * 4;

                    var level_view = (rows[i].GetChildAt(lvl) as ViewGroup).GetChildAt(0);

                    level_view.Click += (s, e) => { LevelSelected(level_id); };

                    level_view.FindViewById<TextView>(Resource.Id.level_number).Text = $"level {level_id + 1}";

                    var level_rate = MainMenuActivity.Instance.core.GetLevelRating(level_id);

                    var star_0 = level_view.FindViewById<ImageView>(Resource.Id.star0);
                    var star_1 = level_view.FindViewById<ImageView>(Resource.Id.star1);
                    var star_2 = level_view.FindViewById<ImageView>(Resource.Id.star2);

                    var star_0_outline = level_view.FindViewById<ImageView>(Resource.Id.star0_outline);
                    var star_1_outline = level_view.FindViewById<ImageView>(Resource.Id.star1_outline);
                    var star_2_outline = level_view.FindViewById<ImageView>(Resource.Id.star2_outline);

                    var lock_img = level_view.FindViewById<ImageView>(Resource.Id.@lock);

                    if (level_rate.unlocked)
                    {
                        lock_img.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

                        star_0_outline.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star_outline));
                        star_1_outline.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star_outline));
                        star_2_outline.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star_outline));

                        if (level_rate.stars >= 1)
                            star_0.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star));
                        else
                            star_0.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

                        if (level_rate.stars >= 2)
                            star_1.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star));
                        else
                            star_1.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

                        if (level_rate.stars >= 3)
                            star_2.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.star));
                        else
                            star_2.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));
                    }
                    else
                    {
                        lock_img.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.@lock));

                        star_0.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));
                        star_1.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));
                        star_2.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));

                        star_0_outline.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));
                        star_1_outline.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));
                        star_2_outline.SetImageDrawable(Resources.GetDrawable(Resource.Drawable.transparent));
                    }
                }
            }
        }
        
        void LevelSelected(int level)
        {
            if(MainMenuActivity.Instance.core.GetLevelRating(level).unlocked)
            {
                StartActivity(typeof(GameActivity));
                MainMenuActivity.Instance.core.SetLevel(level);
            }
        }
    }
}