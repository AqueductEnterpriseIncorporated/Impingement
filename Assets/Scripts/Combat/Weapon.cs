using Impingement.Attributes;
using Photon.Pun;
using UnityEngine;

namespace Impingement.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private GameObject _equippedPrefab = null;
        [SerializeField] private float _weaponRange = 2f;
        [SerializeField] private float _weaponDamage = 5f;
        [SerializeField] private float _weaponPercentageBonus = 0f;
        [SerializeField] private bool _isRightHand = true;
        [SerializeField] private Projectile _projectile = null;

        private const string WeaponName = "Weapon";

        public void Spawn(Transform rightHandTransform, Transform leftHandTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform);
            
            if (_equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHandTransform, leftHandTransform);
                GameObject weapon = Instantiate(_equippedPrefab, handTransform);
                weapon.name = WeaponName;
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

        public void LaunchProjectile(Transform rightHand, Transform leftHand, HealthController target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = PhotonNetwork.Instantiate(_projectile.name, GetTransform(rightHand, leftHand).position,
                Quaternion.identity).GetComponent<Projectile>();
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
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
