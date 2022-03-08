using Impingement.Core;
using Impingement.enums;
using Impingement.Stats;
using Photon.Pun;
using RPG.Saving;
using UnityEngine;

namespace Impingement.Resources
{
    public class HealthController : MonoBehaviour, IPunObservable, ISaveable
    {
        private float _healthPoints = -1f;
        [SerializeField] private bool _isDead;
        [SerializeField] private BaseStats _baseStats = null;
        private PhotonView _photonView;

        private void Start()
        {
            if (_healthPoints < 0)
            {
                _healthPoints = _baseStats.GetStat(enumStats.Health);
            }
            _photonView = GetComponent<PhotonView>();
            _baseStats.OnLevelUp += RegenerateHealth;
        }

        private void OnDestroy()
        {
            _baseStats.OnLevelUp -= RegenerateHealth;
        }

        public bool IsDead()
        {
            return _isDead;
        }

        //public event Death OnDeath;

        public void TakeDamage(GameObject instigator, float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);
            if (_healthPoints == 0)
            {
                _photonView.RPC(nameof(DieRPC), RpcTarget.AllBufferedViaServer);
                AwardExperience(instigator);
                //DieRPC();
            }
        }

        public float GetHealthPoints()
        {
            return _healthPoints;
        }
        
        public float GetMaxHealthPoints()
        {
            return _baseStats.GetStat(enumStats.Health);
        }

        [PunRPC]
        private void DieRPC()
        {
            if (_isDead) { return; }

            _isDead = true;
            AnimationController animationController = GetComponent<AnimationController>();
            animationController.PlayTriggerAnimation("die");
            //_photonView.RPC(nameof(animationController.PlayTriggerAnimation), RpcTarget.AllBufferedViaServer, "die");
            GetComponent<ActionScheduleController>().CancelCurrentAction();
            _photonView.RPC(nameof(SyncHealthState), RpcTarget.AllBufferedViaServer, true);
        }
        
        private void AwardExperience(GameObject instigator)
        {
            if(instigator.TryGetComponent<ExperienceController>(out var experienceController))
            {
                experienceController.GainExperience((int) GetComponent<BaseStats>().GetStat(enumStats.ExperienceReward));
            }
        }

        [PunRPC]
        private void SyncHealthState(bool value)
        {
            _isDead = value;
        }
        
        private void RegenerateHealth()
        {
            _healthPoints = GetMaxHealthPoints();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_isDead);
            }
            else
            {
                _isDead = (bool)stream.ReceiveNext();
            }
        }

        public object CaptureState()
        {
            return _healthPoints;
        }

        public void RestoreState(object state)
        {
            _healthPoints = (float)state;
            if (_healthPoints == 0)
            {
                DieRPC();
            }
        }
    }
}