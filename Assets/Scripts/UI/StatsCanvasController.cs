using UnityEngine;

namespace Impingement.UI
{
    public class StatsCanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject _canvasParent;
        [SerializeField] private KeyCode _toggleKey = KeyCode.I;

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                _canvasParent.SetActive(!_canvasParent.activeSelf);
            }
        }
    }
}
