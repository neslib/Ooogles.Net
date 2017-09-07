using System;
using Ooogles;

namespace Examples.Shared
{
    public static class TextureUtils
    {
        public static Texture CreateSimpleTexture2D()
        {
            const int Width = 2;
            const int Height = 2;
            byte[] Pixels = {
                255,   0,   0,  // Red
                  0, 255,   0,  // Green
                  0,   0, 255,  // Blue
                255, 255,   0}; // Yellow

            // Use tightly packed data
            gl.PixelStore(gl.PixelStoreMode.UnpackAlignment, gl.PixelStoreValue.One);

            // Generate a texture object
            Texture texture = new Texture();

            // Bind the texture object
            texture.Bind();

            // Load the texture: 2x2 Image, 3 bytes per pixel (R, G, B)
            texture.Upload(gl.PixelFormat.Rgb, Width, Height, Pixels);

            // Set the filtering mode
            texture.MinificationFilter = Texture.MinFilter.Nearest;
            texture.MagnificationFilter = Texture.MagFilter.Nearest;

            return texture;
        }

        public static Texture CreateMipmappedTexture2D()
        {
            const int Width = 256;
            const int Height = 256;
            const int CheckerSize = 8;

            byte[] pixels = new byte[Width * Height * 3];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int r, b;
                    if (((x / CheckerSize) & 1) == 0)
                    {
                        r = 255 * ((y / CheckerSize) & 1);
                        b = 255 * (1 - ((y / CheckerSize) & 1));
                    }
                    else
                    {
                        b = 255 * ((y / CheckerSize) & 1);
                        r = 255 * (1 - ((y / CheckerSize) & 1));
                    }

                    pixels[(y * Height + x) * 3 + 0] = (byte)r;
                    pixels[(y * Height + x) * 3 + 1] = 0;
                    pixels[(y * Height + x) * 3 + 2] = (byte)b;
                }
            }

            // Generate a texture object
            Texture texture = new Texture();

            // Bind the texture object
            texture.Bind();

            // Load mipmap level 0
            texture.Upload(gl.PixelFormat.Rgb, Width, Height, pixels);

            // Generate mipmaps
            texture.GenerateMipmap();

            // Set the filtering mode
            texture.MinificationFilter = Texture.MinFilter.NearestMipmapNearest;
            texture.MagnificationFilter = Texture.MagFilter.Linear;

            return texture;
        }

        public static Texture CreateSimpleTextureCubeMap()
        {
            byte[][] Pixels = {
                // Face 0 - Red
                new byte[] { 255,   0,   0 },

                // Face 1 - Green,
                new byte[] {   0, 255,   0 },

                // Face 3 - Blue
                new byte[] {   0,   0, 255 },

                // Face 4 - Yellow
                new byte[] { 255, 255,   0 },

                // Face 5 - Purple
                new byte[] { 255,   0, 255 },

                // Face 6 - White
                new byte[] { 255, 255, 255 } };

            // Generate a texture object
            Texture texture = new Texture(Texture.Type.CubeMap);

            // Bind the texture object
            texture.Bind();

            for (int i = 0; i < Pixels.Length; i++)
            {
                texture.Upload(gl.PixelFormat.Rgb, 1, 1, Pixels[i], 0, gl.PixelDataType.UnsignedByte, i);
            }

            // Set the filtering mode
            texture.MinificationFilter = Texture.MinFilter.Nearest;
            texture.MagnificationFilter = Texture.MagFilter.Nearest;

            return texture;
        }

        public static Texture LoadTexture(string filename)
        {
            TgaImage image = new TgaImage(filename);
            return image.ToTexture();
        }
    }
}
