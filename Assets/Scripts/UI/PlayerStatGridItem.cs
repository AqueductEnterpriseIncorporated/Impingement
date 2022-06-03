using TMPro;
using UnityEngine;

namespace Impingement.UI
{
    public class PlayerStatGridItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _statName;
        [SerializeField] private TMP_Text _statValue;

        public void Setup(string statName, string statValue)
        {
            _statName.text = statName;
            _statValue.text = statValue;
        }
    }
}