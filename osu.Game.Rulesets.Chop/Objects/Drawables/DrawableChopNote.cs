using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Chop.Skinning.Default;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Objects.Drawables;

public partial class DrawableChopNote : DrawableChopHitObject<ChopNote>
{
    private BufferedContainer mainContent = null!;
    private Container throwContainer = null!;
    private SkinnableDrawable approachCircle = null!;

    public DrawableChopNote(ChopNote hitObject)
        : base(hitObject)
    {
    }

    public DrawableChopNote()
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = ChopHitObject.OBJECT_DIMENSIONS;
        Origin = Anchor.Centre;

        AddRangeInternal([
            approachCircle = new SkinnableDrawable(new ChopSkinComponentLookup(ChopSkinComponents.ApproachCircle), _ => new DefaultApproachCircle())
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(0),
            },
            mainContent = new BufferedContainer(cachedFrameBuffer: true)
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0,
                AlwaysPresent = true,
                Children =
                [
                    new SkinnableDrawable(new ChopSkinComponentLookup(ChopSkinComponents.ChopNotePiece))
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                ],
            },
            throwContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children =
                [
                    mainContent.CreateView().With(d =>
                    {
                        d.RelativeSizeAxes = Axes.Both;
                    }),
                    new SliceReceptor
                    {
                        RelativeSizeAxes = Axes.Both,
                        CanHit = canHit,
                        Hit = () => UpdateResult(true),
                    }
                ]
            },
        ]);

        PositionBindable.BindValueChanged(position => Position = position.NewValue, true);
    }

    private bool canHit()
    {
        if (Judged)
            return false;

        var result = HitObject.HitWindows.ResultFor(Time.Current - HitObject.GetEndTime());

        return result != HitResult.Miss;
    }

    protected override void Update()
    {
        base.Update();

        float throwProgress = ThrowProgressAt(Time.Current);

        throwContainer.Position = ThrowPositionAt(throwProgress);
    }

    protected override void CheckForResult(bool userTriggered, double timeOffset)
    {
        if (!userTriggered)
        {
            if (!HitObject.HitWindows.CanBeHit(timeOffset))
            {
                ApplyMinResult();
            }

            return;
        }

        var result = ResultFor(timeOffset);

        if (result == HitResult.Miss)
            return;

        if (result == HitResult.None)
            return;

        ApplyResult(result);
    }

    protected virtual HitResult ResultFor(double timeOffset) => HitObject.HitWindows.ResultFor(timeOffset);

    protected override void UpdateInitialTransforms()
    {
        base.UpdateInitialTransforms();

        approachCircle
            .FadeInFromZero(HitObject.TimePreempt)
            .ScaleTo(1, HitObject.TimePreempt);
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        const double duration = 1000;

        switch (state)
        {
            case ArmedState.Hit:
                this.FadeOut(duration, Easing.OutQuint).Expire();

                approachCircle
                    .ScaleTo(1.5f, 240, Easing.Out)
                    .FadeOut(240, Easing.Out);
                break;

            case ArmedState.Miss:
                this.FadeColour(Color4.Red, duration);
                this.FadeOut(duration, Easing.InQuint).Expire();
                break;
        }
    }
}
