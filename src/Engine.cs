using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public class EngineCore
    {
        private Type? _scene;

        public Point ScreenSize { get; set; }
        public string ContentDirectory { get; set; } = ".";
        public string Font { get; set; } = "";

        public void SetScene<T>() where T : IScene, new() => _scene = typeof(T);

        public void Start()
        {
            using (var g = new MainGame())
            {
                g.ScreenSize = ScreenSize;
                g.Content.RootDirectory = ContentDirectory;
                FontService.SetFont(Path.Join(ContentDirectory, Font));
                if (_scene != null)
                {
                    var s = Activator.CreateInstance(_scene);
                    if (s != null) g.MainScene = (IScene)s;
                }
                g.Run();
            }
        }
    }

    internal static class Config
    {
        public static Point ScreenSize { get; set; }
        public static GameWindow? CurrentWindow { get; set; }
    }

#nullable disable

    internal class MainGame : Game
    {
        private Point _screenSize;
        private GraphicsDeviceManager _gdm;
        private SpriteBatch _spriteBatch;

        public Point ScreenSize
        {
            get => _screenSize;
            set
            {
                _screenSize = value;
                _gdm.PreferredBackBufferWidth = _screenSize.X;
                _gdm.PreferredBackBufferHeight = _screenSize.Y;
                Config.ScreenSize = value;
            }
        }
        public IScene MainScene { get; set; }

        public MainGame()
        {
            _gdm = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(this.GraphicsDevice);

            _gdm.IsFullScreen = false;
            _gdm.SynchronizeWithVerticalRetrace = true;
            _gdm.ApplyChanges();

            Config.CurrentWindow = Window;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            CustomDrawing.Init(_spriteBatch);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            // Clean up after yourself!
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            MouseInputSystem.InputConsumed = false;
            if (MainScene?.EnableUpdate ?? false) MainScene?.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (MainScene?.EnableDraw ?? false) MainScene?.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
    }
}