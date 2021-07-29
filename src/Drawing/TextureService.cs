using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;

namespace Atlas
{
    internal static class TextureService
    {
        private static Dictionary<string, Texture2D> _textures = new();
        private static string[] _extensions = new[] { "tga", "png", "bmp" };

        public static Texture2D GetTexture(string path)
        {
            if (!_textures.TryGetValue(path, out var texture))
            {
                var texturePath = _extensions
                    .Select(e => $"{Path.Join(Config.ContentPath, path)}.{e}")
                    .Where(p => File.Exists(p))
                    .FirstOrDefault();

                if (texturePath == null) throw new FileNotFoundException($"Texture '{path}' not found");

                texture = Texture2D.FromFile(Config.GraphicsDevice, texturePath);
                _textures[path] = texture;
            }

            return texture;
        }
    }
}