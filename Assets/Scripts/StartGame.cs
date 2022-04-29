using Impingement.Control;
using Photon.Pun;
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
            if (PhotonNetwork.InRoom)
            {
                if(!PhotonNetwork.IsMasterClient){return;}
            }
            _startGamePanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PhotonNetwork.InRoom)
            {
                if(!PhotonNetwork.IsMasterClient){return;}
            }
            _startGamePanel.SetActive(false);
        }
    }
}
