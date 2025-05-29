// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Chop.Judgements;
using osu.Game.Rulesets.Chop.UI;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Chop.Objects.Drawables
{
    public partial class DrawableChopHitObject : DrawableHitObject<ChopHitObject>
    {
        protected readonly Bindable<Vector2> PositionBindable = new Bindable<Vector2>();
        protected readonly Bindable<float> ThrowOffsetBindable = new Bindable<float>();

        public HitPolicy HitPolicy { get; set; } = null!;

        public DrawableChopHitObject(ChopHitObject? hitObject)
            : base(hitObject!)
        {
        }

        protected override void OnApply()
        {
            base.OnApply();

            PositionBindable.BindTo(HitObject.PositionBindable);
            ThrowOffsetBindable.BindTo(HitObject.ThrowOffsetBindable);
        }

        protected override void OnFree()
        {
            base.OnFree();

            PositionBindable.UnbindFrom(HitObject.PositionBindable);
            ThrowOffsetBindable.UnbindFrom(HitObject.ThrowOffsetBindable);
        }

        protected float ThrowProgressAt(double time)
        {
            double throwDuration = HitObject.TimePreempt * 2;

            return (float)((time - (HitObject.StartTime - HitObject.TimePreempt)) / throwDuration);
        }

        protected Vector2 ThrowPositionAt(float progress)
        {
            float xOffset = ThrowOffsetBindable.Value * (progress - 0.5f) * 2;
            float yOffset = ChopPlayfield.BASE_SIZE.Y * ((progress - 0.5f) * (progress - 0.5f) * 4);

            return new Vector2(xOffset, yOffset);
        }

        protected override JudgementResult CreateResult(Judgement judgement) => new ChopJudgementResult(HitObject, judgement);

        public void MissForcefully() => ApplyMinResult();

        protected override double InitialLifetimeOffset => HitObject.TimePreempt;
    }

    public partial class DrawableChopHitObject<T> : DrawableChopHitObject
        where T : ChopHitObject
    {
        public DrawableChopHitObject(T hitObject)
            : base(hitObject)
        {
        }

        public DrawableChopHitObject()
            : base(null)
        {
        }
    }
}
