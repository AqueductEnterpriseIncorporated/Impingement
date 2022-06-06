using System;
using System.Collections.Generic;
using Impingement.Attributes;
using Impingement.Control;
using Impingement.enums;
using Impingement.Inventory;
using Impingement.Stats;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Impingement.UI
{
    public class PlayerStatPanel : MonoBehaviour
    {
        [SerializeField] private Transform _parentTransform;
        [SerializeField] private BaseStats _baseStats;
        [SerializeField] private Transform _contentTransform;
        [SerializeField] private PlayerStatGridItem _playerStatGridItem;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _playerLevelText;
        [SerializeField] private InputManager _inputManager;
        [SerializeField] private EquipmentController _equipmentController;
        private Dictionary<string,enumStats> _statsList;

        private void Awake()
        {
            _statsList = new Dictionary<string, enumStats>() { {"Урон", enumStats.Damage},
                {"Здоровье", enumStats.Health},{"Выносливость", enumStats.Stamina},{"Восстановление здоровья", enumStats.HealthRegen},
                {"Восстановление выносливости", enumStats.StaminaRegen}};
            _equipmentController.OnEquipmentUpdated += UpdateStats;
        }

        private void Update()
        {
            if (_inputManager.GetKeyDown("Информация о персонаже"))
            {
                _parentTransform.gameObject.SetActive(!_parentTransform.gameObject.activeSelf);
                if (_parentTransform.gameObject.activeSelf)
                {
                    UpdateStats();
                }
            }
        }

        private void UpdateStats()
        {
            _playerNameText.text = PhotonNetwork.NickName;
            _playerLevelText.text = _baseStats.GetLevel().ToString();
            
            for (int i = 0; i < _contentTransform.childCount; i++)
            {
                Destroy(_contentTransform.GetChild(i).gameObject);
            }

            foreach (var stat in _statsList)
            {
                var newItem = Instantiate(_playerStatGridItem, _contentTransform);
                newItem.Setup(stat.Key, _baseStats.GetStat(stat.Value).ToString());
            }
        }
    }
}