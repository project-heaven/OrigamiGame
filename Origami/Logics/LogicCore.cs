using System.Collections.Generic;
using System.Xml;
using Xamarin.Essentials;

namespace Origami.Logics
{
    public class LogicCore
    {
        Level[] levels;

        public LogicCore() {  }

        int current_level = 0;

        public const float ONE_STAR_PERCENT = 0.83f;
        public const float TWO_STARS_PERCENT = 0.90f;
        public const float THREE_STARS_PERCENT = 0.95f;

        public int LevelCount()
        {
            return levels.Length;
        }

        public bool SetLevel(int level_number)
        {
            if (!levels[level_number].rating.unlocked)
                return false;

            current_level = level_number;
            levels[current_level].ResetAndRefresh();
            return true;
        }

        public void NextLevel()
        {
            if (current_level == levels.Length - 1)
                return;

            if (levels[current_level + 1].rating.unlocked)
                levels[++current_level].ResetAndRefresh();
        }

        public void CurrentLevelCompleted()
        { 
            float rating = Level.GetCorrectPercent();

            int stars;

            if (rating >= THREE_STARS_PERCENT)
                stars = 3;
            else if (rating >= TWO_STARS_PERCENT)
                stars = 2;
            else if (rating >= ONE_STAR_PERCENT)
                stars = 1;
            else
            {
                GameActivity.Instance.LevelFailed(current_level);
                return;
            }

            if (levels[current_level].rating.stars == 0)
            {// first time level passing
                int chapter = current_level / 24;
                int curr_chapter_passed_count = Preferences.Get($"chapter {chapter} passed", 0) + 1;
                Preferences.Set($"chapter {chapter} passed", curr_chapter_passed_count);
                if (curr_chapter_passed_count == 24)
                    Preferences.Set($"chapter {chapter + 1} passed", 0);
            }

            levels[current_level].rating.stars = stars;

            Preferences.Set($"level {current_level} rating", stars);

            if (current_level != levels.Length - 1)
            {
                levels[current_level + 1].rating.unlocked = true;
                levels[current_level + 1].rating.stars = 0;
                Preferences.Set($"level {current_level + 1} rating", 0);
            }

            GameActivity.Instance.LevelCompleted(current_level, stars, current_level == levels.Length - 1);
        }

        public Level CurrentLevel()
        {
            return levels[current_level];
        }

        #if DEBUG
             public static Dictionary<int, int> level_folds_count = new Dictionary<int, int>();
        #endif

        public void Init(string levels_xml)
        {
            XmlDocument levels_doc = new XmlDocument();
            levels_doc.LoadXml(levels_xml);

            int level_count = levels_doc.DocumentElement.ChildNodes.Count;
            levels = new Level[level_count];

            int i = 0;
            foreach (XmlElement level in levels_doc.DocumentElement)
                levels[i++] = new Level(level, i);

            for (i = 0; i < levels.Length; i++)
                if (Preferences.ContainsKey($"level {i} rating"))
                    levels[i].rating = new Level.LevelRating()
                    { stars = Preferences.Get($"level {i} rating", 0), unlocked = true };
                else
                    levels[i].rating = new Level.LevelRating() { stars = -1, unlocked = false };

            levels[0].rating.unlocked = true;
            if (levels[0].rating.stars == -1)
                levels[0].rating.stars = 0;
        }

        public Level.LevelRating GetLevelRating(int level)
        {
            return levels[level].rating;
        }
    }
}