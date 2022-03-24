using Impingement.Attributes;
using UnityEngine;
using UnityEngine.Events;

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
        
        public void SetCombatController(CombatController combatController)
        {
            _combatController = combatController;
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