using UnityEngine;
using System.Collections.Generic;
public class RoomNode
{
    public RoomProperties roomProperties;
    public Vector2Int gridOrigin;
    public int depth;
    public List<Doorway> openDoors;
}
