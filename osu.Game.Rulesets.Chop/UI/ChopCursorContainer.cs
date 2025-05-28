using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osu.Game.Extensions;
using osu.Game.Rulesets.Chop.Input;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.UI;

public partial class ChopCursorContainer : GameplayCursorContainer, ISliceEventHandler
{
    private Vector2 position;

    private ChopCursorPath? currentPath;

    [Resolved]
    private GameHost host { get; set; } = null!;

    public override bool HandleNonPositionalInput => true;

    protected override Drawable CreateCursor() => new CircularContainer
    {
        Size = new Vector2(20),
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

    public bool OnSlice(SliceEvent e)
    {
        if (Precision.AlmostEquals(e.MousePosition, e.LastMousePosition))
            return false;

        currentPath?.AddVertex(e.MousePosition);

        return false;
    }

    public bool OnSliceStarted(SliceStartEvent e)
    {
        ActiveCursor.Hide();

        Add(currentPath = new ChopCursorPath
        {
            AccentColour = Color4.GreenYellow,
            Depth = 1,
            Vertices = [e.LastMousePosition]
        });

        currentPath.ApplyGameWideClock(host);

        currentPath.AddVertex(e.MousePosition);

        return false;
    }

    public bool OnSliceEnded(SliceEndEvent e)
    {
        if (currentPath == null)
            return false;

        ActiveCursor.Show();

        currentPath.AddVertex(e.MousePosition);
        currentPath.OnStrokeEnded();
        currentPath = null;

        return false;
    }
}
