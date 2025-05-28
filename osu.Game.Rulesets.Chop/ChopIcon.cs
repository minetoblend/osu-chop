using osu.Framework.Allocation;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Chop;

public partial class ChopIcon(Ruleset ruleset) : Sprite
{
    [BackgroundDependencyLoader]
    private void load(IRenderer renderer)
    {
        Texture = new TextureStore(renderer, new TextureLoaderStore(ruleset.CreateResourceStore()), false).Get("Textures/chop-icon");
    }
}
