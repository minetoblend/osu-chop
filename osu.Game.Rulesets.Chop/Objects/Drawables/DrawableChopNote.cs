using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK;

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

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(60);
        Origin = Anchor.Centre;
        AddInternal(new Circle
        {
            RelativeSizeAxes = Axes.Both,
        });
    }

    protected override void Update()
    {
        base.Update();

        float throwProgress = ThrowProgressAt(Time.Current);

        Position = ThrowPositionAt(throwProgress);
    }

    protected override double InitialLifetimeOffset => HitObject.TimePreempt;
}
