using UnityEngine;

namespace Impingement.Dungeon
{
    public class DungeonProgressionManager : MonoBehaviour
    {
        public int AreaLevel = 1;
        public Vector2Int DungeonSize = new Vector2Int(3, 2);

        public void CompleteLevel()
        {
            AreaLevel++;
            DungeonSize += new Vector2Int(Random.Range(0,1), Random.Range(0,1));
        }
    }
}