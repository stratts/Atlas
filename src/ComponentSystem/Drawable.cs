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
    }

    public class DrawableSystem : BaseComponentSystem<Drawable>, IRenderSystem
    {
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
            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scene.Camera.Zoom));

            foreach (var d in _components)
            {
                if (!d.Enabled) continue;
                var pos = d.Parent.ScenePosition.Floor();
                var bounds = d.DrawBounds != Rectangle.Empty ? d.DrawBounds : new Rectangle(Point.Zero, d.Parent.Size.ToPoint());
                bounds.Location += pos.ToPoint();

                if (bounds.Intersects(scene.Camera.Viewport) || bounds == Rectangle.Empty)
                {
                    d.Draw?.Invoke(spriteBatch, pos - scene.Camera.Position.Floor());
                    //CustomDrawing.DrawRect(bounds.Location.ToVector2() - scene.Camera.Position.Floor(), bounds.Size.ToVector2(), Color.Red * 0.5f);
                }
            }

            spriteBatch.End();
        }
    }
}