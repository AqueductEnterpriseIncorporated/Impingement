using Impingement.Control;
using UnityEngine;

namespace Impingement.UI
{
    public class PlayersPanel: MonoBehaviour
    {
        [SerializeField] public GameObject PanelParent;
        [SerializeField] private PlayerPanelItem _panelItem;

        public void AddPlayer(PlayerController playerController)
        {
            var localPrefab = Instantiate(_panelItem, PanelParent.transform, true);
            localPrefab.SetupPlayer(playerController);
        }
    }
}