using UnityEngine;

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
        
        public void CompleteLevel()
        {
            AreaLevel++;
            if (_dungeonSize != _maximumDungeonSize)
            {
                _dungeonSize += new Vector2Int(Random.Range(0, 2), Random.Range(0, 2));
            }
        }
    }
}