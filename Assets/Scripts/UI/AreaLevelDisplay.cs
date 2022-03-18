using Impingement.Dungeon;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Impingement.UI
{
    public class AreaLevelDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private GameObject _textParent;
        private DungeonProgressionManager _dungeonProgressionManager;

        private void Awake()
        {
            _dungeonProgressionManager = FindObjectOfType<DungeonProgressionManager>();
        }

        private void Update()
        {
            if (SceneManager.GetActiveScene().name == "Dungeon")
            {
                _textParent.SetActive(true);

                _tmpText.text = _dungeonProgressionManager.AreaLevel.ToString();
            }
            else
            {
                _textParent.SetActive(false);
            }
        }
    }
}