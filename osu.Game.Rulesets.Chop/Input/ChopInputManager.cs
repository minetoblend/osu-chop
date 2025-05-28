// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.StateChanges.Events;
using osu.Framework.Input.States;
using osu.Framework.Utils;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Chop.Input
{
    public partial class ChopInputManager : RulesetInputManager<ChopAction>
    {
        private const double update_interval = 1000 / 120.0;
        private const float slice_velocity_threshold = 15f;

        private Vector2 sliceStartPosition;
        private Vector2 currentPosition;
        private bool sliceActive;

        public new ChopInputState CurrentState => (ChopInputState)base.CurrentState;

        public ChopInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }

        protected override InputState CreateInitialState() => new ChopInputState(base.CreateInitialState());

        protected override void HandleMousePositionChange(MousePositionChangeEvent e)
        {
            base.HandleMousePositionChange(e);

            currentPosition = ToLocalSpace(CurrentState.Mouse.Position);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            CurrentState.Slice.Position = ToLocalSpace(CurrentState.Mouse.Position);
            CurrentState.Slice.LastUpdate = Time.Current;
        }

        protected override void Update()
        {
            base.Update();

            var state = CurrentState.Slice;

            while (state.LastUpdate + update_interval < Time.Current)
            {
                var lastPosition = state.Position;

                state.Position = Interpolation.ValueAt(state.LastUpdate + update_interval, state.Position, currentPosition, state.LastUpdate, Time.Current);

                state.LastUpdate += update_interval;

                var delta = state.Position - lastPosition;

                float velocity = delta.Length;

                if (velocity > slice_velocity_threshold)
                {
                    if (!sliceActive)
                    {
                        sliceActive = true;
                        sliceStartPosition = currentPosition;
                        PropagateEvent(new SliceStartEvent(CurrentState, lastPosition, sliceStartPosition));
                    }

                    PropagateEvent(new SliceEvent(CurrentState, lastPosition, sliceStartPosition));
                }
                else if (sliceActive)
                {
                    sliceActive = false;
                    PropagateEvent(new SliceEndEvent(CurrentState, lastPosition, sliceStartPosition));
                }
            }
        }

        protected Drawable? PropagateEvent(ChopEvent e)
        {
            foreach (var d in NonPositionalInputQueue)
            {
                if (d is not ISliceEventHandler handler)
                    continue;

                switch (e)
                {
                    case SliceEvent sliceEvent:
                        if (handler.OnSlice(sliceEvent))
                            return d;

                        break;

                    case SliceStartEvent sliceStartEvent:
                        if (handler.OnSliceStarted(sliceStartEvent))
                            return d;

                        break;

                    case SliceEndEvent sliceEndEvent:
                        if (handler.OnSliceEnded(sliceEndEvent))
                            return d;

                        break;
                }
            }

            return null;
        }
    }

    public enum ChopAction { }
}
