using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Skinning.Default;

public partial class DefaultApproachCircle : CompositeDrawable
{
    public DefaultApproachCircle()
    {
        RelativeSizeAxes = Axes.Both;
        AddInternal(new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            BorderColour = Color4.White,
            BorderThickness = 4,
            Child = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true,
            }
        });
    }
}
