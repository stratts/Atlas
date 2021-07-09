
namespace Atlas
{
    public interface IUpdateContext
    {
        float ElapsedTime { get; }
    }

    public class UpdateContext : IUpdateContext
    {
        public float ElapsedTime { get; internal set; }

        public void Update(IUpdateContext context) {
            ElapsedTime = context.ElapsedTime;
        }
    }
}