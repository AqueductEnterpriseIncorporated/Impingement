using UnityEngine;

namespace Impingement.UI
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _settingsPanel;
        public bool IsOpened;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsOpened)
                {
                    IsOpened = true;
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
            IsOpened = false;
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