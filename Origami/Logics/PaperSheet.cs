using Android.Graphics;
using System.Collections.Generic;

namespace Origami.Logics
{
    public class PaperSheet
    {
        public static Color Color;

        public PaperSheet() {  }

        public void LoadQuad(float padding)
        {
            // Add all-quad
            triangles = new List<Triangle>()
            {
                new Triangle()
                {
                    verts = new Vector2[] { new Vector2(1.0f - padding, 1.0f - padding), new Vector2(padding, 1.0f - padding), new Vector2(padding, padding) }
                },
                new Triangle()
                {
                    verts = new Vector2[] { new Vector2(1.0f - padding, 1.0f - padding), new Vector2(1.0f - padding, padding), new Vector2(padding, padding) }
                }
            };
        }

        // Coordinates stored in normalized coordinates [0..1] origin - left top
        List<Triangle> triangles = new List<Triangle>();

        // Folds all triangles from left side of line to right
        public PaperSheet Fold(FloatLine line)
        {
            PaperSheet new_paper = new PaperSheet();

            foreach (var triangle in triangles)
                new_paper.triangles.AddRange(triangle.Fold(line));

            return new_paper;
        }

        public void Render(Canvas canvas)
        {
            Paint fill_paint = new Paint();
            fill_paint.Color = Color;
            fill_paint.SetStyle(Paint.Style.Fill);

            foreach (var triangle in triangles)
                RenderTriangle(canvas, triangle, fill_paint);
        }

        void RenderTriangle(Canvas canvas, Triangle triangle, Paint paint)
        {
            float width = canvas.Width;
            float height = canvas.Height;

            var path = new Path();
            path.SetFillType(Path.FillType.EvenOdd);
            path.MoveTo(triangle.verts[0].x * width, triangle.verts[0].y * height);
            path.LineTo(triangle.verts[1].x * width, triangle.verts[1].y * height);
            path.LineTo(triangle.verts[2].x * width, triangle.verts[2].y * height);
            path.LineTo(triangle.verts[0].x * width, triangle.verts[0].y * height);
            path.Close();

            canvas.DrawPath(path, paint);
        }
    
        // Bruteforce.
        public float GetCorrectPercent(Bitmap bitmap, LineSegment[] resultOutline)
        {
            int correct = 0;
            int incorrect = 0;

            for(float x = 0; x < 1; x += 0.01f)
                for(float y = 0; y < 1; y += 0.01f)
                { 
                    int pixel_color = bitmap.GetPixel((int)(x * bitmap.Width), (int)(y * bitmap.Height));

                    bool sheet_covered = Color.R == Color.GetRedComponent(pixel_color);
                    bool should_be_inside = PointInsideOutline(new Vector2(x, y), resultOutline);

                    if (!sheet_covered)
                        continue;

                    if (should_be_inside)
                        correct++;
                    else
                        incorrect++;
                }

            return (float)correct / (correct + incorrect);
        }

        bool PointInsideOutline(Vector2 point, LineSegment[] outline)
        {
            foreach (var outline_segment in outline)
                if (outline_segment.GetLine().GetPointSide(point) == PointSide.LEFT)
                    return false;

            return true;
        }
    }
}