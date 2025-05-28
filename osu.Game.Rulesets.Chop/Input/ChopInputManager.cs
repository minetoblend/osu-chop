// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Input.States;
using osu.Game.Rulesets.Chop.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Chop.Input
{
    [Cached]
    public partial class ChopInputManager : RulesetInputManager<ChopAction>
    {
        public ChopPlayfield Playfield = null!;

        private const double update_interval = 1000 / 200.0;

        private Vector2 sliceStartPosition;
        private Vector2 currentPosition;
        private bool sliceActive;
        private double lastSliceTime;

        public new ChopInputState CurrentState => (ChopInputState)base.CurrentState;

        public ChopInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }

        protected override InputState CreateInitialState() => new ChopInputState(base.CreateInitialState());

        protected override void HandleMousePositionChange(MousePositionChangeEvent e)
        {
            base.HandleMousePositionChange(e);

            currentPosition = CurrentState.Mouse.Position;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            CurrentState.Slice.Position = CurrentState.Mouse.Position;
            CurrentState.Slice.LastUpdate = Time.Current;
        }

        protected override void Update()
        {
            base.Update();

            const float slice_velocity_threshold = 5f;

            var state = CurrentState.Slice;

            var lastPosition = state.Position;

            state.Position = currentPosition;

            state.LastUpdate += update_interval;

            var delta = Playfield.ToLocalSpace(state.Position) - Playfield.ToLocalSpace(lastPosition);

            float velocity = delta.Length / (float)Time.Elapsed;

            if (velocity > slice_velocity_threshold)
            {
                if (!sliceActive)
                {
                    sliceActive = true;
                    sliceStartPosition = currentPosition;
                    PropagateEvent(new SliceStartEvent(CurrentState, lastPosition, sliceStartPosition));
                }

                PropagateEvent(new SliceEvent(CurrentState, lastPosition, sliceStartPosition));

                lastSliceTime = Time.Current;
            }
            else if (sliceActive && Time.Current > lastSliceTime + 40)
            {
                sliceActive = false;
                PropagateEvent(new SliceEndEvent(CurrentState, lastPosition, sliceStartPosition));
            }
        }

        protected Drawable? PropagateEvent(ChopEvent e)
        {
            foreach (var drawable in NonPositionalInputQueue)
            {
                if (drawable is not ISliceEventHandler handler)
                    continue;

                e.Target = drawable;

                switch (e)
                {
                    case SliceEvent sliceEvent:
                        if (handler.OnSlice(sliceEvent))
                            return drawable;

                        break;

                    case SliceStartEvent sliceStartEvent:
                        if (handler.OnSliceStarted(sliceStartEvent))
                            return drawable;

                        break;

                    case SliceEndEvent sliceEndEvent:
                        if (handler.OnSliceEnded(sliceEndEvent))
                            return drawable;

                        break;
                }
            }

            return null;
        }
    }

    public enum ChopAction { }
}
