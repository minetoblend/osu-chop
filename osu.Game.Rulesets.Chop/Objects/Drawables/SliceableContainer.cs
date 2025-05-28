using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osuTK;

namespace osu.Game.Rulesets.Chop.Objects.Drawables;

public partial class SliceableContainer : Container
{
    private readonly BufferedContainer mainContent;
    private readonly Container slicedPieces;

    protected override Container<Drawable> Content { get; }

    public SliceableContainer()
    {
        InternalChildren =
        [
            mainContent = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true,
                Child = Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                }
            },
            slicedPieces = new Container
            {
                RelativeSizeAxes = Axes.Both,
            }
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        slicedPieces.Child = createView();
    }

    public override void ClearTransformsAfter(double time, bool propagateChildren = false, string targetMember = null)
    {
    }

    public override void FinishTransforms(bool propagateChildren = false, string targetMember = null)
    {
    }

    public bool Slice(Vector2 startPosition, Vector2 endPosition)
    {
        var line = new Line(startPosition, endPosition);

        float angle = line.Theta * 180f / MathF.PI + 90;

        var center = ScreenSpaceDrawQuad.Centre;
        float radius = ScreenSpaceDrawQuad.Height / 2;

        if (!new Line(center - line.OrthogonalDirection * radius, center + line.OrthogonalDirection * radius).TryIntersectWith(line, out float distance))
            return false;

        float slicePosition = Math.Clamp(distance, 0.15f, 0.85f);

        slicedPieces.Children =
        [
            new SlicedView
            {
                RelativeSizeAxes = Axes.Both,
                SliceAngle = angle,
                Child = createView(),
                SlicePosition = slicePosition,
            },
            new SlicedView
            {
                RelativeSizeAxes = Axes.Both,
                SliceAngle = angle + 180,
                Child = createView(),
                SlicePosition = 1 - slicePosition,
            }
        ];

        float velocity = 0.5f + Random.Shared.NextSingle() * 0.5f;

        slicedPieces.Children[0].MoveTo((line.OrthogonalDirection * -100 + line.DirectionNormalized * 50) * velocity, 1000, Easing.Out);
        slicedPieces.Children[1].MoveTo((line.OrthogonalDirection * 100 + line.DirectionNormalized * 50) * velocity, 1000, Easing.Out);

        return true;
    }

    public void Reset()
    {
        slicedPieces.Child = createView();
    }

    private Drawable createView() => mainContent.CreateView().With(d => d.RelativeSizeAxes = Axes.Both);

    public partial class SlicedView : Container
    {
        private readonly Container rotationContainer;
        private readonly Container scaleContainer;
        private readonly Container inverseScaleContainer;
        private readonly Container content;

        protected override Container<Drawable> Content => content;

        private float sliceAngle;

        public float SliceAngle
        {
            get => sliceAngle;
            set
            {
                sliceAngle = value;

                rotationContainer.Rotation = value;
                content.Rotation = -value;
            }
        }

        public float SlicePosition
        {
            get => scaleContainer.Width;
            set
            {
                scaleContainer.Width = value;
                inverseScaleContainer.Width = 1 / value;
            }
        }

        public SlicedView()
        {
            InternalChild = rotationContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Child = scaleContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Child = inverseScaleContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = content = new Container
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                }
            };
        }
    }
}
