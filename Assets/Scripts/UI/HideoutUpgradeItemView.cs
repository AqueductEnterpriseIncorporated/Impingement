using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Impingement.UI
{
    public class HideoutUpgradeItemView : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private UpgradePanelController _upgradePanelController;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private RawImage _image;
        [SerializeField] private Color _notEnoughColor;
        [SerializeField] private Color _enoughColor;
        [Header("Data")]
        public string ItemName;
        public bool IsUnlocked;
        public bool IsEquipped;
        [SerializeField] private string _description;
        [SerializeField] private int _price;
        [SerializeField] private Texture _imageToSet;
        [SerializeField] private GameObject _item;
        [SerializeField] private Transform _itemSpawnPoint;
        [Header("Local")]
        public GameObject LocalPrefab;
        public bool IsSpawned;
        private void OnEnable()
        {
            _upgradePanelController = FindObjectOfType<UpgradePanelController>();
            _nameText.text = ItemName;
            _descriptionText.text = _description;
            _priceText.text = _price.ToString();
            if (_imageToSet != null)
            {
                _image.texture = _imageToSet;
            }
        }

        private void Update()
        {
            if (!IsUnlocked)
            {
                _priceText.color = _upgradePanelController.InteractingPlayer.MyCurrency >= _price
                    ? _enoughColor
                    : _notEnoughColor;
            }
            else
            {
                _priceText.text = "";
            }

            if (IsEquipped)
            {
                _buttonText.text = "Перестать использовать";
                return;
            }
            _buttonText.text = IsUnlocked ? "Использовать" : "Купить";
        }

        public void Buy()
        {
            if (IsUnlocked)
            {
                Equip();
                return;
            }
            
            if (_upgradePanelController.InteractingPlayer.MyCurrency >= _price)
            {
                _upgradePanelController.InteractingPlayer.MyCurrency -= _price;
                IsUnlocked = true;
                Save(); 
            }
        }

        public void SpawnItem()
        {
            //LocalPrefab = PhotonNetwork.Instantiate("/Hideout/" + _item.name, _spawnPoint.position, _spawnPoint.rotation);
            LocalPrefab =
                Instantiate(_item, _itemSpawnPoint.position, _itemSpawnPoint.rotation);
            IsSpawned = true;
        }

        private void DeleteItem(GameObject item)
        {
            //PhotonNetwork.Destroy(item);
            Destroy(item);
        }

        private void Equip()
        {
            if (IsEquipped)
            {
                DeleteItem(LocalPrefab);
                IsEquipped = false;
            }
            else
            {
                SpawnItem();
                IsEquipped = true;
            }
            Save();
        }
        
        private void Save()
        {
            _upgradePanelController.GetController().SaveData(_upgradePanelController.GetAllItems());
        }
    }
}