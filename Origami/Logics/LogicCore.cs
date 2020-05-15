using System.Xml;
using Xamarin.Essentials;

namespace Origami.Logics
{
    public class LogicCore
    {
        public Level[] levels;

        public LogicCore() {  }

        int current_level = 0;

        public bool SetLevel(int level_number)
        {
            if (!levels[level_number].rating.unlocked)
                return false;

            current_level = level_number;
            return true;
        }

        public void NextLevel()
        {
            if (current_level != levels.Length - 1 && levels[current_level + 1].rating.unlocked)
                current_level++;
        }

        public void CurrentLevelCompleted()
        {
            float rating = levels[current_level].GetCorrectPercent();

            int stars = 0;

            // FIXME: Hardcoded values 
            if (rating > 0.8f)
                stars++;
            if (rating > 0.9f)
                stars++;
            if (rating > 0.99f)
                stars++;

            levels[current_level].rating.stars = stars;
            Preferences.Set($"level {current_level} rating", stars);

            if (current_level != levels.Length - 1)
            {
                levels[current_level + 1].rating.unlocked = true;
                Preferences.Set($"level {current_level + 1} rating", 0);
            }

            GameActivity.Instance.LevelComplete(stars);
        }

        public Level CurrentLevel()
        {
            return levels[current_level];
        }

        public void Init(string levels_xml)
        {
            XmlDocument levels_doc = new XmlDocument();
            levels_doc.LoadXml(levels_xml);

            int level_count = levels_doc.DocumentElement.ChildNodes.Count;
            levels = new Level[level_count];

            int i = 0;
            foreach (XmlElement level in levels_doc.DocumentElement)
                levels[i++] = new Level(level);

            LoadLevelRatings();
        }

        void LoadLevelRatings()
        {
            for (int i = 0; i < levels.Length; i++)
                if (Preferences.ContainsKey($"level {i} rating"))
                    levels[i].rating = new Level.LevelRating() 
                    { stars = Preferences.Get($"level {i} rating", 0), unlocked = true };
                else
                    levels[i].rating = new Level.LevelRating() { stars = 0, unlocked = false };

            levels[0].rating.unlocked = true;
        }

        public Level.LevelRating GetLevelRating(int level)
        {
            if (level < 5)
                return levels[level].rating;
            else
                return new Level.LevelRating() { stars = 0, unlocked = false };
        }
    }
}