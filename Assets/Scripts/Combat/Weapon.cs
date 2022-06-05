using System;
using System.Globalization;
using Impingement.Attributes;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Impingement.Combat
{
    public class Weapon : MonoBehaviour
    {
        public Collider WeaponCollider;
        public float ColliderTimer;
        public bool IsHitted;
        public bool IsSplash;
        [SerializeField] private UnityEvent _onHit;
        [SerializeField] private CombatController _combatController;
        [SerializeField] private MeshRenderer _meshRenderer;
        
        public void SetCombatController(CombatController combatController)
        {
            _combatController = combatController;
        }

        public void SetColor()
        {
            try
            {
                if (!PlayerPrefs.HasKey("SwordColor"))
                {
                    var randomColor = new Color(Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256), 1);
                    _meshRenderer.material.color = randomColor;
                }
                else
                {
                    string colorCode = PlayerPrefs.GetString("SwordColor");
                    ColorUtility.TryParseHtmlString(colorCode, out var newColor);
                    _meshRenderer.material.color = newColor;
                }
            }
            catch
            {
                //ignored
            }
        }

        public void SaveColor()
        {
            PlayerPrefs.SetString("SwordColor", _meshRenderer.material.color.ToString());
        }
        
        public void OnHit()
        {
            _onHit.Invoke();
        }

        public void DetectCollider(Collider other)
        {
            if (other.TryGetComponent<HealthController>(out var target))
            {
                if (!target.IsPlayer && !_combatController.GetHealthController().IsPlayer)
                {
                    return;
                }

                if (IsSplash)
                {
                    IsHitted = false;
                }

                if (!IsHitted)
                {
                    if (target.GetComponent<CombatController>() == _combatController)
                    {
                        return;
                    }

                    _combatController.DealDamageAI(target);
                }

                IsHitted = true;
            }
        }
    }
}