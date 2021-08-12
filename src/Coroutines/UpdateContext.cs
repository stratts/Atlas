
namespace Atlas
{
    public interface IUpdateContext
    {
        float ElapsedTime { get; }
    }

    public class UpdateContext : IUpdateContext
    {
        public Scene Scene { get; internal set; }
        public float ElapsedTime { get; internal set; }

        public UpdateContext(Scene scene) => Scene = scene;

        public void Update(IUpdateContext context)
        {
            ElapsedTime = context.ElapsedTime;
        }
    }
}