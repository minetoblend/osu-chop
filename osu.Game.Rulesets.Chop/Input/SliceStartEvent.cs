﻿using osuTK;

namespace osu.Game.Rulesets.Chop.Input;

public class SliceStartEvent : ChopEvent
{
    public SliceStartEvent(ChopInputState state, Vector2 lastPosition, Vector2 sliceStartPosition)
        : base(state, lastPosition, sliceStartPosition)
    {
    }
}
