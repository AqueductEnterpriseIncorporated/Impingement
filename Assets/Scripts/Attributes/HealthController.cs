﻿using System;
using GameDevTV.Utils;
using Impingement.Core;
using Impingement.enums;
using Impingement.Stats;
using Photon.Pun;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace Impingement.Attributes
{
    public class HealthController : MonoBehaviour, IPunObservable, ISaveable
    {
        [Serializable] public class TakeDamageEvent : UnityEvent<float> { }
        [SerializeField] private UnityEvent _onDie;
        [SerializeField] private BaseStats _baseStats = null;
        [SerializeField] private TakeDamageEvent _takeDamage;
        [SerializeField] private bool _showDamageText;
        [SerializeField] private bool _isDead;
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


        public void TakeDamage(GameObject instigator, float damage)
        {
            _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);
            if(_showDamageText)
            {
                _takeDamage.Invoke(damage);
            }
            if (_healthPoints.value == 0)
            {
                _onDie.Invoke();
                _photonView.RPC(nameof(DieRPC), RpcTarget.AllBufferedViaServer);
                AwardExperience(instigator);
                //DieRPC();
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

        public object CaptureState()
        {
            return _healthPoints;
        }

        public void RestoreState(object state)
        {
            _healthPoints.value = (float)state;
            if (_healthPoints.value == 0)
            {
                DieRPC();
            }
        }
    }
}