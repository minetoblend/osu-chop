﻿using System;
using osu.Framework.Allocation;
using osu.Framework.Caching;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Rendering;
using osu.Framework.Graphics.Textures;
using osuTK.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace osu.Game.Rulesets.Chop.Graphics;

public partial class TexturedStroke : Stroke
{
    [Resolved]
    private IRenderer renderer { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        validateTexture();
    }

    public override float PathRadius
    {
        get => base.PathRadius;
        set
        {
            if (base.PathRadius == value)
                return;

            base.PathRadius = value;

            InvalidateTexture();
        }
    }

    private Color4? customBackgroundColour;

    /// <summary>
    /// The background colour to be used for the frame buffer this path is rendered to.
    /// For <see cref="TexturedStroke"/>, this automatically defaults to the colour at 0 (the outermost colour of the path) to avoid aliasing issues.
    /// </summary>
    public override Color4 BackgroundColour
    {
        get => customBackgroundColour ?? base.BackgroundColour;
        set => customBackgroundColour = base.BackgroundColour = value;
    }

    private readonly Cached textureCache = new Cached();

    protected void InvalidateTexture()
    {
        textureCache.Invalidate();
        Invalidate(Invalidation.DrawNode);
    }

    private void validateTexture()
    {
        if (textureCache.IsValid)
            return;

        int textureWidth = (int)PathRadius * 2;

        //initialise background
        var raw = new Image<Rgba32>(textureWidth, 1);

        const float aa_portion = 0.02f;

        for (int i = 0; i < textureWidth; i++)
        {
            float progress = (float)i / (textureWidth - 1);

            var colour = ColourAt(progress);
            raw[i, 0] = new Rgba32(colour.R, colour.G, colour.B, colour.A * Math.Min(progress / aa_portion, 1));
        }

        if (Texture?.Width == textureWidth)
        {
            Texture.SetData(new TextureUpload(raw));
        }
        else
        {
            var texture = new DisposableTexture(renderer.CreateTexture(textureWidth, 1, true));
            texture.SetData(new TextureUpload(raw));
            Texture = texture;
        }

        if (customBackgroundColour == null)
            base.BackgroundColour = ColourAt(0).Opacity(0);

        textureCache.Validate();
    }

    /// <summary>
    /// Retrieves the colour from a position in the texture of the <see cref="TexturedCursorPath"/>.
    /// </summary>
    /// <param name="position">The position within the texture. 0 indicates the outermost-point of the path, 1 indicates the centre of the path.</param>
    protected virtual Color4 ColourAt(float position) => Color4.White;
}
