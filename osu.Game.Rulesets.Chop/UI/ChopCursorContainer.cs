using System;
using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osu.Game.Extensions;
using osu.Game.Rulesets.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.UI;

public partial class ChopCursorContainer : GameplayCursorContainer
{
    private Vector2 position;
    private CursorState state;
    private Vector2 sliceStartPosition;

    [Resolved]
    private GameHost host { get; set; } = null!;

    public event Action? SliceStarted;

    public event Action? SliceEnded;

    protected override Drawable CreateCursor() => new CircularContainer
    {
        Size = new Vector2(20),
        Origin = Anchor.Centre,
        Masking = true,
        BorderColour = Color4.White,
        BorderThickness = 3,
        EdgeEffect = new EdgeEffectParameters
        {
            Radius = 150,
            Colour = Color4.GreenYellow.Opacity(0.1f),
            Type = EdgeEffectType.Glow
        },
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Color4.GreenYellow,
        }
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
                    beginSlice();

                currentPath?.AddVertex(state.Position);
            }
            else if (currentPath != null)
                endSlice();
        }

        currentPath?.UpdateCursorPosition(position);
    }

    private void beginSlice()
    {
        sliceStartPosition = state.Position;

        SliceStarted?.Invoke();

        Add(currentPath = new ChopCursorPath
        {
            AccentColour = Color4.GreenYellow,
            Depth = 1,
        });

        currentPath.ApplyGameWideClock(host);
    }

    private void endSlice()
    {
        Debug.Assert(currentPath != null);

        currentPath.OnStrokeEnded();
        currentPath = null;

        SliceEnded?.Invoke();
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
