using osuTK;

namespace osu.Game.Rulesets.Chop.Input;

public class SliceEvent : ChopEvent
{
    public SliceEvent(ChopInputState state, Vector2 lastPosition, Vector2 sliceStartPosition)
        : base(state, lastPosition, sliceStartPosition)
    {
    }
}
