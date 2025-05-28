using osu.Framework.Input.Events;
using osuTK;

namespace osu.Game.Rulesets.Chop.Input;

public class ChopEvent : UIEvent
{
    public new ChopInputState CurrentState => (ChopInputState)base.CurrentState;

    public new Vector2 MousePosition => Target!.ToLocalSpace(CurrentState.Slice.Position);

    public readonly Vector2 ScreenSpaceLastMousePosition;

    public readonly Vector2 ScreenSpaceSliceStartPosition;

    public Vector2 LastMousePosition => Target!.ToLocalSpace(ScreenSpaceLastMousePosition);

    public Vector2 SliceStartPosition => Target!.ToLocalSpace(ScreenSpaceSliceStartPosition);

    public ChopEvent(ChopInputState state, Vector2 lastPosition, Vector2 sliceStartPosition)
        : base(state)
    {
        ScreenSpaceLastMousePosition = lastPosition;
        ScreenSpaceSliceStartPosition = sliceStartPosition;
    }
}
