// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Chop.Objects;
using osu.Game.Rulesets.Chop.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Chop.UI
{
    [Cached]
    public partial class ChopPlayfield : Playfield
    {
        public static readonly Vector2 BASE_SIZE = new Vector2(512, 384);

        [BackgroundDependencyLoader]
        private void load()
        {
            AddRangeInternal(new Drawable[]
            {
                HitObjectContainer,
            });

            RegisterPool<ChopNote, DrawableChopNote>(20, 100);
        }

        protected override GameplayCursorContainer CreateCursor() => new ChopCursorContainer();
    }
}
