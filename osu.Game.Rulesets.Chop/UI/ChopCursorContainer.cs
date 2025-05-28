using osu.Framework.Graphics;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Utils;
using osuTK;

namespace osu.Game.Rulesets.Chop.UI;

public partial class ChopCursorContainer : CursorContainer
{
    private Vector2 position;
    private CursorState state;
    private Vector2 sliceStartPosition;

    protected override Drawable CreateCursor() => new Circle
    {
        Size = new Vector2(20),
        Origin = Anchor.Centre,
    };

    protected override void LoadComplete()
    {
        base.LoadComplete();

        var inputManager = GetContainingInputManager()!;

        state = new CursorState(ToLocalSpace(inputManager.CurrentState.Mouse.Position), Vector2.Zero, Time.Current);
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        position = e.MousePosition;

        return base.OnMouseMove(e);
    }

    protected override void Update()
    {
        base.Update();

        updateCursorPath();
    }

    private ChopCursorPath? currentPath;

    private void updateCursorPath()
    {
        while (state.Interpolate(position, Time.Current, out var newState))
        {
            state = newState;

            const float threshold = 15f;

            if (state.Velocity > threshold)
            {
                if (currentPath == null)
                {
                    sliceStartPosition = state.Position;

                    Add(currentPath = new ChopCursorPath
                    {
                        Depth = 1
                    });
                }

                currentPath.AddVertex(state.Position);
            }
            else if (currentPath != null)
            {
                currentPath.OnStrokeEnded();
                currentPath = null;
            }
        }

        currentPath?.UpdateCursorPosition(position);
    }

    private readonly record struct CursorState(Vector2 Position, Vector2 Delta, double Time)
    {
        private const float cursor_framerate = 120;

        public bool Interpolate(Vector2 newPosition, double currentTime, out CursorState newState)
        {
            const double interval = 1000 / cursor_framerate;

            if (Time + interval > currentTime)
            {
                newState = default;
                return false;
            }

            var position = Interpolation.ValueAt(Time + interval, Position, newPosition, Time, currentTime);

            newState = new CursorState(position, position - Position, Time + interval);
            return true;
        }

        public float Velocity => Delta.Length;
    }
}
