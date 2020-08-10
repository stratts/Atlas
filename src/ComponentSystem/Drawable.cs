using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Industropolis.Engine
{
    public delegate void DrawMethod(SpriteBatch spriteBatch, Vector2 position);

    public class Scissor : Component
    {
        public LayoutBorder Bounds { get; set; }
    }

    public class Drawable : Component
    {
        public Rectangle DrawBounds { get; set; } = Rectangle.Empty;
        float Opacity { get; set; }
        public DrawMethod? Draw { get; set; }
        public Rectangle? ScissorArea { get; set; }
    }

    public class ScissorSystem : BaseComponentSystem<Scissor>
    {
        public ScissorSystem()
        {
            ComponentRemoved += c => DisableScissor(c.Parent);
        }

        public override void UpdateComponents(Scene scene, IReadOnlyList<Scissor> components, float elapsed)
        {
            foreach (var c in components)
            {
                var areaPos = new Vector2(c.Bounds.Left, c.Bounds.Top);
                var areaSize = (c.Parent.Size - areaPos - new Vector2(c.Bounds.Right, c.Bounds.Bottom)).ToPoint();
                EnableScissor(c.Parent, new Rectangle(areaPos.ToPoint(), areaSize));
            }
        }

        private void DisableScissor(Node node)
        {
            if (node.GetComponent<Drawable>() is Drawable d) d.ScissorArea = null;
            foreach (var child in node.Children) DisableScissor(node);
        }

        private void EnableScissor(Node node, Rectangle area)
        {
            if (node.GetComponent<Drawable>() is Drawable d) d.ScissorArea = area;

            foreach (var child in node.Children)
            {
                var offsetArea = area;
                offsetArea.Offset(-child.Position);
                EnableScissor(child, offsetArea);
            }
        }
    }

    public class DrawableSystem : BaseComponentSystem<Drawable>, IRenderSystem
    {
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
            ProcessChanges();
            var matrix = Matrix.CreateScale(scene.Camera.Zoom);
            spriteBatch.Begin(transformMatrix: matrix);

            foreach (var d in _components)
            {
                if (!d.Enabled) continue;
                var pos = d.Parent.ScenePosition.Floor();
                var bounds = d.DrawBounds != Rectangle.Empty ? d.DrawBounds : new Rectangle(Point.Zero, d.Parent.Size.ToPoint());
                bounds.Location += pos.ToPoint();

                if (d.ScissorArea.HasValue)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(transformMatrix: matrix,
                        rasterizerState: new RasterizerState() { ScissorTestEnable = true });

                    var r = d.ScissorArea.Value;
                    r.Offset(pos - scene.Camera.Position.Floor());

                    spriteBatch.GraphicsDevice.ScissorRectangle = new Rectangle(
                        Vector2.Transform(r.Location.ToVector2(), matrix).ToPoint(),
                        Vector2.Transform(r.Size.ToVector2(), matrix).ToPoint());
                }

                if (bounds.Intersects(scene.Camera.Viewport) || bounds == Rectangle.Empty)
                {
                    d.Draw?.Invoke(spriteBatch, pos - scene.Camera.Position.Floor());
                    //CustomDrawing.DrawRect(bounds.Location.ToVector2() - scene.Camera.Position.Floor(), bounds.Size.ToVector2(), Color.Red * 0.5f);
                    /*if (d.ScissorArea.HasValue)
                    {
                        var s = d.ScissorArea.Value;
                        CustomDrawing.DrawRect(s.Location.ToVector2() + pos - scene.Camera.Position.Floor(), s.Size.ToVector2(), Color.Blue * 0.5f);
                    }*/
                }

                if (d.ScissorArea.HasValue)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(transformMatrix: matrix);
                }
            }

            spriteBatch.End();
        }
    }
}