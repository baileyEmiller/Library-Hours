using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Entrance, Empty, Hallway, Easy, Normal, Hard, Trap, Treasure, Boss}

public class RoomProperties : MonoBehaviour
{
    public RoomType type;
    public Vector2Int roomSize = new Vector2Int(1, 1);
    public List<Doorway> doorways;
    public int minDepth = 0;
    public int maxDepth = 0;
    public float weight = 1.0f;
    public List<string> exclusionTags;
}
