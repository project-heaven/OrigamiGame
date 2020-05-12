namespace Origami.Logics
{
    public class LogicCore
    {
        public Level level;

        public void Init()
        {
            level = new Level();
            level.LoadFromXml("");
        }
    }
}