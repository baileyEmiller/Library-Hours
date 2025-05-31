using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "Scriptable Objects/Room")]
public class Room : ScriptableObject
{
    public GameObject prefab;
    public RoomType type;
    public Vector2Int roomSize = new Vector2Int(1, 1);
    public List<Doorway> doorways;
    public int minDepth = 0;
    public int maxDepth = 0;
    public float weight = 1.0f;
    public List<string> exclusionTags;

    Room()
    {
        RoomProperties props = prefab.GetComponent<RoomProperties>();
        props.type = type;
        props.roomSize = roomSize;
        props.minDepth = minDepth;
        props.maxDepth = maxDepth;
        props.weight = weight;
        props.exclusionTags = exclusionTags;
    }
}
