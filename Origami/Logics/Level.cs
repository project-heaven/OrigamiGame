using Android.Widget;

using System.Xml;

namespace Origami.Logics
{
    public class Level
    {
        PaperSheet sheet;
        PaperSheet foldedSheet;

        public void LoadFromXml(string xml)
        {
            XmlDocument xml_doc = new XmlDocument();
            //xml_doc.LoadXml(xml);

            sheet = new PaperSheet();
            sheet.LoadQuad();
        }

        Vector2 fold_start_pos;

        public void TouchStart(Vector2 position_normalized)
        {
            fold_start_pos = sheet.GetClosestCorner(position_normalized);
        }

        public void TouchMove(Vector2 position_normalized)
        {
            FloatLine fold_line = new FloatLine();
            fold_line.passThrough = (fold_start_pos + position_normalized) * 0.5f;
            Vector2 fold_vec = fold_start_pos - position_normalized;
            fold_line.angle = new Vector2(-fold_vec.y, fold_vec.x);

            foldedSheet = sheet.Fold(fold_line);

            GameActivity.Instance.RedrawField();
        }

        public void TouchEnd(Vector2 position_normalized)
        {
            sheet = foldedSheet;

            GameActivity.Instance.RedrawField();
        }

        public void RenderField(ImageView image_view)
        {
            foldedSheet.Render(image_view);
        }
    }
}