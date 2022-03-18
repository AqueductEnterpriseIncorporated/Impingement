using UnityEngine;

namespace Impingement.UI
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _settingsPanel;
        private bool _isOpened;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!_isOpened)
                {
                    _isOpened = true;
                    _menuPanel.SetActive(true);
                }
                else
                {
                    Resume();
                }
            }
        }

        public void Resume()
        {
            _isOpened = false;
            _menuPanel.SetActive(false);
        }

        public void Settings()
        {
            
        }
        
        public void Exit()
        {
            Application.Quit();
        }
    }
}