using osu.Framework.Input.Events;
using osuTK;

namespace osu.Game.Rulesets.Chop.Input;

public class ChopEvent : UIEvent
{
    public new ChopInputState CurrentState => (ChopInputState)base.CurrentState;

    public new Vector2 MousePosition => CurrentState.Slice.Position;

    public readonly Vector2 LastMousePosition;

    public readonly Vector2 SliceStartPosition;

    public ChopEvent(ChopInputState state, Vector2 lastPosition, Vector2 sliceStartPosition)
        : base(state)
    {
        LastMousePosition = lastPosition;
        SliceStartPosition = sliceStartPosition;
    }
}
