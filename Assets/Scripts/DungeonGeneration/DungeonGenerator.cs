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
    private Dictionary<Vector2, RoomNode> grid = new Dictionary<Vector2, RoomNode>();
    private Queue<RoomNode> frontier = new Queue<RoomNode>();

    private void Start()
    {
        GenerateDungeon();
    }

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
            gridOrigin = Vector2.zero,
            depth = 0,
            openDoors = new List<Doorway>(startRoom.doorways)
        };
        PlaceNode(startNode);
        frontier.Enqueue(startNode);
        while (generatedRooms.Count < targetRoomCount && frontier.Count > 0)
        {
            RoomNode currentRoom = frontier.Dequeue();

            foreach (Doorway doorway in currentRoom.openDoors)
            {
                RoomProperties selectedTemplate = SelectRoomTemplate(currentRoom.depth + 1, doorway);

                if (selectedTemplate != null)
                {
                    RoomNode newRoomNode = new RoomNode
                    {
                        roomProperties = selectedTemplate,
                        gridOrigin = new Vector2(),
                        depth = currentRoom.depth + 1,
                        openDoors = new List<Doorway>(selectedTemplate.doorways)
                    };
                    Vector2 potentialNewRoomOrigin = CalculateRoomOriginFromDoorway(newRoomNode, currentRoom, doorway);
                    newRoomNode.gridOrigin = potentialNewRoomOrigin;

                    print($"Placed Room with {selectedTemplate.doorways.FindAll(d => d.isConnected == false).Count()} open doors at depth {newRoomNode.depth} at origin {newRoomNode.gridOrigin}");

                    if (CanPlaceRoomNode(newRoomNode, doorway))
                    {
                        PlaceNode(newRoomNode);
                        frontier.Enqueue(newRoomNode);
                        Doorway connectedDoor;
                        switch (doorway.direction)
                        {
                            case DoorDirection.PosX:
                                connectedDoor = newRoomNode.openDoors.Find((d) => d.direction == DoorDirection.NegX);
                                break;
                            case DoorDirection.NegX:
                                connectedDoor = newRoomNode.openDoors.Find(d => d.direction == DoorDirection.PosX);
                                break;
                            case DoorDirection.PosZ:
                                connectedDoor = newRoomNode.openDoors.Find(d => d.direction == DoorDirection.NegZ);
                                break;
                            case DoorDirection.NegZ:
                                connectedDoor = newRoomNode.openDoors.Find(d => d.direction == DoorDirection.PosZ);
                                break;
                            default:
                                connectedDoor = null;
                                break;
                        }
                    }
                } else
                {
                    Debug.LogWarning("No room was selected at depth " + currentRoom.depth + 1);
                }
            }
        }

        RoomNode lastRoom = null;
        foreach (RoomNode node in generatedRooms)
        {
            foreach (Doorway door in node.openDoors)
            {
                if (!door.isConnected)
                {
                    var candidates = roomTemplates.FindAll(r => r.type == RoomType.Boss);
                    if (candidates != null && candidates.Count > 0)
                    {
                        RoomProperties selectedTemplate = candidates[Random.Range(0, candidates.Count)];
                        RoomNode newRoomNode = new RoomNode
                        {
                            roomProperties = selectedTemplate,
                            gridOrigin = new Vector2(0, 0),
                            depth = node.depth + 1,
                            openDoors = new List<Doorway>(selectedTemplate.doorways)    
                        };
                        var potentialOrigin = CalculateRoomOriginFromDoorway(newRoomNode, node, door);
                        newRoomNode.gridOrigin = potentialOrigin;

                        if (CanPlaceRoomNode(newRoomNode, door))
                        {
                            lastRoom = newRoomNode;
                        }
                    }
                }
            }
        }
        if (lastRoom != null)
        {
            PlaceNode(lastRoom);
        } else
        {
            Debug.LogError("No boss room could be placed!");
        }

            InstantiateRooms();
    }

    private void ResetDungeon()
    {
        generatedRooms = new List<RoomNode>();
        grid = new Dictionary<Vector2, RoomNode>();
        frontier = new Queue<RoomNode>();
    }

    private void PlaceNode(RoomNode roomNode)
    {
        generatedRooms.Add(roomNode);
        for (int x = 0; x < roomNode.roomProperties.roomSize.x; x++)
        {
            for (int y = 0; y < roomNode.roomProperties.roomSize.y; y++)
            {
                grid[roomNode.gridOrigin + new Vector2(x, y)] = roomNode;
            }
        }
    }

    private bool CanPlaceRoomNode(RoomNode node, Doorway doorway)
    {
        for (int x = 0; x < node.roomProperties.roomSize.x; x++)
        {
            for (int y = 0; y < node.roomProperties.roomSize.y; y++)
            {
                if (grid.ContainsKey(node.gridOrigin + new Vector2(x, y)))
                {
                    return false;
                }
            }
        }


        bool hasDoorInThisDirection = false;

        foreach (Doorway door in node.openDoors)
        {
            if (door.direction == DoorDirection.PosX && doorway.direction == DoorDirection.NegX) hasDoorInThisDirection = true;
            if (door.direction == DoorDirection.NegX && doorway.direction == DoorDirection.PosX) hasDoorInThisDirection = true;
            if (door.direction == DoorDirection.PosZ && doorway.direction == DoorDirection.NegZ) hasDoorInThisDirection = true;
            if (door.direction == DoorDirection.NegZ && doorway.direction == DoorDirection.PosZ) hasDoorInThisDirection = true;
        }
        return hasDoorInThisDirection;
    }

    private RoomProperties SelectRoomTemplate(int currentDepth, Doorway doorway)
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
                if (door.direction == DoorDirection.PosX && doorway.direction == DoorDirection.NegX) hasDoorInThisDirection = true;
                if (door.direction == DoorDirection.NegX && doorway.direction == DoorDirection.PosX) hasDoorInThisDirection = true;
                if (door.direction == DoorDirection.PosZ && doorway.direction == DoorDirection.NegZ) hasDoorInThisDirection = true;
                if (door.direction == DoorDirection.NegZ && doorway.direction == DoorDirection.PosZ) hasDoorInThisDirection = true;
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
        List<GameObject> instantiatedRooms = new List<GameObject>();
        foreach (RoomNode node in generatedRooms)
        {
            GameObject roomObject = Instantiate(node.roomProperties.gameObject, new Vector3(node.gridOrigin.x * gridCellSize, 0, node.gridOrigin.y * gridCellSize), Quaternion.identity, root);
            instantiatedRooms.Add(roomObject);
        }
        instantiatedRooms[0].GetComponentInChildren<NavMeshSurface>().BuildNavMesh();
    }

    private Vector2 CalculateRoomOriginFromDoorway(RoomNode toNode, RoomNode fromNode, Doorway doorway)
    {
        // Find the corresponding doorway in the new room
        DoorDirection oppositeDirection = GetOppositeDirection(doorway.direction);
        Doorway toDoorway = toNode.openDoors.Find(d => d.direction == oppositeDirection);
        
        if (toDoorway == null)
        {
            Debug.LogError($"No matching doorway found in new room for direction {oppositeDirection}");
            return fromNode.gridOrigin;
        }

        // Calculate the doorway position in the fromNode
        Vector2 fromDoorwayPos = fromNode.gridOrigin + doorway.Offset();
        
        Debug.Log($"FromNode origin: {fromNode.gridOrigin}, doorway offset: {doorway.Offset()}, doorway world pos: {fromDoorwayPos}");
        Debug.Log($"ToNode doorway offset: {toDoorway.Offset()}");

        switch (doorway.direction)
        {
            case DoorDirection.PosX:
                // Place new room to the right of the fromNode
                // The new room's doorway should align with the fromNode's doorway
                Vector2 posXOrigin = fromDoorwayPos - toDoorway.Offset();
                Debug.Log($"PosX: From doorway pos {fromDoorwayPos} - to doorway offset {toDoorway.Offset()} = {posXOrigin}");
                return posXOrigin;
            case DoorDirection.NegX:
                // Place new room to the left of the fromNode
                Vector2 negXOrigin = fromDoorwayPos - toDoorway.Offset();
                Debug.Log($"NegX: From doorway pos {fromDoorwayPos} - to doorway offset {toDoorway.Offset()} = {negXOrigin}");
                return negXOrigin;
            case DoorDirection.PosZ:
                // Place new room above the fromNode
                Vector2 posZOrigin = fromDoorwayPos - toDoorway.Offset();
                Debug.Log($"PosZ: From doorway pos {fromDoorwayPos} - to doorway offset {toDoorway.Offset()} = {posZOrigin}");
                return posZOrigin;
            case DoorDirection.NegZ:
                // Place new room below the fromNode
                Vector2 negZOrigin = fromDoorwayPos - toDoorway.Offset();
                Debug.Log($"NegZ: From doorway pos {fromDoorwayPos} - to doorway offset {toDoorway.Offset()} = {negZOrigin}");
                return negZOrigin;
            default:
                return fromNode.gridOrigin;
        }
    }

    private DoorDirection GetOppositeDirection(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.PosX: return DoorDirection.NegX;
            case DoorDirection.NegX: return DoorDirection.PosX;
            case DoorDirection.PosZ: return DoorDirection.NegZ;
            case DoorDirection.NegZ: return DoorDirection.PosZ;
            default: return DoorDirection.PosX;
        }
    }
}
