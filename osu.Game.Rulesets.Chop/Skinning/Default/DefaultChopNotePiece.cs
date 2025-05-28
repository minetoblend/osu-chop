using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Chop.Objects;

namespace osu.Game.Rulesets.Chop.Skinning.Default;

public partial class DefaultChopNotePiece : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Size = ChopHitObject.OBJECT_DIMENSIONS;

        AddInternal(new Circle
        {
            RelativeSizeAxes = Axes.Both,
        });
    }
}
