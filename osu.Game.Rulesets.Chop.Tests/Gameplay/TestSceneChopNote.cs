using System;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Chop.UI;

namespace osu.Game.Rulesets.Chop.Tests.Gameplay;

public partial class TestSceneChopNote : ChopSkinnableTestScene
{
    [Test]
    public void TestNote()
    {
        SetContents(_ =>
        {
            var playfield = new ChopPlayfield();

            Scheduler.AddDelayed(() =>
            {
                var note = new ChopNote
                {
                    X = Random.Shared.NextSingle() * 512,
                    Y = Random.Shared.NextSingle() * 192,
                    StartTime = Time.Current + 1000,
                    TimePreempt = 500,
                };

                note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty { ApproachRate = 8 });

                playfield.Add(note);
            }, 1000, true);

            return playfield;
        });
    }
}
