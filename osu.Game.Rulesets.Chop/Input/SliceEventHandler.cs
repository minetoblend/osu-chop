namespace osu.Game.Rulesets.Chop.Input;

public interface ISliceEventHandler
{
    public bool OnSlice(SliceEvent e);
    public bool OnSliceStarted(SliceStartEvent e);
    public bool OnSliceEnded(SliceEndEvent e);
}
