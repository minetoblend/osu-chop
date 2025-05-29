﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Rulesets.Chop.Input;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Chop.Objects.Drawables;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Chop.UI
{
    [Cached]
    public partial class ChopPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(512, 384);

        [Resolved]
        private ChopInputManager inputManager { get; set; } = null!;

        private HitPolicy hitPolicy = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            inputManager.Playfield = this;

            AddRangeInternal([
                HitObjectContainer,
                hitPolicy = new HitPolicy
                {
                    HitObjectContainer = HitObjectContainer,
                }
            ]);

            RegisterPool<ChopNote, DrawableChopNote>(20, 100);

            NewResult += onNewResult;
        }

        protected override GameplayCursorContainer CreateCursor() => new ChopCursorContainer();

        protected override HitObjectLifetimeEntry CreateLifetimeEntry(HitObject hitObject) => new ChopLifetimeEntry(hitObject);

        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            if (drawableHitObject is DrawableChopHitObject chopHitObject)
                chopHitObject.HitPolicy = hitPolicy;
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            hitPolicy.HandleHit(judgedObject);
        }

        private class ChopLifetimeEntry : HitObjectLifetimeEntry
        {
            public ChopLifetimeEntry(HitObject hitObject)
                : base(hitObject)
            {
            }

            protected override double InitialLifetimeOffset => ((ChopHitObject)HitObject).TimePreempt;
        }
    }
}
