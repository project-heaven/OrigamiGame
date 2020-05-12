using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Nio.Channels;
using System;

namespace Origami
{
    public class LevelsGridViewAdapter : BaseAdapter
    {
        public static int button_count;

        public delegate void ButtonClickHandler(int id);
        public static event ButtonClickHandler ButtonClick;

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
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageView imageView;

            if (convertView == null)
            {  // if it's not recycled, initialize some attributes
                imageView = new ImageView(context);
                imageView.LayoutParameters = new GridView.LayoutParams(300, 300);
                imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
                imageView.SetPadding(20, 20, 20, 20);
                int local_position_copy = position;
                imageView.Click += (s, e) => { ButtonClick?.Invoke(local_position_copy); };
            }
            else
                imageView = (ImageView)convertView;

            imageView.SetImageResource(Resource.Drawable.level_icon);
            return imageView;
        }
    }
}