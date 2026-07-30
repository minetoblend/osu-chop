using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Chop.Scoring;

public class ChopHitWindows : HitWindows
{
    public static readonly DifficultyRange GREAT_WINDOW_RANGE = new DifficultyRange(120, 80, 40);
    public static readonly DifficultyRange OK_WINDOW_RANGE = new DifficultyRange(250, 200, 100);
    public static readonly DifficultyRange MEH_WINDOW_RANGE = new DifficultyRange(400, 350, 200);

    /// <summary>
    /// chop ruleset has a fixed miss window regardless of difficulty settings.
    /// </summary>
    public const double MISS_WINDOW = 600;

    private double great;
    private double ok;
    private double meh;

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

    public override void SetDifficulty(double difficulty)
    {
        great = IBeatmapDifficultyInfo.DifficultyRange(difficulty, GREAT_WINDOW_RANGE);
        ok = IBeatmapDifficultyInfo.DifficultyRange(difficulty, OK_WINDOW_RANGE);
        meh = IBeatmapDifficultyInfo.DifficultyRange(difficulty, MEH_WINDOW_RANGE);
    }

    public override double WindowFor(HitResult result)
    {
        switch (result)
        {
            case HitResult.Great:
                return great;

            case HitResult.Ok:
                return ok;

            case HitResult.Meh:
                return meh;

            case HitResult.Miss:
                return MISS_WINDOW;

            default:
                throw new ArgumentOutOfRangeException(nameof(result), result, null);
        }
    }
}
