using System;
using Impingement.Core;
using Steamworks;
using TMPro;
using UnityEngine;

namespace Impingement.Steam
{
    public class SteamAuth : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private StartSceneManager _startSceneManager;

        public string SteamName;
        public CSteamID SteamId;
        public bool IsAuthorized;
        Callback<GetAuthSessionTicketResponse_t> m_AuthTicketResponseCallback;
        HAuthTicket m_AuthTicket;
        string m_SessionTicket;

        private void Start()
        {
            try
            {
                SignInWithSteam();
            }
            catch (Exception e)
            {
                FindObjectOfType<StartSceneManager>().Error(e.Message);
                throw;
            }
            
            if (!SteamManager.Initialized)
            {
                IsAuthorized = false;
                return;
            }

            SteamName = SteamFriends.GetPersonaName();
            SteamId = SteamUser.GetSteamID();
            _inputField.text = SteamName;
        }

        private void SignInWithSteam()
        {
            // It's not necessary to add event handlers if they are 
            // already hooked up.
            // Callback.Create return value must be assigned to a 
            // member variable to prevent the GC from cleaning it up.
            // Create the callback to receive events when the session ticket
            // is ready to use in the web API.
            // See GetAuthSessionTicket document for details.
            m_AuthTicketResponseCallback = Callback<GetAuthSessionTicketResponse_t>.Create(OnAuthCallback);
            var buffer = new byte[1024];
            m_AuthTicket = SteamUser.GetAuthSessionTicket(buffer, buffer.Length, out var ticketSize);

            Array.Resize(ref buffer, (int)ticketSize);

            // The ticket is not ready yet, wait for OnAuthCallback.
            m_SessionTicket = BitConverter.ToString(buffer).Replace("-", string.Empty);
        }

        void OnAuthCallback(GetAuthSessionTicketResponse_t callback)
        {
            // Call Unity Authentication SDK to sign in or link with Steam.
            
            Debug.Log("Steam Login success. Session Ticket: " + m_SessionTicket);
            IsAuthorized = true;
            _startSceneManager.Login(SteamId.m_SteamID.ToString());
        }
    }
}