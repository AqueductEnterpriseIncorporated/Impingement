using System;
using Impingement.Stats;
using Playfab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Impingement.Control
{
    public class PlayfabPlayerDataController : MonoBehaviour
    {
        private PlayfabManager _playfabManager;
        private ExperienceController _experienceController;

        private void Start()
        {
            _playfabManager = FindObjectOfType<PlayfabManager>();
            _experienceController = GetComponent<ExperienceController>();
                
            _playfabManager.LoadData(OnDataReceivedPlayerData);
        }

        private void OnDataReceivedPlayerData(GetUserDataResult getUserDataResult)
        {
            if (getUserDataResult == null) { return; }

            if (getUserDataResult.Data.ContainsKey("Experience"))
            {
                _experienceController.GainExperience(Convert.ToInt32(getUserDataResult.Data["Experience"].Value));
            }
        }
    }
}