using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Chop.Scoring;

public class ChopHitWindows : HitWindows
{
    /// <summary>
    /// chop ruleset has a fixed miss window regardless of difficulty settings.
    /// </summary>
    public const double MISS_WINDOW = 600;

    internal static readonly DifficultyRange[] CHOP_RANGES =
    {
        new DifficultyRange(HitResult.Great, 120, 80, 40),
        new DifficultyRange(HitResult.Ok, 250, 200, 100),
        new DifficultyRange(HitResult.Meh, 400, 350, 200),
        new DifficultyRange(HitResult.Miss, MISS_WINDOW, MISS_WINDOW, MISS_WINDOW),
    };

    public override bool IsHitResultAllowed(HitResult result)
    {
        switch (result)
        {
            case HitResult.Great:
            case HitResult.Ok:
            case HitResult.Meh:
            case HitResult.Miss:
                return true;
        }

        return false;
    }

    protected override DifficultyRange[] GetRanges() => CHOP_RANGES;
}
