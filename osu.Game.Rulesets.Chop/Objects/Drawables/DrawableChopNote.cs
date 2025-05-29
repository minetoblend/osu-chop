using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Chop.Judgements;
using osu.Game.Rulesets.Chop.Skinning.Default;
using osu.Game.Rulesets.Chop.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Objects.Drawables;

public partial class DrawableChopNote : DrawableChopHitObject<ChopNote>
{
    private SliceableContainer sliceContainer = null!;
    private Container throwContainer = null!;
    private SkinnableDrawable approachCircle = null!;
    private SliceReceptor sliceReceptor = null!;

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
            throwContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children =
                [
                    sliceContainer = new SliceableContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(2),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Child = new Container
                        {
                            Size = ChopHitObject.OBJECT_DIMENSIONS,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children =
                            [
                                new SkinnableDrawable(new ChopSkinComponentLookup(ChopSkinComponents.ChopNotePiece))
                                {
                                    RelativeSizeAxes = Axes.Both,
                                }
                            ]
                        }
                    },
                    sliceReceptor = new SliceReceptor
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
        var action = HitPolicy.CheckHittable(this, Time.Current, result);

        if (result == HitResult.Miss || action != ClickAction.Hit)
            return;

        if (result == HitResult.None)
            return;

        if (!sliceContainer.Slice(sliceReceptor.LastSliceStartPosition, sliceReceptor.LastSliceEndPosition))
            return;

        var slicePosition = ToLocalSpace(sliceReceptor.LastSliceEndPosition);
        var sliceDirection = slicePosition - ToLocalSpace(sliceReceptor.LastSliceEndPosition);

        ApplyResult<(HitResult result, Vector2 position, Vector2 direction)>(static (r, state) =>
        {
            var chopResult = (ChopJudgementResult)r;

            chopResult.Type = state.result;
            chopResult.SlicePosition = state.position;
            chopResult.SliceDirection = state.direction;
        }, (result, slicePosition, sliceDirection));
    }

    protected virtual HitResult ResultFor(double timeOffset) => HitObject.HitWindows.ResultFor(timeOffset);

    protected override void UpdateInitialTransforms()
    {
        base.UpdateInitialTransforms();

        approachCircle
            .FadeOut()
            .FadeTo(0.5f, HitObject.TimePreempt)
            .ScaleTo(1, HitObject.TimePreempt, Easing.Out);
    }

    protected override void UpdateHitStateTransforms(ArmedState state)
    {
        const double duration = 1000;

        switch (state)
        {
            case ArmedState.Idle:
                sliceContainer.Reset();
                break;

            case ArmedState.Hit:
                this.FadeOut(duration, Easing.OutQuint).Expire();

                approachCircle
                    .ScaleTo(1.5f, 240, Easing.Out)
                    .FadeOut(240, Easing.Out);
                break;

            case ArmedState.Miss:
                sliceContainer.Reset();
                this.FadeColour(Color4.Red, 500);
                this.FadeOut(duration, Easing.InQuint).Expire();

                approachCircle
                    .MoveToY(50, 300, Easing.InQuad)
                    .FadeColour(Color4.Red, 300)
                    .ScaleTo(0.7f, 300, Easing.In)
                    .FadeOut(300);

                break;
        }
    }
}
