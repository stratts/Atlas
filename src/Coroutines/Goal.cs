using System.Collections.Generic;

namespace Atlas
{
    public class BaseGoal<T> where T : IUpdateContext
    {
        private Coroutine<T>? _currentAction;
        private int _currentIdx;
        private List<CoroutineAction<T>> _actions = new List<CoroutineAction<T>>();
        private bool _complete = false;

        public BaseGoal() { }

        public BaseGoal(CoroutineAction<T> action) => AddAction(action);

        public CoroutineAction<T> ToAction()
        {
            IEnumerator<bool> Action(T context)
            {
                var goal = new BaseGoal<T>();
                goal.AddActions(_actions);

                while (true)
                {
                    if (goal.Update(context)) break;
                    yield return false;
                }

                yield return true;
            }

            return Action;
        }

        public void Reset()
        {
            _currentAction = null;
            _complete = false;
            _currentIdx = 0;
        }

        public bool Update(T context)
        {
            if (_complete || _actions.Count == 0) return true;
            if (_currentAction == null) _currentAction = new Coroutine<T>(_actions[_currentIdx]);
            var actionComplete = _currentAction.Update(context);
            if (actionComplete)
            {
                _currentIdx++;

                if (_currentIdx >= _actions.Count)
                {
                    _complete = true;
                    return true;
                }
                else
                {
                    _currentAction = new Coroutine<T>(_actions[_currentIdx]);
                    return false;
                }
            }
            return false;
        }

        public void AddAction(CoroutineAction<T> action) => _actions.Add(action);

        public void AddActions(IEnumerable<CoroutineAction<T>> actions)
        {
            foreach (var action in actions) AddAction(action);
        }
    }
}