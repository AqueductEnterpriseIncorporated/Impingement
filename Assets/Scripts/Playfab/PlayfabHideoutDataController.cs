using System;
using System.Collections.Generic;
using Impingement.Serialization.SerializationClasses;
using Impingement.SerializationAPI;
using Impingement.UI;
using PlayFab.ClientModels;
using UnityEngine;

namespace Impingement.Playfab
{
    public class PlayfabHideoutDataController : MonoBehaviour
    {
        [SerializeField] UpgradePanelController _upgradePanelController;  
        private List<HideoutUpgradeItemView> _itemViews = null;

        private void Start()
        {
            _itemViews = new List<HideoutUpgradeItemView>(_upgradePanelController.GetAllItems());
            FindObjectOfType<PlayfabManager>().LoadJson(OnDataReceived);   
        }
        
        private void OnDataReceived(GetUserDataResult result)
        {
            if (result != null && result.Data.ContainsKey("HideoutData") && result.Data["HideoutData"].Value != null)
            {            
                var json = result.Data["HideoutData"].Value;
                if(json.Length <= 1) {return;}
                var hideoutData = (SerializableHideoutData) StringSerializationAPI.Deserialize(typeof(SerializableHideoutData), json);
                
                for (int i = 0; i < hideoutData.UpgradeItems.Count; i++)
                {
                    for (int j = 0; j < _itemViews.Count; j++)
                    {
                        if (_itemViews[j].ItemName == hideoutData.UpgradeItems[i].Name)
                        {
                            _itemViews[j].IsUnlocked = hideoutData.UpgradeItems[i].Unlocked;
                            _itemViews[j].IsEquipped = hideoutData.UpgradeItems[i].Equipped;
                            if(hideoutData.UpgradeItems[i].Equipped)
                            {
                                _itemViews[j].SpawnItem();                                                
                            }
                        }
                    }
                }
            }
        }

        public void SaveData(HideoutUpgradeItemView[] itemViews)
        {
            SerializableHideoutData hideoutData = new SerializableHideoutData
            {
                UpgradeItems = new List<UpgradeItems>(),
            };

            foreach (var itemView in itemViews)
            {
                hideoutData.UpgradeItems.Add(new UpgradeItems
                {
                    Equipped = itemView.IsEquipped,
                    Name = itemView.ItemName,
                    Unlocked = itemView.IsUnlocked
                });
            }
            
            var data = StringSerializationAPI.Serialize(typeof(SerializableHideoutData),  hideoutData);
            FindObjectOfType<PlayfabManager>().UploadJson("HideoutData", data);
        }
    }
}