// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;
using osuTK;

namespace osu.Game.Rulesets.Chop.Replays
{
    public class ChopReplayFrame : ReplayFrame
    {
        public List<ChopAction> Actions = new List<ChopAction>();
        public Vector2 Position;

        public ChopReplayFrame(ChopAction? button = null)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }
    }
}
