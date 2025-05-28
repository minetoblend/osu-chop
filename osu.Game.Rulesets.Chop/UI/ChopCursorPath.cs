using System.Collections.Generic;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Rulesets.Chop.Graphics;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.UI;

public partial class ChopCursorPath : TexturedStroke
{
    private const double vertex_ttl = 150;

    private readonly List<Vector2> vertices = new List<Vector2>();
    private readonly List<double> insertionTimes = new List<double>();

    private bool active = true;

    public Color4 AccentColour { get; set; } = Color4.Gray;

    public ChopCursorPath()
    {
        AutoSizeAxes = Axes.Both;
        PathRadius = 20;
    }

    public void AddVertex(Vector2 position)
    {
        vertices.Add(position);
        insertionTimes.Add(Time.Current);

        Vertices = vertices;
    }

    public void UpdateCursorPosition(Vector2 position)
    {
        Position = position;

        if (Vertices.Count == 0)
            return;

        ReplaceVertex(Vertices.Count - 1, position);
    }

    public void OnStrokeEnded()
    {
        active = false;
        LifetimeEnd = Time.Current + vertex_ttl;
    }

    protected override void Update()
    {
        base.Update();

        bool changed = false;

        for (int i = 0; i < vertices.Count; i++)
        {
            if (Time.Current >= insertionTimes[i] + vertex_ttl)
            {
                changed = true;

                insertionTimes.RemoveAt(i);
                vertices.RemoveAt(i);
                i--;
            }
            else
            {
                break;
            }
        }

        if (changed)
            Vertices = vertices;

        if (active && Vertices.Count > 0)
            OriginPosition = PositionInBoundingBox(Vertices[^1]);
    }

    protected override Color4 ColourAt(float position)
    {
        if (position < 0.1f)
            return Color4.Transparent;

        if (position < 0.3f)
            return Color4.White;

        return Interpolation.ValueAt(position, AccentColour, AccentColour.Lighten(2), 0.3f, 1.5f);
    }

    protected override float ThicknessAt(float progress)
    {
        const float center_point = 0.85f;

        if (progress < center_point)
            return Interpolation.ValueAt(progress, 0f, 1f, 0, center_point);

        return Interpolation.ValueAt(progress, 1f, 0f, center_point, 1, Easing.InCirc);
    }
}
