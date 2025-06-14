using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int targetRoomCount = 50;
    public int gridCellSize = 10;
    public Transform root;

    public List<RoomProperties> roomTemplates;
    [System.Serializable]
    public class DepthPlacementRule
    {
        public int minDepth;
        public int maxDepth;
        public List<RoomType> allowedRooms;
        public float spawnChanceModifier = 1.0f;
        public RoomType requiredType = RoomType.Empty;
    }
    public List<DepthPlacementRule> depthPlacementRules;

    private List<RoomNode> generatedRooms = new List<RoomNode>();
    private Dictionary<Vector2Int, RoomNode> grid = new Dictionary<Vector2Int, RoomNode>();
    private Queue<RoomNode> frontier = new Queue<RoomNode>();

    public void GenerateDungeon()
    {
        ResetDungeon();
        RoomProperties startRoom = roomTemplates.Find(r => r.type == RoomType.Entrance);
        if (startRoom == null)
        {
            Debug.LogError("No start room found!");
            return;
        }

        RoomNode startNode = new RoomNode
        {
            roomProperties = startRoom,
            gridOrigin = Vector2Int.zero,
            depth = 0,
            openDoors = new List<Doorway>(startRoom.doorways)
        };
        PlaceNode(startNode);
        frontier.Enqueue(startNode);
        print($"{generatedRooms.Count} {frontier.Count}");
        while (generatedRooms.Count < targetRoomCount && frontier.Count > 0)
        {
            RoomNode currentRoom = frontier.Dequeue();
            print($"Working on room of type {currentRoom.roomProperties.type} at depth {currentRoom.depth}. It has {currentRoom.openDoors.Count} open doorways");

            foreach (Doorway doorway in currentRoom.openDoors)
            {
                Vector2Int potentialNewRoomOrigin = CalculateRoomOriginFromDoorway(currentRoom, doorway);
                RoomProperties selectedTemplate = SelectRoomTemplate(currentRoom.depth + 1, potentialNewRoomOrigin, doorway);

                if (selectedTemplate != null)
                {
                    RoomNode newRoomNode = new RoomNode
                    {
                        roomProperties = selectedTemplate,
                        gridOrigin = potentialNewRoomOrigin,
                        depth = currentRoom.depth + 1,
                        openDoors = new List<Doorway>(selectedTemplate.doorways)
                    };

                    if (CanPlaceRoomNode(newRoomNode, doorway))
                    {
                        PlaceNode(newRoomNode);
                        frontier.Enqueue(newRoomNode);
                        doorway.isConnected = true;
                        Doorway connectedDoor;
                        switch (doorway.direction)
                        {
                            case DoorDirection.East:
                                connectedDoor = newRoomNode.openDoors.Find((d) => d.direction == DoorDirection.West);
                                break;
                            case DoorDirection.South:
                                connectedDoor = newRoomNode.openDoors.Find(d => d.direction == DoorDirection.North);
                                break;
                            case DoorDirection.West:
                                connectedDoor = newRoomNode.openDoors.Find(d => d.direction == DoorDirection.East);
                                break;
                            case DoorDirection.North:
                                connectedDoor = newRoomNode.openDoors.Find(d => d.direction == DoorDirection.South);
                                break;
                        }
                    }
                } else
                {
                    Debug.LogWarning("No room was selected at depth " + currentRoom.depth + 1);
                }
            }
        }
        InstantiateRooms();
    }

    private void ResetDungeon()
    {
        generatedRooms = new List<RoomNode>();
        grid = new Dictionary<Vector2Int, RoomNode>();
        frontier = new Queue<RoomNode>();
    }

    private void PlaceNode(RoomNode roomNode)
    {
        generatedRooms.Add(roomNode);
        for (int x = 0; x < roomNode.roomProperties.roomSize.x; x++)
        {
            for (int y = 0; y < roomNode.roomProperties.roomSize.y; y++)
            {
                grid[roomNode.gridOrigin + new Vector2Int(x, y)] = roomNode;
            }
        }
    }

    private bool CanPlaceRoomNode(RoomNode node, Doorway doorway)
    {
        for (int x = 0; x < node.roomProperties.roomSize.x; x++)
        {
            for (int y = 0; y < node.roomProperties.roomSize.y; y++)
            {
                if (grid.ContainsKey(node.gridOrigin + new Vector2Int(x, y)))
                {
                    return false;
                }
            }
        }


        bool hasDoorInThisDirection = false;

        foreach (Doorway door in node.openDoors)
        {
            if (door.direction == DoorDirection.North && doorway.direction == DoorDirection.South) hasDoorInThisDirection = true;
            if (door.direction == DoorDirection.South && doorway.direction == DoorDirection.North) hasDoorInThisDirection = true;
            if (door.direction == DoorDirection.East && doorway.direction == DoorDirection.West) hasDoorInThisDirection = true;
            if (door.direction == DoorDirection.West && doorway.direction == DoorDirection.East) hasDoorInThisDirection = true;
        }
        print($"Has door in the {doorway.direction} direction: {hasDoorInThisDirection} {node.openDoors.Count}");
        return hasDoorInThisDirection;
    }

    private RoomProperties SelectRoomTemplate(int currentDepth, Vector2Int potentialOrigin, Doorway doorway)
    {
        List<RoomProperties> candidates = new List<RoomProperties>();

        DepthPlacementRule currentRule = depthPlacementRules.Find(r => currentDepth >= r.minDepth && currentDepth <= r.maxDepth);
        if (currentRule != null)
        {
            if (currentRule.requiredType != RoomType.Empty)
            {
                RoomProperties requiredRoom = roomTemplates.Find(r => r.type == currentRule.requiredType);
                if (requiredRoom != null)
                {
                    return requiredRoom;
                }
            }

            foreach (RoomProperties roomProperties in roomTemplates)
            {
                if (currentRule.allowedRooms.Contains(roomProperties.type) && roomProperties.minDepth <= currentDepth && roomProperties.maxDepth >= currentDepth)
                {
                    candidates.Add(roomProperties);
                }
            }
        }
        else
        {
            foreach (RoomProperties roomProperties in roomTemplates)
            {
                if (roomProperties.minDepth <= currentDepth && roomProperties.maxDepth >= currentDepth)
                {
                    candidates.Add(roomProperties);
                }
            }
        }

        candidates = candidates.FindAll(r =>
        {
            bool hasDoorInThisDirection = false;
            foreach (Doorway door in r.doorways)
            {
                if (door.direction == DoorDirection.North && doorway.direction == DoorDirection.South) hasDoorInThisDirection = true;
                if (door.direction == DoorDirection.South && doorway.direction == DoorDirection.North) hasDoorInThisDirection = true;
                if (door.direction == DoorDirection.East && doorway.direction == DoorDirection.West) hasDoorInThisDirection = true;
                if (door.direction == DoorDirection.West && doorway.direction == DoorDirection.East) hasDoorInThisDirection = true;
            }
            return hasDoorInThisDirection;
        });

        float totalWeight = candidates.Sum(r => r.weight);
        float randVal = Random.Range(0, totalWeight);
        foreach (RoomProperties roomProperties in candidates)
        {
            randVal -= roomProperties.weight;
            if (randVal <= 0)
            {
                return roomProperties;
            }
        }
        return null;
    }

    private void InstantiateRooms()
    {
        foreach (RoomNode node in generatedRooms)
        {
            GameObject roomObject = Instantiate(node.roomProperties.gameObject, new Vector3(node.gridOrigin.x * gridCellSize, 0, node.gridOrigin.y * gridCellSize), Quaternion.identity, root);
            roomObject.GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
        }
    }

    private Vector2Int CalculateRoomOriginFromDoorway(RoomNode roomNode, Doorway doorway)
    {
        switch(doorway.direction)
        {
            case DoorDirection.North:
                return roomNode.gridOrigin + new Vector2Int(0, -roomNode.roomProperties.roomSize.y);
            case DoorDirection.East:
                return roomNode.gridOrigin + new Vector2Int(-roomNode.roomProperties.roomSize.x, 0);
            case DoorDirection.South:
                return roomNode.gridOrigin + new Vector2Int(0, roomNode.roomProperties.roomSize.y);
            case DoorDirection.West:
                return roomNode.gridOrigin + new Vector2Int(roomNode.roomProperties.roomSize.x, 0);
            default:
                return roomNode.gridOrigin;
        }
    }
}
