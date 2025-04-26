using System.Linq;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public class DungeonGraphNode
    {
        private DungeonGraphNode[] children = new DungeonGraphNode[5];
        private string roomType;

        public void appendChild(DungeonGraphNode node)
        {
            children.Append(node);
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

        public DungeonRoom(Vector2 position, Transform rootTransform, GameObject prefab)
        {
            this.position = position;
            gameObject = Instantiate(prefab, position, Quaternion.identity, rootTransform);
        }

        public void makeConnections(DungeonRoom room)
        {
            if(this.position.x - room.position.x == 1 && this.position.y == room.position.y)
            {
                this.leftConnection = room;
                room.rightConnection = this;
            } else if (this.position.x - room.position.x == -1 && this.position.y == room.position.y)
            {
                this.rightConnection = room;
                room.leftConnection = this;
            } else if (this.position.y - room.position.y == 1 && this.position.x == room.position.x)
            {
                this.downConnection = room;
                room.upConnection = this;
            } else if (this.position.y - room.position.y == -1 && this.position.x == room.position.x)
            {
                this.upConnection = room;
                room.downConnection = this;
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
    }

    public int generationSteps;
    public int generationRounds;
    public DungeonRoom[] rooms = new DungeonRoom[100];
    public DungeonGraphNode rootDungeonNode = new DungeonGraphNode();
    public GameObject[] roomPrefabs = new GameObject[100];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rooms[0] = new DungeonRoom(new Vector2(0, 0), transform, roomPrefabs[Random.Range(0, roomPrefabs.Count())]);
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

                Vector2 position = getRandomDirection();
                while (!canConnect(currentRoom, position))
                {
                    position = getRandomDirection();
                }


                DungeonRoom newRoom = new DungeonRoom(currentRoom.position + position, transform, roomPrefabs[Random.Range(0, roomPrefabs.Count())]);
                rooms[i + 1] = newRoom;
                setConnection(currentRoom, newRoom, position);
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

        for (int i = 0; i < generationSteps * generationRounds; i++)
        {
            print(rooms[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector2 getRandomDirection()
    {
        int rand = UnityEngine.Random.Range(0, 4);
        switch(rand)
        {
            case 0:
                return new Vector2(1, 0);
            case 1:
                return new Vector2(-1, 0);
            case 2:
                return new Vector2(0, 1);
            case 3:
                return new Vector2(0, -1);
            default:
                return new Vector2(0, 1);
        }
    }

    bool canConnect(DungeonRoom room,  Vector2 position)
    {
        if (room.upConnection != null && position.y == 1) return false;
        if (room.downConnection != null && position.y == -1) return false;
        if (room.rightConnection != null && position.x == 1) return false;
        if (room.leftConnection != null && position.x == -1) return false;
        return true;
    }

    bool hasOpenConnections(DungeonRoom room)
    {
        return !(room.upConnection != null && room.downConnection != null && room.rightConnection != null && room.leftConnection != null);
    }

    void setConnection(DungeonRoom root, DungeonRoom room, Vector2 position)
    {
        if (position.y == 1) {
            root.upConnection = room;
            room.downConnection = root;
        }
        if(position.y == -1) { 
            root.downConnection = room;
            room.upConnection = root;
        }
        if(position.x == 1) { 
            root.rightConnection = room;
            room.leftConnection = root;
        }
        if(position.x == -1) { 
            root.leftConnection = room;
            room.rightConnection = root;
        }
    }
}
