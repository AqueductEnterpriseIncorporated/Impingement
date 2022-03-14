using UnityEngine;

namespace Impingement.Dungeon
{
    public class DungeonProgressionManager : MonoBehaviour
    {
        public int AreaLevel = 1;
        private Vector2Int _dungeonSize = new Vector2Int(3, 2);

        public Vector2Int GetDungeonSize()
        {
            return _dungeonSize;
        }
        
        public void CompleteLevel()
        {
            AreaLevel++;
            _dungeonSize += new Vector2Int(Random.Range(0,2), Random.Range(0,2));
        }
    }
}