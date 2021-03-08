using System.Collections.Generic;

namespace Atlas
{
    public class Goal
    {
        private Coroutine? _currentAction;
        private int _currentIdx;
        private List<CoroutineAction> _actions = new List<CoroutineAction>();
        private bool _complete = false;

        public CoroutineAction ToAction()
        {
            IEnumerator<bool> Action(IUpdateContext context)
            {
                var goal = new Goal();
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

        public bool Update(IUpdateContext context)
        {
            if (_complete || _actions.Count == 0) return true;
            if (_currentAction == null) _currentAction = new Coroutine(_actions[_currentIdx]);
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
                    _currentAction = new Coroutine(_actions[_currentIdx]);
                    return false;
                }
            }
            return false;
        }

        public void AddAction(CoroutineAction action) => _actions.Add(action);

        public void AddActions(IEnumerable<CoroutineAction> actions)
        {
            foreach (var action in actions) AddAction(action);
        }
    }
}