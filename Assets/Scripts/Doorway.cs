using UnityEngine;

public enum DoorDirection { North, South, East, West }

public class Doorway : MonoBehaviour
{
    public DoorDirection direction;
    public bool isConnected = false;
    public Vector2Int offset;
}
