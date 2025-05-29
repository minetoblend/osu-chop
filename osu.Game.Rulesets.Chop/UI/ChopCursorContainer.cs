using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Game.Extensions;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.UI;

public partial class ChopCursorContainer : GameplayCursorContainer
{
    private Vector2 latestPosition;
    private bool mouseDidMove;

    private ChopCursorPath path = null!;

    [Resolved]
    private GameHost host { get; set; } = null!;

    public override bool HandleNonPositionalInput => true;

    protected override Drawable CreateCursor() => new CircularContainer
    {
        Size = new Vector2(10),
        Origin = Anchor.Centre,
        Masking = true,
        BorderColour = Color4.White,
        BorderThickness = 3,
        EdgeEffect = new EdgeEffectParameters
        {
            Radius = 150,
            Colour = Color4.GreenYellow.Opacity(0.1f),
            Type = EdgeEffectType.Glow
        },
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Color4.GreenYellow,
        }
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(path = new ChopCursorPath
        {
            AccentColour = Color4.GreenYellow,
            Depth = 1,
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        ActiveCursor.ApplyGameWideClock(host);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        mouseDidMove |= Vector2.DistanceSquared(latestPosition, e.MousePosition) >= 5;
        latestPosition = e.MousePosition;

        return base.OnMouseMove(e);
    }

    protected override void Update()
    {
        base.Update();

        if (mouseDidMove)
        {
            path.AddVertex(latestPosition);
            mouseDidMove = false;
        }
    }
}
