using Impingement.Dungeon;
using TMPro;
using UnityEngine;

namespace Impingement.UI
{
    public class AreaLevelDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _tmpText;
        [SerializeField] private GameObject _textParent;
        [SerializeField] private DungeonProgressionManager _dungeonProgressionManager;

        private void Awake()
        {
            _dungeonProgressionManager = FindObjectOfType<DungeonProgressionManager>();
        }

        private void Update()
        { 
            _tmpText.text = _dungeonProgressionManager.AreaLevel.ToString();
        }
    }
}