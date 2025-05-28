using osuTK;

namespace osu.Game.Rulesets.Chop.Input;

public class SliceState
{
    public bool Active { get; set; }

    public Vector2 Position;

    public double LastUpdate;
}
