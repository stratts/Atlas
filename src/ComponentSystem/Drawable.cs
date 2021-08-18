using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Necs;

namespace Atlas
{
    public delegate void DrawMethod(SpriteBatch spriteBatch, DrawContext context);

    public struct Drawable
    {
        public Rectangle DrawBounds { get; set; }
        public DrawMethod? Draw { get; set; }
        public Rectangle? ScissorArea { get; set; }
        public Modulate? Modulate { get; set; }
    }

    public struct DrawContext
    {
        public SpriteBatch SpriteBatch;
        public Vector2 Position;
        public Vector2 Size;
        public Modulate Modulate;
    }

    public struct RenderContext
    {
        public Scene Scene { get; }
        public SpriteBatch SpriteBatch { get; }

        public RenderContext(Scene scene, SpriteBatch spriteBatch) => (Scene, SpriteBatch) = (scene, spriteBatch);
    }

    public class DrawableSystem : IComponentSystem<RenderContext>
    {
        private Rectangle? _currentScissor;
        private SamplerState? _samplerState;
        private Matrix _matrix;
        private Vector2 _camera;
        private Rectangle _viewport;
        private Modulate _disabledModulate = new Modulate() { Enabled = false };

        public void BeforeProcess(RenderContext context)
        {
            _samplerState = context.Scene.NearestNeighbour ? SamplerState.PointClamp : null;
            var matrix = Matrix.CreateScale(context.Scene.Camera.Zoom);
            context.SpriteBatch.Begin(transformMatrix: matrix, samplerState: _samplerState);
            _camera = context.Scene.Camera.Position;
            _viewport = context.Scene.Camera.Viewport;
        }

        public void Process(RenderContext context, EcsContext ecs)
        {
            ecs.Query((ref Drawable d, ref Transform t) =>
            {
                var spriteBatch = context.SpriteBatch;
                var pos = t.ScenePos.Floored();
                var bounds = d.DrawBounds != Rectangle.Empty ? d.DrawBounds : new Rectangle(Point.Zero, t.Size.ToPoint());
                bounds.Location += pos.ToPoint();

                if (d.ScissorArea.HasValue)
                {
                    var r = d.ScissorArea.Value;
                    r.Offset(pos - Vector2.Floor(_camera));

                    if (r != _currentScissor)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(transformMatrix: _matrix, samplerState: _samplerState,
                            rasterizerState: new RasterizerState() { ScissorTestEnable = true });
                        _currentScissor = r;
                    }

                    spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                        Vector2.Transform(r.Location.ToVector2(), _matrix).ToPoint(),
                        Vector2.Transform(r.Size.ToVector2(), _matrix).ToPoint());
                }
                else if (_currentScissor != null)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(transformMatrix: _matrix, samplerState: _samplerState);
                    _currentScissor = null;
                }

                if (bounds.Intersects(_viewport) || bounds == Rectangle.Empty)
                {
                    var drawContext = new DrawContext()
                    {
                        SpriteBatch = spriteBatch,
                        Position = pos - _camera.Floored(),
                        Size = t.Size,
                        Modulate = d.Modulate != null ? d.Modulate : _disabledModulate
                    };
                    //d.AltPriority = (ulong)Math.Abs(pos.X);
                    d.Draw?.Invoke(spriteBatch, drawContext);
                    //CustomDrawing.DrawRect(bounds.Location.ToVector2() - scene.Camera.Position.Floor(), bounds.Size.ToVector2(), Color.Red * 0.5f);
                    /*if (d.ScissorArea.HasValue)
                    {
                        var s = d.ScissorArea.Value;
                        CustomDrawing.DrawRect(s.Location.ToVector2() + pos - scene.Camera.Position.Floor(), s.Size.ToVector2(), Color.Blue * 0.5f);
                    }*/
                }
            });
        }

        public void AfterProcess(RenderContext context)
        {
            context.SpriteBatch.End();
        }
    }
}