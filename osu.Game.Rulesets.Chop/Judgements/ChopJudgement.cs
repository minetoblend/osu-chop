﻿using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Chop.Judgements;

public class ChopJudgement : Judgement
{
    public override HitResult MaxResult => HitResult.Great;
}
