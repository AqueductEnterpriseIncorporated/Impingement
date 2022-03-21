using UnityEngine;
using Random = UnityEngine.Random;

namespace Impingement.Dungeon
{
    public class DungeonProgressionManager : MonoBehaviour
    {
        public int AreaLevel = 1;
        [SerializeField] private Vector2Int _maximumDungeonSize = new Vector2Int(6, 6);
        private Vector2Int _dungeonSize = new Vector2Int(2, 3);
        
        public Vector2Int GetDungeonSize()
        {
            return _dungeonSize;
        }

        public void Reset()
        {
            _dungeonSize = new Vector2Int(2, 3);
            AreaLevel = 1;
        }
        
        public void CompleteLevel()
        {
            AreaLevel++;
            if (_dungeonSize != _maximumDungeonSize)
            {
                var randomX = Random.Range(0, 2);
                var randomY = Random.Range(0, 2);
                var randomX2 = Random.Range(0, 2);
                var randomY2 = Random.Range(0, 2);
                var newSize = new Vector2Int(Mathf.Min(randomX, randomX2), Mathf.Min(randomY, randomY2));
                _dungeonSize += newSize;
            }
        }
    }
}