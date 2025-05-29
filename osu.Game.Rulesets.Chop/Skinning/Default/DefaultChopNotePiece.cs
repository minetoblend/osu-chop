using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Skinning.Default;

public partial class DefaultChopNotePiece : CompositeDrawable
{
    private readonly Bindable<Color4> accentColour = new Bindable<Color4>(Color4.Black);

    private Circle circlePiece = null!;

    [Resolved]
    private DrawableHitObject? drawableObject { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = ChopHitObject.OBJECT_DIMENSIONS;

        InternalChildren =
        [
            circlePiece = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                BorderColour = ColourInfo.GradientVertical(Color4.White, Color4.Gray),
                BorderThickness = 6,
            },
        ];

        if (drawableObject != null)
            accentColour.BindTo(drawableObject.AccentColour);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        accentColour.BindValueChanged(colour =>
        {
            circlePiece.Child.Colour = ColourInfo.GradientVertical(colour.NewValue.Lighten(0.5f), colour.NewValue);
        }, true);
    }
}
