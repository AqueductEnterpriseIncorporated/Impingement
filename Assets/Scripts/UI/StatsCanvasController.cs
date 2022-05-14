using System;
using UnityEngine;

namespace Impingement.UI
{
    public class StatsCanvasController : MonoBehaviour
    {
        [SerializeField] private GameObject _canvasParent;
        [SerializeField] private GameObject _bottomBackground;
        [SerializeField] private RectTransform _minimapRect;
        [SerializeField] private KeyCode _toggleKey = KeyCode.I;
        private float _minimapXOffset;
        private Vector2 _defaultMinimapPosition;

        private void Start()
        {
            _defaultMinimapPosition = _minimapRect.anchoredPosition;
            var canvasRect = _canvasParent.GetComponent<RectTransform>();
            _minimapXOffset = canvasRect.rect.width;
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey) || Input.GetKeyDown(KeyCode.Escape))
            {
                _canvasParent.SetActive(!_canvasParent.activeSelf);
                _bottomBackground.SetActive(_canvasParent.activeSelf);
            }

            MoveMinimap(_canvasParent.activeSelf);
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
