// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Chop.Scoring;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Chop.Objects
{
    public class ChopHitObject : HitObject, IHasPosition, IHasComboInformation
    {
        /// <summary>
        /// The radius of hit objects (i.e. the radius of a <see cref="ChopNote"/>).
        /// </summary>
        public const float OBJECT_RADIUS = 32;

        /// <summary>
        /// The width and height any element participating in display of a chop note (or similarly sized object) should be.
        /// </summary>
        public static readonly Vector2 OBJECT_DIMENSIONS = new Vector2(OBJECT_RADIUS * 2);

        /// <summary>
        /// Minimum preempt time at AR=10.
        /// </summary>
        public const double PREEMPT_MIN = 450;

        /// <summary>
        /// Median preempt time at AR=5.
        /// </summary>
        public const double PREEMPT_MID = 1200;

        /// <summary>
        /// Maximum preempt time at AR=0.
        /// </summary>
        public const double PREEMPT_MAX = 1800;

        public override Judgement CreateJudgement() => new Judgement();

        public readonly Bindable<Vector2> PositionBindable = new Bindable<Vector2>();

        public Vector2 Position
        {
            get => PositionBindable.Value;
            set => PositionBindable.Value = value;
        }

        public float X
        {
            get => Position.X;
            set => Position = new Vector2(value, Y);
        }

        public float Y
        {
            get => Position.Y;
            set => Position = new Vector2(X, value);
        }

        public readonly Bindable<float> ThrowOffsetBindable = new Bindable<float>();

        public float ThrowOffset
        {
            get => ThrowOffsetBindable.Value;
            set => ThrowOffsetBindable.Value = value;
        }

        public double TimePreempt { get; private set; }

        protected override void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, IBeatmapDifficultyInfo difficulty)
        {
            base.ApplyDefaultsToSelf(controlPointInfo, difficulty);

            TimePreempt = (float)IBeatmapDifficultyInfo.DifficultyRange(difficulty.ApproachRate, PREEMPT_MAX, PREEMPT_MID, PREEMPT_MIN);
        }

        protected override HitWindows CreateHitWindows() => new ChopHitWindows();

        #region IHasComboInformation

        public Bindable<int> IndexInCurrentComboBindable { get; } = new Bindable<int>();

        public int IndexInCurrentCombo
        {
            get => IndexInCurrentComboBindable.Value;
            set => IndexInCurrentComboBindable.Value = value;
        }

        public Bindable<int> ComboIndexBindable { get; } = new Bindable<int>();

        public int ComboIndex
        {
            get => ComboIndexBindable.Value;
            set => ComboIndexBindable.Value = value;
        }

        public Bindable<int> ComboIndexWithOffsetsBindable { get; } = new Bindable<int>();

        public int ComboIndexWithOffsets
        {
            get => ComboIndexWithOffsetsBindable.Value;
            set => ComboIndexWithOffsetsBindable.Value = value;
        }

        public int ComboOffset { get; set; }

        public Bindable<bool> LastInComboBindable { get; } = new Bindable<bool>();

        public bool LastInCombo { get; set; }

        public bool NewCombo { get; set; }

        public void UpdateComboInformation(IHasComboInformation? lastObj)
        {
            // Note that this implementation is shared with the osu!catch ruleset's implementation.
            // If a change is made here, CatchHitObject.cs should also be updated.
            int index = lastObj?.ComboIndex ?? 0;
            int indexWithOffsets = lastObj?.ComboIndexWithOffsets ?? 0;
            int inCurrentCombo = (lastObj?.IndexInCurrentCombo + 1) ?? 0;

            // - For the purpose of combo colours, spinners never start a new combo even if they are flagged as doing so.
            // - At decode time, the first hitobject in the beatmap and the first hitobject after a spinner are both enforced to be a new combo,
            //   but this isn't directly enforced by the editor so the extra checks against the last hitobject are duplicated here.
            if (NewCombo || lastObj == null)
            {
                inCurrentCombo = 0;
                index++;
                indexWithOffsets += ComboOffset + 1;

                if (lastObj != null)
                    lastObj.LastInCombo = true;
            }

            ComboIndex = index;
            ComboIndexWithOffsets = indexWithOffsets;
            IndexInCurrentCombo = inCurrentCombo;
        }

        #endregion
    }
}
