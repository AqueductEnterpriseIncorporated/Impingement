using System;
using Impingement.Stats;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Impingement.Inventory
{
    public class RandomDropper : ItemDropper
    {
        [Tooltip("How far can the pickups be spawned from the dropper.")]
        [SerializeField] private float _distance = 1;
        [SerializeField] private DropLibrary _dropLibrary;
        [SerializeField] private BaseStats _baseStats;
        private PhotonView _photonView;
        
        const int ATTEMPTS = 30;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        public void RandomDrop()
        {
            var drops = _dropLibrary.GetRandomDrops(_baseStats.GetLevel());
            foreach (var drop in drops)
            {
                DropItem(drop.Item, drop.Number);
            }
        }

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < ATTEMPTS; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * _distance;
                if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out var hit, 0.1f,
                        UnityEngine.AI.NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }
            
            return transform.position;
        }
    }
}