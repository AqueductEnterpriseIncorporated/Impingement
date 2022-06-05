using System;
using GameDevTV.Utils;
using Impingement.Combat;
using Impingement.Core;
using Impingement.enums;
using Impingement.Playfab;
using Impingement.Stats;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Impingement.Attributes
{
    public class HealthController : MonoBehaviour, IPunObservable
    {
        public string CharacterName;

        public bool IsPlayer;
        public bool IsInvulnerable;

        [Serializable] public class TakeDamageEvent : UnityEvent<float> { }

        [SerializeField] private AudioSource[] _takeDamageClips;
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private BaseStats _baseStats = null;
        [SerializeField] private TakeDamageEvent _takeDamage;
        [SerializeField] private PlayfabPlayerDataController _playfabPlayerDataController;
        [SerializeField] private CombatTarget _combatTarget;
        [SerializeField] private bool _showDamageText;
        [SerializeField] private bool _isDead;
        [SerializeField] private GameObject _deathPanel;
        private LazyValue<float> _healthPoints;
        private PhotonView _photonView;

        private void Awake()
        {
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return _baseStats.GetStat(enumStats.Health);
        }

        private void Start()
        {
            _healthPoints.ForceInit();
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

        public CombatTarget GetCombatTarget()
        {
            return _combatTarget;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            if (!IsInvulnerable)
            {
                _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);
            }

            if(_showDamageText)
            {
                _takeDamage.Invoke(damage);
            }
            if (_healthPoints.value == 0)
            {
                if (IsPlayer)
                {
                    _deathPanel.SetActive(true);
                    _playfabPlayerDataController.ResetPlayerData();
                }
                else
                {
                    GetComponent<Collider>().enabled = false;
                }
                _onDie.Invoke();
                if (PhotonNetwork.InRoom)
                {
                    _photonView.RPC(nameof(DieRPC), RpcTarget.AllBufferedViaServer);
                }
                else
                {
                    DieRPC();
                }

                AwardExperience(instigator);
                //DieRPC();
            }
            else
            {
                _takeDamageClips[Random.Range(0, _takeDamageClips.Length)].Play();
            }
        }

        public float GetHealthPoints()
        {
            return _healthPoints.value;
        }
        
        public float GetMaxHealthPoints()
        {
            return _baseStats.GetStat(enumStats.Health);
        }
        
        public void Heal(float healthToRestore)
        {
            _healthPoints.value = Mathf.Min(_healthPoints.value + healthToRestore, GetMaxHealthPoints());
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
            if (PhotonNetwork.InRoom)
            {
                _photonView.RPC(nameof(SyncHealthState), RpcTarget.AllBufferedViaServer, true);
            }
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
            _healthPoints.value = GetMaxHealthPoints();
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
    }
}