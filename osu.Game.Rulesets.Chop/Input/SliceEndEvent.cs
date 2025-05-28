using osuTK;

namespace osu.Game.Rulesets.Chop.Input;

public class SliceEndEvent : ChopEvent
{
    public SliceEndEvent(ChopInputState state, Vector2 lastPosition, Vector2 sliceStartPosition)
        : base(state, lastPosition, sliceStartPosition)
    {
    }
}
