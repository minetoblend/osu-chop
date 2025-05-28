// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Chop.Replays
{
    public class ChopAutoGenerator : AutoGenerator<ChopReplayFrame>
    {
        public new Beatmap<ChopHitObject> Beatmap => (Beatmap<ChopHitObject>)base.Beatmap;

        public ChopAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected override void GenerateFrames()
        {
            Frames.Add(new ChopReplayFrame());

            foreach (ChopHitObject hitObject in Beatmap.HitObjects)
            {
                Frames.Add(new ChopReplayFrame
                {
                    Time = hitObject.StartTime,
                    Position = hitObject.Position,
                    // todo: add required inputs and extra frames.
                });
            }
        }
    }
}
