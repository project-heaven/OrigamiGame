using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace Origami
{
    public class LevelsGridViewAdapter : BaseAdapter
    {
        public static int button_count;

        Context context;

        public LevelsGridViewAdapter(Context context)
        {
            this.context = context;
        }

        public override int Count => button_count;

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        static Color buttonBackColor = MainMenuActivity.Instance.Resources.GetColor(Resource.Color.buttonBackgroundColor);
        static Color buttonColor = MainMenuActivity.Instance.Resources.GetColor(Resource.Color.buttonColor);

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.level_icon, parent, false);
            }
                
            view.FindViewById<TextView>(Resource.Id.level_number).Text = $"УРОВЕНЬ {position + 1}";

            var level_rate = MainMenuActivity.Instance.core.GetLevelRating(position);

            var star_0 = view.FindViewById<ImageView>(Resource.Id.star0);
            var star_1 = view.FindViewById<ImageView>(Resource.Id.star1);
            var star_2 = view.FindViewById<ImageView>(Resource.Id.star2);

            var lock_img = view.FindViewById<ImageView>(Resource.Id.@lock);

            if (level_rate.unlocked)
            {
                lock_img.SetColorFilter(buttonBackColor);

                if (level_rate.stars >= 1)
                    star_0.SetImageResource(Resource.Drawable.star_filled);
                else
                    star_0.SetImageResource(Resource.Drawable.star);

                if (level_rate.stars >= 2)
                    star_1.SetImageResource(Resource.Drawable.star_filled);
                else
                    star_1.SetImageResource(Resource.Drawable.star);

                if (level_rate.stars >= 3)
                    star_2.SetImageResource(Resource.Drawable.star_filled);
                else
                    star_2.SetImageResource(Resource.Drawable.star);


                star_0.SetColorFilter(buttonColor);
                star_1.SetColorFilter(buttonColor);
                star_2.SetColorFilter(buttonColor);
            }
            else
            {
                lock_img.SetColorFilter(buttonColor);

                star_0.SetColorFilter(buttonBackColor);
                star_1.SetColorFilter(buttonBackColor);
                star_2.SetColorFilter(buttonBackColor);
            }

            return view;
        }
    }
}