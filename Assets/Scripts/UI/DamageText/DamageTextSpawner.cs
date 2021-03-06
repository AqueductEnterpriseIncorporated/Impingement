using UnityEngine;

namespace Impingement.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText _damageTextPrefab;
        
        public void Spawn(float damage)
        {
            if(PlayerPrefs.GetInt("ShowDamageText") == 0) { return; }
            var damageTextPrefab = Instantiate(_damageTextPrefab, transform.position, Quaternion.identity);
            damageTextPrefab.SetText(damage.ToString());
        }
    }
}