namespace osu.Game.Rulesets.Chop.Objects.Drawables;

public partial class DrawableChopNote : DrawableChopHitObject<ChopNote>
{
    public DrawableChopNote(ChopNote hitObject)
        : base(hitObject)
    {
    }

    public DrawableChopNote()
    {
    }

    protected override void Update()
    {
        base.Update();

        float throwProgress = ThrowProgressAt(Time.Current);

        Position = ThrowPositionAt(throwProgress);
    }
}
