using Android.Graphics;
using System.Collections.Generic;

namespace Origami.Logics
{
    public class PaperSheet
    {
        public static Color Color;

        public static Color OutlineColor;

        public PaperSheet() {  }

        public void LoadQuad(float padding)
        {
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
            {
                var new_triangles = triangle.Fold(line);
                foreach (var tr in new_triangles)
                    foreach (var vert in tr.verts)
                        if (vert.x > 1 || vert.y > 1 || vert.x < 0 || vert.y < 0)
                            return null;

                new_paper.triangles.AddRange(new_triangles);
            }
                

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

            for(float x = 0; x < 1; x += 0.015f)
                for(float y = 0; y < 1; y += 0.015f)
                {
                    int pixel_color = bitmap.GetPixel((int)(x * bitmap.Width), (int)(y * bitmap.Height));

                    bool sheet_covered = pixel_color != 0 && Color.GetGreenComponent(pixel_color) != OutlineColor.G;
                    bool should_be_inside = PointInsideOutline(new Vector2(x, y), resultOutline);

                    if ((sheet_covered & !should_be_inside) || (!sheet_covered & should_be_inside))
                        incorrect++;
                    else
                        correct++;
                }

            return (float)correct / (correct + incorrect);
        }

        bool PointInsideOutline(Vector2 point, LineSegment[] outline)
        {
            LineSegment ray = new LineSegment(point, point + new Vector2(100, 0));

            int intersections = 0;

            foreach (var outline_segment in outline)
                if (outline_segment.IntersectSegment(ray))
                    intersections++;

            return intersections % 2 == 1;
        }
    }
}