using System;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Game.Rulesets.Chop.Input;
using Vector2 = osuTK.Vector2;

namespace osu.Game.Rulesets.Chop.Objects.Drawables;

public partial class SliceReceptor : CompositeDrawable, ISliceEventHandler
{
    public required Func<bool> CanHit { get; init; }

    public required Action Hit { get; init; }

    public bool OnSlice(SliceEvent e)
    {
        var start = e.LastMousePosition;
        var end = e.MousePosition;

        if (intersect(e.LastMousePosition, e.MousePosition, out var position))
        {
            if (new Line(start, end).DistanceToPoint(position) > 1)
                return false;

            if (!CanHit())
                return false;

            Hit();
            return true;
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

    public bool OnSliceStarted(SliceStartEvent e)
    {
        return false;
    }

    public bool OnSliceEnded(SliceEndEvent e)
    {
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
