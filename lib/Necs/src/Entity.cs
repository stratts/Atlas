using System.Collections.Generic;

namespace Necs
{
    public class Entity
    {
        private EcsContext _context = new EcsContext();
        private List<Entity> _children = new();

        internal ref ComponentInfo Info => ref _context.GetEntityInfo(Id);
        internal EntityData Data => _context.GetEntityData(Id);

        internal EcsContext Context => _context;

        public ulong Id { get; }
        public ulong Tree => Info.Tree;

        public Entity()
        {
            var info = ComponentInfo.Create();
            info.Name = GetType().Name;
            info.IsEntity = true;
            Id = info.Id;
            _context.AddComponent(info, new EntityData(this));
        }

        public virtual void AddChild(Entity child)
        {
            _context.AddEntity(child);
            _context.AddComponentToEntity(Id, child.Id);
            _children.Add(child);
        }

        public virtual void RemoveChild(Entity child)
        {
            _context.RemoveComponentFromEntity(Id, child.Id);
            _children.Remove(child);
        }

        public void AddComponent<T>(T component)
        {
            _context.AddComponentToEntity(Id, component);
        }

        public ref T GetComponent<T>() => ref _context.GetEntityComponent<T>(Id);

        internal void SetContext(EcsContext context)
        {
            if (_context == context) return;
            _context = context;
            foreach (var child in _children) child.SetContext(context);
        }
    }

    struct EntityData
    {
        public Entity Parent { get; }
        public HashSet<ulong> Children { get; }
        public Dictionary<ulong, byte> Branches { get; }

        public EntityData(Entity parent)
        {
            Parent = parent;
            Branches = new();
            Children = new();
        }
    }
}