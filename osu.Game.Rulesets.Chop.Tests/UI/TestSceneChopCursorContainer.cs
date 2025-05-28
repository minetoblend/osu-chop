using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Game.Graphics.Cursor;
using osu.Game.Rulesets.Chop.UI;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Chop.Tests.UI;

public partial class TestSceneChopCursorContainer : OsuTestScene
{
    public TestSceneChopCursorContainer()
    {
        Add(new CursorWrapper());
    }

    private partial class CursorWrapper : Container, IProvideCursor
    {
        private ChopCursorContainer cursor = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Both;

            Add(cursor = new ChopCursorContainer());
        }

        public CursorContainer Cursor => cursor;
        public bool ProvidingUserCursor => true;
    }
}
