using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Atlas
{
    public class EngineCore
    {
        public Point ScreenSize { get; set; }
        public string ContentDirectory { get; set; } = ".";
        public string Font { get; set; } = "";

        public void Start<T>() where T : IScene, new()
        {
            using (var g = new MainGame())
            {
                g.ScreenSize = ScreenSize;
                g.Content.RootDirectory = ContentDirectory;
                g.SceneBuilder = () => new T();
                Config.ContentPath = ContentDirectory;
                FontService.SetFont(Path.Join(ContentDirectory, Font));
                g.Run();
            }
        }
    }

    public static class Config
    {
        public static Point ScreenSize { get; internal set; }
        public static GameWindow? CurrentWindow { get; internal set; }
        public static string ContentPath { get; internal set; } = ".";
        public static GraphicsDevice GraphicsDevice { get; internal set; } = null!;
    }

#nullable disable

    internal class MainGame : Game
    {
        private Point _screenSize;
        private GraphicsDeviceManager _gdm;
        private SpriteBatch _spriteBatch;
        private IScene _scene;

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

        public Func<IScene> SceneBuilder { get; set; }

        public MainGame()
        {
            _gdm = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _spriteBatch = new SpriteBatch(this.GraphicsDevice);
            Config.GraphicsDevice = GraphicsDevice;

            _gdm.IsFullScreen = false;
            _gdm.SynchronizeWithVerticalRetrace = true;
            _gdm.ApplyChanges();

            Config.CurrentWindow = Window;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            CustomDrawing.Init(_spriteBatch);
            _scene = SceneBuilder();

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
            if (_scene != null && _scene.EnableUpdate) _scene.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (_scene != null && _scene.EnableDraw) _scene.Draw(_spriteBatch);
            base.Draw(gameTime);
        }
    }
}