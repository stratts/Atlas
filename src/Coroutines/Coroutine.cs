using System.Collections.Generic;

namespace Atlas
{
    public delegate IEnumerator<bool> CoroutineAction<T>(T context) where T : IUpdateContext;

    public class Coroutine<T> where T : IUpdateContext
    {
        private CoroutineAction<T> _action;
        private IEnumerator<bool>? _current;

        public bool Completed { get; private set; }

        public Coroutine(CoroutineAction<T> action) => _action = action;

        public bool Update(T context)
        {
            if (Completed) return true;
            if (_current == null) _current = _action.Invoke(context);
            Completed = !_current.MoveNext() || _current.Current == true;
            return Completed;
        }

        public void Reset()
        {
            Completed = false;
            _current = null;
        }
    }
}