// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Chop.Input;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Chop.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Chop.UI
{
    [Cached]
    public partial class DrawableChopRuleset : DrawableRuleset<ChopHitObject>
    {
        public DrawableChopRuleset(ChopRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod>? mods = null)
            : base(ruleset, beatmap, mods)
        {
        }

        protected override Playfield CreatePlayfield() => new ChopPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new ChopFramedReplayInputHandler(replay);

        public override DrawableHitObject<ChopHitObject>? CreateDrawableRepresentation(ChopHitObject h) => null;

        protected override PassThroughInputManager CreateInputManager() => new ChopInputManager(Ruleset?.RulesetInfo);

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new ChopPlayfieldAdjustmentContainer();

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;
    }
}
