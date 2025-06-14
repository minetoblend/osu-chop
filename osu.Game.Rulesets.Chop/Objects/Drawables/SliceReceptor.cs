﻿using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using Vector2 = osuTK.Vector2;

namespace osu.Game.Rulesets.Chop.Objects.Drawables;

public partial class SliceReceptor : CompositeDrawable
{
    public required Func<bool> CanHit { get; init; }

    public required Action Hit { get; init; }

    public Vector2 LastSliceStartPosition;
    public Vector2 LastSliceEndPosition;

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        if (Precision.AlmostEquals(e.MousePosition, e.LastMousePosition))
            return false;

        // let's increase the length a bit so we don't accidentally glitch through the center
        const float min_length = 10f;

        var lineDirection = (e.MousePosition - e.LastMousePosition).Normalized();
        float length = Math.Max(Vector2.Distance(e.LastMousePosition, e.MousePosition), min_length);

        var line = new Line(e.MousePosition - lineDirection * length, e.MousePosition);

        var center = DrawSize / 2;

        float radius = center.Y;

        if (new Line(center - line.OrthogonalDirection * radius, center + line.OrthogonalDirection * radius).TryIntersectWith(line, out _))
        {
            LastSliceStartPosition = ToScreenSpace(line.StartPoint);
            LastSliceEndPosition = ToScreenSpace(line.EndPoint);

            Hit();
        }

        return false;
    }

    private bool intersect(Vector2 start, Vector2 end, out Vector2 position)
    {
        float radius = (DrawWidth + DrawHeight) / 4;

        int intersections = findLineCircleIntersections(DrawWidth / 2, DrawHeight / 2, radius, start, end, out var intersection1, out var intersection2);

        if (intersections == 1)
        {
            position = intersection1;
            return true;
        }

        if (intersections == 2)
        {
            double dist1 = Vector2.Distance(intersection1, start);
            double dist2 = Vector2.Distance(intersection2, start);

            position = dist1 < dist2 ? intersection1 : intersection2;
            return true;
        }

        position = default;
        return false;
    }

    // Find the points of intersection.
    private int findLineCircleIntersections(float cx, float cy, float radius,
                                            Vector2 point1, Vector2 point2, out
                                                Vector2 intersection1, out Vector2 intersection2)
    {
        float dx = point2.X - point1.X;
        float dy = point2.Y - point1.Y;

        float a = dx * dx + dy * dy;
        float b = 2 * (dx * (point1.X - cx) + dy * (point1.Y - cy));
        float c = (point1.X - cx) * (point1.X - cx) + (point1.Y - cy) * (point1.Y - cy) - radius * radius;

        float det = b * b - 4 * a * c;

        if ((a <= 0.0000001) || (det < 0))
        {
            // No real solutions.
            intersection1 = new Vector2(float.NaN, float.NaN);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }
        else if (det == 0)
        {
            // One solution.
            float t = -b / (2 * a);
            intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
            intersection2 = new Vector2(float.NaN, float.NaN);
            return 1;
        }
        else
        {
            // Two solutions.
            float t = (float)((-b + Math.Sqrt(det)) / (2 * a));
            intersection1 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
            t = (float)((-b - Math.Sqrt(det)) / (2 * a));
            intersection2 = new Vector2(point1.X + t * dx, point1.Y + t * dy);
            return 2;
        }
    }

    public override bool HandleNonPositionalInput => true;
}
