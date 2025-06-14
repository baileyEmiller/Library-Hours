using UnityEngine;

public enum DoorDirection { North, South, East, West }

public class Doorway : MonoBehaviour
{
    public DoorDirection direction;
    public bool isConnected = false;
    public Vector2Int offset;

    void OnTriggerEnter(Collider collider)
    {
        print("Collided with doorway");
        GetComponentInParent<RoomProperties>().onDoorEntered(collider.gameObject);
    }
}
