using TMPro;
using UnityEngine;

namespace Impingement.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] private BaseStats _baseStats;
        [SerializeField] private TMP_Text _expValueText;

        private void Update()
        {
            _expValueText.text = _baseStats.GetLevel().ToString();
        }
    }
}