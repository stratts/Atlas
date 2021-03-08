
namespace Atlas
{
    public interface IUpdateContext
    {
        float ElapsedTime { get; }
    }

    public class UpdateContext : IUpdateContext
    {
        public float ElapsedTime { get; internal set; }
    }
}