// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Chop.Beatmaps
{
    public class ChopBeatmapConverter : BeatmapConverter<ChopHitObject>
    {
        public ChopBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => Beatmap.HitObjects.All(h => h is IHasPosition && h is IHasCombo);

        protected override IEnumerable<ChopHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap, CancellationToken cancellationToken)
        {
            yield return new ChopNote
            {
                Samples = original.Samples,
                StartTime = original.StartTime,
                Position = ((original as IHasPosition)?.Position ?? Vector2.Zero) * new Vector2(1, 0.5f),
                ThrowOffset = (Random.Shared.NextSingle() - 0.5f) * 200,
                NewCombo = (original as IHasCombo)?.NewCombo ?? false,
            };
        }
    }
}
