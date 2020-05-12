namespace Origami.Logics
{
    public class LogicCore
    {
        public Level[] levels;

        public LogicCore()
        {
            levels = new Level[1];

            levels[0] = new Level();
            levels[0].LoadFromXml("");
        }

        public Level CurrentLevel()
        {
            return levels[0];
        }

        public void LoadLevels(string xml)
        {
            
        }
    }
}