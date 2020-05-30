using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Origami
{
    public class ChapterPagerAdapter : PagerAdapter
    {
        public struct Chapter
        {
            public int image_id;
            public int level_count;
            public int passed_count;
            public string name;
        }

        public static Chapter[] chapters;

        View[] chapter_screens;
        LayoutInflater inflater;
        int chapter_screen_count;
        ChapterSelectActivity chapterSelect;

        public ChapterPagerAdapter(Context context, ChapterSelectActivity chapterSelect)
        {
            inflater = LayoutInflater.From(context);
            chapter_screen_count = chapters.Length / 3;
            chapter_screens = new View[chapter_screen_count];
            this.chapterSelect = chapterSelect;
        }

        public override int Count
        {
            get { return chapter_screen_count; }
        }

        public override Java.Lang.Object InstantiateItem(View container, int position)
        {
            if(chapter_screens[position] == null)
            {
                chapter_screens[position] = inflater.Inflate(Resource.Layout.chapter_screen, container as ViewGroup, false);

                int[] chapter_ids = new int[] { Resource.Id.chapter0, Resource.Id.chapter1, Resource.Id.chapter2 };

                for(int i = 0; i < chapter_ids.Length; i++)
                {
                    int global_chapter_id = i + position * chapter_ids.Length;
                    var chapter = chapter_screens[position].FindViewById<LinearLayout>(chapter_ids[i]);
                    chapter.Click += (s, e) => chapterSelect.ChapterSelected(global_chapter_id);
                    chapter.FindViewById<ImageView>(Resource.Id.origami_image).SetImageResource(chapters[global_chapter_id].image_id);
                    chapter.FindViewById<TextView>(Resource.Id.chapter_name).Text = chapters[global_chapter_id].name;
                    chapter.FindViewById<TextView>(Resource.Id.chapter_progress).Text = $"{chapters[global_chapter_id].passed_count} / {chapters[global_chapter_id].level_count}";
                }
            }
                
            container.JavaCast<ViewPager>().AddView(chapter_screens[position]);
            return chapter_screens[position];
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