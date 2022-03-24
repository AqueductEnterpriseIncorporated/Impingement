using Impingement.Attributes;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private Weapon _equippedPrefab = null;
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 5f;
        [SerializeField] private float _weaponPercentageBonus = 0f;
        [SerializeField] private float _staminaAmountToSpend;
        [SerializeField] private bool _useStamina;
        [SerializeField] private bool _isRightHand = true;
        [SerializeField] private Projectile _projectile = null;

        private const string WeaponName = "Weapon";

        public bool GetUseStamina()
        {
            return _useStamina;
        }
        
        public float GetStaminaPointsToSpend()
        {
            return _staminaAmountToSpend;
        }
        
        public Weapon Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);

            Weapon weapon = null;
            
            if (_equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);
                weapon = Instantiate(_equippedPrefab, handTransform);
                weapon.gameObject.name = WeaponName;
            }
            
            var  overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform oldWeapon = rightHandTransform.Find(WeaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHandTransform.Find(WeaponName);
            }
            if(oldWeapon == null) { return; }

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            return _isRightHand ? rightHandTransform : leftHandTransform;
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, HealthController target,
            GameObject instigator, float calculatedDamage, Vector3 direction = default)
        {
            Projectile projectileInstance = null;
            if (PhotonNetwork.InRoom)
            {
                projectileInstance = PhotonNetwork.Instantiate("Weapons/" + _projectile.name,
                    GetTransform(rightHand, leftHand).position,
                    Quaternion.identity).GetComponent<Projectile>();
            }
            else
            {
                projectileInstance = Instantiate(_projectile,
                    GetTransform(rightHand, leftHand).position,
                    Quaternion.identity).GetComponent<Projectile>();
            }

            if (target != null)
            {
                projectileInstance.SetTarget(target, instigator, calculatedDamage);
                return;
            }

            projectileInstance.SetDirection(direction, instigator, calculatedDamage);
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }
        
        public float GetPercentageBonus()
        {
            return _weaponPercentageBonus;
        }
        
        public float GetRange()
        {
            return _weaponRange;
        }
    }
}
