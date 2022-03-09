using Impingement.enums;
using UnityEngine;

namespace Impingement.structs
{
    [System.Serializable]
    public struct CursorMapping
    {
        public enumCursorType Type;
        public Texture2D texture;
        public Vector2 hotspot;
    }
}