using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Skinning.Default;

public partial class DefaultChopNotePiece : CompositeDrawable
{
    private readonly Bindable<Color4> accentColour = new Bindable<Color4>(Color4.Black);

    [BackgroundDependencyLoader]
    private void load(DrawableHitObject? drawableObject)
    {
        Size = ChopHitObject.OBJECT_DIMENSIONS;

        AddInternal(new Circle
        {
            RelativeSizeAxes = Axes.Both,
        });

        if (drawableObject != null)
            accentColour.BindTo(drawableObject.AccentColour);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        accentColour.BindValueChanged(colour => Colour = colour.NewValue, true);
    }
}
