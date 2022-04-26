using Impingement.Control;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameObject _UI;
    [SerializeField] private GameObject _startGamePanel;
    private PlayerController _playerController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerController = other.GetComponent<PlayerController>();
            if(!_playerController.GetPhotonView().IsMine){return;}
            _startGamePanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!_playerController.GetPhotonView().IsMine){return;}

            _startGamePanel.SetActive(false);
        }
    }
}
