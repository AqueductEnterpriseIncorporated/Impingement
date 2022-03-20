using System;
using System.Collections;
using System.IO;
using System.Net;
using Impingement.Playfab;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Impingement.UI
{
    public class BugPanelController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _theme;
        [SerializeField] private TMP_Text _message;
        [SerializeField] private GameObject _tyPanel;
        [SerializeField] private GameObject[] _objectToDisable;
        [SerializeField] private Button[] _buttons;
        private readonly string _defaultScreenshotName = "Bug.png";

        public void SendReport()
        {
            StartCoroutine(SendReportCoroutine());
        }
        
        private IEnumerator SendReportCoroutine()
        {
            if(_theme.text.Length < 3 && _message.text.Length < 3) { yield break; }
            FindObjectOfType<PlayfabManager>().SendReport(_theme.text, _message.text);
            _tyPanel.SetActive(true);
            SetActiveObjects(false); 
            //yield return new WaitForSeconds(0.5f);
            ScreenCapture.CaptureScreenshot( Directory.GetCurrentDirectory() + "/" +_defaultScreenshotName);
            yield return new WaitForSeconds(0.5f);
            SetActiveObjects(true);

            //PostToImgur( Directory.GetCurrentDirectory() + "/" +_defaultScreenshotName, "");
        }

        private void SetActiveObjects(bool value)
        {
            foreach (var ui in _objectToDisable)
            {
                ui.SetActive(value);
            }
        }

        public void EnableButtons(bool value)
        {
            foreach (var button in _buttons)
            {
                button.enabled = value;
            }
        }
        
        private void PostToImgur(string imagFilePath, string apiKey)
        {
            byte[] imageData;

            FileStream fileStream = File.OpenRead(imagFilePath);
            imageData = new byte[fileStream.Length];
            fileStream.Read(imageData, 0, imageData.Length);
            fileStream.Close();

            string uploadRequestString = "image=" + Uri.EscapeDataString(System.Convert.ToBase64String(imageData)) + "&key=" + apiKey;

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://api.imgur.com/2/upload");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ServicePoint.Expect100Continue = false;

            StreamWriter streamWriter = new StreamWriter(webRequest.GetRequestStream());
            streamWriter.Write(uploadRequestString);
            streamWriter.Close();

            WebResponse response = webRequest.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader responseReader = new StreamReader(responseStream);

            string responseString = responseReader.ReadToEnd();
        }
    }
}