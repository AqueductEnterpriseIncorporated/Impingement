using System;
using System.Linq;
using Impingement.Control;
using UnityEngine;

namespace Impingement.UI
{
    public class StatsCanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject _canvasParent;
        [SerializeField] private GameObject[] _objectsToHide;
        [SerializeField] private RectTransform _minimapRect;
        [SerializeField] private KeyCode _toggleKey;
        [SerializeField] private InputManager _inputManager;
        private float _minimapXOffset;
        private Vector2 _defaultMinimapPosition;

        private void Start()
        {
            _defaultMinimapPosition = _minimapRect.anchoredPosition;
            var canvasRect = _canvasParent.GetComponent<RectTransform>();
            _minimapXOffset = canvasRect.rect.width;
            _toggleKey = _inputManager.ButtonKeysList
                .FirstOrDefault(i => i.ButtonName == "Инвентарь")!.ActiveButtonKeyCode;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                foreach (var objectToHide in _objectsToHide)
                {
                    objectToHide.SetActive(!objectToHide.activeSelf);
                }
            }

            MoveMinimap(_objectsToHide[0].activeSelf);
        }

        private void MoveMinimap(bool canvasParentActiveSelf)
        {
            if (canvasParentActiveSelf)
            {
                _minimapRect.anchoredPosition =
                    _defaultMinimapPosition - new Vector2(_minimapXOffset, 0);
            }
            else
            {
                _minimapRect.anchoredPosition = _defaultMinimapPosition;
            }
        }
    }
}
