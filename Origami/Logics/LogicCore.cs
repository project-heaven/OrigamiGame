using System.Xml;

namespace Origami.Logics
{
    public class LogicCore
    {
        public Level[] levels;

        public LogicCore() { }

        public Level CurrentLevel()
        {
            return levels[0];
        }

        public void LoadLevels(string xml)
        {
            XmlDocument levels_doc = new XmlDocument();
            levels_doc.LoadXml(xml);

            levels = new Level[levels_doc.DocumentElement.ChildNodes.Count];

            int i = 0;
            foreach (XmlElement level in levels_doc.DocumentElement)
                levels[i++] = new Level(level);
        }
    }
}