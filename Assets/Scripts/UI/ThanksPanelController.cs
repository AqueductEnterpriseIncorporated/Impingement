using UnityEngine;

namespace Impingement.UI
{
    public class ThanksPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject _reportPanel;
        [SerializeField] private GameObject _parent;
        [SerializeField] private MenuPanel _menuPanel;

        public void Close()
        {
            _reportPanel.SetActive(false);
            _parent.SetActive(false);
            _menuPanel.IsOpened = false;
        }
    }
}