using osu.Framework.Graphics;
using osu.Game.Rulesets.Chop.Skinning.Default;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Chop.Skinning;

public class ChopSkinTransformer : SkinTransformer
{
    public ChopSkinTransformer(ISkin skin)
        : base(skin)
    {
    }

    public override Drawable? GetDrawableComponent(ISkinComponentLookup lookup)
    {
        if (lookup is ChopSkinComponentLookup chopLookup)
        {
            switch (chopLookup.Component)
            {
                case ChopSkinComponents.ChopNotePiece:
                    return new DefaultChopNotePiece();
            }
        }

        return base.GetDrawableComponent(lookup);
    }
}
