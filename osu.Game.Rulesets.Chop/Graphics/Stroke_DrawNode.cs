using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Rendering.Vertices;
using osu.Framework.Graphics.Shaders;
using osuTK;
using osuTK.Graphics;
using PrimitiveTopology = osu.Framework.Graphics.Rendering.PrimitiveTopology;
using Texture = osu.Framework.Graphics.Textures.Texture;

namespace osu.Game.Rulesets.Chop.Graphics;

public partial class Stroke
{
    private class StrokeDrawNode(Stroke source) : DrawNode(source)
    {
        private const int max_res = 24;

        protected new Stroke Source => (Stroke)base.Source;

        private readonly List<StrokeVertex> segments = new List<StrokeVertex>();

        private Texture? texture = null!;
        private Vector2 drawSize;
        private IShader? pathShader;

        private IVertexBatch<TexturedVertex3D>? triangleBatch;

        public override void ApplyState()
        {
            base.ApplyState();

            segments.Clear();
            segments.AddRange(Source.segments);

            texture = Source.Texture;
            drawSize = Source.DrawSize;
            pathShader = Source.pathShader;
        }

        protected override void Draw(IRenderer renderer)
        {
            base.Draw(renderer);

            if (texture?.Available != true || segments.Count == 0 || pathShader == null)
                return;

            triangleBatch ??= renderer.CreateLinearBatch<TexturedVertex3D>(max_res * 200 * 3, 10, PrimitiveTopology.Triangles);

            renderer.PushLocalMatrix(DrawInfo.Matrix);
            renderer.PushDepthInfo(DepthInfo.Default);

            // Blending is removed to allow for correct blending between the wedges of the path.
            renderer.SetBlend(BlendingParameters.None);

            pathShader.Bind();

            texture.Bind();

            updateVertexBuffer();

            pathShader.Unbind();

            renderer.PopDepthInfo();
            renderer.PopLocalMatrix();
        }

        private void updateVertexBuffer()
        {
            Debug.Assert(texture != null);
            Debug.Assert(segments.Count > 0);

            RectangleF texRect = texture.GetTextureRect(new RectangleF(0.5f, 0.5f, texture.Width - 1, texture.Height - 1));

            Debug.Assert(triangleBatch != null);

            for (int i = 0; i < segments.Count - 1; i++)
            {
                StrokeVertex first = segments[i];
                StrokeVertex second = segments[i + 1];

                var firstCenter = (first.StartPoint + first.EndPoint) * 0.5f;
                var secondCenter = (second.StartPoint + second.EndPoint) * 0.5f;

                Vector3 firstMiddlePoint = new Vector3((first.StartPoint + first.EndPoint) * 0.5f) { Z = 1 };
                Vector3 secondMiddlePoint = new Vector3((second.StartPoint + second.EndPoint) * 0.5f) { Z = 1 };
                Color4 firstMiddleColour = colourAt(firstCenter);
                Color4 secondMiddleColour = colourAt(secondCenter);

                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = new Vector3(first.StartPoint),
                    TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                    Colour = colourAt(first.StartPoint)
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = new Vector3(second.StartPoint),
                    TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                    Colour = colourAt(second.StartPoint)
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = firstMiddlePoint,
                    TexturePosition = first.CenterTexturePositionFor(ref texRect),
                    Colour = firstMiddleColour
                });

                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = new Vector3(second.StartPoint),
                    TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                    Colour = colourAt(second.StartPoint)
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = secondMiddlePoint,
                    TexturePosition = second.CenterTexturePositionFor(ref texRect),
                    Colour = secondMiddleColour
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = firstMiddlePoint,
                    TexturePosition = first.CenterTexturePositionFor(ref texRect),
                    Colour = firstMiddleColour
                });

                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = new Vector3(first.EndPoint),
                    TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                    Colour = colourAt(first.EndPoint)
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = new Vector3(second.EndPoint),
                    TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                    Colour = colourAt(second.EndPoint)
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = firstMiddlePoint,
                    TexturePosition = first.CenterTexturePositionFor(ref texRect),
                    Colour = firstMiddleColour
                });

                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = new Vector3(second.EndPoint),
                    TexturePosition = new Vector2(texRect.Left, texRect.Centre.Y),
                    Colour = colourAt(second.EndPoint)
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = secondMiddlePoint,
                    TexturePosition = second.CenterTexturePositionFor(ref texRect),
                    Colour = secondMiddleColour
                });
                triangleBatch.Add(new TexturedVertex3D
                {
                    Position = firstMiddlePoint,
                    TexturePosition = first.CenterTexturePositionFor(ref texRect),
                    Colour = firstMiddleColour
                });
            }
        }

        private Color4 colourAt(Vector2 localPos) => DrawColourInfo.Colour.TryExtractSingleColour(out SRGBColour colour)
            ? colour.SRGB
            : DrawColourInfo.Colour.Interpolate(relativePosition(localPos)).SRGB;

        private Vector2 relativePosition(Vector2 localPos) => Vector2.Divide(localPos, drawSize);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            triangleBatch?.Dispose();
        }
    }
}
