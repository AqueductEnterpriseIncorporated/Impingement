using UnityEngine;

namespace Impingement.Core
{
    public class ActionScheduleController : MonoBehaviour
    {
        private IAction _action;

        public void StartAction(IAction action)
        {
            if(_action == action) { return; }

            _action?.Cancel();

            _action = action;
        }

        public void CancelCurrentAction()
        {
            StartAction(null);
        }
    }
}