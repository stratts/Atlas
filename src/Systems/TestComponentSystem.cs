using System.Collections.Generic;

namespace Industropolis
{
    public class TestComponentSystem : BaseComponentSystem<TestComponent>
    {
        private HashSet<Node> seen = new HashSet<Node>();

        public override void UpdateComponents(IReadOnlyList<TestComponent> components, float elapsed)
        {
            foreach (var component in components)
            {
                var node = component.Parent;
                if (seen.Contains(node)) continue;
                seen.Add(node);
                System.Console.WriteLine($"Processed a new TestComponent attached to {node.GetType().Name}");
            }
        }
    }
}