using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Caching;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Framework.Layout;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Chop.Graphics;

public partial class Stroke : Drawable, IBufferedDrawable
{
    public IShader TextureShader { get; private set; } = null!;
    private IShader pathShader = null!;

    [Resolved]
    private IRenderer renderer { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load(ShaderManager shaders)
    {
        TextureShader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE);
        pathShader = shaders.Load(VertexShaderDescriptor.TEXTURE_3, FragmentShaderDescriptor.TEXTURE);
    }

    private float pathRadius = 10f;

    public virtual float PathRadius
    {
        get => pathRadius;
        set
        {
            if (value == pathRadius)
                return;

            pathRadius = value;

            pathRadius = value;

            vertexBoundsCache.Invalidate();
            segmentsCache.Invalidate();

            Invalidate(Invalidation.DrawSize);
        }
    }

    private readonly List<Vector2> vertices = new List<Vector2>();

    public IReadOnlyList<Vector2> Vertices
    {
        get => vertices;
        set
        {
            vertices.Clear();
            vertices.AddRange(value);

            vertexBoundsCache.Invalidate();
            segmentsCache.Invalidate();

            Invalidate(Invalidation.DrawSize);
        }
    }

    public void ReplaceVertex(int index, Vector2 pos)
    {
        vertices[index] = pos;

        vertexBoundsCache.Invalidate();
        segmentsCache.Invalidate();

        Invalidate(Invalidation.DrawSize);
    }

    private Axes autoSizeAxes;

    /// <summary>
    /// Controls which <see cref="Axes"/> are automatically sized w.r.t. the bounds of the vertices.
    /// It is not allowed to manually set <see cref="Size"/> (or <see cref="Width"/> / <see cref="Height"/>)
    /// on any <see cref="Axes"/> which are automatically sized.
    /// </summary>
    public virtual Axes AutoSizeAxes
    {
        get => autoSizeAxes;
        set
        {
            if (value == autoSizeAxes)
                return;

            if ((RelativeSizeAxes & value) != 0)
                throw new InvalidOperationException("No axis can be relatively sized and automatically sized at the same time.");

            autoSizeAxes = value;
            OnSizingChanged();
        }
    }

    public override float Width
    {
        get
        {
            if (AutoSizeAxes.HasFlagFast(Axes.X))
                return base.Width = vertexBounds.Width;

            return base.Width;
        }
        set
        {
            if ((AutoSizeAxes & Axes.X) != 0)
                throw new InvalidOperationException($"The width of a {nameof(Stroke)} with {nameof(AutoSizeAxes)} can not be set manually.");

            base.Width = value;
        }
    }

    public override float Height
    {
        get
        {
            if (AutoSizeAxes.HasFlagFast(Axes.Y))
                return base.Height = vertexBounds.Height;

            return base.Height;
        }
        set
        {
            if ((AutoSizeAxes & Axes.Y) != 0)
                throw new InvalidOperationException($"The height of a {nameof(Stroke)} with {nameof(AutoSizeAxes)} can not be set manually.");

            base.Height = value;
        }
    }

    public override Vector2 Size
    {
        get
        {
            if (AutoSizeAxes != Axes.None)
                return base.Size = vertexBounds.Size;

            return base.Size;
        }
        set
        {
            if ((AutoSizeAxes & Axes.Both) != 0)
                throw new InvalidOperationException($"The Size of a {nameof(Stroke)} with {nameof(AutoSizeAxes)} can not be set manually.");

            base.Size = value;
        }
    }

    private readonly Cached<RectangleF> vertexBoundsCache = new Cached<RectangleF>();

    private RectangleF vertexBounds
    {
        get
        {
            if (vertexBoundsCache.IsValid)
                return vertexBoundsCache.Value;

            if (vertices.Count > 0)
            {
                float minX = 0;
                float minY = 0;
                float maxX = 0;
                float maxY = 0;

                foreach (var v in vertices)
                {
                    minX = Math.Min(minX, v.X - PathRadius);
                    minY = Math.Min(minY, v.Y - PathRadius);
                    maxX = Math.Max(maxX, v.X + PathRadius);
                    maxY = Math.Max(maxY, v.Y + PathRadius);
                }

                return vertexBoundsCache.Value = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            }

            return vertexBoundsCache.Value = new RectangleF(0, 0, 0, 0);
        }
    }

