using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osuTK;

namespace osu.Game.Rulesets.Chop.Judgements;

public class ChopJudgementResult : JudgementResult
{
    public Vector2 SlicePosition;
    public Vector2 SliceDirection;

    public ChopJudgementResult(HitObject hitObject, Judgement judgement)
        : base(hitObject, judgement)
    {
    }
}
