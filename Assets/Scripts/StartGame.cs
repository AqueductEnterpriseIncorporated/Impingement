using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject _UI;
    [SerializeField] private GameObject _startGamePanel;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _UI.SetActive(true);
            _startGamePanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _UI.SetActive(false);
            _startGamePanel.SetActive(false);
        }
    }
}
