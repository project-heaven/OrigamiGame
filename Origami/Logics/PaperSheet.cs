using Android.Graphics;
using System.Collections.Generic;

namespace Origami.Logics
{
    public class PaperSheet
    {
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

        public Vector2 GetClosestCorner(Vector2 point)
        {
            List<Vector2> all_verts = new List<Vector2>(triangles.Count * 3);
            foreach(var triangle in triangles)
            {
                all_verts.Add(triangle.verts[0]);
                all_verts.Add(triangle.verts[1]);
                all_verts.Add(triangle.verts[2]);
            }

            var convex_hull_ids = Common.ConvexHull(all_verts);

            float min_sqr_dist = float.PositiveInfinity;
            Vector2 corner = new Vector2();

            foreach (var id in convex_hull_ids)
            {
                float sqr_dist = (all_verts[id] - point).LengthSqr();
                if(sqr_dist < min_sqr_dist)
                {
                    min_sqr_dist = sqr_dist;
                    corner = all_verts[id];
                }
            }

            return corner;
        }

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
            fill_paint.Color = new Color(10, 40, 160, 50);
            fill_paint.SetStyle(Paint.Style.Fill);

            Paint stroke_paint = new Paint();
            stroke_paint.Color = new Color(160, 40, 10, 255);
            stroke_paint.SetStyle(Paint.Style.Stroke);
            stroke_paint.StrokeWidth = 7;

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
    
        public float CorrectPercent(LineSegment[] result)
        {
            return 1.0f;
        }
    }
}