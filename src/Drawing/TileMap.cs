using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

namespace Atlas
{
    public class LayeredTileMap : Node
    {
        private Dictionary<int, TileMap> _layers = new Dictionary<int, TileMap>();

        public TileSet TileSet { get; }
        public int TileSize => TileSet.TileSize;
        public Point MapSize { get; }
        public Dictionary<int, TileMap> Layers => _layers;

        public LayeredTileMap(string path, int tileSize, int width, int height) :
            this(new TileSet(path, tileSize), width, height)
        { }

        public LayeredTileMap(TileSet tileSet, int width, int height)
        {
            TileSet = tileSet;
            MapSize = new Point(width, height);
            Size = MapSize.ToVector2() * tileSet.TileSize;
        }

        public LayeredTileMap(TileSet tileSet, LayeredTileMapData data)
        {
            TileSet = tileSet;
            MapSize = new Point(data.Width, data.Height);
            Size = MapSize.ToVector2() * tileSet.TileSize;

            foreach (var (layer, mapData) in data.Layers)
            {
                var map = new TileMap(tileSet, mapData);
                AddLayer(int.Parse(layer), map);
            }
        }

        public LayeredTileMapData GetData()
        {
            var layers = new Dictionary<string, TileMapData>();
            foreach (var (layer, map) in _layers) layers[layer.ToString()] = map.GetData();

            return new LayeredTileMapData()
            {
                Width = MapSize.X,
                Height = MapSize.Y,
                TileSize = TileSize,
                Layers = layers
            };
        }

        public bool WithinBounds(Point pos) => !(pos.X < 0 || pos.Y < 0 || pos.X > MapSize.X - 1 || pos.Y > MapSize.Y - 1);

        private void AddLayer(int layer, TileMap map)
        {
            map.Layer = (uint)(layer + int.MaxValue / 2);
            AddChild(map);
            _layers[layer] = map;
        }

        public TileMap GetLayer(int layer)
        {
            if (!_layers.TryGetValue(layer, out var map))
            {
                map = new TileMap(TileSet, MapSize.X, MapSize.Y);
                AddLayer(layer, map);
            }

            return map;
        }

        public void RemoveLayer(int layer)
        {
            if (_layers.TryGetValue(layer, out var map))
            {
                RemoveChild(map);
                _layers.Remove(layer);
            }
            else throw new ArgumentException("Layer has not been added to map");
        }
    }

    public struct LayeredTileMapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public Dictionary<string, TileMapData> Layers { get; set; }
    }

    public struct TileMapData
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileSize { get; set; }
        public int?[] Tiles { get; set; }
    }

    public class TileMap : Node
    {
        private int?[] _tiles;

        public int? this[int x, int y]
        {
            get => _tiles[y * MapSize.X + x];
            set => _tiles[y * MapSize.X + x] = value;
        }

        public TileSet TileSet { get; }
        public int TileSize => TileSet.TileSize;
        public Point MapSize { get; }

        public TileMap(string path, int tileSize, int width, int height) :
            this(new TileSet(path, tileSize), width, height)
        { }

        public TileMap(TileSet tileSet, int width, int height)
        {
            TileSet = tileSet;
            _tiles = new int?[width * height];
            MapSize = new Point(width, height);
            Size = MapSize.ToVector2() * tileSet.TileSize;
            AddComponent(new Drawable() { Draw = Draw });
        }

        public TileMap(TileSet tileSet, TileMapData data)
        {
            TileSet = tileSet;
            _tiles = (int?[])data.Tiles.Clone();
            MapSize = new Point(data.Width, data.Height);
            Size = MapSize.ToVector2() * tileSet.TileSize;
            AddComponent(new Drawable() { Draw = Draw });
        }

        public bool WithinBounds(Point pos) => !(pos.X < 0 || pos.Y < 0 || pos.X > MapSize.X - 1 || pos.Y > MapSize.Y - 1);

        public TileMapData GetData()
        {
            return new TileMapData()
            {
                Width = MapSize.X,
                Height = MapSize.Y,
                TileSize = TileSize,
                Tiles = _tiles
            };
        }

        private void Draw(SpriteBatch spriteBatch, DrawContext ctx)
        {
            for (int x = 0; x < MapSize.X; x++)
            {
                for (int y = 0; y < MapSize.Y; y++)
                {
                    int? tile = this[x, y];
                    if (!tile.HasValue) continue;

                    var sourceRect = TileSet.GetTileRect(tile.Value);
                    spriteBatch.Draw(TileSet.Texture, ctx.Position + new Vector2(x, y) * TileSize, sourceRect, ctx.Modulate.ModulateColor(Colors.White));
                }
            }
        }
    }
}