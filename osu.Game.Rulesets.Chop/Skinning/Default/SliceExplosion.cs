using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Chop.Skinning.Default;

public partial class SliceExplosion : CompositeDrawable
{
    private Sprite sprite = null!;

    [Resolved]
    private ISkinSource skin { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddInternal(sprite = new Sprite
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(0.75f),
            Texture = skin.GetTexture("Gameplay/slice-explosion"),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Blending = BlendingParameters.Additive
        });
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        sprite.ScaleTo(0)
              .ScaleTo(new Vector2(1f, 1.25f), 25)
              .Then()
              .ScaleTo(new Vector2(1))
              .ScaleTo(new Vector2(5, 0.25f), 100, Easing.Out)
              .Then()
              .FadeOut(20);

        this.Delay(300).FadeOut().Expire();
    }
}
