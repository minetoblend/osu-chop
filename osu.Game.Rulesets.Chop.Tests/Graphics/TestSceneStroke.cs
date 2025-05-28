using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osu.Game.Rulesets.Chop.Graphics;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Tests.Graphics;

public partial class TestSceneStroke : OsuTestScene
{
    private TestStroke stroke = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        List<Vector2> vertices = new List<Vector2>();

        for (int i = 0; i < 50; i++)
        {
            vertices.Add(new Vector2(i * 10, 100 + MathF.Cos(i * 0.3f) * 50f));
        }

        Add(stroke = new TestStroke
        {
            RelativeSizeAxes = Axes.Both,
            PathRadius = 25f,
            Vertices = vertices
        });
    }

    protected override void Update()
    {
        base.Update();

        stroke.Position = stroke.Vertices[0];
        stroke.OriginPosition = stroke.PositionInBoundingBox(stroke.Vertices[0]);
    }

    [Test]
    public void TestStrokeDrawable()
    {
        AddSliderStep("radius", 0f, 50f, 25f, value => stroke.PathRadius = value);
    }

    private partial class TestStroke : TexturedStroke
    {
        protected override Color4 ColourAt(float position)
        {
            if (position < 0.1f)
                return Color4.Transparent;

            if (position < 0.3f)
                return Color4.White;

            return Interpolation.ValueAt(position, Color4.Gray, Color4.White, 0.3f, 1.5f);
        }

        protected override float ThicknessAt(float progress)
        {
            return progress < 0.5f
                ? Interpolation.ValueAt(progress, 0f, 1f, 0, 0.5)
                : Interpolation.ValueAt(progress, 1f, 0f, 0.5, 1);
        }
    }
}
