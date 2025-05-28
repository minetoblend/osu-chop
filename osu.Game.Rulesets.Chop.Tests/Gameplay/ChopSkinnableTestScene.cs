using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Chop.Input;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Chop.Tests.Gameplay;

public abstract partial class ChopSkinnableTestScene : SkinnableTestScene
{
    private Container content;

    protected override Container<Drawable> Content
    {
        get
        {
            if (content == null)
                base.Content.Add(content = new ChopInputManager(new ChopRuleset().RulesetInfo));

            return content;
        }
    }

    protected override Ruleset CreateRulesetForSkinProvider() => new ChopRuleset();
}
