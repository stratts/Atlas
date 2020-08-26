using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public delegate void DrawMethod(SpriteBatch spriteBatch, Vector2 position);

    public class Drawable : Component
    {
        public Rectangle DrawBounds { get; set; } = Rectangle.Empty;
        float Opacity { get; set; }
        public DrawMethod? Draw { get; set; }
        public Rectangle? ScissorArea { get; set; }
    }

    public class DrawableSystem : BaseComponentSystem<Drawable>, IRenderSystem
    {
        private Rectangle? _currentScissor;

        public IEnumerable<Drawable> Components => _components;

        public override void UpdateComponents(Scene scene, IReadOnlyList<Drawable> components, float elapsed)
        {

        }

        protected override int SortMethod(Drawable a, Drawable b)
        {
            return a.Priority.CompareTo(b.Priority);
        }

        public void Draw(Scene scene, SpriteBatch spriteBatch)
        {
            var samplerState = scene.NearestNeighbour ? SamplerState.PointClamp : null;
            ProcessChanges();
            var matrix = Matrix.CreateScale(scene.Camera.Zoom);
            spriteBatch.Begin(transformMatrix: matrix, samplerState: samplerState);

            foreach (var d in _components)
            {
                if (!d.Enabled) continue;
                var pos = d.Parent.ScenePosition.Floored();
                var bounds = d.DrawBounds != Rectangle.Empty ? d.DrawBounds : new Rectangle(Point.Zero, d.Parent.Size.ToPoint());
                bounds.Location += pos.ToPoint();

                if (d.ScissorArea.HasValue)
                {
                    var r = d.ScissorArea.Value;
                    r.Offset(pos - Vector2.Floor(scene.Camera.Position));

                    if (r != _currentScissor)
                    {
                        spriteBatch.End();
                        spriteBatch.Begin(transformMatrix: matrix, samplerState: samplerState,
                            rasterizerState: new RasterizerState() { ScissorTestEnable = true });
                        _currentScissor = r;
                    }

                    spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                        Vector2.Transform(r.Location.ToVector2(), matrix).ToPoint(),
                        Vector2.Transform(r.Size.ToVector2(), matrix).ToPoint());
                }
                else if (_currentScissor != null)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(transformMatrix: matrix, samplerState: samplerState);
                    _currentScissor = null;
                }

                if (bounds.Intersects(scene.Camera.Viewport) || bounds == Rectangle.Empty)
                {
                    d.Draw?.Invoke(spriteBatch, pos - scene.Camera.Position.Floored());
                    //CustomDrawing.DrawRect(bounds.Location.ToVector2() - scene.Camera.Position.Floor(), bounds.Size.ToVector2(), Color.Red * 0.5f);
                    /*if (d.ScissorArea.HasValue)
                    {
                        var s = d.ScissorArea.Value;
                        CustomDrawing.DrawRect(s.Location.ToVector2() + pos - scene.Camera.Position.Floor(), s.Size.ToVector2(), Color.Blue * 0.5f);
                    }*/
                }
            }

            spriteBatch.End();
        }
    }
}