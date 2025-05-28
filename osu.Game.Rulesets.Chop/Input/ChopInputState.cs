using osu.Framework.Input.States;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Chop.Input;

public class ChopInputState : RulesetInputManagerInputState<ChopAction>
{
    public ChopInputState(InputState state)
        : base(state)
    {
    }

    public readonly SliceState Slice = new SliceState();
}
