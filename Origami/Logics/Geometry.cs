using Java.Lang;
using System.Collections.Generic;

namespace Origami.Logics
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y) { this.x = x; this.y = y; }

        public Vector2 Normalized()
        {
            float length = (float)Math.Sqrt(x * x + y * y);
            return new Vector2(x / length, y / length);
        }

        public float Dot(Vector2 rhs) { return x * rhs.x + y * rhs.y; }

        public float LengthSqr() { return x * x + y * y; }

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs) { return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y); }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs) { return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y); }

        public static Vector2 operator *(Vector2 lhs, float rhs) { return new Vector2(lhs.x * rhs, lhs.y * rhs); }

        public static Vector2 operator *(float lhs, Vector2 rhs) { return new Vector2(lhs * rhs.x, lhs * rhs.y); }
    }

    public struct Triangle
    {
        public Vector2[] verts;

        public List<Triangle> Fold(FloatLine line)
        {
            List<Triangle> triangles = new List<Triangle>();

            List<Vector2> left_side_points = new List<Vector2>(3);
            List<Vector2> right_side_points = new List<Vector2>(3);

            foreach (var point in verts)
                if (line.GetPointSide(point) == PointSide.LEFT)
                    left_side_points.Add(point);
                else
                    right_side_points.Add(point);

            switch (left_side_points.Count)
            {
                case 0: {
                    triangles.Add(new Triangle() { verts = (Vector2[])verts.Clone() });

                    break;
                }
                case 1: {
                    // When triangle separated by line there are triangle and quadrilateral in result.
                    FloatLine[] intersected_sides = new FloatLine[]
                    {
                        new FloatLine(left_side_points[0], left_side_points[0] - right_side_points[0]),
                        new FloatLine(left_side_points[0], left_side_points[0] - right_side_points[1])
                    };
                    Vector2[] line_intersections = new Vector2[2];
                    for (int i = 0; i < 2; i++)
                        line_intersections[i] = line.LineIntersection(intersected_sides[i]);

                    // Add segment.
                    var new_segment = new LineSegment {
                        start = line_intersections[0],
                        end = line_intersections[1]
                    };

                    // Generate triangle.
                    Triangle new_triangle = new Triangle();
                    // Mirror one triangle point because it lies to left.
                    new_triangle.verts = new Vector2[] { left_side_points[0], line_intersections[0], line_intersections[1] };
                    new_triangle.verts[0] = line.MirrorPoint(new_triangle.verts[0]);
                    triangles.Add(new_triangle);
                    // Generate quadrilateral.
                    Vector2[] quadrilateral_points = new Vector2[4];
                    quadrilateral_points[0] = right_side_points[0];
                    quadrilateral_points[1] = right_side_points[1];
                    quadrilateral_points[2] = line_intersections[1];
                    quadrilateral_points[3] = line_intersections[0];
                    var quadrilateral_triangles = TriangulateQuadrilateral(quadrilateral_points);
                    triangles.AddRange(quadrilateral_triangles);

                    break;
                }
                case 2: {
                    // When triangle separated by line there are triangle and quadrilateral in result.
                    FloatLine[] intersected_sides = new FloatLine[]
                    {
                        new FloatLine(right_side_points[0], right_side_points[0] - left_side_points[0]),
                        new FloatLine(right_side_points[0], right_side_points[0] - left_side_points[1])
                    };
                    Vector2[] line_intersections = new Vector2[2];
                    for (int i = 0; i < 2; i++)
                        line_intersections[i] = line.LineIntersection(intersected_sides[i]);
                    // Generate triangle.
                    Triangle new_triangle = new Triangle();
                    new_triangle.verts = new Vector2[] { right_side_points[0], line_intersections[0], line_intersections[1] };
                    triangles.Add(new_triangle);
                    // Generate quadrilateral and mirror two points because they lie to left.
                    Vector2[] quadrilateral_points = new Vector2[4];

                    quadrilateral_points[0] = line.MirrorPoint(left_side_points[1]);
                    quadrilateral_points[1] = line.MirrorPoint(left_side_points[0]);
                    quadrilateral_points[2] = line_intersections[0];
                    quadrilateral_points[3] = line_intersections[1];
                    var quadrilateral_triangles = TriangulateQuadrilateral(quadrilateral_points);

                    triangles.AddRange(quadrilateral_triangles);

                    break;
                }
                case 3: {
                    Triangle new_triangle = new Triangle() { verts = (Vector2[])verts.Clone() };
                    for (int i = 0; i < 3; i++)
                        new_triangle.verts[i] = line.MirrorPoint(new_triangle.verts[i]);

                    triangles.Add(new_triangle);

                    break;
                }
            }

            return triangles;
        }

        Triangle[] TriangulateQuadrilateral(Vector2[] points_cw)
        {
            return new Triangle[2]
            {
                new Triangle() { verts = new Vector2[] { points_cw[0], points_cw[1], points_cw[2] } },
                new Triangle() { verts = new Vector2[] { points_cw[0], points_cw[2], points_cw[3] } }
            };
        }
    }

    enum PointSide { LEFT, RIGHT }

    public struct FloatLine
    {
        public Vector2 passThrough;
        public Vector2 angle;

        public FloatLine(Vector2 pass_through, Vector2 angle)
        {
            passThrough = pass_through;
            this.angle = angle;
        }

        public Vector2 LineIntersection(FloatLine line)
        {
            Vector2 intersection = new Vector2(0, 0);

            // y1 = k1 * x + t1
            // y2 = k2 * x + t2
            // k1 * x + t1 = k2 * x + t2
            // x * (k1 - k2) = t2 - t1
            // x = (t2 - t1) / (k1 - k2)
            // y = k1 * x + t1
            float k1 = angle.y / angle.x;
            float k2 = line.angle.y / line.angle.x;
            float t1 = passThrough.y - k1 * passThrough.x;
            float t2 = line.passThrough.y - k2 * line.passThrough.x;

            if (float.IsInfinity(k1))
            {
                // Means that this line is vertical
                intersection.x = passThrough.x;
                intersection.y = k2 * intersection.x + t2;
                return intersection;
            }

            if (float.IsInfinity(k2))
            {
                // Means that passed line is vertical
                intersection.x = line.passThrough.x;
                intersection.y = k1 * intersection.x + t1;
                return intersection;
            }

            intersection.x = (t2 - t1) / (k1 - k2);
            intersection.y = k1 * intersection.x + t1;
            return intersection;
        }

        // SIGNED DISTANCE TO LINE:
        //
        // General line equation is a*x + b*y + c = 0
        // a*point_x + b*point_y + c is distance from point to line
        // line equation by anglular coefficient is y = k*x + t
        // y*b = -c - a*x
        // y = (-a/b)*x - c/b
        // a/b = -k
        // c/b = -t
        // let a = angle.y and b = -angle.x
        // then c = -b*t = angle.x * t.

        // Distance is : angle.y * point.x - angle.x * point.y + angle.x * t
        // t is : passThrough.y - k * passThrough.x
        // k is : angle.y / angle.x
        // therefore t is : passThrough.y - angle.y / angle.x * passThrough.x
        // therefore distance is : angle.y * point.x - angle.x * point.y + angle.x * (passThrough.y - angle.y / angle.x * passThrough.x)
        // simplyfying : angle.y * point.x - angle.x * point.y + angle.x * passThrough.y - angle.y * passThrough.x
        // angle.y * (point.x - passThrough.x) + angle.x * (passThrough.y - point.y).

        internal PointSide GetPointSide(Vector2 point)
        {
            float dist_unnormalized = angle.y * (point.x - passThrough.x) + angle.x * (passThrough.y - point.y);
            if (dist_unnormalized >= 0.0)
                return PointSide.LEFT;
            else
                return PointSide.RIGHT;
        }

        internal Vector2 MirrorPoint(Vector2 point)
        {
            float dist = angle.y * (point.x - passThrough.x) + angle.x * (passThrough.y - point.y);
            float inv_angle_len = 1.0f / (float)Math.Sqrt(angle.LengthSqr());
            dist *= inv_angle_len;
            // Distance to move along normal {right pointing normal is normalize(-angle.y, angle.x)}.
            float point_move = -dist * 2.0f;
            Vector2 normal = new Vector2(angle.y, -angle.x) * inv_angle_len;
            return point + normal * point_move;
        }
    }

    public struct LineSegment
    {
        public Vector2 start;
        public Vector2 end;

        public LineSegment(Vector2 start, Vector2 end)
        {
            this.start = start;
            this.end = end;
        }

        public FloatLine GetLine()
        {
            return new FloatLine(start, end - start);
        }

        public List<LineSegment> Fold(FloatLine line)
        {
            List<LineSegment> segments = new List<LineSegment>();

            var start_side = line.GetPointSide(start);
            var end_side = line.GetPointSide(end);

            if (start_side == end_side) // No intersection.
            {
                if(start_side == PointSide.LEFT)
                {
                    // Mirror.
                    LineSegment new_segment = this;
                    //new_segment.layer = ++PaperSheet.top_layer;
                    new_segment.start = line.MirrorPoint(new_segment.start);
                    new_segment.end = line.MirrorPoint(new_segment.end);
                    segments.Add(new_segment);
                }
                else
                    segments.Add(this);
            }
            else
            {
                Vector2 intersection = line.LineIntersection(new FloatLine(start, end - start));

                LineSegment end_point_segment = new LineSegment(end, intersection);
                LineSegment start_point_segment = new LineSegment(start, intersection);

                if (start_side == PointSide.LEFT)
                    start_point_segment.start = line.MirrorPoint(start_point_segment.start);                 
                else
                    end_point_segment.start = line.MirrorPoint(end_point_segment.start);              

                segments.Add(start_point_segment);
                segments.Add(end_point_segment);
            }

            return segments;
        }
    }
}