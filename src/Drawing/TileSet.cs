using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class TileSet
    {
        private string _path;

        public int TileSize { get; }
        public Texture2D Texture { get; }
        public int Width => Texture.Width / TileSize;

        public TileSet(string path, int tileSize)
        {
            _path = path;
            Texture = Texture2D.FromFile(Config.GraphicsDevice, Path.Join(Config.ContentPath, path));
            TileSize = tileSize;
        }

        public bool TextureEmpty(int index)
        {
            var data = new uint[TileSize * TileSize];
            var rect = GetTileRect(index);
            Texture.GetData(0, rect, data, 0, TileSize * TileSize);
            foreach (var value in data)
            {
                if (value > 0) return false;
            }
            return true;
        }

        public Rectangle GetTileRect(int index)
        {
            int xPos = (index * TileSize) % Texture.Width;
            int yPos = ((index * TileSize) / Texture.Width) * TileSize;

            return new Rectangle(new Point(xPos, yPos), new Point(TileSize));
        }

        public int TextureCount => (Texture.Width * Texture.Height) / (TileSize * TileSize);

        public IEnumerable<int> GetTiles()
        {
            for (int i = 0; i < TextureCount; i++)
            {
                yield return i;
            }
        }

        public Sprite GetSprite(int index) => new Sprite(_path, new Point(TileSize)) { CurrentFrame = index };
    }
}