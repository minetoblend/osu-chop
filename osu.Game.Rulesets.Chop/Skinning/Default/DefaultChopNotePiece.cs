using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Skinning.Default;

public partial class DefaultChopNotePiece : CompositeDrawable
{
    private readonly Bindable<Color4> accentColour = new Bindable<Color4>(Color4.Black);

    private Circle circlePiece = null!;
    private Circle explodePiece = null!;

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
            new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.2f),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Colour = Color4.LightGray
            },
            explodePiece = new Circle
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Alpha = 0,
                Blending = BlendingParameters.Additive
            }
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

        if (drawableObject != null)
        {
            drawableObject.ApplyCustomUpdateState += applyStateTransforms;

            applyStateTransforms(drawableObject, drawableObject.State.Value);
        }
    }

    private void applyStateTransforms(DrawableHitObject drawableObject, ArmedState state)
    {
        switch (state)
        {
            case ArmedState.Hit:
                explodePiece.FadeTo(0.5f, 50)
                            .Then()
                            .FadeOut(600, Easing.Out);

                explodePiece.ScaleTo(1.3f, 600, Easing.Out);
                break;
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (drawableObject != null)
            drawableObject.ApplyCustomUpdateState -= applyStateTransforms;
    }
}
