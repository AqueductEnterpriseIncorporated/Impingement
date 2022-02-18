using System;
using Impingement.Combat;
using Impingement.Core;
using Impingement.Movement;
using Unity.Netcode;
using UnityEngine;

namespace Impingement.Control
{
    public class AIController: NetworkBehaviour
    {
        [SerializeField] private float _chaseDistance = 5f;
        private CombatController _combatController;
        private HealthController _healthController;
        [SerializeField] GameObject[] _players;
        private PlayerController _target;

        private void Start()
        {
            _combatController = GetComponent<CombatController>();
            _healthController = GetComponent<HealthController>();
        }

        private void Update()
        {
            if (_healthController.IsDead())
            {
                _combatController.Cancel();
                return;
            }
            
            var players = GameObject.FindGameObjectsWithTag("Player");
            if (players.Length == 0) { return; }

            foreach (var player in players)
            {
                if(player.GetComponent<HealthController>().IsDead()) { continue; }
                var distance = Vector3.Distance(player.transform.position, transform.position);

                if (distance <= _chaseDistance)
                {
                    if (_combatController.CanAttack(player))
                    {
                        _combatController.SetTarget(player);
                    }
                }
                else
                {
                    if (_combatController.GetTarget() == null) { continue;}
                    if (Vector3.Distance(_combatController.GetTarget().transform.position, transform.position) >
                        _chaseDistance || !_combatController.CanAttack(_combatController.GetTarget().gameObject))
                    {
                        _combatController.Cancel();
                    }
                }
            }

        }
    }
}