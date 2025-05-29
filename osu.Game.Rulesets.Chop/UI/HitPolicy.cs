using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Chop.Input;
using osu.Game.Rulesets.Chop.Judgements;
using osu.Game.Rulesets.Chop.Objects.Drawables;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Chop.UI;

public partial class HitPolicy : CompositeDrawable, ISliceEventHandler
{
    public HitPolicy()
    {
        RelativeSizeAxes = Axes.Both;
    }

    public IHitObjectContainer? HitObjectContainer { get; set; }

    public ClickAction CheckHittable(DrawableHitObject hitObject, double time, HitResult _)
    {
        if (HitObjectContainer == null)
            throw new InvalidOperationException($"{nameof(HitObjectContainer)} should be set before {nameof(CheckHittable)} is called.");

        DrawableHitObject? blockingObject = null;

        foreach (var obj in enumerateHitObjectsUpTo(hitObject.HitObject.StartTime))
        {
            if (hitObjectCanBlockFutureHits(obj))
                blockingObject = obj;
        }

        // If there is no previous hitobject, allow the hit.
        if (blockingObject == null)
            return ClickAction.Hit;

        // A hit is allowed if:
        // 1. The last blocking hitobject has been judged.
        // 2. The current time is after the last hitobject's start time.
        // Hits at exactly the same time as the blocking hitobject are allowed for maps that contain simultaneous hitobjects (e.g. /b/372245).
        return (blockingObject.Judged || time >= blockingObject.HitObject.StartTime) ? ClickAction.Hit : ClickAction.Shake;
    }

    private (double time, Vector2 position, Vector2 direction)? lastHit;

    public void HandleHit(DrawableHitObject hitObject)
    {
        if (hitObject.Result.IsHit && hitObject.Result is ChopJudgementResult chopResult)
        {
            lastHit = (chopResult.TimeAbsolute, chopResult.SlicePosition, chopResult.SliceDirection);
        }

        if (HitObjectContainer == null)
            throw new InvalidOperationException($"{nameof(HitObjectContainer)} should be set before {nameof(HandleHit)} is called.");

        // Hitobjects which themselves don't block future hitobjects don't cause misses (e.g. slider ticks, spinners).
        if (!hitObjectCanBlockFutureHits(hitObject))
            return;

        if (CheckHittable(hitObject, hitObject.HitObject.StartTime + hitObject.Result.TimeOffset, hitObject.Result.Type) != ClickAction.Hit)
            throw new InvalidOperationException($"A {hitObject} was hit before it became hittable!");

        // Miss all hitobjects prior to the hit one.
        foreach (var obj in enumerateHitObjectsUpTo(hitObject.HitObject.StartTime))
        {
            if (obj.Judged)
                continue;

            if (hitObjectCanBlockFutureHits(obj))
                ((DrawableChopHitObject)obj).MissForcefully();
        }
    }

    /// <summary>
    /// Whether a <see cref="HitObject"/> blocks hits on future <see cref="HitObject"/>s until its start time is reached.
    /// </summary>
    /// <param name="hitObject">The <see cref="HitObject"/> to test.</param>
    private static bool hitObjectCanBlockFutureHits(DrawableHitObject hitObject)
        => hitObject is DrawableChopNote;

    private IEnumerable<DrawableHitObject> enumerateHitObjectsUpTo(double targetTime)
    {
        foreach (var obj in HitObjectContainer!.AliveObjects)
        {
            if (obj.HitObject.StartTime >= targetTime)
                yield break;

            yield return obj;

            foreach (var nestedObj in obj.NestedHitObjects)
            {
                if (nestedObj.HitObject.StartTime >= targetTime)
                    break;

                yield return nestedObj;
            }
        }
    }

    public bool OnSlice(SliceEvent e) => false;

    public bool OnSliceStarted(SliceStartEvent e) => false;

    public bool OnSliceEnded(SliceEndEvent e)
    {
        lastHit = null;
        return false;
    }
}
