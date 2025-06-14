using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Entrance, Empty, Hallway, Easy, Normal, Hard, Trap, Treasure, Boss}

public class RoomProperties : MonoBehaviour
{
    [System.Serializable]
    public class Spawnable
    {
        public GameObject prefab;
        public Vector3 spawnPosition;
    }

    public RoomType type;
    public Vector2Int roomSize = new Vector2Int(1, 1);
    public List<Doorway> doorways;
    public int minDepth = 0;
    public int maxDepth = 0;
    public float weight = 1.0f;
    public List<string> exclusionTags;
    public List<Spawnable> spawnOnFirstEnter = new List<Spawnable>();
    public List<Spawnable> spawnOnEnter = new List<Spawnable>();

    private bool entered = false;

    public void onDoorEntered(GameObject door)
    {
        if (!entered)
        {
            entered = true;
            foreach (Spawnable spawnable in spawnOnFirstEnter)
            {
                Instantiate(spawnable.prefab, spawnable.spawnPosition + transform.position, Quaternion.identity);
            }
        }
        foreach(Spawnable spawnable in spawnOnEnter)
        {
            Instantiate(spawnable.prefab, spawnable.spawnPosition + transform.position, Quaternion.identity);
        }

    }
}
