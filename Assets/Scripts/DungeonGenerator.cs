using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public class DungeonGenerator : MonoBehaviour
{
    public class DungeonGraphNode
    {
        private DungeonGraphNode parent = null;
        private DungeonGraphNode[] children = new DungeonGraphNode[5];
        private int[] depthList = new int[5];
        private DungeonRoom room;

        public DungeonRoom getRoom() { return room; }

        public DungeonGraphNode(DungeonRoom room)
        {
            this.room = room;
        }

        public void appendChild(DungeonGraphNode node)
        {
            node.parent = this;
            for (int i = 0; i < children.Length; i++) {
                if (children[i] == null)
                {
                    children[i] = node;
                    break;
                }
            }
        }

        public bool hasChildren()
        {
            return children.Count() > 0;
        }

        public bool hasChild(DungeonGraphNode node)
        {
            return children.Contains(node);
        }

        public DungeonGraphNode findDeepestNode()
        {
            if (this.hasChildren()) {
                int maxDepth = 0;
                DungeonGraphNode deepestNode = null;
                for (int i = 0; i < children.Length; i++) {
                    if (children[i] == null) continue;
                    if (children[i].findDepth() > maxDepth) {
                        maxDepth = children[i].findDepth();
                        deepestNode = children[i];
                    }
                }
                return deepestNode.findDeepestNode();
            } else
            {
                return this;
            }
        }

        public int findDepth(int depth = 0)
        {
            if(this.parent != null)
            {
                return this.parent.findDepth(depth) + 1;
            } else
            {
                return depth;
            }
        }
    }

    public class DungeonRoom
    {
        public Vector2 position;
        public GameObject gameObject;
        public DungeonRoom upConnection = null;
        public DungeonRoom downConnection = null;
        public DungeonRoom leftConnection = null;
        public DungeonRoom rightConnection = null;
        public DungeonGraphNode node;
        public RoomProperties roomProperties;

        public DungeonRoom(Vector2 position, Vector2 direction, Transform rootTransform, GameObject prefab, DungeonRoom connection)
        {
            roomProperties = prefab.GetComponent<RoomProperties>();

            this.position = position + (direction * getRequiredOffsetSize(connection));
            gameObject = Instantiate(prefab, new Vector3(this.position.x, 0, this.position.y), Quaternion.identity, rootTransform);
            node = new DungeonGraphNode(this);
        }

        public void makeConnections(DungeonRoom room)
        {
            if(this.position.x - room.position.x == getRequiredOffsetSize(room).x && this.position.y == room.position.y)
            {
                this.leftConnection = room;
                room.rightConnection = this;
                connectGraph(room);
            } else if (this.position.x - room.position.x == -getRequiredOffsetSize(room).x && this.position.y == room.position.y)
            {
                this.rightConnection = room;
                room.leftConnection = this;
                connectGraph(room);
            } else if (this.position.y - room.position.y == getRequiredOffsetSize(room).y && this.position.x == room.position.x)
            {
                this.downConnection = room;
                room.upConnection = this;
                connectGraph(room);
            } else if (this.position.y - room.position.y == -getRequiredOffsetSize(room).y && this.position.x == room.position.x)
            {
                this.upConnection = room;
                room.downConnection = this;
                connectGraph(room);
            }
        }

        public void connectGraph(DungeonRoom other)
        {
            if(!other.node.hasChild(node))
            {
                node.appendChild(other.node);
            }
        }

        public override string ToString()
        {
            return "Dungeon Room {\n" +
                "Position: " + position + "\n" +
                "Up Connection: " + (upConnection != null) + "\n" +
                "Down Connection: " + (downConnection != null) + "\n" +
                "Left Connection: " + (leftConnection != null) + "\n" +
                "Right Connection: " + (rightConnection != null) + "\n" +
                "}";
        }

        Vector2 getRequiredOffsetSize(DungeonRoom other)
        {
            if(other == null) return Vector2.zero;
            return new Vector2((this.roomProperties.size.x / 2) + (other.roomProperties.size.x / 2), (this.roomProperties.size.y / 2) + (other.roomProperties.size.y / 2));
        }
    }

    public int generationSteps;
    public int generationRounds;
    public DungeonRoom[] rooms = new DungeonRoom[100];
    public GameObject[] roomPrefabs = new GameObject[100];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rooms[0] = new DungeonRoom(new Vector2(0, 0), new Vector2(0, 0), transform, roomPrefabs[Random.Range(0, roomPrefabs.Count())], null);
        for (int y = 0; y < generationRounds; y++)
        {
            bool firstGen = true;
            for (int i = 0 + (generationSteps * y); i < generationSteps + (generationSteps * y); i++)
            {
                DungeonRoom currentRoom = rooms[i];
                if (firstGen) currentRoom = rooms[0];
                if (currentRoom == null) break;
                if (!hasOpenConnections(currentRoom))
                {
                    for (int j = 0; j < rooms.Length; j++)
                    {
                        if (rooms[j] != null && hasOpenConnections(rooms[j]))
                        {
                            currentRoom = rooms[j];
                        }
                    }
                }

                Vector2 direction = getRandomDirection(currentRoom);
                while (!canConnect(currentRoom, direction))
                {
                    direction = getRandomDirection(currentRoom);
                }
                GameObject prefab = roomPrefabs[Random.Range(0, roomPrefabs.Count())];
                RoomProperties prefabProperties = prefab.GetComponent<RoomProperties>();
                Vector2 offset = new Vector2((currentRoom.roomProperties.size.x / 2) + (prefabProperties.size.x / 2), (currentRoom.roomProperties.size.y / 2) + (prefabProperties.size.y / 2));
                Vector2 position = currentRoom.position + (direction * offset);

                for (int j = 0; i < rooms.Length; j++) {
                    if (rooms[j] != null) break;
                    if(position.x > rooms[j].position.x - ((rooms[j].roomProperties.size.x/2) + 1) && 
                        position.x < rooms[j].position.x + ((rooms[j].roomProperties.size.x/2) + 1) &&
                        position.y > rooms[j].position.y - ((rooms[j].roomProperties.size.y/2) + 1) &&
                        position.y < rooms[j].position.y + ((rooms[j].roomProperties.size.y/2) + 1))
                    {
                        for (int k = 0; k < rooms.Length; k++)
                        {
                            if (rooms[k] != null && hasOpenConnections(rooms[k]))
                            {
                                currentRoom = rooms[k];
                            }
                        }
                    }
                }


                DungeonRoom newRoom = new DungeonRoom(currentRoom.position, direction, transform, prefab, currentRoom);
                rooms[i + 1] = newRoom;
                setConnection(currentRoom, newRoom, direction);
                for (int j = 0; j < rooms.Length; j++)
                {
                    if (rooms[j] != null)
                    {
                        rooms[j].makeConnections(newRoom);
                    }
                }
                firstGen = false;
            }
        }

        //print(rooms[0].node.findDeepestNode().getRoom());

        for (int i = 0; i < generationSteps * generationRounds; i++)
        {
            print(rooms[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2 getRandomDirection(DungeonRoom room)
    {
        int rand = UnityEngine.Random.Range(0, 4);
        switch(rand)
        {
            case 0:
                return new Vector2(room.gameObject.transform.localScale.x, 0);
            case 1:
                return new Vector2(-room.gameObject.transform.localScale.x, 0);
            case 2:
                return new Vector2(0, room.gameObject.transform.localScale.y);
            case 3:
                return new Vector2(0, -room.gameObject.transform.localScale.y);
            default:
                return new Vector2(0, room.gameObject.transform.localScale.y);
        }
    }

    bool canConnect(DungeonRoom room, Vector2 direction)
    {
        if (room.upConnection != null && direction.y == 1) return false;
        if (room.downConnection != null && direction.y == -1) return false;
        if (room.rightConnection != null && direction.x == 1) return false;
        if (room.leftConnection != null && direction.x == -1) return false;
        return true;
    }

    bool hasOpenConnections(DungeonRoom room)
    {
        return !(room.upConnection != null && room.downConnection != null && room.rightConnection != null && room.leftConnection != null);
    }

    void setConnection(DungeonRoom root, DungeonRoom room, Vector2 direction)
    {
        if (direction.y == 1) {
            root.upConnection = room;
            room.downConnection = root;
        }
        if(direction.y == -1) { 
            root.downConnection = room;
            room.upConnection = root;
        }
        if(direction.x == 1) { 
            root.rightConnection = room;
            room.leftConnection = root;
        }
        if(direction.x == -1) { 
            root.leftConnection = room;
            room.rightConnection = root;
        }
    }
}
