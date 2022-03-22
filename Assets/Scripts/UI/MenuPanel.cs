using UnityEngine;

namespace Impingement.UI
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject[] _objectsToClose;
        public bool IsOpened;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsOpened)
                {
                    Open();
                }
                else
                {
                    CloseAll();
                }
            }
        }

        public void Open()
        {
            _menuPanel.SetActive(true);
            IsOpened = true;
        }

        public void Close()
        {
            //IsOpened = false;
            _menuPanel.SetActive(false);
        }

        private void CloseAll()
        {
            foreach (var uiObject in _objectsToClose)
            {
                uiObject.SetActive(false);
            }
            IsOpened = false;
        }
        
        public void Exit()
        {
            Application.Quit();
        }
    }
}