using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Origami
{
    public class LevelPagerAdapter : PagerAdapter
    {
        View[] level_screens;
        LayoutInflater inflater;
        int level_screen_count;
        LevelSelectActivity levelSelect;

        public LevelPagerAdapter(Context context, LevelSelectActivity levelSelect)
        {
            inflater = LayoutInflater.From(context);
            level_screen_count = 3;
            level_screens = new View[level_screen_count];
            this.levelSelect = levelSelect;
        }

        public override int Count
        {
            get { return level_screen_count; }
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            if(level_screens[position] == null)
            {
                level_screens[position] = inflater.Inflate(Resource.Layout.level_screen, container as ViewGroup, false);

                int[] row_ids = new int[] { Resource.Id.row0, Resource.Id.row1 };
                int[] level_ids = new int[] { Resource.Id.level_0, Resource.Id.level_1, Resource.Id.level_2, Resource.Id.level_3 };

                int global_level_offset = ChapterSelectActivity.selected_chapter * row_ids.Length * level_ids.Length * level_screen_count + position * row_ids.Length * level_ids.Length;

                for (int row = 0; row < row_ids.Length; row++)
                    for(int level = 0; level < level_ids.Length; level++)
                    {
                        int global_level_id = global_level_offset + level + row * level_ids.Length;
                        var row_layout = level_screens[position].FindViewById<LinearLayout>(row_ids[row]);
                        var level_layout = row_layout.FindViewById<AbsoluteLayout>(level_ids[level]);

                        level_layout.Click += (s, e) => MainMenuActivity.audioPlayer.PlayClick();
                        level_layout.Click += (s, e) => { levelSelect.LevelSelected(global_level_id); };

                        level_layout.FindViewById<TextView>(Resource.Id.level_number).Text = $"level {global_level_id + 1}";

                        var level_rate = MainMenuActivity.Instance.core.GetLevelRating(global_level_id);

                        var star_0 = level_layout.FindViewById<ImageView>(Resource.Id.star0);
                        var star_1 = level_layout.FindViewById<ImageView>(Resource.Id.star1);
                        var star_2 = level_layout.FindViewById<ImageView>(Resource.Id.star2);

                        var star_0_outline = level_layout.FindViewById<ImageView>(Resource.Id.star0_outline);
                        var star_1_outline = level_layout.FindViewById<ImageView>(Resource.Id.star1_outline);
                        var star_2_outline = level_layout.FindViewById<ImageView>(Resource.Id.star2_outline);

                        var lock_img = level_layout.FindViewById<ImageView>(Resource.Id.@lock);

                        var star_drawable = levelSelect.Resources.GetDrawable(Resource.Drawable.star);
                        var transparent_drawable = levelSelect.Resources.GetDrawable(Resource.Drawable.transparent);
                        var star_outline_drawable = levelSelect.Resources.GetDrawable(Resource.Drawable.star_outline);

                        if (level_rate.unlocked)
                        {
                            lock_img.SetImageDrawable(transparent_drawable);

                            star_0_outline.SetImageDrawable(star_outline_drawable);
                            star_1_outline.SetImageDrawable(star_outline_drawable);
                            star_2_outline.SetImageDrawable(star_outline_drawable);

                            if (level_rate.stars >= 1)
                                star_0.SetImageDrawable(star_drawable);
                            else
                                star_0.SetImageDrawable(transparent_drawable);

                            if (level_rate.stars >= 2)
                                star_1.SetImageDrawable(star_drawable);
                            else
                                star_1.SetImageDrawable(transparent_drawable);

                            if (level_rate.stars >= 3)
                                star_2.SetImageDrawable(star_drawable);
                            else
                                star_2.SetImageDrawable(transparent_drawable);
                        }
                        else
                        {
                            lock_img.SetImageDrawable(levelSelect.Resources.GetDrawable(Resource.Drawable.@lock));

                            star_0.SetImageDrawable(transparent_drawable);
                            star_1.SetImageDrawable(transparent_drawable);
                            star_2.SetImageDrawable(transparent_drawable);

                            star_0_outline.SetImageDrawable(transparent_drawable);
                            star_1_outline.SetImageDrawable(transparent_drawable);
                            star_2_outline.SetImageDrawable(transparent_drawable);
                        }
                    }
            }
                
            container.JavaCast<ViewPager>().AddView(level_screens[position]);
            return level_screens[position];
        }

        public override void DestroyItem(View container, int position, Java.Lang.Object view)
        {
            container.JavaCast<ViewPager>().RemoveView(view as View);
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object obj)
        {
            return view == obj;
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String("title");
        }
    }
}