    private readonly List<StrokeVertex> segmentsBacking = new List<StrokeVertex>();
    private readonly Cached segmentsCache = new Cached();

    private List<StrokeVertex> segments => segmentsCache.IsValid ? segmentsBacking : generateSegments();

    private List<StrokeVertex> generateSegments()
    {
        segmentsBacking.Clear();

        if (vertices.Count > 1)
        {
            Vector2 offset = vertexBounds.TopLeft;

            segmentsBacking.Add(new StrokeVertex(vertices[0] - offset, Vector2.Zero, 0));

            int lastIndex = 0;

            for (int i = 1; i < vertices.Count - 1; i++)
            {
                var prev = vertices[lastIndex];
                var curr = vertices[i];
                var next = vertices[i + 1];

                if (Precision.AlmostEquals(prev, curr))
                    continue;

                lastIndex = i;

                if (Precision.AlmostEquals(curr, next))
                    continue;

                var prevLine = new Line(prev, curr);
                var nextLine = new Line(curr, next);

                var direction = (prevLine.DirectionNormalized + nextLine.DirectionNormalized).Normalized() * pathRadius;

                segmentsBacking.Add(new StrokeVertex(curr - offset, direction, ThicknessAt(i / (vertices.Count + 1f))));
            }

            segmentsBacking.Add(new StrokeVertex(vertices[^1] - offset, Vector2.Zero, 0));
        }

        segmentsCache.Validate();
        return segmentsBacking;
    }

    protected virtual float ThicknessAt(float progress) => 1;

    private Texture? texture;

    protected Texture? Texture
    {
        get => texture ?? renderer?.WhitePixel;
        set
        {
            if (texture == value)
                return;

            texture?.Dispose();
            texture = value;

            Invalidate(Invalidation.DrawNode);
        }
    }

    public DrawColourInfo? FrameBufferDrawColour => base.DrawColourInfo;

    public Vector2 FrameBufferScale { get; } = Vector2.One;

    public override DrawColourInfo DrawColourInfo => new DrawColourInfo(Color4.White, base.DrawColourInfo.Blending);

    private Color4 backgroundColour = new Color4(0, 0, 0, 0);

    public virtual Color4 BackgroundColour
    {
        get => backgroundColour;
        set
        {
            backgroundColour = value;
            Invalidate(Invalidation.DrawNode);
        }
    }

    public long PathInvalidationID { get; private set; }

    protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
    {
        bool result = base.OnInvalidate(invalidation, source);

        // Colour is being applied to the buffer instead of the actual drawable, thus removing the need to redraw the path on colour invalidation.
        invalidation &= ~Invalidation.Colour;

        if (invalidation != Invalidation.None)
            PathInvalidationID++;

        return result;
    }

    public Vector2 PositionInBoundingBox(Vector2 pos) => pos - vertexBounds.TopLeft;

    private readonly BufferedDrawNodeSharedData sharedData = new BufferedDrawNodeSharedData(new[] { RenderBufferFormat.D16 }, clipToRootNode: true);

    protected override DrawNode CreateDrawNode() => new PathBufferedDrawNode(this, new StrokeDrawNode(this), sharedData);

    private class PathBufferedDrawNode : BufferedDrawNode
    {
        protected new Stroke Source => (Stroke)base.Source;

        public PathBufferedDrawNode(Stroke source, StrokeDrawNode child, BufferedDrawNodeSharedData sharedData)
            : base(source, child, sharedData)
        {
        }

        private long pathInvalidationID = -1;

        public override void ApplyState()
        {
            base.ApplyState();

            pathInvalidationID = Source.PathInvalidationID;
        }

        protected override long GetDrawVersion() => pathInvalidationID;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        texture?.Dispose();
        texture = null;

        sharedData.Dispose();
    }

    private readonly struct StrokeVertex(Vector2 position, Vector2 direction, float thickness)
    {
        public readonly Vector2 Position = position;

        public readonly Vector2 Direction = direction;

        public Vector2 OrthogonalDirection => new Vector2(-Direction.Y, Direction.X);

        public readonly float Thickness = thickness;

        public Vector2 StartPoint => Position - OrthogonalDirection * Thickness;

        public Vector2 EndPoint => Position + OrthogonalDirection * Thickness;

        public Vector2 CenterTexturePositionFor(ref RectangleF texRect) => new(texRect.Left + Thickness * texRect.Width, texRect.Centre.Y);
    }
}
