using Android.Graphics;
using Android.Widget;
using System.Xml;

namespace Origami.Logics
{
    public class Level
    {
        PaperSheet FoldedSheet {
            get { return foldedSheet; }
            set 
            {
                foldedSheet = value;
                GameActivity.Instance?.RedrawField();
            } 
        }
        PaperSheet foldedSheet;

        PaperSheet[] fold_stages;

        int max_folds;
        int last_fold_id;

        const float SHEET_PADDING = 0.1f;

        FloatLine[] folds;
        enum LevelType
        {
            RESULT,
            SEQUENCE
        }
        LevelType type;
        LineSegment[] resultOutline;

        public Level(XmlElement level_element)
        {
            if (level_element.GetAttribute("type") == "result")
                type = LevelType.RESULT;
            else
                type = LevelType.SEQUENCE;

            foreach (XmlElement data in level_element)
                if (data.Name == "folds")
                    LoadFolds(data);
                else if (data.Name == "result")
                    LoadResultOutline(data);

            fold_stages = new PaperSheet[max_folds + 1];
            fold_stages[0] = new PaperSheet();
            fold_stages[0].LoadQuad(SHEET_PADDING);
            last_fold_id = 0;

            FoldedSheet = fold_stages[0];
        }

        void LoadFolds(XmlElement folds_xml)
        {
            max_folds = folds_xml.ChildNodes.Count;
            folds = new FloatLine[max_folds];
            int i = 0;
            foreach (XmlElement fold in folds_xml)
            {
                float.TryParse(fold.GetAttribute("x0"), out float x0);
                float.TryParse(fold.GetAttribute("y0"), out float y0);
                float.TryParse(fold.GetAttribute("x1"), out float x1);
                float.TryParse(fold.GetAttribute("y1"), out float y1);

                Vector2 start = new Vector2(x0, y0);
                Vector2 end = new Vector2(x1, y1);
                folds[i++] = new FloatLine(start, end - start);
            }
        }

        void LoadResultOutline(XmlElement result_xml)
        {
            Vector2[] points = new Vector2[result_xml.ChildNodes.Count];
            int i = 0;
            foreach(XmlElement point in result_xml)
            {
                float.TryParse(point.GetAttribute("x"), out float x);
                float.TryParse(point.GetAttribute("y"), out float y);

                points[i] = new Vector2(x, y);
                // Apply sheet padding
                points[i] = points[i] * (1.0f - 2.0f * SHEET_PADDING);
                points[i] = points[i] + new Vector2(SHEET_PADDING, SHEET_PADDING);

                i++;
            }

            resultOutline = new LineSegment[points.Length];
            for (i = 0; i < points.Length; i++)
                resultOutline[i] = new LineSegment(points[i], points[(i + 1) % points.Length]);
        }

        public void Undo()
        {
            if (last_fold_id == 0)
                return;

            FoldedSheet = fold_stages[--last_fold_id];
        }

        public void Reset()
        {
            last_fold_id = 0;
            FoldedSheet = fold_stages[0];
        }

        Vector2 fold_start_pos;

        public void TouchStart(Vector2 position_normalized)
        {
            fold_start_pos = fold_stages[last_fold_id].GetClosestCorner(position_normalized);
        }

        public void TouchMove(Vector2 position_normalized)
        {
            FloatLine fold_line = new FloatLine();
            fold_line.passThrough = (fold_start_pos + position_normalized) * 0.5f;
            Vector2 fold_vec = fold_start_pos - position_normalized;
            fold_line.angle = new Vector2(-fold_vec.y, fold_vec.x);

            FoldedSheet = fold_stages[last_fold_id].Fold(fold_line);
        }

        public void TouchEnd(Vector2 position_normalized)
        {
            fold_stages[++last_fold_id] = FoldedSheet;

            if(last_fold_id == fold_stages.Length - 1)
            {
                // reached last fold
            }
        }

        public void RenderField(ImageView image_view)
        {
            Bitmap bmp = Bitmap.CreateBitmap(image_view.Width, image_view.Height, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bmp);

            FoldedSheet.Render(canvas);

            if (type == LevelType.RESULT)
                RenderResultOutline(canvas);
            else
                RenderFoldLine(canvas);

            image_view.SetImageBitmap(bmp);
        }

        void RenderResultOutline(Canvas canvas)
        {
            float width = canvas.Width;
            float height = canvas.Height;

            Paint paint = new Paint();
            paint.Color = new Color(160, 40, 10, 255);
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 5;

            foreach (var line in resultOutline)
                canvas.DrawLine(line.start.x * width, line.start.y * height, line.end.x * width, line.end.y * height, paint);
        }

        void RenderFoldLine(Canvas canvas)
        {

        }
    }
}